using System;
using Alpaka.Scenes.Battle;

namespace Alpaka {
	public class SceneAnimation {

		public enum SceneAnimationType {
			ADD_MESSAGE,
			DAMAGE_BAR,
			ATTACK,
			ARENA,
			PHASE,
			DEATH,
			CONDITION,
			ADD_EFFECT,
			REMOVE_EFFECT,
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
