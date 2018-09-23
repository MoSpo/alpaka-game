using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D {
    class Text : Drawable{
        private DiscreteVector2 BoundingBox;
        private BitmapFont font;
        private string text;
        private string displayText;

        public Text(BitmapFont font, int BoundX, int BoundY) {
            this.font = font;
            BoundingBox = new DiscreteVector2(BoundX, BoundY);
            text = null;
            displayText = null;
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y, Color tintColor, bool IsMouseOver) {
            //spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(200, 200, 200, 200);
            //if (IsMouseOver) {
            //} else {
            spriteBatch.DrawString(font, text, new Vector2(x, y), tintColor);//tintColor);
        }

        public override void Update(double dt) {
        }

        private string WrapText(string text, float maxLineWidth) {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = font.MeasureString(" ").Width;

            foreach (string word in words) {
                Vector2 size = font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth) {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                } else {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }


        public void SetText(string text) {
            this.text = text;
            displayText = WrapText(text, BoundingBox.X);
        }
    }
}
