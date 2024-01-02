using Rage;
using System;
using System.Collections.Generic;
using System.IO;

namespace DSS.Utils
{
    internal static class Extensions
    {
        internal static ManagedVehicle GetManagedVehicle(this Vehicle veh)
        {
            if (!veh) return null;

            foreach (var managedVehicle in EntryPoint.ManagedVehicles)
            {
                if (managedVehicle.Vehicle == veh)
                    return managedVehicle;
            }

            var mVeh = veh.IsSirenOn ? new ManagedVehicle(veh, true) : new ManagedVehicle(veh);
            EntryPoint.ManagedVehicles.Add(mVeh);
            return mVeh;
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