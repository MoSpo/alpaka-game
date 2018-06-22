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

		public ArenaEffects(ContentManager Content) {
			effects = new ArenaEffect[] {
				new ArenaEffect(Content),
				new ArenaEffect(Content),
				new ArenaEffect(Content),
				new ArenaEffect(Content),
				new ArenaEffect(Content),
			};
			poses = new Vector2[] {
				new Vector2(0,0),
				new Vector2(0,0),
								new Vector2(0,0),
								new Vector2(0,0),
								new Vector2(0,0),
								new Vector2(0,0),
								new Vector2(0,0),
								new Vector2(0,0),
			};
			timer = 0;
		}

		public void Update(double dt) {
			foreach (ArenaEffect eff in effects) eff.Update(dt);
		}

		public void Rotate(double dt) {
			if (rot != 0) {
				timer += dt;
				if (timer >= 1) {
					
					timer = 0;
					if (rot > 0) {
						rot--;
						pos++;

					} else if (rot < 0) {
						rot++;
						pos--;
					}

					if (pos == 255) {
						pos = 7;
					} else if (pos == 8) {
						pos = 0;
					}

					for (int i = 0; i < effects.Length; i++) effects[i].setPosition(poses[(8 + i + pos) % 8]);

				}

				if (rot > 0) {
					for (int i = 0; i < effects.Length; i++) effects[i].setPosition(poses[(8 + i + pos) % 8]);
					//angle += dt * Math.PI / 4;
				} else if (rot < 0) {
					for (int i = 0; i < effects.Length; i++) effects[i].setPosition(poses[(8 + i + pos) % 8]);
					//angle -= dt * Math.PI / 4;
				}

			}
		}

		public void Draw(SpriteBatch spriteBatch) {
			foreach (ArenaEffect eff in effects) eff.Draw(spriteBatch);
		}
	}
}
