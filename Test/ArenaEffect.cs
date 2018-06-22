using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Alpaka {
	public class ArenaEffect {

		public bool InFocus = true;
		public bool useTimer = false;
		double timer = 0;

		Texture2D block;

		public double x;
		public double y;

		public int amt = 3;

		Color normalColor = new Color(255, 0, 0, 255);
		Color fadedColor = new Color(127, 0, 0, 127);

		Color currentColor = new Color(0, 0, 0, 0);

		public ArenaEffect(ContentManager Content) {
			//displays stuff in the arena
			//arena stuffs all have different heights. Two  things of the same height fade in and out of each other.
			//Things of larger height are drawn first and things of smaller height are drawn last.
			block = Content.Load<Texture2D>("block");
		}

        public void Update(double dt) {
			if (useTimer) {
				timer += dt;
			}
        }

		public void Foreground() {
			InFocus = false;
			useTimer = true;
		}

		public void Background() {
			InFocus = true;
			useTimer = true;
		}

		public void setPosition(Vector2 vec) {
			x = vec.X;
			y = vec.Y;
		}
        public void setPosition(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public void blend(double ti, Color startColor, Color endColor) {
			currentColor.R = (byte)(startColor.R + (endColor.R - startColor.R) * ti);
			currentColor.B = (byte)(startColor.B + (endColor.B - startColor.B) * ti);
			currentColor.G = (byte)(startColor.G + (endColor.G - startColor.G) * ti);
			currentColor.A = (byte)(startColor.A + (endColor.A - startColor.A) * ti);
		}

		public void Draw(SpriteBatch spriteBatch) {
			if (useTimer && timer <= 0.6) {
                if (InFocus) {
                    blend(timer / 0.6, fadedColor, normalColor);
                } else {
                    blend(timer / 0.6, normalColor, fadedColor);
                }
                for (int i = 0; i < amt; i++) spriteBatch.Draw(block, new Vector2((int)x - 64, (int)y - 60 - 70 * i), currentColor);

			} else {
                if (timer > 1) {
                    timer = 0;
                    useTimer = false;
                }
                if (InFocus) {
					for (int i = 0; i < amt; i++) spriteBatch.Draw(block, new Vector2((int)x - 64, (int )y - 60 -70 * i), normalColor);
				} else {
					for (int i = 0; i < amt; i++) spriteBatch.Draw(block, new Vector2((int)x - 64, (int) y -60 - 70 * i), fadedColor);			
				}
			}
        }
    }
}
