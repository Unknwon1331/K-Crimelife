using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
	internal class TattooList
	{
		[JsonProperty("Collection")]
		public uint collection { get; set; }

		[JsonProperty("Overlay")]
		public uint overlay { get; set; }

	}

}
