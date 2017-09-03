using System;

namespace theorycraft
{
	public class RandomAI : AI
	{
		public Character Actor { get; set; }
		public Party FriendlyParty { get; set; }
		public Party HostileParty { get; set; }
		public Ability Ability { get; set; }

		public RandomAI () {
		}

		public Action ChooseAction (Character actor, Party friendlyParty, Party hostileParty)
		{
			this.Actor = actor;
			this.FriendlyParty = friendlyParty;
			this.HostileParty = hostileParty;

			ChooseAbility();

			if (this.Ability.PartyTarget) {
				return new Action (this.Actor, this.HostileParty, this.Ability);
			} 
			else {
				return new Action (this.Actor, this.ChooseTarget(), this.Ability);
			}
		}

		public void ChooseAbility () {
			Random rand = new Random();
			int abilNum = rand.Next(this.Actor.Abilities.Count);
			this.Ability = this.Actor.Abilities[abilNum];
		}

		public Character ChooseTarget ()
		{
			Random rand = new Random();
			Party targetParty = null;
			if (this.Ability.Beneficial) {
				targetParty = this.FriendlyParty;
			} 
			else {
				targetParty = this.HostileParty;
			}
			int charNum = rand.Next(targetParty.CharacterList.Count);

			return targetParty.CharacterList[charNum];
		}
	}
}

