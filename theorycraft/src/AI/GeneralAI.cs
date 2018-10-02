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

			Trait groupHealingTrait = GetGroupHealingTrait();
            int totalGroupHealth = friendlyParty.CharacterList.Sum(x => x.Hitpoints);
            int totalGroupMaxHealth = friendlyParty.CharacterList.Sum(x => x.MaxHitpoints);
            if (groupHealingTrait != null && Actor.Mana >= groupHealingTrait.Mana && totalGroupMaxHealth - totalGroupHealth > 100)
				return new Action(Actor, friendlyParty, groupHealingTrait);

			Trait directHealingTrait = GetDirectHealingTrait();
			Character injuredAlly = FindLowestHPInjuredAlly();
			if (directHealingTrait != null && Actor.Mana >= directHealingTrait.Mana && injuredAlly != null)
				return new Action(Actor, injuredAlly, directHealingTrait);

			Trait groupDamageTrait = GetGroupDamageTrait();
			int aliveEnemiesCount = hostileParty.CharacterList.FindAll (x => x.Alive).Count;
			if (groupDamageTrait != null && Actor.Mana >= groupDamageTrait.Mana && aliveEnemiesCount > 1)
				return new Action(Actor, hostileParty, groupDamageTrait);

			Trait directDamageTrait = GetDirectDamageTrait();
			if (directDamageTrait != null && Actor.Mana >= directDamageTrait.Mana)
				return new Action(Actor, FindLowestHPTarget(), directDamageTrait);

			Trait meleeTrait = GetMeleeTrait();
			Character meleeTarget = FindLowestHPMeleeTarget();
			if (meleeTrait != null && meleeTarget != null)
				return new Action(Actor, meleeTarget, meleeTrait);

			return null;
		}

		private Trait GetMeleeTrait() {
			return Actor.Traits.Find (x => x.Type == TraitType.Melee);
		}

        private List<Trait> GetAllMeleeActions() {
			return Actor.Traits.FindAll (x => x.Type == TraitType.Melee);
		}

		private Trait GetDirectDamageTrait() {
			return Actor.Traits.Find (x => x.Type == TraitType.DirectDamage);
		}

        private List<Trait> GetDirectDamageActions() {
			return Actor.Traits.FindAll (x => x.Type == TraitType.DirectDamage);
		}

		private Trait GetDirectHealingTrait() {
			return Actor.Traits.Find (x => x.Type == TraitType.DirectHealing);
		}

		private Trait GetGroupHealingTrait()
		{
            return Actor.Traits.Find(x => x.Type == TraitType.GroupHealing);
		}

        private List<Trait> GetDirectHealingActions() {
			return Actor.Traits.FindAll (x => x.Type == TraitType.DirectHealing);
		}

		private Trait GetGroupDamageTrait() {
			return Actor.Traits.Find (x => x.Type == TraitType.GroupDamage);
		}

        private List<Trait> GetGroupDamageActions() {
			return Actor.Traits.FindAll (x => x.Type == TraitType.GroupDamage);
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

