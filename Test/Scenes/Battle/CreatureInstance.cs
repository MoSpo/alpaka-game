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

		private Dictionary<CreatureStats,byte> SkillPoints = new Dictionary<CreatureStats, byte>();
    
        private Dictionary<CreatureStats, int> Stats = new Dictionary<CreatureStats, int>();

        private byte TotalSkillPoints = 0;
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

        public CreatureInstance(Creature BaseCreature) {
            //AllCreatures temp = new AllCreatures(); CreatureType = temp.GetCreature(0);
            CreatureType = BaseCreature;
            Nickname = CreatureType.Name;

            //
            AllActions temp2 = new AllActions();
            for (byte i = 0; i < 6; i++) {
                Actions[i] = temp2.GetAction((short)(i + 0));
            }
			Actions[6] = new CreatureAction(
				"Attack", //NAME
				CreatureType.Elements[0],
				ActionCategory.PHYSICAL,
				50,          //SPEED
				0,           //PRIORITY MODIFIER
				255,//30,          //POWER
				255,        //USAGE
				0,           //MANA
				null
				);
			//

			for (byte i = 0; i < 6; i++) {
				ActionAmountUsed[i] = Actions[i].Usage;
                //ActionAmountUsed[i] = Actions[i].Usage; NEED TO SET THIS UP STILL
			}

			foreach (CreatureStats Stat in Enum.GetValues(typeof(CreatureStats))) {
                SetSkillPoints(Stat, 1);
                CalculateStat(Stat);
            }

            for (byte i = 0; i < 12; i++) {
                hasCondition[i] = false;
            }

			Health = GetTotalStat(CreatureStats.HEALTH);
			Kin = (short)Math.Floor(1.0 * GetTotalStat(CreatureStats.KIN) / 6);

		}

        private void CalculateStat(CreatureStats StatName) {
            //int Points = (int)Math.Floor((double)((((SkillPoints[StatName] * 80) / ((CreatureType.BaseStats[StatName] + 80) * 2)) ^ (2 / 3)) * SkillPoints[StatName] * 4));
            int Points = (int)Math.Floor(Math.Pow(80 / ((double)CreatureType.BaseStats[StatName] + 80), (double)3 / 4) * Math.Log(2*SkillPoints[StatName] + 1) * 80);

            if (StatName == CreatureStats.HEALTH || StatName == CreatureStats.KIN) Stats[StatName] = (int)Math.Floor(((CreatureType.BaseStats[StatName] * 2.1)*4) + Points *1.5);
            else Stats[StatName] = (int)Math.Floor((CreatureType.BaseStats[StatName] * 2.1) + Points);
        }

        public void SetSkillPoints(CreatureStats Stat, byte Amt) {
            if (Amt > 10) Amt = 10;
            if (TotalSkillPoints + Amt > 28) Amt = (byte)(28 - TotalSkillPoints);
            SkillPoints[Stat] = Amt;
            TotalSkillPoints += Amt;
            CalculateStat(Stat);
        }

        public byte IncreaseSkillPoints(CreatureStats Stat) {
            if (SkillPoints[Stat] == 10) return 10;
            if (TotalSkillPoints == 28) return SkillPoints[Stat];
            SkillPoints[Stat]++;
            TotalSkillPoints++;
            CalculateStat(Stat);
            return SkillPoints[Stat];
        }

        public byte DecreaseSkillPoints(CreatureStats Stat) {
            if (SkillPoints[Stat] == 0) return 0;
            SkillPoints[Stat]--;
            TotalSkillPoints--;
            CalculateStat(Stat);
            return SkillPoints[Stat];
        }

        public int GetTotalStat(CreatureStats StatName) {
            return Stats[StatName];
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

        public CreatureAction GetAction(byte ActionNumber) {
            return Actions[ActionNumber];
        }

		public byte GetActionUsage(byte ActionNumber) {
			return ActionAmountUsed[ActionNumber];
		}

		public void DepleteActionUsage(byte ActionNumber) {
			if (ActionAmountUsed[ActionNumber] > 0) ActionAmountUsed[ActionNumber]--;
		}

		public byte GetKin(byte ActionNumber) {
			return Actions[ActionNumber].Mana;
		}
	}
}
