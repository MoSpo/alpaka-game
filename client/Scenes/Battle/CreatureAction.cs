using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Battle {
    class CreatureAction {
        public string Name;
		public string Style;
        public CreatureElement Element;
        public ActionCategory Catagory;
        public byte Speed;
        public byte Priority;
        public byte Power;
        public byte Usage;
        public byte Mana;

		public bool IsSwitch = false;
		public SceneAnimation Animation;
        public BattleEffect ActionEffect; //+ effect priority
        //dictionary effect trigger+speed to effect script
        public CreatureAction(string Name, CreatureElement Element, ActionCategory Catagory, byte Speed, short Priority, byte Power, byte Usage, byte Mana, BattleEffect ActionEffect) {
            this.Name = Name;
            this.Element = Element;
            this.Catagory = Catagory;
            this.Speed = Speed;
            this.Priority = (byte)(4 + Priority);
            this.Power = Power;
            this.Usage = Usage;
            this.Mana = Mana;
            this.ActionEffect = ActionEffect;
			if (ActionEffect != null) {
				ActionEffect.Name = Name;
                ActionEffect.SetElement(Element);
			}

			Style = "attacked";

        }
    }
    /*
    class SimpleCreatureAction {
        public string Name;
        public byte Speed;
        public byte PriorityMod;
        public byte Power;
        public byte Usage;
        public byte Mana;

        public byte EffectPriority;
        public Dictionary<short, long> EffectScript; //dictionary effect trigger+speed to effect script
    }*/

}
