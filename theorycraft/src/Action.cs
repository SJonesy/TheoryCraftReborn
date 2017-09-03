using System;

namespace theorycraft
{
	public class Action
	{
		public Character Actor { get; set; }
		public Character TargetCharacter { get; set; }
		public Party TargetParty { get; set; }
		public Ability Ability { get; set; }

		public Action (Character actor, Character target, Ability ability)
		{
			this.Actor = actor;
			this.TargetCharacter = target;
			this.TargetParty = null;
			this.Ability = ability;
		}

		public Action (Character actor, Party target, Ability ability)
		{
			this.Actor = actor;
			this.TargetParty = target;
			this.TargetCharacter = null;
			this.Ability = ability;
		}
	}
}

