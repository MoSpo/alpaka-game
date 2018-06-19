using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alpaka {
	public class RightBar {

		Texture2D bar;
        Texture2D elements;
        Texture2D statuses;

        Thing body;
		Thing blank;
		Thing hbar;
		Thing mbar;
        Thing status;

        RightStatBar[] statbars;

        public byte st;

        double timer;
		public bool useTimer;

        public int totalHealth;
        public int health;
        int h;
        public int oldhealth;

        public int totalKin;
        public int kin;
        int m;
        public int oldkin;
        bool isHealth = true;

        byte ele1 = 14;
        byte ele2 = 0;
        byte ele3 = 0;

        Thing el1;
        Thing el2;
        Thing el3;

        Game1 g;

        public RightBar(ContentManager Content, Game1 g) {
			bar = Content.Load<Texture2D>("creaturebarflipped");
			body = new Thing(bar, 196, 100, 140, 180);
			blank = new Thing(bar, 0, 50, 336, 50);
			hbar = new Thing(bar, 0, 0, 336, 25);
			mbar = new Thing(bar, 0, 25, 336, 25);
			statbars = new RightStatBar[] {
				new RightStatBar(bar),
				new RightStatBar(bar),
				new RightStatBar(bar),
				new RightStatBar(bar),
				new RightStatBar(bar),
				new RightStatBar(bar)
			};

            elements = Content.Load<Texture2D>("elements");

            el1 = new Thing(elements, 0, 32 * ele1, 32, 32);
            el2 = new Thing(elements, 0, 32 * ele2, 32, 32);
            el3 = new Thing(elements, 0, 32 * ele3, 32, 32);

            st = 0;

            statuses = Content.Load<Texture2D>("status");
            status = new Thing(statuses, 0, 0, 32, 32);

            totalHealth = 100;
            health = 100;
            oldhealth = 100;
            totalKin = 100;
            kin = 0;
            oldkin = 0;
            this.g = g;
			useTimer = false;
		}

		public void Update(double dt) {
			if (useTimer) {
				timer += dt;
			}
		}

		public void setHealth(int sh) {
			health = sh;
			useTimer = true;
			isHealth = true;
		}

		public void setKin(int sk) {
			kin = sk;
			useTimer = true;
			isHealth = false;
		}

        public void setMaxHealth(int sh) {
            totalHealth = sh;
            health = sh;
            oldhealth = sh;
            h = 0;
        }

        public void setMaxKin(int sm) {
            totalKin = sm;
            kin = sm;
            oldkin = sm;
            m = 0;
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y) {
            el1.Draw(304 -136 + x, 34 + y, spriteBatch);
            el2.Draw(304 -172 + x, 34 + y, spriteBatch);
            el3.Draw(304 -208 + x, 34 + y, spriteBatch);

            body.Draw(x + 196, y + 50, spriteBatch);
			blank.Draw(x, y, spriteBatch);
			if (useTimer) {
				if (isHealth) {
					h =  250 - (int)((oldhealth + (health - oldhealth) * timer) * 250 / totalHealth);
                    hbar.sourcex =  27 + h;
					hbar.width = 250 - h;
					hbar.Draw(x + 27 + h, y, spriteBatch);

					if (timer > 1) {
						timer = 0;
						oldhealth = health;
						useTimer = false;
                        g.nextAnim = true;

                    }
                } else {
					m = 200 - (int)((oldkin + (kin - oldkin) * timer) * 200 / totalKin);
                    mbar.sourcex = 77 + m;
					mbar.width = 200 - m;
					mbar.Draw(x + 77 + m, y + 25, spriteBatch);
					if (timer > 1) {
						timer = 0;
						oldkin = kin;
						useTimer = false;
                        g.nextAnim = true;

                    }
                }
			} else {
				hbar.sourcex = (int)(27 +  250 - (double)((double)health / (double)totalHealth) * 250);
				mbar.sourcex = (int)(77 + 200 - (double)((double)kin / (double)totalKin) * 200);

				hbar.width = (int)(250 + 250 - (double)((double)health / (double)totalHealth) * 250);
				mbar.width = (int)(200 +  200 - (double)((double)kin / (double)totalKin) * 200);

				hbar.Draw(x + (int)(27 + 250 - (double)((double)health / (double)totalHealth) * 250), y, spriteBatch);
				mbar.Draw(x + (int)(77 + 200 - (double)((double)kin / (double)totalKin) * 200), y + 25, spriteBatch);
                
            }
            if (st > 0) {
				status.sourcey = ((st - 1) % 4)*32;
				status.sourcex = (int)(Math.Floor((double)(st-1) / 4))*32;
                status.Draw(288 + x, 18 + y, spriteBatch);
            }

            spriteBatch.DrawString(g.font, ((int)((250 -h)*(double)totalHealth/(double)250)).ToString(), new Vector2(x + 250 - 3, y + 6), Color.Black);
        }

    }

	public class RightStatBar {
		Texture2D bar;
		Thing stats;
		Thing[] flares;

		byte points;
		byte oldpoints;

		double timer;
		bool useTimer;


		public RightStatBar(Texture2D bar) {
			this.bar = bar;
			stats = new Thing(bar, 160, 250, 56, 32);
			flares = new Thing[] {
				new Thing(bar, 160, 250, 56, 32),
				new Thing(bar, 160, 250, 56, 32),
				new Thing(bar, 160, 250, 56, 32),
				new Thing(bar, 160, 250, 56, 32),
				new Thing(bar, 160, 250, 56, 32)
			};
			points = 5;
			oldpoints = 5;
			timer = 0;
			useTimer = false;
		}

		public void Update(double dt) {
			if (useTimer) {
				timer += dt;
			}
		}

		public void IncreasePoints(bool increase) {
			if (points < 10 && points > 0) {
				if (increase) {
					points += 1;
					useTimer = true;
				} else {
					points -= 1;
					useTimer = true;
				}
			}
		}

		public void blend(Thing t, double ti, Color startColor, Color endColor) {
			t.color.R = (byte)(startColor.R + (endColor.R - startColor.R) * ti);
			t.color.B = (byte)(startColor.B + (endColor.B - startColor.B) * ti);
			t.color.G = (byte)(startColor.G + (endColor.G - startColor.G) * ti);
			t.color.A = (byte)(startColor.A + (endColor.A - startColor.A) * ti);
		}

		public void Draw(SpriteBatch spriteBatch, int x, int y) {
			if (useTimer) {

				int p = points - 5;
				if (p < 0) p *= -1;

				int q = oldpoints - 5;
				if (q < 0) q *= -1;
				if (p > q) {
					stats.Draw(x + (int)(p * Bezier(timer / 0.2)), y, spriteBatch);
					if (p < 5) {
						flares[p].sourcex = 0;
					} else {
						flares[p].sourcex = 0;
					}
					blend(flares[p], Bezier((timer - 0.2) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

					for (int i = 0; i < p; i++) {
						flares[i].Draw(x, y, spriteBatch);
					}
				} else {

				}

				if (timer > 1) {
					timer = 0;
					oldpoints = points;
					useTimer = false;
				}
			} else {
				stats.Draw(x, y, spriteBatch);
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
