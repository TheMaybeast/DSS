using DSS.Utils;
using Rage;
using Rage.Native;

namespace DSS.Threads
{
    internal class PlayerController
    {
        private static Vehicle _prevVehicle;
        private static ManagedVehicle _managedVehicle;
        public static bool ActvManu;
        public static bool ActvHorn;

        internal static void MainLoop()
        {
            while (true)
            {
                GameFiber.Yield();

                // Don't do anything if player is not in a vehicle
                var playerPed = Game.LocalPlayer.Character;
                if (!playerPed.IsInAnyVehicle(false)) continue;

                // Don't do anything if player is not the current driver of an existing and alive vehicle
                var veh = playerPed.CurrentVehicle;
                if (!veh || veh.IsDead || veh.GetPedOnSeat(-1) != playerPed) continue;

                // Disables control actions to avoid conflicting keys with vanilla game
                Game.DisableControlAction(0, (GameControl)Settings.CON_HZRD, true);
                Game.DisableControlAction(0, (GameControl)Settings.CON_INDRIGHT, true);
                Game.DisableControlAction(0, (GameControl)Settings.CON_INDLEFT, true);

                // Registers new ManagedVehicle
                if (_managedVehicle == null || _prevVehicle != veh)
                {
                    _managedVehicle = veh.GetManagedVehicle();
                    _prevVehicle = veh;
                    veh.IsInteriorLightOn = false;
                }

                // Adds Brake Light Functionality
                if (!_managedVehicle.Blackout && NativeFunction.Natives.IS_VEHICLE_STOPPED<bool>(veh))
                    NativeFunction.Natives.SET_VEHICLE_BRAKE_LIGHTS(veh, true);

                // Adds emergency lights/sirens functionality
                if (veh.HasSiren)
                {
                    Game.DisableControlAction(0, (GameControl)Settings.CON_TOGGLESIREN, true);
                    Game.DisableControlAction(0, (GameControl)Settings.CON_NEXTSIREN, true);
                    Game.DisableControlAction(0, (GameControl)Settings.CON_PREVSIREN, true);
                    Game.DisableControlAction(0, (GameControl)Settings.CON_AUXSIREN, true);
                    Game.DisableControlAction(0, (GameControl)Settings.CON_TOGGLELIGHTS, true);
                    Game.DisableControlAction(0, (GameControl)Settings.CON_INTLT, true);
                    Game.DisableControlAction(0, (GameControl)Settings.CON_HORN, true);

                    if (!Game.IsPaused)
                    {
                        // Toggle lighting
                        if(Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_TOGGLELIGHTS))
                        {
                            switch (veh.IsSirenOn)
                            {
                                case true:
                                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                    _managedVehicle.LightsOn = false;
                                    veh.IsSirenOn = false;
                                    _managedVehicle.SirenStage = SirenStage.Off;
                                    Sirens.Update(_managedVehicle);
                                    break;
                                case false:
                                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                    _managedVehicle.LightsOn = true;
                                    veh.IsSirenOn = true;
                                    veh.IsSirenSilent = true;
                                    break;
                            }
                        }

                        // Toggle Aux Siren
                        if(Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_AUXSIREN))
                        {
                            if (_managedVehicle.AuxOn)
                            {
                                Sound.ClearTempSoundID(_managedVehicle.AuxID);
                                _managedVehicle.AuxOn = false;
                            }
                            else
                            {
                                _managedVehicle.AuxID = Sound.TempSoundID();
                                _managedVehicle.AuxOn = true;
                                NativeFunction.Natives.PLAY_SOUND_FROM_ENTITY(_managedVehicle.AuxID,
                                    _managedVehicle.SoundSet == null
                                        ? "VEHICLES_HORNS_SIREN_1"
                                        : _managedVehicle.SoundSet.SirenTones[0], _managedVehicle.Vehicle, 0, 0, 0);
                            }
                        }

                        // Siren Switches
                        if(_managedVehicle.LightsOn)
                        {
                            if(Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_TOGGLESIREN))
                            {
                                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                                _managedVehicle.SirenStage = _managedVehicle.SirenStage == SirenStage.Off ? SirenStage.One : SirenStage.Off;
                                Sirens.Update(_managedVehicle);
                            }
                        }
                        if(_managedVehicle.SirenStage > SirenStage.Off)
                        {
                            // Move Up Siren Stage
                            if(Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_NEXTSIREN))
                                Sirens.MoveUpStage(_managedVehicle);
                            // Move Down Siren Stage
                            if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_PREVSIREN))
                                Sirens.MoveDownStage(_managedVehicle);
                        }

                        // Interior Light
                        if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_INTLT))
                        {
                            _managedVehicle.InteriorLight = !_managedVehicle.InteriorLight;
                            veh.IsInteriorLightOn = _managedVehicle.InteriorLight;
                        }

                        // Manual                                                              
                        if (_managedVehicle.SirenStage == SirenStage.Off)
                            ActvManu = Controls.IsDisabledControlPressed(0, (GameControl)Settings.CON_NEXTSIREN);
                        else
                            ActvManu = false;

                        // Horn
                        ActvHorn = Controls.IsDisabledControlPressed(0, (GameControl)Settings.CON_HORN);

                        // Manage Horn and Manual siren
                        int hman_state = 0;
                        if (ActvHorn && !ActvManu)
                            hman_state = 1;
                        else if (!ActvHorn && ActvManu)
                            hman_state = 2;
                        else if (ActvHorn && ActvManu)
                            hman_state = 3;

                        Sirens.SetAirManuState(_managedVehicle, hman_state);
                    }
                }

                // Indicators
                if (Game.IsPaused) continue;

                // Left Indicator
                if(Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_INDLEFT) && NativeFunction.Natives.xA571D46727E2B718<bool>(0))
                {
                    _managedVehicle.IndStatus =
                        _managedVehicle.IndStatus == VehicleIndicatorLightsStatus.LeftOnly
                            ? VehicleIndicatorLightsStatus.Off
                            : VehicleIndicatorLightsStatus.LeftOnly;
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                    veh.IndicatorLightsStatus = _managedVehicle.IndStatus;
                }

                // Right Indicator
                if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_INDRIGHT) && NativeFunction.Natives.xA571D46727E2B718<bool>(0))
                {
                    _managedVehicle.IndStatus =
                        _managedVehicle.IndStatus == VehicleIndicatorLightsStatus.RightOnly
                            ? VehicleIndicatorLightsStatus.Off
                            : VehicleIndicatorLightsStatus.RightOnly;
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                    veh.IndicatorLightsStatus = _managedVehicle.IndStatus;
                }

                // Hazards
                if (Controls.IsDisabledControlJustReleased(0, (GameControl)Settings.CON_HZRD) && NativeFunction.Natives.xA571D46727E2B718<bool>(0))
                {
                    _managedVehicle.IndStatus =
                        _managedVehicle.IndStatus == VehicleIndicatorLightsStatus.Both
                            ? VehicleIndicatorLightsStatus.Off
                            : VehicleIndicatorLightsStatus.Both;
                    NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, Settings.SET_AUDIONAME, Settings.SET_AUDIOREF, true);
                    veh.IndicatorLightsStatus = _managedVehicle.IndStatus;
                }
            }
        }
    }
}
