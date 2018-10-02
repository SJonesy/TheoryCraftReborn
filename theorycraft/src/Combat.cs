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
			string output = "";
			int longestString = 0;
			for (var i = 0; i < parties.Count; i++) {
				foreach (var character in parties[i].CharacterList) {
					character.PartyId = i;
					this.combatants.Add(character);
				}
				string currentString = String.Format("{0}: total point cost is {1}\n", parties[i].Name, parties[i].PointCost());
				if (currentString.Length > longestString)
					longestString = currentString.Length;
				output += currentString;
			}
			Console.WriteLine(new String('=', longestString));
			Console.Write(output);
			Console.WriteLine(new String('=', longestString));

			// Round
			Party winner = null;
			int roundCount = 1;
			while (winner == null) {
				Random rand = new Random();
				Console.WriteLine ("*** ROUND " + roundCount.ToString() + " ***");
				// Roll Initiative
				foreach (var combatant in combatants) {
					int maxInitiative = combatant.Stats[Stat.Dexterity] * 5;
					combatant.Initiative = rand.Next(maxInitiative);
				}
				// Sort Combatants by Initiative 
				combatants.Sort();
				// Loop through combatants
				foreach (var combatant in combatants) {
					if (!combatant.Alive)
						continue;

                    if (combatant.Effects.Contains(Effect.Poison)) {
                        Trait poisonDamage = Trait.GetTrait("poison_damage");
                        Action poisonAction = new Action(combatant, combatant, poisonDamage);
                        int minDamage = 1;
                        int maxDamage = poisonAction.Trait.Power;
                        DoSingleDamage(poisonAction, minDamage, maxDamage, rand);
                    }

					if (!combatant.Alive)
						continue;

					if (combatant.Mana < combatant.MaxMana)
						combatant.Mana += combatant.ManaRegen;

					Party friendlyParty = null;
					Party hostileParty = null;
					for (var i = 0; i < parties.Count; i++) {
						if (i == combatant.PartyId)
							friendlyParty = parties[i];
						else
							hostileParty = parties[i];
					}

					// Do an action
					Action action = combatant.AI.ChooseAction (combatant, friendlyParty, hostileParty);

					if (action == null)
						continue;

					if (action.Trait.Mana > 0)
						action.Actor.Mana -= action.Trait.Mana;

					switch (action.Trait.Type) {
						case TraitType.Melee:
							int minDamage = combatant.Stats[Stat.Strength]; 
							int maxDamage = combatant.Stats[Stat.Strength] + action.Trait.Power;
							DoSingleDamage (action, minDamage, maxDamage, rand);
							break;
						case TraitType.DirectDamage:
							minDamage = action.Trait.Power;
							maxDamage = combatant.Stats[Stat.Intelligence] + action.Trait.Power;
							DoSingleDamage (action, minDamage, maxDamage, rand);
							break;
						case TraitType.DirectHealing:
							int minHeal = action.Trait.Power;
							int maxHeal = combatant.Stats[Stat.Wisdom] + action.Trait.Power;
							DoSingleHealing (action, minHeal, maxHeal, rand);
							break;
						case TraitType.GroupDamage:
							foreach (Character target in hostileParty.CharacterList) {
								Action targetedAction = new Action (action.Actor, target, action.Trait);
								minDamage = (combatant.Stats [Stat.Intelligence] / 4);
								maxDamage = combatant.Stats [Stat.Intelligence] + action.Trait.Power;
								DoSingleDamage (targetedAction, minDamage, maxDamage, rand);
							}
							break;
                        case TraitType.GroupHealing:
                            foreach (Character target in friendlyParty.CharacterList) {
                                if (!target.Alive)
                                    continue;
                                Action targetedAction = new Action(action.Actor, target, action.Trait);
								minHeal = action.Trait.Power;
								maxHeal = combatant.Stats[Stat.Wisdom] + action.Trait.Power;
                                DoSingleHealing (targetedAction, minHeal, maxHeal, rand);
                            }
                            break;
                        case TraitType.GroupBuff:
                            foreach (Character target in friendlyParty.CharacterList) {
                                Action targetedAction = new Action(action.Actor, target, action.Trait);
                                DoSingleBuff(targetedAction);
                            }
                            break;
					}

					// Check for victory
					List<Party> deadPartyList = new List<Party> ();
					foreach (var party in parties) {
						if (party.CharacterList.Find(x => x.Hitpoints > 0) == null) {
							deadPartyList.Add(party);
						}
					}
					if (deadPartyList.Count == parties.Count - 1) {
						foreach (var party in parties) {
							if (deadPartyList.Contains(party))
								continue;
							Console.ForegroundColor = ConsoleColor.Green;
							Console.BackgroundColor = ConsoleColor.White;
							Console.Write("The winner is " + party.Name);
							Console.ResetColor();
							Console.Write("\n");
							winner = party;
							DisplayMatchup(parties);
							return winner;
						}
					}
					else if (deadPartyList.Count == parties.Count) {
						Console.WriteLine("The match is a draw");
						DisplayMatchup(parties);
						winner = new Party();
						return winner;
					}
				}

				DisplayMatchup(parties);

				roundCount++;
				// TODO: make Round Limit configurable
				if (roundCount == 500) {
					Console.WriteLine ("Round limit hit");
					return new Party ();
				}
			}

			return winner;
		}

		public Party Brawl(List<Party> parties) {
			// Return Winner
			return new Party ();
		}

        private void DoSingleBuff(Action action) {
            foreach (Buff buff in action.Trait.Buffs) {
                if (!action.TargetCharacter.Buffs.Contains(buff)) {
                    action.TargetCharacter.Buffs.Add(buff);
                }
            }

			Console.ForegroundColor = GetColor(action.Trait.TextColor);
			if (action.Trait.BackgroundColor != null)
				Console.BackgroundColor = GetColor(action.Trait.BackgroundColor);
			string output = action.Trait.Text
				.Replace("@actor", action.Actor.Name)
				.Replace("@target", action.TargetCharacter.Name);
			Console.Write(output);
			Console.ResetColor();
			Console.Write("\n");
        }

		private void DoSingleDamage(Action action, int minDamage, int maxDamage, Random rand) {
			int damage = rand.Next(minDamage, maxDamage);
			if (action.Trait.Type == TraitType.Melee)
				damage -= action.TargetCharacter.AC;
			if (action.Trait.Type == TraitType.DirectDamage) {
				float resist;
				action.TargetCharacter.Resists.TryGetValue(action.Trait.ResistType, out resist);
				damage -= (int)(resist * damage);
			}
            if (damage <= 0) {
                damage = 1;
            }
            else if (action.Actor.Buffs.Contains(Buff.PoisonedWeapon) && !action.TargetCharacter.Effects.Contains(Effect.Poison)) {
                action.TargetCharacter.Effects.Add(Effect.Poison);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("{0} has been poisoned by {1}.", action.TargetCharacter.Name, action.Actor.Name);
				Console.ResetColor();
				Console.Write("\n");
                action.Actor.Buffs.Remove(Buff.PoisonedWeapon);
            }
			action.TargetCharacter.Hitpoints -= damage;
			Console.ForegroundColor = GetColor(action.Trait.TextColor);
			if (action.Trait.BackgroundColor != null)
				Console.BackgroundColor = GetColor(action.Trait.BackgroundColor);
			string output = action.Trait.Text
				.Replace("@actor", action.Actor.Name)
				.Replace("@target", action.TargetCharacter.Name)
				.Replace("@damage", damage.ToString());
			Console.Write(output);
			Console.ResetColor();
			Console.Write("\n");
         
            if (action.TargetCharacter.Hitpoints <= 0) {
				action.TargetCharacter.Alive = false;
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine("{0} has died.", action.TargetCharacter.Name);
				Console.ResetColor();
			}
		}

		private void DoSingleHealing(Action action, int minHealing, int maxHealing, Random rand) {
			int healing = rand.Next(minHealing, maxHealing);
            if (action.TargetCharacter.Effects.Contains(Effect.Poison))
                healing = (int)Math.Round(healing * .66);
            healing = Math.Min(healing, action.TargetCharacter.MaxHitpoints - action.TargetCharacter.Hitpoints);

            if (healing == 0)
                return;

			action.TargetCharacter.Hitpoints += healing;
			Console.ForegroundColor = GetColor(action.Trait.TextColor);
			if (action.Trait.BackgroundColor != null)
				Console.BackgroundColor = GetColor(action.Trait.BackgroundColor);
			string output = action.Trait.Text
				.Replace("@actor", action.Actor.Name)
				.Replace("@target", action.TargetCharacter.Name)
				.Replace("@healing", healing.ToString());
			Console.WriteLine(output);
			Console.ResetColor();
		}

		private void DisplayMatchup(List<Party> parties) {
			foreach (Party party in parties) {
				string output = String.Format ("{0}: ", party.Name);
				foreach (Character character in party.CharacterList) {
					if (character.Alive)
						output += String.Format("[{0} {1}/{2}hp {3}/{4}mp] ", character.Name, character.Hitpoints, character.MaxHitpoints, character.Mana, character.MaxMana);
				}
				Console.WriteLine(output);
			}
		}

		private ConsoleColor GetColor(string color) {
			switch (color.ToLower()) {
				case "black":
					return ConsoleColor.Black;
				case "darkblue":
					return ConsoleColor.DarkBlue;
				case "darkcyan":
					return ConsoleColor.DarkCyan;
				case "darkgreen":
					return ConsoleColor.DarkGreen;
				case "darkred":
					return ConsoleColor.DarkRed;
				case "darkmagenta":
					return ConsoleColor.DarkMagenta;
				case "darkyellow":
					return ConsoleColor.DarkYellow;
				case "darkgray":
					return ConsoleColor.DarkGray;
				case "blue":
					return ConsoleColor.Blue;
				case "green":
					return ConsoleColor.Green;
				case "cyan":
					return ConsoleColor.Cyan;
				case "red":
					return ConsoleColor.Red;
				case "magenta":
					return ConsoleColor.Magenta;
				case "yellow":
					return ConsoleColor.Yellow;
				case "white":
					return ConsoleColor.White;
				default:
					return ConsoleColor.Gray;
			}
		}
	}
}
