using System;
using System.Linq;
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

        public int PointCost() {
            int pointCost = 0;
            foreach (Character character in CharacterList)
                pointCost += character.PointCost;
            return pointCost;
        }
    }

    public struct PartyCharacter {
        public string Name { get; set; }
        public string Race { get; set; }
        public List<String> Traits { get; set; }
        public Row Row { get; set; }
    }
}

