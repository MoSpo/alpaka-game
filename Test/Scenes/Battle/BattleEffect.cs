using System.Collections.Generic;
using System.Linq;

namespace Alpaka.Scenes.Battle {
	class BattleEffect {
		public string Name;
        private int CurrentEffectGroup;
		public byte Priority;

		public byte Lifespan;
		public bool IsInfinate = false;

		public CreatureElement Element; //TODO: ADD ELEMENTS
		public List<byte> Placement;
		public List<EffectScript> Scripts;

		public byte CurrentPlacement;

		public int BasePhysical;
		public int BaseMystical;

		public SceneAnimation EffectAnimation; //TODO: ASSIGN DEFAULT

		public Player User;

		public byte CurrentLifespan;

		public BattleEffect(byte Priority, byte Lifespan, byte[] Placement, EffectScript[] Scripts) {
			this.Priority = Priority;
			this.Lifespan = Lifespan;
			CurrentLifespan = Lifespan;
			if (Placement == null) {
				this.Placement = null;
			} else {
				this.Placement = Placement.ToList();
			}
			this.Scripts = Scripts.ToList();

		}

        public void SetElement(CreatureElement Element) {
            this.Element = Element;
        }

        public void SetEffectGroup(int GroupNumber) {
            CurrentEffectGroup = GroupNumber;
        }

        public int GetEffectGroup() {
            return CurrentEffectGroup;
        }

        public void SetBaseAttacks(int BasePhysical, int BaseMystical) {
			this.BasePhysical = BasePhysical;
			this.BaseMystical = BaseMystical;
		}

		public BattleEffect(BattleEffect Target, Player User) {
			Name = Target.Name;
			Priority = Target.Priority;
			Lifespan = Target.Lifespan;
			CurrentLifespan = Target.Lifespan;
			Placement = Target.Placement;
            Element = Target.Element;
			BasePhysical = Target.BasePhysical;
			BaseMystical = Target.BaseMystical;
			Scripts = new List<EffectScript>(Target.Scripts);
			this.User = User;
		}

		public EffectScript GetTriggeredEffect(EffectTrigger whichToTrigger) {

			foreach (EffectScript Script in Scripts) {
				if (Script.Trigger == whichToTrigger) {
					return Script;
				}
			}
			return null;
		}

		public void DecreaseLifespan() {
			if (!IsInfinate) {
			CurrentLifespan--;
			}
		}

		public bool IsLifespanDepleted() {
			return CurrentLifespan == 0; //TODO: BUG IF INITIALISED TO 0
		}

		public bool HasTrigger(EffectTrigger trigger) {
			foreach (EffectScript Script in Scripts) {
				if (Script.Trigger == trigger) {
					return true;
				}
			}
			return false;

		}
	}
}
