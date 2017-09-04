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
		public List<Slot> Slots { get; set; }
		public SortedDictionary<Slot, string> Inventory { get; set; }
		public List<Ability> Abilities { get; set; }
		public int MaxHitpoints { get; set; }
		public int MaxMana { get; set; }
		public int ManaRegen { get; set; }
		public int MaxStamina { get; set; }
		public Stat BaseStats { get; set; }
		public Resist BaseResists { get; set; }
		public AI AI { get; set; }
		public Row Row { get; set; }
		public int AC { get; set; }

		// Change during combat
		public int Hitpoints { get; set; }
		public int Mana { get; set; }
		public int Stamina { get; set; }
		public int Initiative { get; set; }
		public Stat Stats { get; set; }
		public Resist Resists { get; set; }
		public Boolean Alive { get; set; }
		public List<Effect> Effects { get; set; }

		public Character () { }

		public Character (string name, string raceName, List<string> items)
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
			this.Slots = race.Slots;
			this.Size = race.Size;
			this.BaseResists = race.Resists;
			this.Resists = race.Resists;
			this.BaseStats = race.Stats;
			this.Stats = race.Stats;
			this.PointCost += race.Points;
			this.Abilities = new List<Ability>();
			this.Inventory = new SortedDictionary<Slot, string>();
			this.MaxHitpoints = (race.Stats.Constitution * 4) + 50;
			this.MaxMana = (race.Stats.Intelligence * 2) + (race.Stats.Wisdom * 2) + 25;
			this.Mana = MaxMana;
			this.ManaRegen = (int)Math.Round((double)(race.Stats.Intelligence / 10));
			this.Alive = true;
			this.Hitpoints = this.MaxHitpoints;

			//TODO: load AI choice
			this.AI = new GeneralAI();

			foreach (var a in race.Abilities) 
			{
				Ability abil = LoadAbility(a);
				this.Abilities.Add(abil);
			}

			foreach (var item in items) 
			{
				yaml = File.ReadAllText(itemPath + item + ".yaml");
				deserializer = new DeserializerBuilder()
					.WithNamingConvention(new CamelCaseNamingConvention())
					.Build();
				var gear = deserializer.Deserialize<Item>(yaml);

				Boolean availableSlot = true;

				foreach (Slot s in gear.Slots) {
					if (!this.Slots.Contains(s)) {
						availableSlot = false;
					}
				}
				if (availableSlot) {
					foreach (var gs in gear.Slots) {
						this.Slots.Remove(gs);
						this.Inventory.Add(gs, item);
					}
					PointCost += gear.Points;
					AC += gear.AC;
					foreach (var a in gear.Abilities) {
						Ability abil = LoadAbility(a);
						if (!this.Abilities.Contains(abil))
							this.Abilities.Add(abil);
					}
				}
			}
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
			output += String.Format("Stats: {0}STR {1}CON {2}CHA {3}WIS {4}DEX {5}INT\n", this.Stats.Strength, this.Stats.Constitution, this.Stats.Charisma, this.Stats.Wisdom, this.Stats.Dexterity, this.Stats.Intelligence);
			output += String.Format("Resists: {0}MAG {1}COL {2}FIR {3}HOL {4}NEC {5}POI {6}PSI\n", this.Resists.Magic, this.Resists.Cold, this.Resists.Fire, this.Resists.Holy, this.Resists.Necrotic, this.Resists.Poison, this.Resists.Psionic);
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

