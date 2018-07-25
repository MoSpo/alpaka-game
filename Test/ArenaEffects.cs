using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alpaka {
	public class ArenaEffects {
		public ArenaEffect[] effects;

		double timer;
		public int rot;
		int pos;

		Vector2[] poses;

		public ArenaEffects(ContentManager Content, SpriteFont font) {
			effects = new ArenaEffect[] {
				new ArenaEffect(Content, font),
				new ArenaEffect(Content, font),
				new ArenaEffect(Content, font),
				new ArenaEffect(Content, font),
				new ArenaEffect(Content, font),
                new ArenaEffect(Content, font),
                new ArenaEffect(Content, font),
                new ArenaEffect(Content, font),

            };

			poses = new Vector2[] {
				new Vector2(145,350),
				new Vector2(205,405),
				new Vector2(360,420),
				new Vector2(515,405),
				new Vector2(575,350),
				new Vector2(515,315),
				new Vector2(360,300),
				new Vector2(205,315),
			};

            for (int i = 0; i < effects.Length; i++) {
                //effects[i].amt = (i % 3) == 0 ? 1 : 0;
                effects[i].setPosition(poses[i]);
                if (effects[i].y > 350) effects[i].InFocus = false;
            }

            timer = 0;
		}

		public void Update(double dt) {
			foreach (ArenaEffect eff in effects) eff.Update(dt);
		}

		public void Add(int pos, string name) {
			ArenaEffect eff = effects[pos];
			eff.text[eff.amt] = name;
			eff.amt++;
		}

		public void Remove(int pos, string name) {
			ArenaEffect eff = effects[pos];
			for (int i = 0; i < 3; i++) {
				if (i == eff.amt-1) {
					eff.amt--;
					return;
				}
				if (eff.text[i].Equals(name)) {
					eff.text[i] = eff.text[i + 1];
					eff.text[i + 1] = name;
				}
			}
		}

		public void Rotate(double dt) {
			if (rot != 0) {
				timer += dt;
				if (timer >= 1) {
					
					timer = 0;
					if (rot > 0) {
						rot--;
						pos--;

					} else if (rot < 0) {
						rot++;
						pos++;
					}

					if (pos == 255) {
						pos = 7;
					} else if (pos == 8) {
						pos = 0;
					}

					for (int i = 0; i < effects.Length; i++) effects[i].setPosition(poses[(8 + i + pos) % 8]);

				}

				if (rot > 0) {
					for (int i = 0; i < effects.Length; i++) effects[i].setPosition(poses[(8 + i + pos) % 8].X + (poses[(8 + i - 1 + pos) % 8].X - poses[(8 + i + pos) % 8].X) * timer, poses[(8 + i + pos) % 8].Y + (poses[(8 + i - 1 + pos) % 8].Y - poses[(8 + i + pos) % 8].Y) * timer);
					//angle += dt * Math.PI / 4;
				} else if (rot < 0) {
					for (int i = 0; i < effects.Length; i++) effects[i].setPosition(poses[(8 + i + pos) % 8].X + (poses[(8 + i + 1 + pos) % 8].X - poses[(8 + i + pos) % 8].X) * timer, poses[(8 + i + pos) % 8].Y + (poses[(8 + i + 1 + pos) % 8].Y - poses[(8 + i + pos) % 8].Y) * timer);
                    //angle -= dt * Math.PI / 4;
                }

                for (int i = 0; i < effects.Length; i++) {
                    if (!effects[i].useTimer) {
                        if (effects[i].y > 350 && effects[i].InFocus) effects[i].Foreground();
                        else if (effects[i].y < 405 && !effects[i].InFocus) effects[i].Background();
                    }
                }

            }
		}

		public void DrawForeground(SpriteBatch spriteBatch) {
			foreach (ArenaEffect eff in effects) if(!eff.InFocus) eff.Draw(spriteBatch);
		}
        public void DrawBackground(SpriteBatch spriteBatch) {
            foreach (ArenaEffect eff in effects) if (eff.InFocus) eff.Draw(spriteBatch);
        }
    }
}
