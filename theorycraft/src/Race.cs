using System;
using System.Collections.Generic;

namespace theorycraft
{
	public class Race
	{
		public string Name { get; set; }
		public int Points { get; set; }
		public Sizes Size { get; set;}
		public List<String> Traits { get; set; }
		public Dictionary<Stat, int> Stats { get; set; }
		public Dictionary<Resist, float> Resists { get; set; }
	}
}
