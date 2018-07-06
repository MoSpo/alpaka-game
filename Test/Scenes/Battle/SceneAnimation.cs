using System;
using Alpaka.Scenes.Battle;

namespace Alpaka {
	public class SceneAnimation {

		public enum SceneAnimationType {
			ADD_MESSAGE,
			HEALTH_BAR,
            KIN_BAR,
			ATTACK,
			ARENA,
			PHASE,
            SWITCH,
			DEATH,
            USER_DEATH_SELECT,
            OPPONENT_DEATH_SELECT,
            CONDITION,
			ADD_EFFECT,
			REMOVE_EFFECT,
            END_TURN,
            ELEMENT_CHANGE,
		}

		public SceneAnimationType Type;
		public double[] Values;
		public string Message;

		public SceneAnimation(SceneAnimationType Type, double[] Values, string Message) {
			this.Type = Type;
			this.Values = Values;
			this.Message = Message;
		}

		//bool SceneRun(BattleScene scene, double dt) {
		//	return false;
		//}

	}
}
