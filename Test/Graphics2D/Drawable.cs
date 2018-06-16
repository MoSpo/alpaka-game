using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D {
    public abstract class Drawable {

        public TextureData textureSelection;

        public DiscreteVector2 GetChildPosition(int index) {
            return new DiscreteVector2(textureSelection.Child[index].X, textureSelection.Child[index].Y);
        }


        public virtual void Draw(SpriteBatch spriteBatch, int x, int y, Color tintColor, bool IsMouseOver) {
        }

        public virtual void Update(double dt) {
        }

        public virtual void Animate(byte AnimationIndex) {
        }

        public virtual bool MouseOver(int x, int y) {
            return false;
        }


    }
}
