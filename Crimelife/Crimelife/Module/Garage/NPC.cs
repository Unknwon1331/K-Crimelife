using GTANetworkAPI;

namespace Crimelife
{
	public class Npc
	{
		public PedHash PedHash { get; set; }

		public Vector3 Position { get; set; }

		public float Heading { get; set; }

		public uint Dimension { get; set; }

		public Npc(PedHash pedHash, Vector3 position, float heading, uint dimension)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			PedHash = pedHash;
			Position = position;
			Heading = heading;
			Dimension = dimension;
			Main.ServerNpcs.Add(this);
			foreach (Player target in NAPI.Pools.GetAllPlayers())
			{
				if (target == null || target.Exists) continue;
				DbPlayer validPlayer = target.GetPlayer();
				validPlayer.player.TriggerEvent("loadNpc", new object[6]
				{
					PedHash,
					Position.X,
					Position.Y,
					Position.Z,
					Heading,
					Dimension
				});
			}
		}
	}
}
