﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class WateringStateMachine : IStateMachine
    {
        private const int _emergencyMoist = 0;

        public GreenhouseState CurrentState { get; set; }
        public GreenhouseState EndState { get; set; }
        
        public WateringStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
            EndState = GreenhouseState.WAITING_FOR_DATA;
        }

        public void DetermineGreenhouseState(double value, int hiLimit, int? loLimit = default(int?))
        {
            CurrentState = GreenhouseState.PROCESSING_DATA;

            if(value < hiLimit)
            {
                if (value == _emergencyMoist)
                {
                    CurrentState = GreenhouseState.EMERGENCY;
                }
                else 
                {
                    EndState = GreenhouseState.WATERING;
                }
            }
            else
            {
                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                EndState = GreenhouseState.WAITING_FOR_DATA;
            }
        }
    }
}
