using DLS.Utils;
using Rage;
using Rage.Native;

namespace DLS.Threads
{
    class PlayerController
    {
        private static Vehicle prevVehicle;
        private static ActiveVehicle activeVehicle;
        private static DLSModel dlsModel;
        private static bool isDLS = false;
        public static bool actv_manu;
        public static bool actv_horn;

        internal static void MainLoop()
        {
            while (true)
            {
                Ped playerPed = Game.LocalPlayer.Character;
                if (playerPed.IsInAnyVehicle(false))
                {
                    Vehicle veh = playerPed.CurrentVehicle;
                    // Inside here basic vehicle functionality will be available
                    // eg. Indicators and Internal Lights
                    if (veh.GetPedOnSeat(-1) == playerPed)
                    {
                        if (Settings.SET_INDENABLED)
                        {
                            if (Settings.SET_INDENABLED)
                            {
                                Game.DisableControlAction(0, (GameControl)Settings.CON_HZRD, true);
                                Game.DisableControlAction(0, (GameControl)Settings.CON_INDRIGHT, true);
                                Game.DisableControlAction(0, (GameControl)Settings.CON_INDLEFT, true);
                            }                            
                            Game.DisableControlAction(0, (GameControl)Settings.CON_BLKO, true);
                            if (Settings.SET_UIENABLED)
                                Game.DisableControlAction(0, (GameControl)Settings.UI_TOGGLE, true);
                        }

                        // Registers new Vehicle
                        if (activeVehicle == null || prevVehicle != veh)
                        {
                            activeVehicle = veh.GetActiveVehicle(true);

                            dlsModel = veh.GetDLS();
                            if (dlsModel == null)
                                isDLS = false;
                            else
                                isDLS = true;
                            prevVehicle = veh;
                            veh.IsInteriorLightOn = false;
                        }

                        // Adds Brake Light Functionality
                        if (!activeVehicle.Blackout && Settings.SET_BRAKELIGHT && NativeFunction.Natives.IS_VEHICLE_STOPPED<bool>(veh))
                            NativeFunction.Natives.SET_VEHICLE_BRAKE_LIGHTS(veh, true);

                        // Inside here additional DLS functionality will be available
                        if (veh.HasSiren && (isDLS || (!isDLS && Settings.SET_SCNDLS)))
                        {
                            Game.DisableControlAction(0, (GameControl)Settings.CON_TOGGLESIREN, true);
                            Game.DisableControlAction(0, (GameControl)Settings.CON_NEXTSIREN, true);
                            Game.DisableControlAction(0, (GameControl)Settings.CON_PREVSIREN, true);
                            Game.DisableControlAction(0, (GameControl)Settings.CON_AUXSIREN, true);
                            Game.DisableControlAction(0, (GameControl)Settings.CON_NEXTLIGHTS, true);
                            Game.DisableControlAction(0, (GameControl)Settings.CON_PREVLIGHTS, true);
                            Game.DisableControlAction(0, (GameControl)Settings.CON_HORN, true);
                            Game.DisableControlAction(0, (GameControl)Settings.CON_SB, true);


                            // Disables the vehicle's radio as a temporary fix.
                            // TODO: Figure a workaround.
                            NativeFunction.Natives.SET_VEHICLE_RADIO_ENABLED(veh, false);

                            if (!Game.IsPaused)
                            {
                                // Toggle UI
                                if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.UI_TOGGLE) && Settings.SET_UIENABLED)
                                    UIManager.IsUIOn = !UIManager.IsUIOn;

                                // (DLS) Move next stage
                                if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_NEXTLIGHTS) && isDLS)
                                    Lights.MoveUpStage(activeVehicle);

                                // (DLS) Move previous stage
                                if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_PREVLIGHTS) && isDLS)
                                    Lights.MoveDownStage(activeVehicle);

                                // (non-DLS) Toggle lighting
                                if(Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_NEXTLIGHTS) && !isDLS)
                                {
                                    switch (veh.IsSirenOn)
                                    {
                                        case true:
                                            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                            activeVehicle.LightStage = LightStage.Off;
                                            veh.IsSirenOn = false;
                                            activeVehicle.SirenStage = SirenStage.Off;
                                            Utils.Sirens.Update(activeVehicle);
                                            break;
                                        case false:
                                            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                            activeVehicle.LightStage = LightStage.Three;
                                            veh.IsSirenOn = true;
                                            veh.IsSirenSilent = true;
                                            break;
                                    }
                                }

                                // (DLS) Traffic Advisory
                                if(activeVehicle.TAType != "off" && 
                                    Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_TA) && isDLS)
                                    Lights.MoveUpStageTA(activeVehicle);

                                // Toggle Aux Siren
                                if(Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_AUXSIREN))
                                {
                                    if (activeVehicle.AuxOn)
                                    {
                                        Sound.ClearTempSoundID(activeVehicle.AuxID);
                                        activeVehicle.AuxOn = false;
                                    }
                                    else
                                    {
                                        activeVehicle.AuxID = Sound.TempSoundID();
                                        activeVehicle.AuxOn = true;
                                        NativeFunction.Natives.PLAY_SOUND_FROM_ENTITY(activeVehicle.AuxID, "VEHICLES_HORNS_SIREN_1", activeVehicle.Vehicle, 0, 0, 0);
                                    }
                                }

                                // Siren Switches
                                if(activeVehicle.LightStage == LightStage.Three)
                                {
                                    if(Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_TOGGLESIREN))
                                    {
                                        NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                        if (activeVehicle.SirenStage == SirenStage.Off)
                                            activeVehicle.SirenStage = SirenStage.One;
                                        else
                                            activeVehicle.SirenStage = SirenStage.Off;
                                        Utils.Sirens.Update(activeVehicle, isDLS);
                                    }
                                }
                                if(activeVehicle.SirenStage > SirenStage.Off)
                                {
                                    // Move Up Siren Stage
                                    if(Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_NEXTSIREN))
                                        Utils.Sirens.MoveUpStage(activeVehicle, isDLS, dlsModel);
                                    // Move Down Siren Stage
                                    if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_PREVSIREN))
                                        Utils.Sirens.MoveDownStage(activeVehicle, isDLS, dlsModel);
                                }

                                // (DLS) Steady Burn
                                if(Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_SB) && isDLS &&
                                    dlsModel.SpecialModes.SteadyBurn.SteadyBurnEnabled.ToBoolean())
                                {
                                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                    activeVehicle.SBOn = !activeVehicle.SBOn;
                                    Lights.Update(activeVehicle);
                                }

                                // Blackout
                                if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_BLKO))
                                {
                                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                    activeVehicle.Blackout = !activeVehicle.Blackout;
                                    if (activeVehicle.Blackout)
                                    {
                                        activeVehicle.IndStatus = IndStatus.Off;
                                        activeVehicle.LightStage = LightStage.Off;
                                        activeVehicle.SBOn = false;
                                        activeVehicle.TAStage = TAStage.Off;
                                        Lights.Update(activeVehicle);
                                        Lights.UpdateIndicator(activeVehicle);
                                        NativeFunction.Natives.SET_VEHICLE_LIGHTS(veh, 1);
                                    }
                                    else
                                        NativeFunction.Natives.SET_VEHICLE_LIGHTS(veh, 0);
                                }

                                // Interior Light
                                if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_INTLT))
                                {
                                    activeVehicle.InteriorLight = !activeVehicle.InteriorLight;
                                    veh.IsInteriorLightOn = activeVehicle.InteriorLight;
                                }
                                    

                                // Manual                                                              
                                if (activeVehicle.SirenStage == SirenStage.Off)
                                {
                                    if (Controls.IsDisabledControlPressed(0, (GameControl)Settings.CON_NEXTSIREN))
                                        actv_manu = true;
                                    else
                                        actv_manu = false;
                                }
                                else
                                    actv_manu = false;

                                // Horn
                                if (Controls.IsDisabledControlPressed(0, (GameControl)Settings.CON_HORN))
                                    actv_horn = true;
                                else
                                    actv_horn = false;

                                // Manage Horn and Manual siren
                                int hman_state = 0;
                                if (actv_horn && !actv_manu)
                                    hman_state = 1;
                                else if (!actv_horn && actv_manu)
                                    hman_state = 2;
                                else if (actv_horn && actv_manu)
                                    hman_state = 3;

                                Utils.Sirens.SetAirManuState(activeVehicle, isDLS, hman_state);
                            }
                        }

                        // Indicators
                        if (Settings.SET_INDENABLED && !Game.IsPaused)
                        {
                            // Left Indicator
                            if(Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_INDLEFT) && NativeFunction.Natives.xA571D46727E2B718<bool>(0))
                            {
                                if (activeVehicle.IndStatus == IndStatus.Left)
                                {
                                    activeVehicle.IndStatus = IndStatus.Off;
                                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                }
                                else
                                {
                                    activeVehicle.IndStatus = IndStatus.Left;
                                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                }
                                Lights.UpdateIndicator(activeVehicle);
                            }
                            // Right Indicator
                            if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_INDRIGHT) && NativeFunction.Natives.xA571D46727E2B718<bool>(0))
                            {
                                if (activeVehicle.IndStatus == IndStatus.Right)
                                {
                                    activeVehicle.IndStatus = IndStatus.Off;
                                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                }
                                else
                                {
                                    activeVehicle.IndStatus = IndStatus.Right;
                                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                }
                                Lights.UpdateIndicator(activeVehicle);
                            }
                            // Hazards
                            if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_HZRD) && NativeFunction.Natives.xA571D46727E2B718<bool>(0))
                            {
                                if (activeVehicle.IndStatus == IndStatus.Both)
                                {
                                    activeVehicle.IndStatus = IndStatus.Off;
                                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                }
                                else
                                {
                                    activeVehicle.IndStatus = IndStatus.Both;
                                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                }
                                Lights.UpdateIndicator(activeVehicle);
                            }
                        }
                    }
                }
                GameFiber.Yield();
            }
        }
    }
}
