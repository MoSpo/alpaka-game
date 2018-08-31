using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alpaka {
	public class LeftBar {

		Texture2D bar;
        Texture2D elements;
        Texture2D statuses;

        Thing body;
		Thing blank;
		Thing hbar;
		Thing mbar;

        Thing status;

        LeftStatBar[] statbars;

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
		public bool isHealth = true;

        byte ele1 = 0;
        byte ele2 = 0;
        byte ele3 = 0;

        public String name;


        Thing el1;
        Thing el2;
        Thing el3;

        ElementThing[] elementBuffs;

        Game1 g;

		public LeftBar(ContentManager Content, Game1 g) {
			bar = Content.Load<Texture2D>("creaturebar");
            statuses = Content.Load<Texture2D>("status");

            body = new Thing(bar, 0, 100, 140, 180);
			blank = new Thing(bar, 0, 50, 336, 50);
			hbar = new Thing(bar, 0, 0, 336, 25);
			mbar = new Thing(bar, 0, 25, 336, 25);
			statbars = new LeftStatBar[] {
				new LeftStatBar(bar, statuses),
				new LeftStatBar(bar, statuses),
				new LeftStatBar(bar, statuses),
				new LeftStatBar(bar, statuses),
				new LeftStatBar(bar, statuses),
				new LeftStatBar(bar, statuses)
			};

            elements = Content.Load<Texture2D>("elements");

            el1 = new Thing(elements, 0, 32 * ele1,32,32);
            el2 = new Thing(elements, 0, 32 * ele2, 32, 32);
            el3 = new Thing(elements, 0, 32 * ele3, 32, 32);

            Texture2D buffs = Content.Load<Texture2D>("buffs");

            elementBuffs = new ElementThing[6] {
                new ElementThing(elements, buffs, 0, 0, 16, 16),
                new ElementThing(elements, buffs, 0, 0, 16, 16),
                new ElementThing(elements, buffs, 0, 0, 16, 16),
                new ElementThing(elements, buffs, 0, 0, 16, 16),
                new ElementThing(elements, buffs, 0, 0, 16, 16),
                new ElementThing(elements, buffs, 0, 0, 16, 16)
            };

            st = 0;

            status = new Thing(statuses, 0, 0, 32, 32);

            totalHealth = 100;
            health = 100;
			oldhealth = 100;
            totalKin = 100;
			kin = 0;
			oldkin = 0;
			useTimer = false;
            this.g = g;
		}

		public void Update(double dt){//, int x, int y) {
            if (useTimer) {
                timer += dt;
            } else {
            //    foreach (LeftStatBar l in statbars) {
            //        l.mouseOver = l.GetX() + 30 >= x && l.GetY() + 18 >= y && l.GetX() - 5 <= x && l.GetY() <= y;
            //        l.Use
            //    }
            }
            foreach (LeftStatBar l in statbars) {
                l.Update(dt);
            }
		}

        public void AddBoost(int stat, int boost) {
            bool isBoost = boost == 1;
            if(isBoost) statbars[stat].IncreasePoints(true);
            else statbars[stat].IncreasePoints(false);
        }

        public void ResetBuffs() {
            foreach (ElementThing e in elementBuffs) {
                if (e.GetAmount() == 0) e.IncreaseAmount(); e.element = 0;
                if (e.GetAmount() == 2) e.DecreaseAmount(); e.element = 0;
                }
            }

        public void ResetBoosts() {
            foreach (LeftStatBar s in statbars) s.Reset();
        }

        public void AddBuff(int element, int buff) {

            bool isBuff = buff == 1;

            foreach (ElementThing e in elementBuffs) {
                if (e.element == element) {
                    if (isBuff) {
                        if (e.GetAmount() == 0) e.IncreaseAmount(); e.element = 0;
                    } else {
                        if (e.GetAmount() == 2) e.DecreaseAmount(); e.element = 0;
                    }
                    return;
                }
            }

            foreach (ElementThing e in elementBuffs) {
                if (e.GetAmount() == 1) {
                    if (isBuff) {
                        e.IncreaseAmount();
                    } else {
                        e.DecreaseAmount();
                    }
                    e.element = element;
                    return;
                }
            }
        }

		public void setHealth(int sh) {
			health = sh;
			useTimer = true;
			isHealth = true;
		}

        public void setMaxHealth(int sh) {
            totalHealth = sh;
            health = sh;
            oldhealth = sh;
          //  h = 250;
        }

        public void setMaxKin(int sm) {
            totalKin = 1000;
            kin = sm;
            oldkin = sm;
          //  m = 0;
        }

        public void setKin(int sk) {
			kin = sk;
			useTimer = true;
			isHealth = false;
		}

        public void setElements(byte e1, byte e2, byte e3) {
            ele1 = e1;
            ele2 = e2;
            ele3 = e3;
            el1.sourcey = ele1 * 32;
            el2.sourcey = ele2 * 32;
            el3.sourcey = ele3 * 32;
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y) {
            el1.Draw(136 + x, 34 + y, spriteBatch);
            el2.Draw(172 + x, 34 + y, spriteBatch);
            el3.Draw(208 + x, 34 + y, spriteBatch);

            for (int i = 0; i < 6; i++) {
                statbars[i].Draw(x+20, y +74+ i*17, spriteBatch);
            }

            body.Draw(x, y + 50, spriteBatch);

            int ei = 0;
            foreach (ElementThing e in elementBuffs) {
                e.Draw(x + 5, y + 73 + 16 * ei, spriteBatch);
                if (e.GetAmount() != 1) ei++;
            }

            blank.Draw(x, y, spriteBatch);
			if (useTimer) {
				if (isHealth) {
					h = (int)((oldhealth + (health - oldhealth) * timer) * 250/totalHealth);
					hbar.width = 59 + h;
					hbar.Draw(x, y, spriteBatch);

                    mbar.width = (int)(59 + (double)((double)kin / (double)totalKin) * 200);
                    mbar.Draw(x, y + 25, spriteBatch);

                    if (timer > 1) {
						timer = 0;
						oldhealth = health;
						useTimer = false;
                        g.nextAnim = true;
					}
				} else {
					m = (int)((oldkin + (kin - oldkin) * timer) * 200 / totalKin);
					mbar.width = 59 + m;
					mbar.Draw(x, y + 25, spriteBatch);

                    hbar.width = (int)(59 + (double)((double)health / (double)totalHealth) * 250);
                    hbar.Draw(x, y, spriteBatch);

                    if (timer > 1) {
						timer = 0;
						oldkin = kin;
						useTimer = false;
                        g.nextAnim = true;
                    
                    }
                }
                spriteBatch.DrawString(g.font, ((int)((h) * (double)totalHealth / (double)250)).ToString(), new Vector2(x + 60 + 3, y + 6), Color.Black);
            } else {
				hbar.width = (int)(59 + (double)((double)health/ (double)totalHealth) * 250);
				mbar.width = (int)(59 + (double)((double)kin / (double)totalKin) * 200);

				hbar.Draw(x,y, spriteBatch);
				mbar.Draw(x, y + 25, spriteBatch);

            }
            if (st > 0) {
				status.sourcey = ((st - 1) % 4)*32;
                status.sourcex = (int)(Math.Floor((double)(st-1) / 4))*32;
                status.Draw(16 + x, 18 + y, spriteBatch);
            }

            spriteBatch.DrawString(g.font, health.ToString(), new Vector2(x + 60 + 3, y + 6), Color.Black);
            spriteBatch.DrawString(g.font, name, new Vector2(x + 40, y + 51), Color.Black);
        }

    }

    public class ElementThing : Thing {

        public int element;

        private int amt;
        private Texture2D texture2;

        public ElementThing(Texture2D texture, Texture2D texture2, int sourcex, int sourcey, int width, int height) : base(texture, sourcex, sourcey, width, height) {
            amt = 1;
            this.texture2 = texture2;
        }

        public int GetAmount() {
            return amt;
        }
        public void IncreaseAmount() {
            if (amt < 3) {
                amt++;
            }
        }
        public void DecreaseAmount() {
            if (amt > 0) {
                amt--;
            }
        }
        public override void Draw(int x, int y, SpriteBatch spriteBatch) {
            this.x = x;
            this.y = y;
            if (amt != 1) {
                spriteBatch.Draw(texture2, new Rectangle(x, y, width, height), new Rectangle(0, 8 * amt, width, height), color);
                spriteBatch.Draw(texture, new Rectangle(x, y, width, height), new Rectangle(8 * amt, 16 + 32 * element, width, height), color);
            }
        }
    }

    public class LeftStatBar {
		Thing stats;
		Thing[] flares;

		byte points;
		byte oldpoints;

		double timer;
		bool useTimer;

		public LeftStatBar(Texture2D bar, Texture2D status) {

            stats = new Thing(bar, 237, 252, 16, 18);
			flares = new Thing[] {
				new Thing(status, 1, 129, 14, 14),
				new Thing(status, 1, 129, 14, 14),
				new Thing(status, 1, 129, 14, 14),
				new Thing(status, 1, 129, 14, 14),
				new Thing(status, 1, 129, 14, 14)
			};
			points = 5;
			oldpoints = 5;
			timer = 0;
			useTimer = false;
		}

        public int GetX() {
            return stats.x;
        }

        public int GetY() {
            return stats.y;
        }

        //public void MouseOver(bool b) {
        //    mouseOver = b;
        //   if (!useTimer) useTimer = b;

        //}

        public void Update(double dt) {
			if (useTimer) {
				timer += dt;
			}
            }

            private void SetColour(bool IsPositive) {
            foreach(Thing t in flares) {
                if (IsPositive) t.sourcex = 1;
                else t.sourcex = 17;
            }
        }

        public void Reset() {
            points = 5;
            oldpoints = 5;
        }

        public void IncreasePoints(bool increase) {
			if (points < 10 && points > 0) {
				if (increase) {
					points += 1;
                    if (points > 5) SetColour(true);
                    timer = 0;
                        useTimer = true;
				} else {
					points -= 1;
                    if(points < 5) SetColour(false);
                    timer = 0;
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

		public void Draw(int x, int y, SpriteBatch spriteBatch) {
			if (useTimer) {

				int p = points - 5;
				if (p < 0) p *= -1;

				int q = oldpoints - 5;
				if (q < 0) q *= -1;

                int r = q;
                if (p > q) r = p;
				if (p != q) {
                    if (timer < 0.4) {
                        stats.sourcex = 161 + 76 - (int)((r * 16) * Bezier(timer / 0.4));
                        stats.width = 16 + (int)((r * 16) * Bezier(timer / 0.4));
                        stats.Draw(x, y, spriteBatch);
                        for (int i = 0; i < q; i++) {
                            if (-(i + 1) * 15 + (int)((r * 16) * Bezier(timer / 0.4)) > -16) flares[i].Draw(x - (i + 1) * 15 + (int)((r * 16) * Bezier(timer / 0.4)), y + 2, spriteBatch);
                        }
                    } else if (timer < 1.6) {
                        stats.Draw(spriteBatch);

                        if (p > q) blend(flares[p - 1], Bezier((timer - 0.4) / 1.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        else if (p < q) blend(flares[q - 1], Bezier((timer - 0.4) / 1.2), new Color(255, 255, 255, 255), new Color(0, 0, 0, 0));
                            for (int i = 0; i < r; i++) {
                                flares[i].Draw(x - (i + 1) * 15 + r * 16, y + 2, spriteBatch);
                            }
                    } else {
                        stats.sourcex = 161 + 76 - r * 16 + (int)((r * 16) * Bezier((timer - 1.6) / 0.4));
                        stats.width = 16 + r * 16 - (int)((r * 16) * Bezier((timer - 1.6) / 0.4));
                        stats.Draw(x, y, spriteBatch);
                        for (int i = 0; i < p; i++) {
                            if (-(i + 1) * 15 + r * 16 - (int)((r * 16) * Bezier((timer - 1.6) / 0.4)) > -16) flares[i].Draw(x - (i + 1) * 15 + r * 16 - (int)((r * 16) * Bezier((timer - 1.6) / 0.4)), y + 2, spriteBatch);
                        }
                    }
                }

                if (timer > 2) {
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
