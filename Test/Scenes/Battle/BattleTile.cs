using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Battle {
    class BattleTile {
        public BattleEffect[] effects = new BattleEffect[3];
		private byte amount;

        public BattleTile() {
            amount = 0;
        }

        public void AddEffect(BattleEffect effect) {
            if (amount >= 2) {
                effects[0] = null;
                sortEffects();
				effects[amount] = effect;
			} else {
                effects[amount] = effect;
				amount++;
            }
		}

        public void RemoveEffect(BattleEffect effect) {
            for (byte i = 0; i < 3; i++) {
                if (effects[i] == effect) {
                    effects[i] = null;
                    sortEffects();
                }
            }
        }


		public bool EffectInPosition(string name) {
			for (byte i = 0; i < 3; i++) {
				if (effects[i] != null) {
					if (effects[i].Name.Equals(name)) {
						return true;
					}
				}
			}
			return false;
		}

        private void sortEffects() {
            BattleEffect[] temp = new BattleEffect[3];
            byte index = 0;
            for (byte i = 0; i < 3; i++) {
                if (effects[i] != null) {
                    temp[index] = effects[i];
                    index++;
                }
            }
            amount = index;
            effects = temp;
        }
    }
}
