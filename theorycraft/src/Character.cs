using System;
using System.IO;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace theorycraft
{
	public class Character : IComparable<Character>
	{
		// Fixed
		public string Name { get; set; }
		public string Race { get; set; }
		public int PointCost { get; set; }
		public int PartyId { get; set; }
		public Sizes Size { get; set;}
		public List<Ability> Abilities { get; set; }
		public int MaxHitpoints { get; set; }
		public int MaxMana { get; set; }
		public int ManaRegen { get; set; }
		public int MaxStamina { get; set; }
		public Dictionary<Stat, int> BaseStats { get; set; }
		public Dictionary<Resist, float> BaseResists { get; set; }
		public AI AI { get; set; }
		public Row Row { get; set; }
		public int AC { get; set; }

		// Change during combat
		public int Hitpoints { get; set; }
		public int Mana { get; set; }
		public int Stamina { get; set; }
		public int Initiative { get; set; }
		public Dictionary<Stat, int> Stats { get; set; }
		public Dictionary<Resist, float> Resists { get; set; }
		public Boolean Alive { get; set; }
		public List<Effect> Effects { get; set; }

		public Character () { }

		public Character (string name, string raceName, List<string> items, Row row)
		{
			var racePath = "data/races/";
			var itemPath = "data/items/";

			this.Name = name;

			raceName = raceName.ToLower();
			for (var i = 0; i < items.Count; i++) 
			{
				items[i] = items[i].ToLower().Replace (' ', '_');
			}

			var yaml = File.ReadAllText(racePath + raceName + ".yaml");
			var deserializer = new DeserializerBuilder()
				.WithNamingConvention(new CamelCaseNamingConvention())
				.Build();

			var race = deserializer.Deserialize<Race>(yaml);

			this.Race = race.Name;
			this.Size = race.Size;
			this.BaseStats = race.Stats;
			this.BaseResists = race.Resists;
			this.PointCost += race.Points;
			this.Abilities = new List<Ability>();
			this.Alive = true;
			this.Row = row;

			//TODO: load AI choice
			this.AI = new GeneralAI();

			foreach (var a in race.Abilities) 
			{
				Ability abil = LoadAbility(a);
				this.Abilities.Add(abil);
			}

			this.Resists = this.BaseResists;
			this.Stats = this.BaseStats;
			this.MaxHitpoints = (this.Stats[Stat.Constitution] * 4) + 50;
			this.Hitpoints = this.MaxHitpoints;
			this.MaxMana = (this.Stats[Stat.Intelligence] * 2) + (this.Stats[Stat.Wisdom] * 2) + 25;
			this.Mana = MaxMana;
			this.ManaRegen = (int)Math.Round((double)(this.Stats[Stat.Wisdom] / 10));
			this.AC += this.Stats[Stat.Dexterity] / 4;
		}

		private Ability LoadAbility(string abil) {
			var yaml = File.ReadAllText("data/abilities/" + abil + ".yaml");
			var deserializer = new DeserializerBuilder()
				.WithNamingConvention(new CamelCaseNamingConvention())
				.Build();

			var ability = deserializer.Deserialize<Ability>(yaml);

			return ability;
		}

		public string DisplayCharacter() {
			string output;
			output =  String.Format("Name: {0} \t Race: {1} \t Size {2} \t Point Cost: {3}\n", this.Name, this.Race, this.Size, this.PointCost);
			output += String.Format("Abilities: {0}\n", string.Join(", ", this.Abilities));
			output += String.Format("HP:{0}/{0} MP:{1}/{1} SP:{2}/{2}\n", this.MaxHitpoints, this.MaxMana, this.MaxStamina);
			output += String.Format("Stats: {0}STR {1}CON {2}CHA {3}WIS {4}DEX {5}INT\n", this.Stats[Stat.Strength], this.Stats[Stat.Constitution], this.Stats[Stat.Charisma], this.Stats[Stat.Wisdom], this.Stats[Stat.Dexterity], this.Stats[Stat.Intelligence]);
			output += String.Format("Resists: {0}MAG {1}COL {2}FIR {3}HOL {4}NEC {5}POI {6}PSI\n", this.Resists[Resist.Magic], this.Resists[Resist.Cold], this.Resists[Resist.Fire], this.Resists[Resist.Holy], this.Resists[Resist.Necrotic], this.Resists[Resist.Poison], this.Resists[Resist.Psionic]);
			output += String.Format("Equipment:\n");

			foreach (var kvp in this.Inventory) {
				output += String.Format("{0}\t{1}\n", kvp.Key, kvp.Value);
			}
			foreach (var slot in this.Slots) {
				output += String.Format ("{0}\t<unused>\n", slot);
			}

			return output;
		}

		public int CompareTo(Character compareCharacter)
		{
			if (this.Initiative > compareCharacter.Initiative)
				return -1;
			else if (this.Initiative < compareCharacter.Initiative)
				return 1;
			
			return 0;
		}

	}
}

