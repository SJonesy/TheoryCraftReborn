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
		public List<Trait> Traits { get; set; }
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
        public List<Buff> Buffs { get; set; }

		public Character () { }

		public Character (string name, string raceName, Row row, List<String> traits)
		{
			var racePath = "data/races/";

			this.Name = name;

			raceName = raceName.ToLower();

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
			this.Traits = new List<Trait>();
            this.Effects = new List<Effect>();
            this.Buffs = new List<Buff>();
			this.Alive = true;
			this.Row = row;
            this.ManaRegen = 0;

			//TODO: load AI choice
			this.AI = new GeneralAI();

			foreach (var t in race.Traits) 
			{
				Trait trait = LoadTrait(t);
				this.Traits.Add(trait);
			}

            foreach (var t in traits) {
                Trait trait = LoadTrait(t);
                this.Traits.Add(trait);
                this.PointCost += trait.PointCost;
            }

			this.Resists = this.BaseResists;
			this.Stats = this.BaseStats;

            foreach (var t in this.Traits)
			{
				if (t.Type != TraitType.StatChange)
					continue;

				if (t.Stat == Stat.AC)
					this.AC += t.Power;

				if (this.Stats.ContainsKey(t.Stat))
					this.Stats[t.Stat] += t.Power;
			}

			this.MaxHitpoints = (this.Stats[Stat.Constitution] * 4) + 50;
			this.Hitpoints = this.MaxHitpoints;
			this.MaxMana = (this.Stats[Stat.Intelligence] * 2) + (this.Stats[Stat.Wisdom] * 2) + 25;
			this.Mana = MaxMana;
			this.ManaRegen += (int)Math.Round((double)(this.Stats[Stat.Wisdom] / 10));
			this.AC += this.Stats[Stat.Dexterity] / 4;
		}

        private Trait LoadTrait(String t) {
            var yaml = File.ReadAllText("data/traits/" + t + ".yaml");
			var deserializer = new DeserializerBuilder()
				.WithNamingConvention(new CamelCaseNamingConvention())
				.Build();

			var trait = deserializer.Deserialize<Trait>(yaml);

			return trait;
		}

		public string DisplayCharacter() {
			string output;
			output =  String.Format("Name: {0} \t Race: {1} \t Size {2} \t Point Cost: {3}\n", this.Name, this.Race, this.Size, this.PointCost);
			output += String.Format("HP:{0}/{0} MP:{1}/{1} SP:{2}/{2}\n", this.MaxHitpoints, this.MaxMana, this.MaxStamina);
			output += String.Format("Stats: {0}STR {1}CON {2}CHA {3}WIS {4}DEX {5}INT\n", this.Stats[Stat.Strength], this.Stats[Stat.Constitution], this.Stats[Stat.Charisma], this.Stats[Stat.Wisdom], this.Stats[Stat.Dexterity], this.Stats[Stat.Intelligence]);
			output += String.Format("Resists: {0}MAG {1}COL {2}FIR {3}HOL {4}NEC {5}POI {6}PSI\n", this.Resists[Resist.Magic], this.Resists[Resist.Cold], this.Resists[Resist.Fire], this.Resists[Resist.Holy], this.Resists[Resist.Necrotic], this.Resists[Resist.Poison], this.Resists[Resist.Psionic]);
			output += String.Format("Traits: {0}\n", string.Join(", ", this.Traits));

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

