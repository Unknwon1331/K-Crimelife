using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace Crimelife
{
	public class OfferModel
	{

		public int categoryId
		{
			get;
			set;
		}

		public string name
		{
			get;
			set;
		}

		public bool search
		{
			get;
			set;
		}

		public Player phone
		{
			get;
			set;
		}

		public string price
		{
			get;
			set;
		}

		public string description
		{
			get;
			set;
		}

		public OfferModel(int categoryId, string name, bool search, Player phone, string price, string desc)
		{
			this.categoryId = categoryId;
			this.name = name;
			this.search = search;
			this.phone = phone;
			this.price = price;
			this.description = desc;
		}
	}
}
