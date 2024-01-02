using DSS.Threads;
using DSS.Utils;
using Rage;
using Rage.Attributes;
using Rage.Native;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

[assembly: Plugin("Dynamic Siren System", Description = "Lightweight Siren Control", Author = "TheMaybeast", PrefersSingleInstance = true, ShouldTickInPauseMenu = false, SupportUrl = "https://discord.gg/HUcXxkq")]
namespace DSS
{
    internal class EntryPoint
    {
        //Vehicles currently being managed by DSS
        public static List<ManagedVehicle> ManagedVehicles = new List<ManagedVehicle>();
        //List of used Sound IDs
        public static List<int> UsedSoundIDs = new List<int>();
        // List of Siren Sets
        public static Dictionary<string, SoundSet> SirenSets = new Dictionary<string, SoundSet>();

        //If DSS is on Key Lock method
        public static bool keysLocked = false;

        public static void Main()
        {
            //Initiates Log File
            _ = new Log();

            // Checks if .ini file is created.
            Settings.IniCheck();

            //Version check and logging.
            FileVersionInfo rphVer = FileVersionInfo.GetVersionInfo("ragepluginhook.exe");
            Game.LogTrivial("Detected RPH " + rphVer.FileVersion);
            if (rphVer.FileMinorPart < 78)
            {
                Game.LogTrivial("RPH 78+ is required to use this mod");
                "ERROR: RPH 78+ is required but not found".ToLog();
                Game.DisplayNotification($"~y~Unable to load DSS~w~\nRagePluginHook version ~b~78~w~ or later is required, you are on version ~b~{rphVer.FileMinorPart}");
                return;
            }
            AssemblyName pluginInfo = Assembly.GetExecutingAssembly().GetName();
            Game.LogTrivial($"LOADED DSS v{pluginInfo.Version}");

            //Loads SirenSets
            SirenSets = Sirens.GetSirenSets();

            //Creates player controller
            "Loading: DSS - Player Controller".ToLog();
            GameFiber.StartNew(PlayerController.MainLoop, "DSS - Player Controller");
            "Loaded: DSS - Player Controller".ToLog();
        }

        private static void OnUnload(bool isTerminating)
        {
            "Unloading DSS".ToLog();
            if (UsedSoundIDs.Count > 0)
            {
                "Unloading used SoundIDs".ToLog();
                foreach (int id in UsedSoundIDs)
                {
                    NativeFunction.Natives.STOP_SOUND(id);
                    NativeFunction.Natives.RELEASE_SOUND_ID(id);
                    ("Unloaded SoundID " + id).ToLog();
                }
                "Unloaded all used SoundIDs".ToLog();
            }
            if (ManagedVehicles.Count > 0)
            {
                "Refreshing vehicle's default EL".ToLog();
                foreach (ManagedVehicle mVeh in ManagedVehicles)
                {
                    if (mVeh.Vehicle)
                    {
                        mVeh.Vehicle.IsSirenOn = false;
                        mVeh.Vehicle.IsSirenSilent = false;
                        mVeh.Vehicle.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Off;
                        ("Refreshed " + mVeh.Vehicle.Handle).ToLog();
                    }
                    else
                        ("Vehicle does not exist anymore!").ToLog();
                }
                "Refreshed vehicle's default EL".ToLog();
            }
        }
    }
}