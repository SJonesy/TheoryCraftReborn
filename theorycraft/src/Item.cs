using System;
using System.Collections.Generic;

namespace theorycraft
{
	public class Item
	{
		public string Name { get; set; }
		public int Points { get; set; }
		public List<Slot> Slots { get; set; }
		public List<string> Abilities { get; set; }

		public Item ()
		{
		}
	}
}

