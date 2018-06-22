using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Alpaka.Scenes.Battle;
using System.Collections.Generic;

namespace Alpaka {
    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BattleEngine engine;

        Arena arena;
        ArenaEffects effects;
        Menu menu;
        Phase phasecounter;

        Battler user;
        Battler opponent;

        LeftBar leftbar;
        RightBar rightbar;

        Texture2D background;
        Texture2D shade;

        public SpriteFont font;

        List<SceneAnimation> anim;
        int animPointer = 0;

        public bool chosen = false;

        String message;

        public bool nextAnim = false;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 540;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        private void PrintAnim() {
            foreach (SceneAnimation an in anim) {
	            Console.Write(an.ID + ": ");
                if (an.values != null) {
                    foreach (int i in an.values) {
                        Console.Write(i + " ");
                    }
                }
                Console.WriteLine("[" + an.Message + "]");
            }
        }

        private void Animate() {
            if (nextAnim) {
                if (animPointer >= anim.Count) {
                    builtmessage = null;
                    animPointer = 0;
                    nextAnim = false;
                    menu.newMode = Menu.MenuMode.ACTION;
                    menu.useTimer = true;
                    anim.Clear();
                    phasecounter.SetPhase(0, null);
                    return;
                }
                SceneAnimation an = anim[animPointer];
                switch (an.ID) {
                    case 0: //Add message
                    message = an.Message;
                    doTimer = true;
                    nextAnim = false;
                    break;
                    case 1: //Damage Given Animation
                    if (an.values[0] == 1) {
                        leftbar.totalHealth = (int)an.values[1];
                        leftbar.health = (int)an.values[2];
                        leftbar.useTimer = true;
                    } else {
                        rightbar.totalHealth = (int)an.values[1];
                        rightbar.health = (int)an.values[2];
                        rightbar.useTimer = true;

                    }
                    nextAnim = false;
                    break;
                    case 2: // Attack Animation
                    nextAnim = true;
                    break;
                    case 3: //Arena Animation
                    arena.rot = (int)an.values[0];
                    effects.rot = (int)an.values[0];
                    menu.rot = (int)an.values[0];
                    nextAnim = false;
                    break;
                    case 4: //Phase Animation
                    phasecounter.SetPhase((byte)an.values[0], this);
                    nextAnim = false;
                    break;
                    case 5: //Death Animation
                    Console.WriteLine("Default case");
                    break;
                    case 6: //Condition Animation
                    if (an.values[0] == 1) {
                        leftbar.st = (byte)(an.values[1] + 1);
                    } else {
                        rightbar.st = (byte)(an.values[1] + 1);
                    }
                    break;
                    case 7:
                    Console.WriteLine("Default case");
                    break;
                    case 8:
                    Console.WriteLine("Default case");
                    break;
                    case 9:
                    Console.WriteLine("Default case");
                    break;
                }
                animPointer += 1;
            }
        }

        protected override void Initialize() {
            anim = new List<SceneAnimation>();
            engine = new BattleEngine();

            engine.Player2.ActiveCreature.CreatureType.BaseStats[CreatureStats.PACE] -= 1;
            engine.Player2.ActiveCreature.CreatureType.BaseStats[CreatureStats.AWE] -= 1;


            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            arena = new Arena(Content);
            effects = new ArenaEffects(Content);
            menu = new Menu(Content);
            menu.g = this;
            phasecounter = new Phase(Content);
            leftbar = new LeftBar(Content, this);
            leftbar.setMaxHealth(engine.Player1.ActiveCreature.GetTotalStat(CreatureStats.HEALTH));
            rightbar = new RightBar(Content, this);
            rightbar.setMaxHealth(engine.Player1.ActiveCreature.GetTotalStat(CreatureStats.HEALTH));

            font = Content.Load<SpriteFont>("File");

            user = new Battler(Content, "user2");
            opponent = new Battler(Content, "opponent2");

            background = Content.Load<Texture2D>("bkground2");
            shade = Content.Load<Texture2D>("shade");
        }

        protected override void UnloadContent() {
        }

        double timer = 0;
        bool doTimer = false;
        String builtmessage;

        protected override void Update(GameTime gameTime) {
   
            double dt = gameTime.ElapsedGameTime.TotalSeconds;

            if (doTimer) {
                timer += dt;
                if (timer < 0.75) {
                    builtmessage = message.Substring(0, (int)(timer * message.Length / 0.75));
                } else {
                    builtmessage = message;
                }
                if (timer > 1.5) {
                    timer = 0;
                    doTimer = false;
                    nextAnim = true;
                }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState state = Mouse.GetState();
            menu.Update(dt, state.LeftButton == ButtonState.Pressed, state.X, state.Y);
            phasecounter.Update(dt);
            arena.Rotate(dt, this);
            menu.Rotate(dt);
            effects.Rotate(dt);
            effects.Update(dt);
            leftbar.Update(dt);
            rightbar.Update(dt);

            if (chosen) {
                Console.WriteLine(menu.chosenAction);
                Console.WriteLine(menu.chosenMovement);
                engine.Player1.SelectAction((byte)(menu.chosenAction-1));
                engine.Player1.SelectMovement((MovementCategory)menu.chosenMovement);
                engine.Player2.SelectAction(0);
				engine.Player2.SelectMovement(MovementCategory.DO_NOTHING);
                for (int i = 0; i < 10; i++) {
                    List<SceneAnimation> a = engine.Poll();
                    if (a == null) {
                        Console.WriteLine("NULL");
                    } else {
                        anim.AddRange(a);
                    }
                }
                PrintAnim();
                chosen = false;
                nextAnim = true;
            }
            Animate();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(background, Vector2.Zero);
            spriteBatch.End();

            arena.Draw();

            spriteBatch.Begin();
            spriteBatch.Draw(shade, Vector2.Zero);

            effects.DrawBackground(spriteBatch);

            user.Draw(spriteBatch, 0);
            opponent.Draw(spriteBatch, 464);

            effects.DrawForeground(spriteBatch);


            leftbar.Draw(spriteBatch, 7, 22);
            rightbar.Draw(spriteBatch, 720 - 336 - 7, 22);
            menu.Draw(spriteBatch);
            phasecounter.Draw(spriteBatch);


            if (builtmessage != null) {
                spriteBatch.DrawString(font, builtmessage, new Vector2(250, 430), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
