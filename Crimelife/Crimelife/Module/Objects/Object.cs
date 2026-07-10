using GTANetworkAPI;
using GVMP.Handlers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Crimelife.Handlers;
using Crimelife.Types;


namespace Crimelife
{
    class ObjectModule : Crimelife.Module.Module<ObjectModule>
    {
        //Paintball
        GTANetworkAPI.Object skaterprop = NAPI.Object.CreateObject(3681122061, new Vector3(402.69, -648.03, 27.5), new Vector3(0, 0, 180.2123), 255, 0);


    }
}
//Ramon