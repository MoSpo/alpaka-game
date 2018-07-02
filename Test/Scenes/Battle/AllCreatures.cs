using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Battle {
    class AllCreatures {
        List<Creature> AllCreatureList;

        public Creature GetCreature(short CreatureID) {
            return AllCreatureList[CreatureID];
        }

        public AllCreatures() {
            List<Creature> c = new List<Creature>();
            c.Add(new Creature(
                "Space Insect",
                new CreatureElement[] { CreatureElement.COSMIC },
                CreatureType.INSECT,
                60, //HEALTH
                60, //KIN
                80, //STRENGTH
                90, //ENDURANCE
                70, //INTELLIGENCE
                90, //WISDOM
                110, //PACE
                70  //AWE
                ));
            c.Add(new Creature(
                "Ice Guy",
                new CreatureElement[] { CreatureElement.ICE },
                CreatureType.FAIRY,
                60,  //HEALTH
                100, //KIN
                50, //STRENGTH
                70, //ENDURANCE
                100, //INTELLIGENCE
                100, //WISDOM
                90, //PACE
                60  //AWE
                ));
            c.Add(new Creature(
                "Rage",
                new CreatureElement[] { CreatureElement.FIRE, CreatureElement.ETHER },
                CreatureType.FIEND,
                110,  //HEALTH
                50, //KIN
                110, //STRENGTH
                90, //ENDURANCE
                60, //INTELLIGENCE
                50, //WISDOM
                60, //PACE
                100  //AWE
                ));
            c.Add(new Creature(
                "Wingman",
                new CreatureElement[] { CreatureElement.WIND },
                CreatureType.FIEND,
                80,  //HEALTH
                60, //KIN
                80, //STRENGTH
                100, //ENDURANCE
                70, //INTELLIGENCE
                80, //WISDOM
                100, //PACE
                80  //AWE
                ));
            c.Add(new Creature(
                "Clown",
                new CreatureElement[] { CreatureElement.DARK },
                CreatureType.FIEND,
                90,  //HEALTH
                90, //KIN
                70, //STRENGTH
                60, //ENDURANCE
                90, //INTELLIGENCE
                80, //WISDOM
                70, //PACE
                80  //AWE
                ));
            c.Add(new Creature(
                "Quarantine",
                new CreatureElement[] { CreatureElement.NUCLEAR, CreatureElement.STEEL },
                CreatureType.CONSTRUCT,
                100,  //HEALTH
                80, //KIN
                90, //STRENGTH
                90, //ENDURANCE
                90, //INTELLIGENCE
                50, //WISDOM
                70, //PACE
                80  //AWE
                ));
            c.Add(new Creature(
                "Not Present",
                new CreatureElement[] { CreatureElement.VOID },
                CreatureType.FAIRY,
                80,  //HEALTH
                100, //KIN
                100, //STRENGTH
                50, //ENDURANCE
                90, //INTELLIGENCE
                80, //WISDOM
                90, //PACE
                60  //AWE
                ));
            c.Add(new Creature(
                "Calesvol",
                new CreatureElement[] { CreatureElement.ROCK },
                CreatureType.CONSTRUCT,
                100,  //HEALTH
                60, //KIN
                100, //STRENGTH
                90, //ENDURANCE
                60, //INTELLIGENCE
                70, //WISDOM
                60, //PACE
                100  //AWE
                ));
            c.Add(new Creature(
                "Garphene",
                new CreatureElement[] { CreatureElement.ROCK, CreatureElement.STEEL },
                CreatureType.BIRD,
                90,  //HEALTH
                60, //KIN
                100, //STRENGTH
                40, //ENDURANCE
                70, //INTELLIGENCE
                70, //WISDOM
                110, //PACE
                100  //AWE
                ));
            c.Add(new Creature(
                "Aygon",
                new CreatureElement[] { CreatureElement.COSMIC, CreatureElement.VOID },
                CreatureType.FIEND,
                60,  //HEALTH
                70, //KIN
                60, //STRENGTH
                80, //ENDURANCE
                100, //INTELLIGENCE
                110, //WISDOM
                70, //PACE
                90  //AWE
                ));
            c.Add(new Creature(
                "Yeboi",
                new CreatureElement[] { CreatureElement.WATER },
                CreatureType.REPTILE,
                70,  //HEALTH
                90, //KIN
                80, //STRENGTH
                100, //ENDURANCE
                100, //INTELLIGENCE
                90, //WISDOM
                50, //PACE
                60  //AWE
                ));
            c.Add(new Creature(
                "Sandlurker",
                new CreatureElement[] { CreatureElement.EARTH },
                CreatureType.REPTILE,
                50,  //HEALTH
                90, //KIN
                110, //STRENGTH
                100, //ENDURANCE
                60, //INTELLIGENCE
                50, //WISDOM
                110, //PACE
                90  //AWE
                ));

            AllCreatureList = c;
        }
    }
}
