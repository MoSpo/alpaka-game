using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Alpaka {
	public class Battler {

		Thing battler;
        int ID;

		public Battler(ContentManager Content, int ID, String type) {
			Texture2D br = Content.Load<Texture2D>(type);
            this.ID = ID;
			battler = new Thing(br, 0, ID*256, 256, 256);

		}

        public void changeID(int ID) {
            this.ID = ID;
            battler.sourcey = ID * 256;
        }

		public void Draw(SpriteBatch spriteBatch, int x) {
			battler.Draw(x, 112, spriteBatch);
		}
	}
}
