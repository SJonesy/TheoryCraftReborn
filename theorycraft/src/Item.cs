using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace theorycraft
{
	public class Item
	{
		public string Name { get; set; }
		public int Points { get; set; }
		[YamlMember(Alias = "ac", ApplyNamingConventions = false)]
		public int AC { get; set; }
		public List<Slot> Slots { get; set; }
		public List<string> Abilities { get; set; }
		public Dictionary<Resist, float> Resists { get; set; }
		public Dictionary<Stat, int> Stats { get; set; }

		public Item ()
		{
		}
	}
}

