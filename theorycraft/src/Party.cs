using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace theorycraft
{
	public class Party
	{
		public string Name { get; set; }
		public int Points { get; set; }
		public List<Character> CharacterList { get; set; }

		[YamlMember(Alias = "characters", ApplyNamingConventions = false)]
		public List<PartyCharacter> PartyCharacters { get; set; }

		public Party ()
		{
		}
	}

	public struct PartyCharacter {
		public string Name { get; set; }
		public string Race { get; set; }
		public List<string> Items { get; set; }
	}
}

