using System;
using System.Linq;
using System.Collections.Generic;

namespace theorycraft
{
	public class GeneralAI : AI
	{
		private Character Actor { get; set; }
		private Party FriendlyParty { get; set; }
		private Party HostileParty { get; set; }

		public GeneralAI () {
		}

		public Action ChooseAction(Character actor, Party friendlyParty, Party hostileParty) {
			this.Actor = actor;
			this.FriendlyParty = friendlyParty;
			this.HostileParty = hostileParty;

			Ability directHealingAbility = GetDirectHealingAbility();
			Character injuredAlly = FindLowestHPInjuredAlly();
			if (directHealingAbility != null && Actor.Mana >= directHealingAbility.Mana && injuredAlly != null)
				return new Action(Actor, injuredAlly, directHealingAbility);

			Ability directDamageAbility = GetDirectDamageAbility();
			if (directDamageAbility != null && Actor.Mana >= directDamageAbility.Mana)
				return new Action(Actor, FindLowestHPTarget(), directDamageAbility);

			Ability meleeAbility = GetMeleeAbility();
			Character meleeTarget = FindLowestHPMeleeTarget();
			if (meleeAbility != null && meleeTarget != null)
				return new Action(Actor, meleeTarget, meleeAbility);

			return null;
		}

		private Ability GetMeleeAbility() {
			return Actor.Abilities.Find (x => x.Type == AbilityType.Melee);
		}

		private List<Ability> GetAllMeleeAbilities() {
			return Actor.Abilities.FindAll (x => x.Type == AbilityType.Melee);
		}

		private Ability GetDirectDamageAbility() {
			return Actor.Abilities.Find (x => x.Type == AbilityType.DirectDamage);
		}

		private List<Ability> GetDirectDamageAbilities() {
			return Actor.Abilities.FindAll (x => x.Type == AbilityType.DirectDamage);
		}

		private Ability GetDirectHealingAbility() {
			return Actor.Abilities.Find (x => x.Type == AbilityType.DirectHealing);
		}

		private List<Ability> GetDirectHealingAbilities() {
			return Actor.Abilities.FindAll (x => x.Type == AbilityType.DirectHealing);
		}

		private Character FindLowestHPMeleeTarget() {
			List<Character> validTargets = GetValidMeleeTargets();
			Character target = null;
			int lowest = int.MaxValue;

			foreach (Character validTarget in validTargets) {
				if (validTarget.Alive && validTarget.Hitpoints < lowest) {
					lowest = validTarget.Hitpoints;
					target = validTarget;
				}
			}

			return target;
		}

		private Character FindLowestHPTarget() {
			Character target = null;
			int lowest = int.MaxValue;

			foreach (Character validTarget in HostileParty.CharacterList) {
				if (validTarget.Alive && validTarget.Hitpoints < lowest) {
					lowest = validTarget.Hitpoints;
					target = validTarget;
				}
			}

			return target;
		}

		private Character FindLowestHPInjuredAlly() {
			Character target = null;
			int lowest = int.MaxValue;

			foreach (Character validTarget in FriendlyParty.CharacterList) {
				if (validTarget.Alive && validTarget.Hitpoints < lowest && validTarget.Hitpoints < validTarget.MaxHitpoints) {
					lowest = validTarget.Hitpoints;
					target = validTarget;
				}
			}

			return target;
		}

		private List<Character> GetValidMeleeTargets() {
			List<Character> checkRow = HostileParty.CharacterList.FindAll (x => (x.Row == Row.Front) && x.Alive);
			if (checkRow.Count > 0)
				return checkRow;
			checkRow = HostileParty.CharacterList.FindAll (x => (x.Row == Row.Middle) && x.Alive);
			if (checkRow.Count > 0)
				return checkRow;
			checkRow = HostileParty.CharacterList.FindAll (x => (x.Row == Row.Back) && x.Alive);

			return checkRow;
		}
	}
}

