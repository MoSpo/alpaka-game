/*using Alpaka.Graphics2D;
using Alpaka.Scenes;
using Alpaka.Scenes.Menus;
using Alpaka.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;

namespace Alpaka {

    public class MainGame : Game {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RasterizerState rasterizerState;

        public MainGame() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 540;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {

            SceneService.game = this;
            SceneService.input = new InputService();
            SceneService.input.AddKey("Exit", false, Keys.Escape, Buttons.Back);
            SceneService.input.AddKey("Start", false, Keys.Q, Buttons.Back);
            SceneService.input.AddKey("Reset", false, Keys.W, Buttons.Back);
            SceneService.input.AddKey("Select", true, Keys.O, Buttons.Back);

            base.Initialize();
        }

        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            rasterizerState = new RasterizerState() { ScissorTestEnable = true };

            SceneService.textureAtlas = Content.Load<Texture2D>("+++/Untitled");
            SceneService.textureData = Content.Load<Dictionary<string, TextureData>>("pets");

            SceneService.font = Content.Load<BitmapFont>("fonts/font");

            SceneService.OpenBattleScene(Content);
            //SceneService.OpenOverlay(new MenuSelect(null));
        }

        protected override void UnloadContent()  {
        }

        protected override void Update(GameTime gameTime) {

            double dt = gameTime.ElapsedGameTime.TotalSeconds;

            SceneService.Update(dt);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SceneService.Draw3D(gameTime);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, rasterizerState);
            SceneService.Draw2D(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}*/
