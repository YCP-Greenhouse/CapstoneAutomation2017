﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LightingStateMachine : IStateMachine
    {
        public GreenhouseState CurrentState { get; set; }
        public GreenhouseState EndState { get; set; }
        
        public LightingStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
            EndState = GreenhouseState.WAITING_FOR_DATA;
        }

        public void DetermineGreenhouseState(double value, int hiLimit, int? loLimit = default(int?))
        {
            CurrentState = GreenhouseState.PROCESSING_DATA;
            if (value < hiLimit)
            {
                EndState = GreenhouseState.LIGHTING;
            }
            else
            {
                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                EndState = GreenhouseState.WAITING_FOR_DATA;
            }
        }
    }
}