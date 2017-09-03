using System;
using System.Collections.Generic;

namespace theorycraft
{
	public class Race
	{
		public string Name { get; set; }
		public int Points { get; set; }
		public Sizes Size { get; set;}
		public List<Slot> Slots { get; set; }
		public List<String> Abilities { get; set; }
		public Stat Stats { get; set; }
		public Resist Resists { get; set; }
	}
}
