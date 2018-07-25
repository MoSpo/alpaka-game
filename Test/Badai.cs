using System;
using Alpaka.Scenes.Battle;

namespace Alpaka {
	public class Badai {

        Game1 g;
		private Random random;

	    public Badai(Game1 g) {
			this.g = g;
			random = new Random();
		}

		public byte SelectMovement() {
			return (byte)random.Next(0, 4);
		}
		public byte SelectCreature() {
			while (true) {
				byte s = (byte)random.Next(0, 6);
				if (g.getOppCanUseTeam(s)) return s;
			}
		}
		public byte SelectAction() {
			while (true) {
				byte s = (byte)random.Next(0, 8);
				if (g.getOppCanUse(s)) return s;
			}
		}
	}
}
