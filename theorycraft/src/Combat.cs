using System;
using System.Collections.Generic;

namespace theorycraft
{
	public class Combat
	{
		private List<Character> combatants;

		public Combat ()
		{
			combatants = new List<Character>();
		}

		public Party Duel(List<Party> parties) {
			// Initialize Combatants
			for (var i = 0; i < parties.Count; i++) {
				foreach (var character in parties[i].CharacterList) {
					character.PartyId = i;
					character.Hitpoints = character.MaxHitpoints;
					character.AI = new RandomAI();
					this.combatants.Add(character);
				}
			}

			Party winner = null;

			// Round
			while (winner == null) {
				Random rand = new Random();

				// Roll Initiative
				foreach (var combatant in combatants) {
					int maxInitiative = combatant.Stats.Dexterity * 5;
					combatant.Initiative = rand.Next(maxInitiative);
				}
				// Sort Combatants by Initiative 
				combatants.Sort();
				// Loop through combatants
				foreach (var combatant in combatants) {
					if (combatant.Hitpoints <= 0)
						continue;

					Party friendlyParty = null;
					Party hostileParty = null;
					for (var i = 0; i < parties.Count; i++) {
						if (i == combatant.PartyId)
							friendlyParty = parties[i];
						else
							hostileParty = parties[i];
					}

					// Do an ability
					Action action = combatant.AI.ChooseAction (combatant, friendlyParty, hostileParty);
					Console.WriteLine (action.TargetCharacter.Hitpoints);
					switch (action.Ability.Type) {
						case AbilityType.Melee:
							int minDamage = combatant.Stats.Strength; 
							int maxDamage = combatant.Stats.Strength + action.Ability.Damage ?? default(int);
							DoSingleDamage (action, minDamage, maxDamage, rand);
							break;
					}
					Console.WriteLine (action.TargetCharacter.Hitpoints);
				}

				// Check for victory
				int deadPartyCount = 0;
				List<Party> deadPartyList = new List<Party> ();
				foreach (var party in parties) {
					if (party.CharacterList.Find(x => x.Hitpoints > 0) == null) {
						deadPartyCount++;
						deadPartyList.Add(party);
					}
				}
				if (deadPartyCount == parties.Count - 1) {
					foreach (var party in parties) {
						if (deadPartyList.Contains(party))
						    continue;
						Console.WriteLine("The winner is" + party.Name);
						return party;
					}
				}
				if (deadPartyCount == parties.Count) {
					Console.WriteLine("Match is a draw");
					return new Party();
				}
			}

			return winner;
		}

		public Party Brawl(List<Party> parties) {
			// Return Winner
			return new Party ();
		}

		private void DoSingleDamage(Action action, int minDamage, int maxDamage, Random rand) {
			int damage = rand.Next(minDamage, maxDamage);
			action.TargetCharacter.Hitpoints -= damage;
			Console.ForegroundColor = GetColor(action.Ability.TextColor);
			if (action.Ability.BackgroundColor != null)
				Console.BackgroundColor = GetColor(action.Ability.BackgroundColor);
			string output = action.Ability.Text
				.Replace("@actor", action.Actor.Name)
				.Replace("@target", action.TargetCharacter.Name)
				.Replace("@damage", damage.ToString());
			Console.WriteLine(output);
			Console.ResetColor ();
		}

		private ConsoleColor GetColor(string color) {
			if (color == "gray" || color == "grey")
				return ConsoleColor.Gray;
			
			return ConsoleColor.White;
			//    All the foreground colors except DarkCyan, the background color:
			//       The foreground color is Black.
			//       The foreground color is DarkBlue.
			//       The foreground color is DarkGreen.
			//       The foreground color is DarkRed.
			//       The foreground color is DarkMagenta.
			//       The foreground color is DarkYellow.
			//       The foreground color is Gray.
			//       The foreground color is DarkGray.
			//       The foreground color is Blue.
			//       The foreground color is Green.
			//       The foreground color is Cyan.
			//       The foreground color is Red.
			//       The foreground color is Magenta.
			//       The foreground color is Yellow.
			//       The foreground color is White.
			//    
			//    All the background colors except White, the foreground color:
			//       The background color is Black.
			//       The background color is DarkBlue.
			//       The background color is DarkGreen.
			//       The background color is DarkCyan.
			//       The background color is DarkRed.
			//       The background color is DarkMagenta.
			//       The background color is DarkYellow.
			//       The background color is Gray.
			//       The background color is DarkGray.
			//       The background color is Blue.
			//       The background color is Green.
			//       The background color is Cyan.
			//       The background color is Red.
			//       The background color is Magenta.
			//       The background color is Yellow.
		}
	}
}
