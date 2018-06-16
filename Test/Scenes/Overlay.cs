using Alpaka.Graphics2D;
using Alpaka.Scenes.Battle;
using Alpaka.Scenes.Map;
using Alpaka.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes {
    abstract class Overlay {
        public Overlay previousOverlay;
        public InputHandler input;
        protected List<Sprite> sprites;
        protected List<Object2D> objects;
        protected BattleScene battleScene;
        protected MapScene mapScene;

        public Overlay(Overlay previousOverlay) {
            this.previousOverlay = previousOverlay;
            sprites = new List<Sprite>();
            objects = new List<Object2D>();
            input = new InputHandler();
            SceneService.input.inputHandler = input.HandleInput;
            battleScene = previousOverlay.battleScene;
            mapScene = previousOverlay.mapScene;
            Initialise();
        }

        public Overlay(BattleScene battleScene) {
            previousOverlay = null;
            sprites = new List<Sprite>();
            objects = new List<Object2D>();
            input = new InputHandler();
            SceneService.input.inputHandler = input.HandleInput;
            this.battleScene = battleScene;
            mapScene = null;
            Initialise();
        }

        public Overlay(MapScene mapScene) {
            previousOverlay = null;
            sprites = new List<Sprite>();
            objects = new List<Object2D>();
            input = new InputHandler();
            SceneService.input.inputHandler = input.HandleInput;
            battleScene = null;
            this.mapScene = mapScene;
            Initialise();
        }

        protected virtual void Initialise() {
        }

        public virtual void Update(double dt) {
            SceneService.input.PollInput();
        }

        public virtual void Draw(SpriteBatch spriteBatch) {
            foreach (Object2D ob2 in objects) {
                ob2.Draw(spriteBatch);
            }
        }

    }
}
