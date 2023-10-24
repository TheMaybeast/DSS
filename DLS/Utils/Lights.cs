using Rage;
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
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Diverge.L);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Diverge.C);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Diverge.R);
                                break;
                            case "four":
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.L);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.CL);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.CR);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.R);
                                break;
                            case "five":
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.L);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.CL);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.C);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.CR);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.R);
                                break;
                            case "six":
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.L);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.EL);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.CL);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.CR);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.ER);
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.R);
                                break;
                        }
                    }
                    else
                    {
                        if (taCalled)
                            SetTASequence(activeVeh, dlsModel.TrafficAdvisory.Sirens.Split(',').Select(n => int.Parse(n)).ToList(), "00000000000000000000000000000000");
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
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Left.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Left.C);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Left.R);
                                    break;
                                case "four":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Left.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Left.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Left.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Left.R);
                                    break;
                                case "five":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Left.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Left.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Left.C);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Left.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Left.R);
                                    break;
                                case "six":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.EL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.ER);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Left.R);
                                    break;
                            }
                            break;
                        case TAStage.Diverge:
                            switch (dlsModel.TrafficAdvisory.Type)
                            {
                                case "three":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Diverge.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Diverge.C);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Diverge.R);
                                    break;
                                case "four":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Diverge.R);
                                    break;
                                case "five":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.C);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Diverge.R);
                                    break;
                                case "six":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.EL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.ER);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Diverge.R);
                                    break;
                            }
                            break;
                        case TAStage.Right:
                            switch (dlsModel.TrafficAdvisory.Type)
                            {
                                case "three":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Right.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Right.C);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Right.R);
                                    break;
                                case "four":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Right.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Right.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Right.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Right.R);
                                    break;
                                case "five":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Right.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Right.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Right.C);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Right.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Right.R);
                                    break;
                                case "six":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.EL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.ER);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Right.R);
                                    break;
                            }
                            break;
                        case TAStage.Warn:
                            switch (dlsModel.TrafficAdvisory.Type)
                            {
                                case "three":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Warn.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Warn.C);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Three.Warn.R);
                                    break;
                                case "four":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Warn.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Warn.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Warn.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Four.Warn.R);
                                    break;
                                case "five":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Warn.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Warn.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.c.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Warn.C);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Warn.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Five.Warn.R);
                                    break;
                                case "six":
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.l.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.L);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.el.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.EL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cl.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.CL);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.cr.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.CR);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.er.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.ER);
                                    SetTASequence(activeVeh, dlsModel.TrafficAdvisory.r.Split(',').Select(n => int.Parse(n)).ToList(), activeVeh.TAgroup.TaPatterns[activeVeh.TApatternCurrentIndex].Six.Warn.R);
                                    break;
                            }
                            break;
                        case TAStage.Off:
                            if (taCalled)
                                SetTASequence(activeVeh, dlsModel.TrafficAdvisory.Sirens.Split(',').Select(n => int.Parse(n)).ToList(), "00000000000000000000000000000000");
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

        private static void SetTASequence (ActiveVehicle aVeh, List<int> sirens, string sequence)
        {
            DLSModel dls = aVeh.Vehicle.GetDLS();

            foreach (int siren in sirens)
            {
                aVeh.Vehicle.EmergencyLightingOverride.Lights[siren - 1].FlashinessSequence = sequence;
                aVeh.Vehicle.EmergencyLightingOverride.Lights[siren - 1].Flash = true;
                if (dls.TrafficAdvisory?.Color?.ColorValue != null) aVeh.Vehicle.EmergencyLightingOverride.Lights[siren - 1].Color = dls.TrafficAdvisory.Color.ColorValue;
            }
        }
    }
}
