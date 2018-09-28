using System;

namespace theorycraft
{
	public class RandomAI : AI
	{
		private Character Actor { get; set; }
		private Party FriendlyParty { get; set; }
		private Party HostileParty { get; set; }
		private Trait Trait { get; set; }

		public RandomAI () {
		}

		public Action ChooseAction (Character actor, Party friendlyParty, Party hostileParty)
		{
			this.Actor = actor;
			this.FriendlyParty = friendlyParty;
			this.HostileParty = hostileParty;

			Random rand = new Random();
			int abilNum = rand.Next(this.Actor.Traits.Count);
			this.Trait = this.Actor.Traits[abilNum];

			if (this.Trait.PartyTarget) {
				return new Action (this.Actor, this.HostileParty, this.Trait);
			} 
			else {
				return new Action (this.Actor, this.ChooseTarget(), this.Trait);
			}
		}


		private Character ChooseTarget ()
		{
			Random rand = new Random();
			Party targetParty = null;
			if (this.Trait.Beneficial) {
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

