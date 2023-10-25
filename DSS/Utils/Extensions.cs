using Rage;
using System;
using System.Collections.Generic;
using System.IO;

namespace DSS.Utils
{
    internal static class Extensions
    {
        internal static ActiveVehicle GetActiveVehicle(this Vehicle veh)
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
                aVeh = new ActiveVehicle(veh, true);
            else
                aVeh = new ActiveVehicle(veh);
            if(aVeh != null) Entrypoint.activeVehicles.Add(aVeh);
            return aVeh;
        }
        internal static void ToLog(this string log)
        {
            string path = @"Plugins/DSS.log";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine("[" + DateTime.Now.ToString() + "] " + log);
                writer.Close();
            }
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
    }
}