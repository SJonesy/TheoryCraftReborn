using System;

namespace theorycraft
{
	public class Action
	{
		public Character Actor { get; set; }
		public Character TargetCharacter { get; set; }
		public Party TargetParty { get; set; }
		public Trait Trait { get; set; }

        public Action (Character actor, Character target, Trait trait)
		{
			this.Actor = actor;
			this.TargetCharacter = target;
			this.TargetParty = null;
			this.Trait = trait;
		}

        public Action (Character actor, Party target, Trait trait)
		{
			this.Actor = actor;
			this.TargetParty = target;
			this.TargetCharacter = null;
			this.Trait = trait;
		}
	}
}

