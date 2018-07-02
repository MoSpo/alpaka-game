﻿using Microsoft.Xna.Framework;
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

        double GameSpeed = 0.7;

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
				Console.Write(an.Type + ": ");
                if (an.Values != null) {
					foreach (double i in an.Values) {
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
                switch (an.Type) {
					case SceneAnimation.SceneAnimationType.ADD_MESSAGE:
                    message = an.Message;
                    doTimer = true;
                    nextAnim = false;
                    break;
					case SceneAnimation.SceneAnimationType.HEALTH_BAR:
                    if (an.Values[0] == 1) {
                        leftbar.setHealth((int)an.Values[2]);
                    } else {
                        rightbar.setHealth((int)an.Values[2]);
                    }
                    nextAnim = false;
                    break;
                    case SceneAnimation.SceneAnimationType.KIN_BAR:
                    if (an.Values[0] == 1) {
                        leftbar.setKin((int)an.Values[2]);
                    } else {
                        rightbar.setKin((int)an.Values[2]);
                    }
                    nextAnim = false;
                    break;

                    case SceneAnimation.SceneAnimationType.ATTACK:
                    nextAnim = true;
                    break;
					case SceneAnimation.SceneAnimationType.ARENA:
                    arena.rot = (int)an.Values[0];
                    effects.rot = (int)an.Values[0];
                    menu.rot = (int)an.Values[0];
                    nextAnim = false;
                    break;
                    case SceneAnimation.SceneAnimationType.PHASE:
                    phasecounter.SetPhase((byte)an.Values[0], this);
                    nextAnim = false;
                    break;
					case SceneAnimation.SceneAnimationType.DEATH:
                    Console.WriteLine("Default case");
                    break;
					case SceneAnimation.SceneAnimationType.CONDITION:
                    if (an.Values[0] == 1) {
                        leftbar.st = (byte)(an.Values[1] + 1);
                    } else {
                        rightbar.st = (byte)(an.Values[1] + 1);
                    }
                    break;
					case SceneAnimation.SceneAnimationType.ADD_EFFECT:
						effects.Add((int)an.Values[0], an.Message);
                    break;
					case SceneAnimation.SceneAnimationType.REMOVE_EFFECT:
						effects.Remove((int)an.Values[0], an.Message);
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

			font = Content.Load<SpriteFont>("File");

            arena = new Arena(Content);
            effects = new ArenaEffects(Content, font);
            menu = new Menu(Content);
            menu.g = this;
            phasecounter = new Phase(Content);
            leftbar = new LeftBar(Content, this);
            leftbar.setMaxHealth(engine.Player1.ActiveCreature.GetTotalStat(CreatureStats.HEALTH));
            leftbar.setMaxKin(1000);
            leftbar.setElements((byte)engine.Player1.ActiveCreature.CreatureType.Elements[0], (byte)engine.Player1.ActiveCreature.CreatureType.Elements[1], (byte)engine.Player1.ActiveCreature.CreatureType.Elements[2]);
            leftbar.name = engine.Player1.ActiveCreature.Nickname;
            rightbar = new RightBar(Content, this);
            rightbar.setMaxHealth(engine.Player2.ActiveCreature.GetTotalStat(CreatureStats.HEALTH));
            rightbar.setMaxKin(1000);
            rightbar.setElements((byte)engine.Player2.ActiveCreature.CreatureType.Elements[0], (byte)engine.Player2.ActiveCreature.CreatureType.Elements[1], (byte)engine.Player2.ActiveCreature.CreatureType.Elements[2]);
            rightbar.name = engine.Player2.ActiveCreature.Nickname;

            user = new Battler(Content, "user2");
            opponent = new Battler(Content, "opponent2");

            background = Content.Load<Texture2D>("bkground2");
            shade = Content.Load<Texture2D>("shade");
        }

        protected override void UnloadContent() {
        }

        double floattimer = 0;

        double timer = 0;
        bool doTimer = false;
        String builtmessage;

        protected override void Update(GameTime gameTime) {
   
            double dt = gameTime.ElapsedGameTime.TotalSeconds/ GameSpeed;

           /* floattimer += dt*5;
            if (floattimer > Math.PI*2) {
                floattimer = 0;
            }*/

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
                engine.Player2.SelectAction(1);
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


            leftbar.Draw(spriteBatch, 7, 22 + (int)(Math.Sin(floattimer)*2));
            rightbar.Draw(spriteBatch, 720 - 336 - 7, 22 + (int)(Math.Sin(floattimer) * 2));
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
