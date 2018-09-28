using System;
using YamlDotNet.Serialization;

namespace theorycraft
{
	public class Trait
	{
		public string Name { get; set; }
		public int Cooldown { get; set; }
		public int Mana { get; set; }
        public int PointCost { get; set; }
		public TraitType Type { get; set; }
        public Stat Stat { get; set; }
		public int Power { get; set; }
		public Effect[] Effects { get; set; }
		public string Text { get; set; }
		public string TextColor { get; set; }
		public string BackgroundColor { get; set; }
		public bool Beneficial { get; set; }
		public bool PartyTarget { get; set; }
		public Resist ResistType { get; set; }

		public override string ToString ()
		{
			return Name;
		}

	}
}

