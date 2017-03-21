﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GreenhouseController;

namespace ConsoleApplication1
{
    // Adapted from:
    // http://csharp.net-informations.com/communications/csharp-client-socket.htm
    public class SimpleTCPServerTest
    {
        // TODO: Make this broadcast UDP for the limits
        static void Main(string[] args)
        {
            Thread.Sleep(1000);
            TcpListener serverListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            
            TcpClient client = default(TcpClient);
            serverListener.Start();
            Console.WriteLine(" >> Server Started");
            client = serverListener.AcceptTcpClient();
            Console.WriteLine(" >> Accept connection from client");
            NetworkStream networkStream = client.GetStream();

            int[] tlhZones = new int[] { 1, 2, 3, 4, 5 };
            int[] mZones = new int[] { 1, 2, 3, 4, 5, 6 };

            Dictionary<int, DateTime> waterStart = new Dictionary<int, DateTime>();
            Dictionary<int, DateTime> waterEnd = new Dictionary<int, DateTime>();
            Dictionary<int, DateTime> lightStart = new Dictionary<int, DateTime>();
            Dictionary<int, DateTime> lightEnd = new Dictionary<int, DateTime>();
            foreach(int zone in tlhZones)
            {
                lightStart.Add(zone, new DateTime(2017, 3, 20, 20, 0, 0));
                lightEnd.Add(zone, new DateTime(2017, 3, 20, 20, 50, 0));
            }

            foreach(int zone in mZones)
            {
                waterStart.Add(zone, new DateTime(2017, 3, 20, 20, 0, 0));
                waterEnd.Add(zone, new DateTime(2017, 3, 20, 20, 50, 0));
            }
            
            byte[] limitsToSend = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new LimitPacket()
            {
                TempHi = 80,
                TempLo = 65,
                WaterStarts = waterStart,
                WaterEnds = waterEnd,
                LightStarts = lightStart,
                LightEnds = lightEnd
            }));
            networkStream.Write(limitsToSend, 0, limitsToSend.Length);
            networkStream.Flush();

            Console.WriteLine(Encoding.ASCII.GetString(limitsToSend));

            // TODO: add ability to change greenhouse limits
            Console.WriteLine("Would you like to use manual or random mode? Press M for manual, R for random.");
            var key = Console.ReadLine();
            Console.WriteLine();
            if (key == "m" || key == "M")
            {
                #region Manual Controls
                List<int> zones = new List<int>() { 1, 2, 3, 4, 5 };
                List<DataPacket> packetsToSend = new List<DataPacket>();
                Console.WriteLine("Manual mode selected. Currently, the following commands are supported:");
                Console.WriteLine("Q to quit.");
                Console.WriteLine("H for heating.");
                Console.WriteLine("C for cooling.");
                Console.WriteLine("L for lighting.");
                Console.WriteLine("W for watering.");

                bool heat = false;
                bool cool = false;
                bool light = false;
                bool water = false;
                bool stop = false;
                bool invalidCommand = false;
                string command = null;
                while (stop == false)
                {
                    invalidCommand = false;
                    command = Console.ReadLine();
                    Console.WriteLine();
                    foreach (char c in command)
                    {
                        if (c == 'h' || c == 'H')
                        {
                            if (cool != true)
                            {
                                heat = !heat;
                            }
                            else
                            {
                                Console.WriteLine("Invalid command, cannot heat and cool simultaneously! Please try again.");
                                heat = false;
                                cool = false;
                                light = false;
                                water = false;
                                invalidCommand = true;
                                break;
                            }
                        }
                        else if (c == 'c' || c == 'C')
                        {
                            if (heat != true)
                            {
                                cool = !cool;
                            }
                            else
                            {
                                Console.WriteLine("Invalid command, cannot heat and cool simultaneously! Please try again.");
                                heat = false;
                                cool = false;
                                light = false;
                                water = false;
                                invalidCommand = true;
                                break;
                            }
                        }
                        else if (c == 'l' || c == 'L')
                        {
                            light = !light;
                        }
                        else if (c == 'w' || c == 'W')
                        {
                            water = !water;
                        }
                        else if (command == "q" || command == "Q")
                        {
                            stop = true;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid command {command}, please press one of the following keys:");
                            Console.WriteLine("Q to quit.");
                            Console.WriteLine("H for heating.");
                            Console.WriteLine("C for cooling.");
                            Console.WriteLine("L for lighting.");
                            Console.WriteLine("W for watering.");
                        }
                    }
                    if (invalidCommand == false)
                    {
                        for (int i = 1; i < 6; i++)
                        {
                            packetsToSend.Add(
                                new DataPacket()
                                {
                                    Zone = i,
                                    Humidity = 50,
                                    Temperature = 50,
                                    Light = 50,
                                    Moisture = 50,
                                    ManualCool = cool,
                                    ManualHeat = heat,
                                    ManualLight = light,
                                    ManualWater = water
                                });
                        }
                        foreach (var packet in packetsToSend)
                        {
                            string pack = JsonConvert.SerializeObject(packet);
                            byte[] sendBytes = Encoding.ASCII.GetBytes(pack);
                            networkStream.Write(sendBytes, 0, sendBytes.Length);
                            networkStream.Flush();
                            Console.WriteLine(" >> " + $"{pack}");
                            Thread.Sleep(500);
                        }
                        packetsToSend.Clear();
                    }
                }
                #endregion
            }
            else if (key == "T" || key == "t")
            {
                byte[] buffer = new byte[1024];
                while(true)
                {
                    networkStream.Read(buffer, 0, buffer.Length);
                    string received = JsonConvert.DeserializeObject<string>(Encoding.ASCII.GetString(buffer));
                    if (received == "DATA")
                    {
                        Console.WriteLine("Request for data received!");
                        try
                        {
                            JsonSpoof jSpoof = new JsonSpoof();
                            foreach(int zone in tlhZones)
                            {
                                string json = jSpoof.TLHData(zone);
                                byte[] sendBytes = Encoding.ASCII.GetBytes(json);
                                networkStream.Write(sendBytes, 0, sendBytes.Length);
                                networkStream.Flush();
                                Console.WriteLine(" >> " + $"{json}");

                                Thread.Sleep(500);
                            }
                            foreach(int zone in mZones)
                            {
                                string mJson = jSpoof.MoistureData(zone);
                                byte[] mSendBytes = Encoding.ASCII.GetBytes(mJson);
                                networkStream.Write(mSendBytes, 0, mSendBytes.Length);
                                networkStream.Flush();
                                Console.WriteLine(" >> " + $"{mJson}");
                                Thread.Sleep(500);
                            }

                            Console.WriteLine("Data sent!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            else if (key == "r" || key == "R")
            {
                #region Random Data Packets
                int[] zones = new int[] { 1, 2, 3, 4, 5 };
                byte[] bytesFrom = new byte[1024];

                while ((true))
                {
                    try
                    {
                        JsonSpoof jSpoof = new JsonSpoof();

                        foreach (int zone in zones)
                        {
                            string json = jSpoof.TLHData(zone);
                            byte[] sendBytes = Encoding.ASCII.GetBytes(json);
                            networkStream.Write(sendBytes, 0, sendBytes.Length);
                            networkStream.Flush();
                            Console.WriteLine(" >> " + $"{json}");

                            Thread.Sleep(500);
                        }
                        Console.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    Thread.Sleep(15000);
                }
                #endregion
            }
            else
            {
                Console.WriteLine("Invalid character, restarting!");
            }
            client.Close();
            serverListener.Stop();
            Console.WriteLine(" >> exit");
        }

        internal class JsonSpoof
        {
            
            public JsonSpoof() { }
            public string TLHData(int zone)
            {
                int tempMin = 0;
                int tempMax = 120;
                int humidMin = 0;
                int humidMax = 100;
                Random rand = new Random();
                TLHPacket packet = new TLHPacket()
                {
                    Temperature = rand.Next(tempMin, tempMax),
                    Light = rand.Next(20000, 65000),
                    Humidity = rand.Next(humidMin, humidMax),
                    ID = zone
                };

                string spoofData = JsonConvert.SerializeObject(packet);

                return spoofData;
            }

            public string MoistureData(int zone)
            {
                Random rand = new Random();
                MoisturePacket packet = new MoisturePacket()
                {
                    ID = zone,
                    Probe1 = rand.Next(0,100),
                    Probe2 = rand.Next(0, 100),
                    Probe3 = rand.Next(0, 100),
                    Probe4 = rand.Next(0, 100)
                };

                string spoofData = JsonConvert.SerializeObject(packet);

                return spoofData;
            }
        }
    }
}