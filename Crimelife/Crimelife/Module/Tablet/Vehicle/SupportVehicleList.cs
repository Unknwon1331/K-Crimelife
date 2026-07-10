using System.Collections.Generic;
using GTANetworkAPI;

namespace Crimelife
{
	internal class SupportVehicleList : Script
    {/*
		public SupportVehicleList()
			: base("SupportVehicleList")
		{
		}
		*/
	/*	[RemoteEvent]
		public async void requestSupportVehicleList(Player client, int owner)
		{
			DbPlayer player = client.GetPlayer();
			if (player != null && player.IsValid())
			{
				List<VehicleData> vehicleData = SupportVehicleFunctions.GetVehicleData(SupportVehicleFunctions.VehicleCategory.ALL, owner);
				string text = NAPI.Util.ToJson((object)vehicleData);
				client.TriggerEvent("responseVehicleList", text);
			}
		}*/
}
}