using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Battle {
    class CreatureInstance {
        public Creature CreatureType;
        public string Nickname;
        //private CreatureNature SelectedNature;
        //CreatureSkill SelectedSkill;
        //CreatureAbility SelectedAbility;

		public Dictionary<CreatureStats,byte> StatPoints = new Dictionary<CreatureStats, byte>();

		public int Health;
		public int Kin;

        private CreatureAction[] Actions = new CreatureAction[8];
        private byte[] ActionAmountUsed = new byte[6];

        private bool[] hasCondition = new bool[12];

		public bool killed = false;

        //--Creature_instance.mColour;
        //--Creature_instance.mSprite;
        //--Creature_instance.mLevel;
        //--Creature_instance.mAncient;

        public CreatureInstance() {
            AllCreatures temp = new AllCreatures();
            CreatureType = temp.GetCreature(0);
            Nickname = CreatureType.Name;
            AllActions temp2 = new AllActions();
            Actions[0] = temp2.GetAction(2);
			Actions[1] = temp2.GetAction(3);

			for (byte i = 0; i < 6; i++) {
				ActionAmountUsed[i] = 10;
				//ActionAmountUsed[i] = Actions[i].Usage;
			}

			foreach (CreatureStats Stat in Enum.GetValues(typeof(CreatureStats))) {
				StatPoints.Add(Stat, 100);  //TODO: SET UP CREATURES AND CREATURE INSTANCES PROPERLY
            }
            for (byte i = 0; i < 12; i++) {
                hasCondition[i] = false;
            }

			Health = GetTotalStat(CreatureStats.HEALTH);
			Kin = 0;

		}


        public short GetTotalStat(CreatureStats StatName) {
            return (short)(CreatureType.BaseStats[StatName] + StatPoints[StatName]);
        }

        public bool[] GetConditions() {
            return hasCondition;
        }

        public bool HasCondition(byte Condition) {
            return hasCondition[Condition];
        }

        public void GiveCondition(byte Condition) {
            hasCondition[Condition] = true;
        }

        public void RemoveConditions() {
            for (int i = 0; i < 12; i++) {
                hasCondition[i] = false;
            }
        }

		public bool HasElement(CreatureElement Element) {
			return CreatureType.Elements[0] == Element || CreatureType.Elements[1] == Element || CreatureType.Elements[2] == Element;
		}


		public double GetElementBonus(CreatureElement Element) {
			return 1.0; //TODO: FILL IN TYPE EFFECTIVENESS CHART
		}

        public CreatureAction GetAction(byte ActionNumber) {
            return Actions[ActionNumber];
        }

		public byte GetActionUsage(byte ActionNumber) {
			return ActionAmountUsed[ActionNumber];
		}

	}
}
