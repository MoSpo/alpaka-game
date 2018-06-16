using Alpaka.Graphics2D;
using Alpaka.Scenes;
using Alpaka.Scenes.Battle;
using Alpaka.Scenes.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Services {

    static class SceneService {
        private static Scene currentScene;
        private static Overlay currentOverlay;
        public static MainGame game;
        public static InputService input;
        private static BattleScene battleScene;
        private static MapScene mapScene;
        private static bool IsBattle = false;

        public static BitmapFont font;

        public static Texture2D textureAtlas;
        public static Dictionary<string, TextureData> textureData;

        public static void OpenOverlay(Overlay newOverlay) {
            currentOverlay = newOverlay;
        }

        public static void OpenBattleScene(ContentManager Content) {
            if(battleScene == null) {
                battleScene = new BattleScene(Content);
            }
            if (!IsBattle) {
                currentScene = battleScene;
            }
        }

        public static void OpenMapScene(ContentManager Content) {
            if (mapScene == null) {
                mapScene = new MapScene();
            }
            if (IsBattle) {
                currentScene = mapScene;
            }
        }

        public static void CloseScenes() {
            currentScene = null;
        }

        public static void Exit() {
            //Console.WriteLine(currentOverlay.previousOverlay == null);
            if (currentOverlay == null) {
                game.Exit();
            } else if(currentOverlay.previousOverlay == null) {
                currentOverlay = null;
            } else {
                currentOverlay = currentOverlay.previousOverlay;
                input.inputHandler = currentOverlay.input.HandleInput;
            }
        }

        public static void Update(double dt) {
            if (currentScene != null && currentOverlay == null) {
                currentScene.Update(dt);
            }
            if (currentOverlay != null) {
                currentOverlay.Update(dt);
            }
        }

        public static void Draw3D(GameTime gameTime) {
            if (currentScene != null) {
                currentScene.Draw3D(gameTime);
            }
        }

        public static void Draw2D(SpriteBatch spriteBatch) {
            if (currentScene != null) {
                currentScene.Draw2D(spriteBatch);
            }
            if (currentOverlay != null) {
                currentOverlay.Draw(spriteBatch);
            }        }
    }


}
