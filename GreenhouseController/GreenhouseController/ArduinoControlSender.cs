﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class ArduinoControlSender : IDisposable
    {
        // TODO: ERROR CONTROL!
        private bool _success = false;
        private int _retryCount = 0;

        private SerialPort _output;
        public ArduinoControlSender()
        {
            // TODO: construct!
            // _output = new SerialPort("/dev/ttyAMA0", 115200, Parity.None, 8, StopBits.One);
        }

        public void Dispose()
        {
            // TODO: close sockets and stuff here!
            // _output.Close();
        }

        /// <summary>
        /// Takes a list of commands to be sent to the arduino and sends them over the pi's serial port
        /// </summary>
        /// <param name="_commandsToSend">List of GreenhouseCommands from the enum</param>
        public void SendCommand(IStateMachine stateMachine)
        {
            byte[] buffer = new byte[8];
            
            // TODO: send specified commands to arduino
            // _output.Open();

            while(_success != true && _retryCount < 5)
            {
                try
                {
                    stateMachine.CurrentState = GreenhouseState.SENDING_DATA;
                    Console.WriteLine($"Attempting to send state {stateMachine.EndState}");

                    // _output.Write(command.ToString());

                    Console.WriteLine($"State {stateMachine.EndState} sent successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                try
                {
                    // TODO: read response from arduino
                    stateMachine.CurrentState = GreenhouseState.WAITING_FOR_RESPONSE;

                    // Error control
                    // int response = _output.Read(buffer, 0, 8);
                    // if (response is bad) 
                    // {
                    //      _retryCount++;
                    //      retry;
                    // }

                    Console.WriteLine($"State {stateMachine.EndState} executed successfully\n");

                    // Console.WriteLine($"{response}");

                    stateMachine.CurrentState = stateMachine.EndState;
                    _success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex + $"\n State {stateMachine.EndState} unsuccessful\n");
                }
            }
        }
    }
}
