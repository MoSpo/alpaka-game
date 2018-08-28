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
        NetworkClient client;
        Badai ai;

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

        bool IsUserDeathTurn = false;
        bool IsOpponentDeathTurn = false;

        List<SceneAnimation> anim;
        int animPointer = 0;

        double GameSpeed = 0.5;
        int TurnNumber = 1;
        public bool chosen = false;

        string message;

        public bool nextAnim = false;

        public bool isOnline = true;

        public bool userchosen = false;
        public bool firstready = false;
        public bool play = false;

        int j;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 540;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        private void PrintAnim() {
                string s = "";
                foreach (SceneAnimation an in anim) {
                    Console.Write(an.Type + ": ");
                    s += an.Type + ": ";
                    if (an.Values != null) {
                        foreach (double i in an.Values) {
                            Console.Write(i + " ");
                            s += i + " ";
                        }
                    }
                    Console.WriteLine("[" + an.Message + "]");
                    s += "[" + an.Message + "]\r\n";
                }
            if (isOnline) client.SendDebug(s);
        }

        public string getTeam(byte i) {
            return engine.Player1.Team[i].Nickname;
        }
        public bool getCanUseTeam(byte i) {
            return !engine.Player1.Team[i].killed && engine.Player1.Team[i] != engine.Player1.ActiveCreature;
        }
        public string getAction(byte i) {
            return engine.Player1.ActiveCreature.GetAction(i).Name;
        }
        public string getCanUseAction(byte i) {
            return engine.Player1.ActiveCreature.GetActionUsage(i) + "\n" + engine.Player1.ActiveCreature.GetKin(i);
        }
        public bool getCanUse(byte i) {
            return engine.Player1.CanSelectAction(i);
        }
        public int getActionElement(byte i) {
            return (int)engine.Player1.ActiveCreature.GetAction(i).Element;
        }
        public int getActionType(byte i) {
            return (int)engine.Player1.ActiveCreature.GetAction(i).Catagory;
        }
        public bool getSwitch(byte i) {
            return engine.Player1.ActiveCreature.GetAction(i).IsSwitch;
        }
        public bool getOppCanUseTeam(byte i) {
            return !engine.Player2.Team[i].killed && engine.Player2.Team[i] != engine.Player2.ActiveCreature;
        }
        public bool getOppCanUse(byte i) {
            return engine.Player2.CanSelectAction(i);
        }


        public void setMenu() {
            if (IsUserDeathTurn) {
                menu.newMode = Menu.MenuMode.CLOSED_CHOICE;
            } else if (IsOpponentDeathTurn) {
                chosen = true;
            } else {
                menu.newMode = Menu.MenuMode.ACTION;
            }
            menu.useTimer = true;
            anim.Clear();
            phasecounter.SetPhase(0, null);
        }

        private void Animate() {
            if (nextAnim) {
                if (animPointer >= anim.Count) {
                    builtmessage = null;
                    animPointer = 0;
                    nextAnim = false;

                    if (!isOnline) setMenu();

                    if (isOnline) client.SendReady();

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
                    if (an.Values[0] == 1) {
                        if (ActionCategory.PHYSICAL == (ActionCategory)an.Values[1] || ActionCategory.MYSTICAL == (ActionCategory)an.Values[1]) opponent.rumble = 100;
                        if (ActionCategory.DEFENSIVE == (ActionCategory)an.Values[1] || ActionCategory.ADAPTIVE == (ActionCategory)an.Values[1]) opponent.rumble = 10;
                        opponent.UseTimer = true;
                    } else {
                        if (ActionCategory.PHYSICAL == (ActionCategory)an.Values[1] || ActionCategory.MYSTICAL == (ActionCategory)an.Values[1]) user.rumble = 100;
                        if (ActionCategory.DEFENSIVE == (ActionCategory)an.Values[1] || ActionCategory.ADAPTIVE == (ActionCategory)an.Values[1]) user.rumble = 10;
                        user.UseTimer = true;
                    }
                    nextAnim = false;
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
                    case SceneAnimation.SceneAnimationType.SWITCH_OUT:
                    if (an.Values[0] == 1) {
                        user.changeID(0);
                    } else {
                        opponent.changeID(0);
                    }

                    break;
                    case SceneAnimation.SceneAnimationType.CONDITION:
                    if (an.Values[0] == 1) {
                        leftbar.st = (byte)(an.Values[1] + 1);
                    } else {
                        rightbar.st = (byte)(an.Values[1] + 1);
                    }
                    break;
                    case SceneAnimation.SceneAnimationType.ADD_EFFECT:
                    effects.Add((int)an.Values[0], an.Message, (int)an.Values[1]);
                    break;
                    case SceneAnimation.SceneAnimationType.REMOVE_EFFECT:
                    effects.Remove((int)an.Values[0], (int)an.Values[1]);
                    break;
                    case SceneAnimation.SceneAnimationType.USER_DEATH_SELECT:
                    IsUserDeathTurn = true;
                    break;
                    case SceneAnimation.SceneAnimationType.OPPONENT_DEATH_SELECT:
                    IsOpponentDeathTurn = true;
                    break;
                    case SceneAnimation.SceneAnimationType.ELEMENT_CHANGE:
                    if (an.Values[0] == 1) {
                        leftbar.setElements((byte)an.Values[1], (byte)an.Values[2], (byte)an.Values[3]);
                    } else {
                        rightbar.setElements((byte)an.Values[1], (byte)an.Values[2], (byte)an.Values[3]);
                    }

                    break;
                    case SceneAnimation.SceneAnimationType.SWITCH_IN:
                    if (an.Values[0] == 1) {
                        leftbar.setElements((byte)an.Values[5], (byte)an.Values[6], (byte)an.Values[7]);
                        leftbar.health = (int)an.Values[2];
                        leftbar.totalHealth = (int)an.Values[3];
                        leftbar.oldhealth = (int)an.Values[2];
                        leftbar.kin = (int)an.Values[4];
                        leftbar.oldkin = (int)an.Values[4];
                        user.changeID((int)an.Values[1]);
                        leftbar.name = an.Message;
                    } else {
                        rightbar.setElements((byte)an.Values[5], (byte)an.Values[6], (byte)an.Values[7]);
                        rightbar.health = (int)an.Values[2];
                        rightbar.totalHealth = (int)an.Values[3];
                        rightbar.oldhealth = (int)an.Values[2];
                        rightbar.kin = (int)an.Values[4];
                        rightbar.oldkin = (int)an.Values[4];
                        opponent.changeID((int)an.Values[1]);
                        rightbar.name = an.Message;
                    }

                    break;
                }
                animPointer += 1;
            }
        }

        protected override void Initialize() {
            if (isOnline) client = new NetworkClient(this);
            anim = new List<SceneAnimation>();
            engine = new BattleEngine();
            ai = new Badai(this);
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
            leftbar.setMaxKin(engine.Player1.ActiveCreature.Kin);
            leftbar.setElements((byte)engine.Player1.ActiveCreature.CreatureType.Elements[0], (byte)engine.Player1.ActiveCreature.CreatureType.Elements[1], (byte)engine.Player1.ActiveCreature.CreatureType.Elements[2]);
            leftbar.name = engine.Player1.ActiveCreature.Nickname;
            rightbar = new RightBar(Content, this);
            rightbar.setMaxHealth(engine.Player2.ActiveCreature.GetTotalStat(CreatureStats.HEALTH));
            rightbar.setMaxKin(engine.Player2.ActiveCreature.Kin);
            rightbar.setElements((byte)engine.Player2.ActiveCreature.CreatureType.Elements[0], (byte)engine.Player2.ActiveCreature.CreatureType.Elements[1], (byte)engine.Player2.ActiveCreature.CreatureType.Elements[2]);
            rightbar.name = engine.Player2.ActiveCreature.Nickname;

            user = new Battler(Content, engine.Player1.ActiveCreature.CreatureType.ID, "battlers", this);
            opponent = new Battler(Content, engine.Player2.ActiveCreature.CreatureType.ID, "battlers", this);

            background = Content.Load<Texture2D>("bkground3");
            shade = Content.Load<Texture2D>("shade");
        }

        protected override void UnloadContent() {
        }

        double floattimer = 0;

        double timer = 0;
        bool doTimer = false;
        String builtmessage;

        protected override void Update(GameTime gameTime) {

            double dt = gameTime.ElapsedGameTime.TotalSeconds / GameSpeed;

            if (isOnline) client.Update(dt);

            if (isOnline && !firstready && Keyboard.GetState().IsKeyDown(Keys.O)) isOnline = false;
            if (Keyboard.GetState().IsKeyDown(Keys.Q)) GameSpeed *= 0.99;
            if (Keyboard.GetState().IsKeyDown(Keys.E)) GameSpeed *= 1.01;

            if (!isOnline || firstready) {

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

                MouseState state = Mouse.GetState();
                menu.Update(dt, state.LeftButton == ButtonState.Pressed, state.X, state.Y);
                phasecounter.Update(dt);
                arena.Rotate(dt, this);
                menu.Rotate(dt);
                effects.Rotate(dt);
                effects.Update(dt);
                leftbar.Update(dt);
                rightbar.Update(dt);
                user.Update(dt);
                opponent.Update(dt);

                if (isOnline) {
                    if (chosen) {
                        if (!userchosen) {
                            if (IsUserDeathTurn) {
                                client.SendInput(0, (byte)MovementCategory.DO_NOTHING, menu.chosenCreature);
                                engine.Player1.SelectAction(0);
                                engine.Player1.SelectMovement(MovementCategory.DO_NOTHING);
                                engine.Player1.SelectCreature((byte)(menu.chosenCreature - 1));
                                IsUserDeathTurn = false;
                                j = 1;

                            } else if (IsOpponentDeathTurn) {
                                client.SendInput(0, (byte)MovementCategory.DO_NOTHING, 0);
                                engine.Player1.SelectAction(0);
                                engine.Player1.SelectMovement(MovementCategory.DO_NOTHING);
                                engine.Player1.SelectCreature(0);
                                IsOpponentDeathTurn = false;
                                j = 1;

                            } else {
                                client.SendInput((byte)(menu.chosenAction - 1), menu.chosenMovement, menu.chosenCreature);
                                engine.Player1.SelectAction((byte)(menu.chosenAction - 1));
                                engine.Player1.SelectMovement((MovementCategory)menu.chosenMovement);
                                if (menu.chosenCreature != 0) engine.Player1.SelectCreature((byte)(menu.chosenCreature - 1));
                                j = 10;
                            }
                            userchosen = true;
                        }
                        if (play) {
                            TurnNumber++;
                            for (int i = 0; i < j; i++) {
                                List<SceneAnimation> a = engine.Poll();
                                if(a != null) anim.AddRange(a);
                            }
                            PrintAnim();
                            chosen = false;
                            userchosen = false;
                            nextAnim = true;
                        }
                    }
                } else {
                    if (chosen) { //has finished with the menu selection
                        if (IsUserDeathTurn) {
                            engine.Player1.SelectAction(0);
                            engine.Player1.SelectMovement(MovementCategory.DO_NOTHING);
                            if (menu.chosenCreature != 0) engine.Player1.SelectCreature((byte)(menu.chosenCreature - 1));
                            engine.Player2.SelectAction(0);
                            engine.Player2.SelectMovement(MovementCategory.DO_NOTHING);
                            engine.Player2.SelectCreature(0);

                            IsUserDeathTurn = false;
                            List<SceneAnimation> a = engine.Poll();
                            anim.AddRange(a);
                        } else if (IsOpponentDeathTurn) {
                            engine.Player1.SelectAction(0);
                            engine.Player1.SelectMovement(MovementCategory.DO_NOTHING);
                            engine.Player1.SelectCreature(0);
                            engine.Player2.SelectAction(0);
                            engine.Player2.SelectMovement(MovementCategory.DO_NOTHING);
                            engine.Player2.SelectCreature(ai.SelectCreature());

                            IsOpponentDeathTurn = false;
                            List<SceneAnimation> a = engine.Poll();
                            anim.AddRange(a);
                        } else {
                            engine.Player1.SelectAction((byte)(menu.chosenAction - 1));
                            engine.Player1.SelectMovement((MovementCategory)menu.chosenMovement);
                            if (menu.chosenCreature != 0) engine.Player1.SelectCreature((byte)(menu.chosenCreature - 1));
                            engine.Player2.SelectAction(ai.SelectAction());
                            engine.Player2.SelectMovement(ai.SelectMovement());
                            engine.Player2.SelectCreature(ai.SelectCreature());
                            Console.WriteLine(TurnNumber);
                            TurnNumber++;
                            for (int i = 0; i < 10; i++) {
                                List<SceneAnimation> a = engine.Poll();
                                anim.AddRange(a);
                            }
                        }
                        PrintAnim();
                        chosen = false;
                        nextAnim = true;
                    }
                }
                Animate();
            }
                base.Update(gameTime);
        }

        public void setOpponentChoice(byte a, byte b, byte c) {
            engine.Player2.SelectAction(a);
            engine.Player2.SelectMovement(b);
            if (c != 0) engine.Player2.SelectCreature((byte)(c - 1));
        }

        public void setServerNumber(byte n) {
            engine.Player1.playerServerNumber = n;
            if(n ==0)  engine.Player2.playerServerNumber = 1;
            if (n == 1) engine.Player2.playerServerNumber = 0;
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            if (!isOnline || firstready) {

                spriteBatch.Begin();
                spriteBatch.Draw(background, Vector2.Zero);
                spriteBatch.End();

                arena.Draw();

                spriteBatch.Begin();
                //spriteBatch.Draw(shade, Vector2.Zero);

                effects.DrawBackground(spriteBatch);

                user.Draw(spriteBatch, 0);
                opponent.Draw(spriteBatch, 464);

                effects.DrawForeground(spriteBatch);


                leftbar.Draw(spriteBatch, 7, 22 + (int)(Math.Sin(floattimer) * 2));
                rightbar.Draw(spriteBatch, 720 - 336 - 7, 22 + (int)(Math.Sin(floattimer) * 2));
                menu.Draw(spriteBatch);
                phasecounter.Draw(spriteBatch);


                if (builtmessage != null) {
                    spriteBatch.DrawString(font, builtmessage, new Vector2(250, 430), Color.Black);
                }

                spriteBatch.End();

            } else if(isOnline){
                spriteBatch.Begin();
                client.Draw(spriteBatch, font);
                spriteBatch.End();
            }
            base.Draw(gameTime);

        }
    }
}
