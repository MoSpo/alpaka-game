using Alpaka.Graphics2D;
using Alpaka.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes {

    abstract class Scene {
        public InputHandler input;
        protected List<Sprite> sprites;
        protected List<Object2D> objects;

        protected void Initialise() {
            sprites = new List<Sprite>();
            objects = new List<Object2D>();
            input = new InputHandler();
            SceneService.input.inputHandler = input.HandleInput;
        }

        public virtual void Update(double dt) {
            SceneService.input.PollInput();
        }

        public virtual void Draw2D(SpriteBatch spriteBatch) {
            foreach (Object2D ob2 in objects) {
                ob2.Draw(spriteBatch);
            }
        }

        public virtual void Draw3D(GameTime gameTime) {

        }

    }
}
