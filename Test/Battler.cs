using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alpaka {
	public class Battler {

		Thing battler;
        int ID;
		Game1 g;

		public bool UseTimer = false;
		double timer;

		public int rumble;
		int xoff;
		int yoff;

		public Battler(ContentManager Content, int ID, string type, Game1 g) {
			this.g = g;
			Texture2D br = Content.Load<Texture2D>(type);
            this.ID = ID;
			battler = new Thing(br, 0, ID*256, 256, 256);

		}

        public void changeID(int ID) {
            this.ID = ID;
            battler.sourcey = ID * 256;
        }

		public void Update(double dt) {
			if (UseTimer) {
				timer += dt;
				xoff = (int)(20 * (1 - timer) * Math.Cos(timer * rumble));
				yoff = (int)(20 * (1 - timer) * Math.Sin(timer * rumble));
				if (timer > 0.8) {
					timer = 0;
					xoff = 0;
					yoff = 0;
					g.nextAnim = true;
					UseTimer = false;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, int x) {

			battler.Draw(x + xoff, 112 + yoff, spriteBatch);
		}
	}
}
