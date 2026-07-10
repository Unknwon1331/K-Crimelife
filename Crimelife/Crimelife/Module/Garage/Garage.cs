using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    public class Garage
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 CarPoint { get; set; }

        public Vector3 CarPoint2 { get; set; }

   

        public float Rotation { get; set; }

        public float Rotation2 { get; set; }

        public PedHash Npc { get; }


        public Garage(MySqlDataReader reader) //: (reader)
        {

            if (Position.X != 0 && Position.Y != 0)
            {
                Npc = (PedHash)(Enum.TryParse<PedHash>("a_f_y_business_01", true, out PedHash skin) ? skin : PedHash.Autoshop02SMM); //(PedHash)(Enum.TryParse<PedHash>("a_f_y_business_01")), true, out PedHash skin) ? skin : PedHash.Autoshop02SMM;
              //  new Npc((PedHash)(Enum.TryParse<PedHash>("a_f_y_business_01", ignoreCase: true, out PedHash result2) ? ((int)result2) : 664399832), garage.Position, 22, 0u);

            }
        }

        public Garage() { }
    }
}
