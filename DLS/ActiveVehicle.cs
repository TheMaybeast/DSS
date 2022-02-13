using DLS.Utils;
using Rage;

namespace DLS
{
    public class ActiveVehicle
    {
        public ActiveVehicle(Vehicle vehicle, bool playerVehicle = false, LightStage lightStage = LightStage.Off, SirenStage sirenStage = SirenStage.Off)
        {
            Vehicle = vehicle;
            LightStage = lightStage;
            SirenStage = sirenStage;
            PlayerVehicle = playerVehicle;
            DefaultEL = vehicle.EmergencyLighting;
            IsSirenSilent = vehicle.IsSirenSilent;
            if (vehicle && vehicle.GetDLS() != null)
            {
                bool temp = vehicle.IsSirenOn;
                vehicle.IsSirenOn = false;
                InitialLengths = new float[20];
                GameFiber.Yield();
                for (int i = 0; i < InitialLengths.Length; i++)
                {
                    string bone = "siren" + (i + 1);
                    if (vehicle.HasBone(bone))
                    {
                        InitialLengths[i] = vehicle.GetBoneOrientation(bone).LengthSquared();
                    }
                    else
                    {
                        InitialLengths[i] = 1f;
                    }
                }
                vehicle.IsSirenOn = temp;
                DLSModel vehDLS;
                if (vehicle)
                    vehDLS = vehicle.GetDLS();
                else
                    vehDLS = null;
                TAType = vehDLS.TrafficAdvisory.Type;
                if (TAType != "off")
                {
                    TAgroup = Entrypoint.tagroups[vehDLS.TrafficAdvisory.TAgroup];
                    TApatternCurrentIndex = Entrypoint.tagroups[vehDLS.TrafficAdvisory.TAgroup].GetIndexFromTAPatternName(vehDLS.TrafficAdvisory.DefaultTApattern);
                }
                vehicle.EmergencyLightingOverride = Vehicles.GetEL(vehicle, this);
            }
        }
        public Vehicle Vehicle { get; set; }
        public LightStage LightStage { get; set; }
        public string TAType { get; set; } = "off";
        public SirenStage SirenStage { get; set; }
        public TAStage TAStage { get; set; } = TAStage.Off;
        public bool SBOn { get; set; } = false;
        public bool AuxOn { get; set; } = false;
        public bool PlayerVehicle { get; set; }
        public int AuxID { get; set; } = 999;
        public int SoundId { get; set; } = 999;
        public IndStatus IndStatus { get; set; } = IndStatus.Off;
        public TAgroup TAgroup { get; set; } = null;
        public int TApatternCurrentIndex { get; set; } = 999;
        public uint CurrentHash { get; set; } = 0;
        public float[] InitialLengths { get; set; }
        public EmergencyLighting DefaultEL { get; set; }
        public bool IsSirenSilent { get; set; }
        public bool IsOOV { get; set; } = false;
        public LightStage OOVBackupLightStage { get; set; }
        public bool IsWailing { get; set; } = false;
        public LightStage WailBackupLightStage { get; set; }
        public int? AirManuState { get; set; } = null;
        public int? AirManuID { get; set; } = null;
        public bool AutoStartedTA { get; set; } = false;
    }

    public enum LightStage
    {
        Off,
        One,
        Two,
        Three,
        CustomOne,
        CustomTwo,
        Empty
    }

    public enum SirenStage
    {
        Horn = -1,
        Off,
        One,
        Two,
        Warning,
        Warning2        
    }

    public enum TAStage
    {
        Off,
        Left,
        Diverge,
        Right,
        Warn
    }

    public enum IndStatus
    {
        Left,
        Right,
        Both,
        Off
    }

    public enum SirenStatus
    {
        On,
        Off,
        None
    }
}
