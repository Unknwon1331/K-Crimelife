using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    public class Gangwar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Faction Faction { get; set; }
        public Vector3 Zone { get; set; }
        public Vector3 Blippos { get; set; }
        public float Range { get; set; }
        public Flag Flag1 { get; set; }
        public Flag Flag2 { get; set; }
        public Flag Flag3 { get; set; }
        public Flag Flag4 { get; set; }
        public Faction Attacker { get; set; } = null;
        public int FactionPoints { get; set; } = 0;
        public int AttackerPoints { get; set; } = 0;
        public DateTime StopDate { get; set; }
        public Vector3 AttackerSpawn { get; set; }
        public Vector3 DefenderSpawn { get; set; }
        public Vector3 AttackerVehicleSpawn { get; set; }
        public float AttackerVehicleRotation { get; set; }
        public Vector3 DefenderVehicleSpawn { get; set; }
        public float DefenderVehicleRotation { get; set; }
        public Vector3 AttackerParkoutPoint { get; set; }
        public Vector3 DefenderParkoutPoint { get; set; }

        public Gangwar() { }
    }
}
