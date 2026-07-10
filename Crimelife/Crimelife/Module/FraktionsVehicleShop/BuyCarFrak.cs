using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife.Module.FraktionsVehicleShop
{
    public class BuyCarFrak
    {
        public string Vehicle_Name
        {
            get;
            set;
        }

        public int Price
        {
            get;
            set;
        }

        public BuyCarFrak(string vehicle_name, int price)
        {
            Vehicle_Name = vehicle_name;
            Price = price;
        }
    }
}
