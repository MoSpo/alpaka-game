using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Battle {
    class Creature {
        public string Name;
        public CreatureType Type;

        public CreatureElement[] Elements= new CreatureElement[3];
        public CreatureNature[] Natures = new CreatureNature[3];

		public Dictionary<CreatureStats, byte> BaseStats = new Dictionary<CreatureStats, byte>();
        
        public Creature(String name, CreatureElement[] elements, /*natures here*/CreatureType type, byte health, byte kin, byte strength, byte endurance, byte intelligence, byte wisdom, byte pace, byte awe) {
            Name = name;
            Type = type;
            for(int i = 0; i < 3; i++) {
                if(i >= elements.Length) {
                    Elements[i] = CreatureElement.NULL;
                } else {
                    Elements[i] = elements[i];
                }
            }
            BaseStats.Add(CreatureStats.HEALTH, health);
            BaseStats.Add(CreatureStats.KIN, kin);
            BaseStats.Add(CreatureStats.STRENGTH, strength);
            BaseStats.Add(CreatureStats.ENDURANCE, endurance);
            BaseStats.Add(CreatureStats.INTELLIGENCE, intelligence);
            BaseStats.Add(CreatureStats.WISDOM, wisdom);
            BaseStats.Add(CreatureStats.PACE, pace);
            BaseStats.Add(CreatureStats.AWE, awe);
        }

    }

    public enum CreatureStats {
        HEALTH,
        KIN,
        STRENGTH,
        ENDURANCE,
        INTELLIGENCE,
        WISDOM,
        PACE,
        AWE
    }

    enum CreatureType {
        BEAST,
        AQUATIC,
        BIRD,
        INSECT,
        PLANT,
        FAIRY,
        REPTILE,
        FIEND,
        ABSTRACT,
        CONSTRUCT
    }

    enum CreatureNature {
        FEARLESS_COWARDLY,
        OBSERVANT_UNAWARE,
        CONFIDENT_INSECURE,
        HUMBLE_ARROGANT,
        LAZY_ENERGETIC,
        GENTLE_BRUTISH,
        IMPULSIVE_CAUTIOUS,
        MALICIOUS_KIND,
        LOYAL_UNFAITHFUL,
        QUIRKY_TRADITIONAL
    }

    enum CreatureCondition {
        CUT,
        DRAINED,
        BURNED,
        DELIRIOUS,
        BLIND,
        DREAMY,
        PARALYZED,
        FROZEN,
        SCARED,
        CONFOUNDED,
        ASLEEP,
        POISONED
    }

    enum CreatureElement {
        NULL,
        FIRE,
        WATER,
        EARTH,
        WIND,
        ELECTRIC,
        WOOD,
        ROCK,
        ICE,
        STEEL,
        NUCLEAR,
        MAGIC,
        ETHER,
        VOID,
        COSMIC,
        SOUND,
        LIGHT,
        DARK
    }
}
