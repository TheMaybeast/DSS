﻿using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLS.Utils
{
    class Lights
    {
        public static void Update(ActiveVehicle activeVeh)
        {
            switch (activeVeh.LightStage)
            {
                case LightStage.Off:
                    activeVeh.Vehicle.IsSirenOn = true;
                    activeVeh.Vehicle.IsSirenSilent = true;
                    activeVeh.SirenStage = SirenStage.Off;
                    if (activeVeh.AuxOn)
                    {
                        Sound.ClearTempSoundID(activeVeh.AuxID);
                        activeVeh.AuxOn = false;
                    }                    
                    if (activeVeh.AutoStartedTA)
                        activeVeh.TAStage = TAStage.Off;                    
                    //activeVeh.SBOn = false;
                    activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(activeVeh.Vehicle);
                    Sirens.Update(activeVeh);
                    UpdateTA(false, activeVeh);
                    UpdateSB(activeVeh);
                    break;
                case LightStage.One:
                    activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(activeVeh.Vehicle);
                    activeVeh.Vehicle.IsSirenOn = true;
                    activeVeh.Vehicle.IsSirenSilent = true;
                    activeVeh.Vehicle.ShouldVehiclesYieldToThisVehicle = activeVeh.Vehicle.GetDLS().SpecialModes.LSAIYield.Stage1Yield.ToBoolean();
                    UpdateTA(false, activeVeh);
                    UpdateSB(activeVeh);
                    break;
                case LightStage.Two:
                    activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(activeVeh.Vehicle);
                    activeVeh.Vehicle.IsSirenOn = true;
                    activeVeh.Vehicle.IsSirenSilent = true;
                    activeVeh.Vehicle.ShouldVehiclesYieldToThisVehicle = activeVeh.Vehicle.GetDLS().SpecialModes.LSAIYield.Stage2Yield.ToBoolean();
                    UpdateTA(false, activeVeh);
                    UpdateSB(activeVeh);
                    break;
                case LightStage.Three:
                    activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(activeVeh.Vehicle);
                    activeVeh.Vehicle.IsSirenOn = true;
                    activeVeh.Vehicle.IsSirenSilent = true;
                    activeVeh.Vehicle.ShouldVehiclesYieldToThisVehicle = activeVeh.Vehicle.GetDLS().SpecialModes.LSAIYield.Stage3Yield.ToBoolean();
                    UpdateTA(false, activeVeh);
                    UpdateSB(activeVeh);
                    break;
                case LightStage.CustomOne:
                    activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(activeVeh.Vehicle);
                    activeVeh.Vehicle.IsSirenOn = true;
                    activeVeh.Vehicle.IsSirenSilent = true;
                    activeVeh.Vehicle.ShouldVehiclesYieldToThisVehicle = activeVeh.Vehicle.GetDLS().SpecialModes.LSAIYield.Custom1Yield.ToBoolean();
                    UpdateTA(false, activeVeh);
                    UpdateSB(activeVeh);
                    break;
                case LightStage.CustomTwo:
                    activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(activeVeh.Vehicle);
                    activeVeh.Vehicle.IsSirenOn = true;
                    activeVeh.Vehicle.IsSirenSilent = true;
                    activeVeh.Vehicle.ShouldVehiclesYieldToThisVehicle = activeVeh.Vehicle.GetDLS().SpecialModes.LSAIYield.Custom2Yield.ToBoolean();
                    UpdateTA(false, activeVeh);
                    UpdateSB(activeVeh);
                    break;
                default:
                    break;
            }
        }
        public static void UpdateTA(bool taCalled, ActiveVehicle activeVeh)
        {
            DLSModel dlsModel = activeVeh.Vehicle.GetDLS();
            activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(activeVeh.Vehicle);
            if (dlsModel.TrafficAdvisory.Type != "off")
            {
                if (!taCalled)
                {
                    List<LightStage> enableStages = new List<LightStage>();
                    List<LightStage> disableStages = new List<LightStage>();
                    if (dlsModel.TrafficAdvisory.AutoEnableStages != "")
                    {
                        foreach (int i in dlsModel.TrafficAdvisory.AutoEnableStages.Split(',').Select(n => int.Parse(n)).ToList())
                            enableStages.Add((LightStage)i);
                    }
                    if (dlsModel.TrafficAdvisory.AutoDisableStages != "")
                    {
                        foreach (int i in dlsModel.TrafficAdvisory.AutoDisableStages.Split(',').Select(n => int.Parse(n)).ToList())
                            disableStages.Add((LightStage)i);
                    }
                    if (enableStages.Contains(activeVeh.LightStage))
                    {
                        if (activeVeh.TAStage == TAStage.Off)
                        {
                            switch (dlsModel.TrafficAdvisory.DefaultEnabledDirection.ToLower())
                            {
                                case "left":
                                    activeVeh.TAStage = TAStage.Left;
                                    break;
                                case "diverge":
                                    activeVeh.TAStage = TAStage.Diverge;
                                    break;
                                case "right":
                                    activeVeh.TAStage = TAStage.Right;
                                    break;
                                case "warn":
                                    activeVeh.TAStage = TAStage.Warn;
                                    break;
                            }
                            activeVeh.AutoStartedTA = true;
                        }                        
                    }
                    if (disableStages.Contains(activeVeh.LightStage))
                        activeVeh.TAStage = TAStage.Off;
                }
                if (dlsModel.TrafficAdvisory.DivergeOnly.ToBoolean())
                {
                    if (activeVeh.TAStage != TAStage.Off)
                    {
                        switch (dlsModel.TrafficAdvisory.Type)
                        {
                            case "three":
                                foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00110000110000110000110000110000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "11000011000011000011000011000011";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00110000110000110000110000110000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                break;
                            case "four":
                                foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00110000110000110000110000110000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "11000011000011000011000011000000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;

                                }
                                foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "11000011000011000011000011000000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00110000110000110000110000110000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                break;
                            case "five":
                                foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00001100000011000000110000001100";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00110000001100000011000000110000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "11000000110000001100000011000000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00110000001100000011000000110000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00001100000011000000110000001100";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                break;
                            case "six":
                                foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00001100000011000000110000001100";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00110000001100000011000000110000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "11000000110000001100000011000000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "11000000110000001100000011000000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00110000001100000011000000110000";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                {
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00001100000011000000110000001100";
                                    activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                    if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (taCalled)
                        {
                            switch (dlsModel.TrafficAdvisory.Type)
                            {
                                case "three":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "four":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;

                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "five":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "six":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                            }
                        }
                        else
                            return;
                    }
                }
                else
                {
                    switch (activeVeh.TAStage)
                    {
                        case TAStage.Left:
                            switch (dlsModel.TrafficAdvisory.Type)
                            {
                                case "three":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Left.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Left.C;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Left.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "four":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Left.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Left.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Left.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Left.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "five":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Left.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Left.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Left.C;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Left.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Left.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "six":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;

                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.EL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;

                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;

                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;

                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.ER;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;

                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;

                                    }
                                    break;
                            }
                            break;
                        case TAStage.Diverge:
                            switch (dlsModel.TrafficAdvisory.Type)
                            {
                                case "three":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Diverge.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Diverge.C;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Diverge.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "four":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "five":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.C;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "six":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.EL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.ER;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                            }
                            break;
                        case TAStage.Right:
                            switch (dlsModel.TrafficAdvisory.Type)
                            {
                                case "three":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Right.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Right.C;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Right.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "four":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Right.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Right.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Right.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Right.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "five":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Right.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Right.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Right.C;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Right.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Right.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "six":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.EL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.ER;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                            }
                            break;
                        case TAStage.Warn:
                            switch (dlsModel.TrafficAdvisory.Type)
                            {
                                case "three":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Warn.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Warn.C;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Warn.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "four":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Warn.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Warn.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Warn.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Warn.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "five":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Warn.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Warn.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Warn.C;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Warn.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Warn.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                                case "six":
                                    foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.L;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.EL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.CL;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.CR;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.ER;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                    {
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.R;
                                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                        if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                    }
                                    break;
                            }
                            break;
                        case TAStage.Off:
                            if (taCalled)
                            {
                                switch (dlsModel.TrafficAdvisory.Type)
                                {
                                    case "three":
                                        foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        break;
                                    case "four":
                                        foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;

                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        break;
                                    case "five":
                                        foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        break;
                                    case "six":
                                        foreach (int i in dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        foreach (int i in dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList())
                                        {
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = "00000000000000000000000000000000";
                                            activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                                            if (dlsModel.TrafficAdvisory?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.TrafficAdvisory.Color.ColorValue;
                                        }
                                        break;
                                }
                            }
                            else
                                return;
                            if (!activeVeh.SBOn)
                                activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(activeVeh.Vehicle);
                            else
                            {
                                activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(activeVeh.Vehicle);
                                UpdateSB(activeVeh);
                            }
                            break;
                    }
                }
            }
        }
        public static void UpdateSB(ActiveVehicle activeVeh)
        {
            DLSModel dlsModel = activeVeh.Vehicle.GetDLS();
            if (dlsModel.SpecialModes.SteadyBurn.SteadyBurnEnabled.ToBoolean())
            {
                List<int> ssb = dlsModel.SpecialModes.SteadyBurn.Sirens.Replace(" ", "").Split(',').Select(n => int.Parse(n)).ToList();
                if (activeVeh.SBOn)
                {
                    foreach (int i in ssb)
                    {
                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].FlashinessSequence = dlsModel.SpecialModes.SteadyBurn.Pattern;
                        activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Flash = true;
                        if (dlsModel.SpecialModes.SteadyBurn?.Color?.ColorValue != null) activeVeh.Vehicle.EmergencyLightingOverride.Lights[i - 1].Color = dlsModel.SpecialModes.SteadyBurn.Color.ColorValue;
                    }
                }
                else
                {
                    activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(activeVeh.Vehicle);
                    if (activeVeh.TAStage != TAStage.Off)
                    {
                        UpdateTA(true, activeVeh);
                    }
                }
            }
        }
        public static void MoveUpStage(ActiveVehicle activeVeh)
        {
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
            activeVeh.LightStage = activeVeh.Vehicle.GetDLS().AvailableLightStages.Next(activeVeh.LightStage);
            Update(activeVeh);
        }
        public static void MoveDownStage(ActiveVehicle activeVeh)
        {
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
            activeVeh.LightStage = activeVeh.Vehicle.GetDLS().AvailableLightStages.Previous(activeVeh.LightStage);
            Update(activeVeh);
        }
        public static void MoveUpStageTA(ActiveVehicle activeVeh)
        {
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
            activeVeh.TAStage = activeVeh.Vehicle.GetDLS().AvailableTAStages.Next(activeVeh.TAStage);
            activeVeh.AutoStartedTA = false;
            Update(activeVeh);
        }
        public static void UpdateIndicator(ActiveVehicle activeVeh)
        {
            switch (activeVeh.IndStatus)
            {
                case IndStatus.Off:
                    NativeFunction.Natives.SET_VEHICLE_INDICATOR_LIGHTS(activeVeh.Vehicle, 0, false);
                    NativeFunction.Natives.SET_VEHICLE_INDICATOR_LIGHTS(activeVeh.Vehicle, 1, false);
                    break;
                case IndStatus.Left:
                    NativeFunction.Natives.SET_VEHICLE_INDICATOR_LIGHTS(activeVeh.Vehicle, 0, false);
                    NativeFunction.Natives.SET_VEHICLE_INDICATOR_LIGHTS(activeVeh.Vehicle, 1, true);
                    break;
                case IndStatus.Right:
                    NativeFunction.Natives.SET_VEHICLE_INDICATOR_LIGHTS(activeVeh.Vehicle, 0, true);
                    NativeFunction.Natives.SET_VEHICLE_INDICATOR_LIGHTS(activeVeh.Vehicle, 1, false);
                    break;
                case IndStatus.Both:
                    NativeFunction.Natives.SET_VEHICLE_INDICATOR_LIGHTS(activeVeh.Vehicle, 0, true);
                    NativeFunction.Natives.SET_VEHICLE_INDICATOR_LIGHTS(activeVeh.Vehicle, 1, true);
                    break;
            }
        }
        public static SirenStatus GetSirenStatus(ActiveVehicle activeVeh, int sirenID, bool includeBroken = true)
        {
            Vehicle v = activeVeh.Vehicle;
            string bone = "siren" + sirenID;
            if (v.HasBone(bone) && (includeBroken || v.GetBonePosition(bone).DistanceTo(Vector3.Zero) > 1))
            {
                float length = v.GetBoneOrientation(bone).LengthSquared();
                bool on = Math.Round(length, 2) != Math.Round(activeVeh.InitialLengths[sirenID - 1], 2);
                if (on)
                    return SirenStatus.On;
                else
                    return SirenStatus.Off;
            }
            else
                return SirenStatus.None;
        }
    }
}
