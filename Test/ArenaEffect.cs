using System;
namespace Alpaka {
	public class ArenaEffect {

		bool InFocus = true;



		public ArenaEffect() {
			//displays stuff in the arena

			//arena stuffs all have different heights. Two  things of the same height fade in and out of each other.
			//Things of larger height are drawn first and things of smaller height are drawn last.
		}
	}
}
