﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class TemperatureStateMachine : IStateMachine
    {
        private const int _emergencyTemp = 120;

        public GreenhouseState CurrentState { get; set; }

        public TemperatureStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
        }


        public GreenhouseState DetermineState(double value, int hiLimit, int? loLimit = default(int?))
        {
            // Determine which processing state we're in
            if (CurrentState == GreenhouseState.HEATING)
            {
                CurrentState = GreenhouseState.PROCESSING_HEATING;
            }
            else if (CurrentState == GreenhouseState.COOLING)
            {
                CurrentState = GreenhouseState.PROCESSING_COOLING;
            }
            else
            {
                CurrentState = GreenhouseState.PROCESSING_DATA;
            }

            // Determine what state to return/change to
            // If we're coming from an action state and we meet action criteria,
            // go back to the action state
            if (value <= loLimit && CurrentState != GreenhouseState.PROCESSING_HEATING)
            {
                return GreenhouseState.HEATING;
            }
            else if (value <= loLimit && CurrentState == GreenhouseState.PROCESSING_HEATING)
            {
                CurrentState = GreenhouseState.HEATING;
                return GreenhouseState.NO_CHANGE;
            }
            else if (value > hiLimit && CurrentState != GreenhouseState.PROCESSING_COOLING)
            {
                if (value >= _emergencyTemp)
                {
                    return GreenhouseState.EMERGENCY;
                }
                else
                {
                    return GreenhouseState.COOLING;
                }
            }
            else if (value > hiLimit && CurrentState == GreenhouseState.PROCESSING_COOLING)
            {
                if (value >= _emergencyTemp)
                {
                    return GreenhouseState.EMERGENCY;
                }
                else
                {
                    CurrentState = GreenhouseState.COOLING;
                    return GreenhouseState.NO_CHANGE;
                }
            }
            else
            {
                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                return GreenhouseState.WAITING_FOR_DATA;
            }
        }

        public List<Commands> ConvertStateToCommands(GreenhouseState state)
        {
            List<Commands> commandsToSend = new List<Commands>();
            if (state == GreenhouseState.COOLING)
            {
                commandsToSend.Add(Commands.HEAT_OFF);
                commandsToSend.Add(Commands.FANS_ON);
                commandsToSend.Add(Commands.VENT_OPEN);
                commandsToSend.Add(Commands.SHADE_EXTEND);
            }
            else if (state == GreenhouseState.HEATING)
            {
                commandsToSend.Add(Commands.FANS_OFF);
                commandsToSend.Add(Commands.HEAT_ON);
                commandsToSend.Add(Commands.VENT_CLOSE);
                commandsToSend.Add(Commands.SHADE_RETRACT);
            }
            else if (state == GreenhouseState.WAITING_FOR_DATA)
            {
                commandsToSend.Add(Commands.HEAT_OFF);
                commandsToSend.Add(Commands.FANS_OFF);
                commandsToSend.Add(Commands.VENT_CLOSE);
                commandsToSend.Add(Commands.SHADE_RETRACT);
            }

            return commandsToSend;
        }
    }
}
