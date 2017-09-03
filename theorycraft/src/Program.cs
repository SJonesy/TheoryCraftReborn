using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace theorycraft
{
	class MainClass
	{
		public static int Main (string[] args)
		{
			List<Party> parties = new List<Party>();

			if (args.Length <= 1) {
				Console.WriteLine ("You must load at least 2 parties.");
				return 0;
			}

			// Load parties
			foreach (var arg in args) 
			{
				var yaml = File.ReadAllText(arg);
				var deserializer = new DeserializerBuilder()
					.WithNamingConvention(new CamelCaseNamingConvention())
					.Build();
				var party = deserializer.Deserialize<Party>(yaml);
				party.Points = 300;
				party.CharacterList = new List<Character>();
				parties.Add(party);
			}

			// Load characters
			for (var i = 0; i < parties.Count; i++) {
				var party = parties[i];
				Console.WriteLine (party.Name);
				foreach (var partycharacter in party.PartyCharacters) 
				{
					Character character = new Character(partycharacter.Name, partycharacter.Race, partycharacter.Items);
					character.PartyId = i;
					if (character.PointCost < party.Points) 
					{
						party.CharacterList.Add(character);
						party.Points -= character.PointCost;
						Console.WriteLine (character.DisplayCharacter());
					}
				}
			}

			Combat combat = new Combat ();

			if (parties.Count == 2) {
				combat.Duel(parties);
			} 
			else {
				combat.Brawl(parties);
			}

			return 0;
		}
	}
}
