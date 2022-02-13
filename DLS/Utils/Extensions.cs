using Rage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DLS.Utils
{
    internal static class Extensions
    {
        internal static DLSModel GetDLS(this Vehicle veh)
        {
            if (!veh)
                return null;
            if (Entrypoint.DLSModelsDict.ContainsKey(veh.Model))
                return Entrypoint.DLSModelsDict[veh.Model];
            return null;
        }
        internal static ActiveVehicle GetActiveVehicle(this Vehicle veh, bool playerVehicle = false)
        {
            if (!veh)
                return null;
            for (int i = 0; i < Entrypoint.activeVehicles.Count; i++)
            {
                if (Entrypoint.activeVehicles[i].Vehicle == veh)
                    return Entrypoint.activeVehicles[i];
            }
            ActiveVehicle aVeh;
            if (veh.IsSirenOn)
            {
                if (!veh.IsSirenSilent)
                    aVeh = new ActiveVehicle(veh, playerVehicle, LightStage.Three, SirenStage.One);
                else
                    aVeh = new ActiveVehicle(veh, playerVehicle, LightStage.Three, SirenStage.Off);
            }
            else
                aVeh = new ActiveVehicle(veh, playerVehicle);
            if(aVeh != null) Entrypoint.activeVehicles.Add(aVeh);
            return aVeh;
        }
        internal static void ToLog(this string log)
        {
            if (Settings.SET_LOGTOCONSOLE) Game.LogTrivial(log);

            string path = @"Plugins/DLSv2.log";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine("[" + DateTime.Now.ToString() + "] " + log);
                writer.Close();
            }
        }
        internal static int ToInt32(this string text, [CallerMemberName] string callingMethod = null)
        {
            int i = 0;
            try
            {
                i = Convert.ToInt32(text);
            }
            catch (Exception e)
            {
                string message = "ERROR: " + e.Message + " ( " + text + " ) [" + callingMethod + "() -> " + MethodBase.GetCurrentMethod().Name + "()]";
                (message).ToLog();
                Game.LogTrivial(message);
            }
            return i;
        }
        internal static bool ToBoolean(this string text, [CallerMemberName] string callingMethod = null)
        {
            bool i = false;
            try
            {
                i = Convert.ToBoolean(text);
            }
            catch (Exception e)
            {
                string message = "ERROR: " + e.Message + " ( " + text + " ) [" + callingMethod + "() -> " + MethodBase.GetCurrentMethod().Name + "()]";
                (message).ToLog();
                Game.LogTrivial(message);
            }
            return i;
        }
        internal static bool IsPlayerVehicle(this Vehicle veh)
        {
            if (Game.LocalPlayer.Character.CurrentVehicle == veh || Game.LocalPlayer.Character.LastVehicle == veh || veh.Driver == Game.LocalPlayer.Character)
                return true;
            ActiveVehicle vehActive = veh.GetActiveVehicle();            
            foreach (ActiveVehicle activeVeh in Entrypoint.activeVehicles)
            {
                if (activeVeh == vehActive && activeVeh.PlayerVehicle)
                    return true;
            }
            return false;
        }
        internal static int GetIndexFromTAPatternName(this TAgroup taGroup, string name)
        {
            foreach (TApattern taP in taGroup.TaPatterns)
            {
                if (taP.Name == name)
                    return taGroup.TaPatterns.IndexOf(taP);
            }
            return 999;
        }
        internal static bool DoesVehicleHaveLightStage(this DLSModel dlsModel, LightStage lightStage)
        {
            return dlsModel.AvailableLightStages.Contains(lightStage);
        }
        internal static bool DoesVehicleHaveSirenStage(this DLSModel dlsModel, SirenStage sirenStage)
        {
            return dlsModel.AvailableSirenStages.Contains(sirenStage);
        }
        internal static T Next<T>(this List<T> list, T currentItem, int by = 1)
        {
            int index = list.IndexOf(currentItem);
            index = (index + by) % list.Count;
            return list[index];
        }
        internal static T Previous<T>(this List<T> list, T currentItem, int by = 1)
        {
            int index = list.IndexOf(currentItem);
            index = (list.Count + index - by) % list.Count;
            return list[index];
        }
        internal static SirenStage NextSirenStage(this List<SirenStage> list, SirenStage currentItem, bool includeFirst = true)
        {
            if (!list.Contains(currentItem))
                return currentItem;
            int index = list.IndexOf(currentItem) + 1;
            if (index > list.Count - 1)
            {
                if (includeFirst)
                    index = 0;
                else
                    index = 1;
            }
            return list[index];
        }
        public static List<List<T>> Chunk<T>(IEnumerable<T> data, int numArrays)
        {
            var size = data.Count() / numArrays;
            return data
                  .Select((x, i) => new { Index = i, Value = x })
                  .GroupBy(x => x.Index / size)
                  .Select(x => x.Select(v => v.Value).ToList())
                  .ToList();
        }
    }
}