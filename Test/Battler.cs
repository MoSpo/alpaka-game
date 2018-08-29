using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alpaka {
    public class Battler {

        Thing battler;


        Thing minus;
        Thing[] brackets;
        Thing[] numbers;
        Texture2D weak;
        Texture2D normal;
        Texture2D strong;

        int ID;
        Game1 g;

        public bool UseTimer = false;
        double timer;

        public int rumble;
        private int numbersAmount;
        int xoff;
        int yoff;

        public Battler(ContentManager Content, int ID, string type, Game1 g) {
            this.g = g;
            Texture2D ba = Content.Load<Texture2D>(type);
            Texture2D min = Content.Load<Texture2D>("minus");
            Texture2D brk = Content.Load<Texture2D>("brackets");

            weak = Content.Load<Texture2D>("badnumbers");
            normal = Content.Load<Texture2D>("numbers");
            strong = Content.Load<Texture2D>("goodnumbers");

            this.ID = ID;
            battler = new Thing(ba, 0, ID * 256, 256, 256);

            minus = new Thing(min, 0, 0, 64, 64);

            brackets = new Thing[2] {
                 new Thing(brk, 0, 0, 32, 64),
                  new Thing(brk, 32, 0, 32, 64),
            };

            numbers = new Thing[4] {
                 new Thing(normal, 0, 0, 64, 64),
                  new Thing(normal, 0, 0, 64, 64),
                   new Thing(normal, 0, 0, 64, 64),
                    new Thing(normal, 0, 0, 64, 64)
            };

        }

        public IEnumerable<int> GetDigits(int source) {
            int individualFactor = 0;
            int tennerFactor = Convert.ToInt32(Math.Pow(10, source.ToString().Length));
            do {
                source -= tennerFactor * individualFactor;
                tennerFactor /= 10;
                individualFactor = source / tennerFactor;

                yield return individualFactor;
            } while (tennerFactor > 1);
        }

        public void SetNumbers(int num, double effectiveness, double proficient) {
            int type = 1;
            if (effectiveness == 0.5) {
                type = 2;
            } else if (effectiveness == 2) {
                type = 3;
            }

            if (num < 0) {
                num *= -1;
                minus.sourcey = type * 64;
            } else {
                minus.sourcey = 0;
            }

            if (proficient == 1.5) {
                brackets[0].sourcey = type * 64;
                brackets[1].sourcey = type * 64;
            } else {
                brackets[0].sourcey = 0;
                brackets[1].sourcey = 0;
            }

            int i = 0;
            foreach (int n in GetDigits(num)) {
                numbers[i].sourcey = (n + 1) * 64;
                if (type == 1) numbers[i].texture = normal;
                if (type == 2) numbers[i].texture = weak;
                if (type == 3) numbers[i].texture = strong;
                numbersAmount++;
                i++;
            }
            while (i < 4) {
                numbers[i].sourcey = 0;
                i++;
            }
            UseTimer = true;
        }

        public void changeID(int ID) {
            this.ID = ID;
            battler.sourcey = ID * 256;
        }

        public void Update(double dt) {
            if (UseTimer) {
                timer += dt;
                if (numbersAmount <= 0) {
                    xoff = (int)(20 * (1 - timer) * Math.Cos(timer * rumble));
                    yoff = (int)(20 * (1 - timer) * Math.Sin(timer * rumble));
                }

                if (timer > 0.8) {
                    timer = 0;
                    xoff = 0;
                    yoff = 0;
                    if (numbersAmount <= 0) g.nextAnim = true;
                    UseTimer = false;
                }
            }
        }

        public void blend(Thing t, double ti, Color startColor, Color endColor) {
            t.color.R = (byte)(startColor.R + (endColor.R - startColor.R) * ti);
            t.color.B = (byte)(startColor.B + (endColor.B - startColor.B) * ti);
            t.color.G = (byte)(startColor.G + (endColor.G - startColor.G) * ti);
            t.color.A = (byte)(startColor.A + (endColor.A - startColor.A) * ti);
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


        public void Draw(SpriteBatch spriteBatch, int x) {

            int y = 112;

            if (numbersAmount > 0) {
                if (UseTimer) {
                    if (timer < 1) {
                        blend(minus, Bezier(timer / 0.5), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        blend(brackets[0], Bezier((timer - 0.04) / 0.5), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        blend(numbers[0], Bezier((timer - 0.04) / 0.5), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(numbers[1], Bezier((timer - 0.04 * 2) / 0.5), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(numbers[2], Bezier((timer - 0.04 * 3) / 0.5), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(numbers[3], Bezier((timer - 0.04 * 4) / 0.5), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        blend(brackets[1], Bezier((timer - 0.04 * numbersAmount) / 0.5), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        minus.Draw(x - 40, y - (int)(10 * Bezier((timer / 0.5))), spriteBatch);

                        brackets[0].Draw(x - 40 + 64, y - (int)(10 * Bezier(((timer - 0.04) / 0.5))), spriteBatch);

                        numbers[0].Draw(x - 40 + 96, y - (int)(10 * Bezier(((timer - 0.04) / 0.5))), spriteBatch);
                        numbers[1].Draw(x - 40 + 160, y - (int)(10 * Bezier(((timer - 0.04 * 2) / 0.5))), spriteBatch);
                        numbers[2].Draw(x - 40 + 224, y - (int)(10 * Bezier(((timer - 0.04 * 3) / 0.5))), spriteBatch);
                        numbers[3].Draw(x - 40 + 288, y - (int)(10 * Bezier(((timer - 0.04 * 4) / 0.5))), spriteBatch);

                        brackets[1].Draw(x -40 + 96 + 64 * numbersAmount, y - (int)(10 * Bezier(((timer - 0.04 * numbersAmount) / 0.5))), spriteBatch);
                    } else {
                        minus.Draw(spriteBatch);

                        brackets[0].Draw(spriteBatch);

                        numbers[0].Draw(spriteBatch);
                        numbers[1].Draw(spriteBatch);
                        numbers[2].Draw(spriteBatch);
                        numbers[3].Draw(spriteBatch);
                    }
                } else {
                    numbersAmount = 0;
                }
            }

            battler.Draw(x + xoff, y + yoff, spriteBatch);
        }
    }
}
