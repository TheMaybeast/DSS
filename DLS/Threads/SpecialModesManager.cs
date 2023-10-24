using DLS.Utils;
using Rage;
using System;

namespace DLS.Threads
{
    class SpecialModesManager
    {
        public static void ProcessPlayer()
        {
            uint lastProcessTime = Game.GameTime;
            int timeBetweenChecks = 250;
            int yieldAfterChecks = 50;

            while (true)
            {
                int checksDone = 0;

                foreach (ActiveVehicle activeVeh in Entrypoint.activeVehicles)
                {
                    Vehicle veh = activeVeh.Vehicle;
                    DLSModel dlsModel;
                    if (veh)
                        dlsModel = veh.GetDLS();
                    else
                        dlsModel = null;

                    if (veh && veh.IsPlayerVehicle() && dlsModel != null)
                    {
                        if (activeVeh.LightStage != LightStage.Off && !veh.HasDriver && !activeVeh.IsOOV && dlsModel.SpecialModes.PresetSirenOnLeaveVehicle != "none")
                        {
                            if (!veh.IsEngineOn)
                                veh.IsEngineOn = true;
                            activeVeh.OOVBackupLightStage = activeVeh.LightStage;
                            activeVeh.IsOOV = true;
                            activeVeh.LightStage = dlsModel.OOVLightStage;
                            Lights.Update(activeVeh);
                        }
                        else if (veh.HasDriver && activeVeh.IsOOV)
                        {
                            activeVeh.LightStage = activeVeh.OOVBackupLightStage;
                            activeVeh.IsOOV = false;
                            Lights.Update(activeVeh);
                        }
                        else if (dlsModel.WailSetupEnabled)
                        {
                            if (!activeVeh.IsWailing && activeVeh.SirenStage == dlsModel.WailSirenStage)
                            {
                                activeVeh.WailBackupLightStage = activeVeh.LightStage;
                                activeVeh.IsWailing = true;
                                activeVeh.LightStage = dlsModel.WailLightStage;
                                Lights.Update(activeVeh);
                            }
                            else if (activeVeh.IsWailing && activeVeh.SirenStage != dlsModel.WailSirenStage)
                            {
                                activeVeh.IsWailing = false;
                                activeVeh.LightStage = activeVeh.WailBackupLightStage;
                                Lights.Update(activeVeh);
                            }   
                        }
                    }
                }

                checksDone++;
                if (checksDone % yieldAfterChecks == 0)
                {
                    GameFiber.Yield();
                }
                GameFiber.Sleep((int)Math.Max(timeBetweenChecks, Game.GameTime - lastProcessTime));
                lastProcessTime = Game.GameTime;
            }
        }
        public static void ProcessAI()
        {
            uint lastProcessTime = Game.GameTime;
            int timeBetweenChecks = 500;
            int yieldAfterChecks = 50;

            while (true)
            {
                int checksDone = 0;

                Vehicle[] allWorldVehicles = World.GetAllVehicles();
                foreach (Vehicle veh in allWorldVehicles)
                {
                    if (veh && veh.HasSiren && veh.GetDLS() != null && !veh.IsPlayerVehicle())
                    {
                        ActiveVehicle activeVeh = veh.GetActiveVehicle();
                        DLSModel dlsModel;
                        if (veh)
                            dlsModel = veh.GetDLS();
                        else
                            dlsModel = null;
                        if (veh.HasDriver && activeVeh.LightStage != LightStage.Three)
                        {
                            if (dlsModel.DoesVehicleHaveLightStage(LightStage.Three) && activeVeh.LightStage != LightStage.Three)
                            {
                                activeVeh.LightStage = LightStage.Three;
                                activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(veh);
                            }
                            else if (dlsModel.DoesVehicleHaveLightStage(LightStage.Three) && activeVeh.LightStage != LightStage.Two)
                            {
                                activeVeh.LightStage = LightStage.Two;
                                activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(veh);
                            }
                            else if (dlsModel.DoesVehicleHaveLightStage(LightStage.Three) && activeVeh.LightStage != LightStage.One)
                            {
                                activeVeh.LightStage = LightStage.One;
                                activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(veh);
                            }
                            else if (dlsModel.DoesVehicleHaveLightStage(LightStage.Three) && activeVeh.LightStage != LightStage.CustomTwo)
                            {
                                activeVeh.LightStage = LightStage.CustomTwo;
                                activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(veh);
                            }
                            else if (dlsModel.DoesVehicleHaveLightStage(LightStage.Three) && activeVeh.LightStage != LightStage.CustomOne)
                            {
                                activeVeh.LightStage = LightStage.CustomOne;
                                activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(veh);
                            }

                        }
                        else if (!veh.HasDriver && activeVeh.LightStage != dlsModel.OOVLightStage)
                        {
                            if (!veh.IsEngineOn)
                                veh.IsEngineOn = true;
                            activeVeh.LightStage = dlsModel.OOVLightStage;
                            activeVeh.Vehicle.EmergencyLightingOverride = Vehicles.GetEL(veh);
                        }
                    }
                }

                checksDone++;
                if (checksDone % yieldAfterChecks == 0)
                {
                    GameFiber.Yield();
                }
                GameFiber.Sleep((int)Math.Max(timeBetweenChecks, Game.GameTime - lastProcessTime));
                lastProcessTime = Game.GameTime;
            }
        }
    }
}

