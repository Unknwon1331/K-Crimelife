using Crimelife;
using GTANetworkAPI;
using System.Collections.Generic;

namespace Crimelife
{
    public class DbVehicle
    {
        public Vehicle Vehicle { get; set; }

        public string Model { get; set; }


        public string Name { get; set; }

        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Plate { get; set; }

        public List<int> Keys { get; set; }

        public int PrimaryColor { get; set; }

        public int SecondaryColor { get; set; }

        public int PearlescentColor { get; set; }

        public int WindowTint { get; set; }

        public Dictionary<int, int> Tuning { get; set; }

        public bool Parked { get; set; }

        public long mileage { get; set; }

        public double Distance { get; set; }
        public float Rotation { get; set; }

        public bool Impound { get; set; }





        public VehicleData Data { get; set; }

        public double fuel { get; set; }

        public int InvRows { get; set; }

        public Faction Fraktion { get; set; } = null;

        public bool HasData(string key) => this.Vehicle.HasData(key);

        public void ResetData(string key) => this.Vehicle.ResetData(key);

        public object GetData(string key) => this.Vehicle.GetData<object>(key);

        public void SetData(string key, object value) => this.Vehicle.SetData(key, value);

        public void RefreshData(DbVehicle dbVehicle)
        {
            this.Vehicle.ResetData("vehicle");
            this.Vehicle.SetData("vehicle", dbVehicle);
        }

        public string Garage { get; set; }

        public DbVehicle() { }
    }
}
