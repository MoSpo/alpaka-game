using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alpaka {
	public class Phase {

		Texture2D phases;
		Thing thing;
		Thing thing2;
		Thing thing3;
		double timer;
		byte phase;
		byte oldphase;

		bool useTimer;

        Game1 g;

        public Phase(ContentManager Content) {
			phases = Content.Load<Texture2D>("phases");
			thing = new Thing(phases, 0, 0, 56, 32);
			thing2 = new Thing(phases, 0, 0, 56, 32);
			thing3 = new Thing(phases, 0, 0, 56, 32);
			phase = 0;
			oldphase = 0;
			useTimer = false;
		}

		public void blend(Thing t, double ti, Color startColor, Color endColor) {
			t.color.R = (byte)(startColor.R + (endColor.R - startColor.R) * ti);
			t.color.B = (byte)(startColor.B + (endColor.B - startColor.B) * ti);
			t.color.G = (byte)(startColor.G + (endColor.G - startColor.G) * ti);
			t.color.A = (byte)(startColor.A + (endColor.A - startColor.A) * ti);
		}

		public void SetPhase(byte newphase, Game1 g) {
			phase = newphase;
            //phase += 1;
            //if (phase > 9) {
            //	phase = 0;
            //}
            this.g = g;
			useTimer = phase != oldphase;
		}

		public void Update(double dt) {
			if (useTimer) {
				timer += dt/0.2;
			}
		}

		public void Draw(SpriteBatch spriteBatch) {
			if (useTimer) {
				thing.sourcey = oldphase * 32;
				thing2.sourcey = phase * 32;
				blend(thing, Bezier(timer), new Color(255, 255, 255, 255), new Color(0, 0, 0, 0));
				blend(thing2, Bezier(timer), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
				if (phase == 0) {
					thing3.Draw(332, (int)(-10 * Bezier(timer)), spriteBatch);
					thing.Draw(332, (int)(-10*Bezier(timer)), spriteBatch);
					thing2.Draw(332, (int)(-10 *Bezier(timer)), spriteBatch);
				} else if (oldphase == 0) {
					thing3.Draw(332, (int)(-10 + 10 * Bezier(timer)), spriteBatch);
					thing.Draw(332, (int)(-10 +10*Bezier(timer)), spriteBatch);
					thing2.Draw(332, (int)(-10 +10*Bezier(timer)), spriteBatch);
				} else {
					thing3.Draw(332, 0, spriteBatch);
					thing.Draw(332, 0, spriteBatch);
					thing2.Draw(332, 0, spriteBatch);
				}
				if (timer > 1) {
					timer = 0;
					oldphase = phase;
					useTimer = false;
                    if(g != null) g.nextAnim = true;
				}
			} else {
				thing.color = new Color(255, 255, 255, 255);
				thing.sourcey = phase * 32;
				if (phase == 0) {
					thing.Draw(332, -10, spriteBatch);
				} else {
					thing.Draw(332, 0, spriteBatch);
				}
			}

		}

		private double Bezier(double t) {
			if (t < 0) t = 0;
			if (t > 1) t = 1;

			double u = 1 - t;
			double tt = t * t;
			double uu = u * u;
			double uuu = uu * u;
			double ttt = tt * t;

			double p = uuu * 0; //first term
			p += 3 * uu * t * 0.1; //second term
			p += 3 * u * tt * 1.0; //third term
			p += ttt * 1.0; //fourth term

			return p;
		}
	}
}
