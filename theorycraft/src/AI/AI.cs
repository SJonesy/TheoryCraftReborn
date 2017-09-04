using System;

namespace theorycraft
{
	public interface AI
	{
		Action ChooseAction(Character character, Party friendlyParty, Party hostileParty);
	}
}

