using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alpaka {
	public class Battler {

		Thing battler;

		public Battler(ContentManager Content, String type) {
			Texture2D br = Content.Load<Texture2D>(type);
			battler = new Thing(br, 0, 0, 256, 256);
		}

		public void Draw(SpriteBatch spriteBatch, int x) {
			battler.Draw(x, 112, spriteBatch);
		}
	}
}
