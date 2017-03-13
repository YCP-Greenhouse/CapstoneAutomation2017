﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LimitsAnalyzer
    {
        public LimitsAnalyzer() { }

        /// <summary>
        /// Changes the  greenhouse limits based on the limit packet data
        /// </summary>
        /// <param name="limits"></param>
        public void ChangeGreenhouseLimits(LimitPacket limits)
        {
            // TODO: Make these thread-safe somehow!
            if (StateMachineContainer.Instance.Temperature.HighLimit != limits.TempHi)
            {
                StateMachineContainer.Instance.Temperature.HighLimit = limits.TempHi;
            }
            if (StateMachineContainer.Instance.Temperature.LowLimit != limits.TempLo)
            {
                StateMachineContainer.Instance.Temperature.LowLimit = limits.TempLo;
            }
            if (StateMachineContainer.Instance.Lighting.LowLimit != limits.LightLo)
            {
                StateMachineContainer.Instance.Lighting.LowLimit = limits.LightLo;
            }
            if (StateMachineContainer.Instance.Lighting.HighLimit != limits.LightHi)
            {
                StateMachineContainer.Instance.Lighting.HighLimit = limits.LightHi;
            }
            if (StateMachineContainer.Instance.Watering.LowLimit != limits.MoistLim)
            {
                StateMachineContainer.Instance.Watering.LowLimit = limits.MoistLim;
            }

            Console.WriteLine($"Temperature High Limit: {StateMachineContainer.Instance.Temperature.HighLimit}");
            Console.WriteLine($"Temperature Low Limit: {StateMachineContainer.Instance.Temperature.LowLimit}");
            Console.WriteLine($"Lighting High Limit: {StateMachineContainer.Instance.Lighting.HighLimit}");
            Console.WriteLine($"Lighting Low Limit: {StateMachineContainer.Instance.Lighting.LowLimit}");
            Console.WriteLine($"Watering Low Limit: {StateMachineContainer.Instance.Watering.LowLimit}");
        }
    }
}
