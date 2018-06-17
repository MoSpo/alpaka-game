using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Battle {
    class BattlePhase {
        private BattleEngine Battle;
        private List<BattleResolution> Resolutions = new List<BattleResolution>();
        public List<BattleEffect> CurrentPhaseEffects = new List<BattleEffect>();

		private byte PhaseNumber;

        public List<SceneAnimation> Run() {
			
            List<SceneAnimation> Animations = new List<SceneAnimation>();
			Animations.Add(new SceneAnimation(4, new double[] { PhaseNumber }, "#PHASE ANIMATION#"));

			foreach (BattleResolution Resolution in Resolutions) {
					if (Resolution.GetUser() == null || !Resolution.GetUser().IsKilled()) { //TODO: STOP GET USER BEING NULL I.E. REDESIGN END RESOLUTION
					Animations.AddRange(Resolution.Resolve());
				}
            }

            //CurrentPhaseEffects.Sort(Comparer<BattleEffect>.Create((e1, e2) => e1.Speeds[(int)EffectTrigger.EACHTURN_UNTIL_EFFECT_END].CompareTo(e2.Speeds[(int)EffectTrigger.EACHTURN_UNTIL_EFFECT_END])));

			foreach (BattleEffect Effect in CurrentPhaseEffects) {
				if (Effect.HasTrigger(EffectTrigger.EACHTURN_UNTIL_EFFECT_END)) {
					if (Effect.EffectAnimation != null) {
						Animations.Add(Effect.EffectAnimation);
					}
					Animations.AddRange(Battle.InterpretEffect(Effect.GetTriggeredEffect(EffectTrigger.EACHTURN_UNTIL_EFFECT_END), Effect.User, null, Effect.CurrentPlacement));
				}
				Effect.DecreaseLifespan();
				if (Effect.IsLifespanDepleted()) {
					Animations.AddRange(Battle.RemoveEffect(Effect, true));
				}
            }

            return Animations;

        }

        public BattlePhase(BattleEngine Battle, byte PhaseNumber) {
            this.Battle = Battle;
			this.PhaseNumber = PhaseNumber;
        }

        public void Add(BattleResolution Resolution) {
            Resolutions.Add(Resolution);
        }
    }
}
