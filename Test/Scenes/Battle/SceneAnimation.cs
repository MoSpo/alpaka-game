using System;
using Alpaka.Scenes.Battle;

namespace Alpaka {
	public class SceneAnimation {

		public int ID;
		public double[] values;
		public string Message;

		public SceneAnimation(int ID, double[] values, string Message) {
			this.ID = ID;
			this.values = values;
			this.Message = Message;
		}

		//bool SceneRun(BattleScene scene, double dt) {
		//	return false;
		//}

	}
}
