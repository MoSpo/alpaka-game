using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D {
    public class Sprite : Drawable {

        //public TextureData textureSelection; NOW IN DRAWABLE

        public Texture2D textureAltas;

        public byte animationFrames;
        private byte currentFrame;
        private byte AnimationIndex;
        private bool prev;

        public bool isAnimating;
        public bool loopAnimation = true;

        public float animationSpeed;
        private double timer;

        public Sprite(Texture2D textureAltas) {

            this.textureAltas = textureAltas;
            animationFrames = 0;
            AnimationIndex = 0;
            currentFrame = 0;
            prev = false;
            //this.textureSelection = textureSelection;
        }

        public Sprite(Sprite sprite) {

            textureAltas = sprite.textureAltas;
            animationFrames = sprite.animationFrames;
            AnimationIndex = sprite.AnimationIndex;
            isAnimating = sprite.isAnimating;
            loopAnimation = sprite.loopAnimation;
            animationSpeed = sprite.animationSpeed;
            currentFrame = 0;
            prev = false;
            textureSelection = sprite.textureSelection;
        }

        public DiscreteVector2 GetChildPosition(int index) {
            return new DiscreteVector2(textureSelection.Child[index].X, textureSelection.Child[index].Y);
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color tintColor, bool IsMouseOver) {
            //spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(200, 200, 200, 200);
            if (IsMouseOver) {
                spriteBatch.Draw(textureAltas, new Rectangle(x, y, textureSelection.Main.XSize, textureSelection.Main.YSize), new Rectangle(textureSelection.Main.X + textureSelection.Main.XSize * currentFrame, textureSelection.Main.Y + textureSelection.Main.YSize * AnimationIndex, textureSelection.Main.XSize, textureSelection.Main.YSize), Color.Yellow);
            } else {
                spriteBatch.Draw(textureAltas, new Rectangle(x, y, textureSelection.Main.XSize, textureSelection.Main.YSize), new Rectangle(textureSelection.Main.X + textureSelection.Main.XSize * currentFrame, textureSelection.Main.Y + textureSelection.Main.YSize * AnimationIndex, textureSelection.Main.XSize, textureSelection.Main.YSize), tintColor);
            }
      }

        public override void Update(double dt) {
            if(isAnimating) {
                timer += dt;
                if(timer >= animationSpeed/25) {
                    timer = 0;
                    if (currentFrame == animationFrames) {
                        if (prev) {
                            AnimationIndex = 0;
                            prev = false;
                            //isAnimating = false;
                        }
                        currentFrame = 0;
                    } else {
                        currentFrame++;
                    }
                }
            }
        }

        public override void Animate(byte AnimationIndex) {
            base.Animate(AnimationIndex);
            this.AnimationIndex = AnimationIndex;
            currentFrame = 0;
            prev = true;
        }

        public bool IsAnimationEnded() {
            if(currentFrame == animationFrames) {
                return true;
            }
            return false;
        }

        public override bool MouseOver(int x, int y) {
            if (x > 0 && x < textureSelection.Main.XSize && y > 0 && y < textureSelection.Main.YSize) {
                return true;
            } else {
                return false;
            }
        }
    }
}
