using System;

namespace theorycraft
{
	public interface AI
	{
		Character Actor { get; set; }
		Party FriendlyParty { get; set; }
		Party HostileParty { get; set; }
		Ability Ability { get; set; }

		Action ChooseAction(Character character, Party friendlyParty, Party hostileParty);
		void ChooseAbility();
		Character ChooseTarget();
	}
}

