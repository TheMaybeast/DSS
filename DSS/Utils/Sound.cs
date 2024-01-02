using Rage.Native;

namespace DSS.Utils
{
    class Sound
    {
        public static int NewSoundID(ManagedVehicle activeVeh)
        {
            if (activeVeh.SoundId != 999)
            {
                NativeFunction.Natives.STOP_SOUND(activeVeh.SoundId);
                NativeFunction.Natives.RELEASE_SOUND_ID(activeVeh.SoundId);
                EntryPoint.UsedSoundIDs.Remove(activeVeh.SoundId);
            }
            int newID = NativeFunction.Natives.GET_SOUND_ID<int>();
            activeVeh.SoundId = newID;
            EntryPoint.UsedSoundIDs.Add(newID);
            return newID;
        }

        public static int TempSoundID()
        {
            int newID = NativeFunction.Natives.GET_SOUND_ID<int>();
            EntryPoint.UsedSoundIDs.Add(newID);
            return newID;
        }

        public static void ClearTempSoundID(int id)
        {
            NativeFunction.Natives.STOP_SOUND(id);
            NativeFunction.Natives.RELEASE_SOUND_ID(id);
            EntryPoint.UsedSoundIDs.Remove(id);
        }
    }
}
