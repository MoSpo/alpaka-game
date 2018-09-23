using System;
using Alpaka.Graphics2D;
using Alpaka.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Alpaka.Scenes.Menus;

namespace Alpaka.Scenes.Battle {

    class BattleScene : Scene {

        BattleEngine battleEngine;

        private Model model;
        private Texture2D tex;

        private List<Object2D> battlething;
		private List<SceneAnimation> battleEffects;

        Matrix[] modelTransforms;

        int rot;
        byte pos;
        double angle;
        double timer;
        double timer2;
        bool decreaseTimer = false;

        public Animation userAnim = new Animation();
        public Animation opponentAnim = new Animation();


        public BattleScene(ContentManager Content) {

            Texture2D textureAtlas = SceneService.textureAtlas;
            Dictionary<string, TextureData> textureData = SceneService.textureData;

            Animation newanim = new Animation();

            battlething = new List<Object2D>();
            Initialise();

            battleEngine = new BattleEngine();
            rot = 0;
            pos = 0;
            angle = 0;
            timer = 0;
            timer2 = 0;


            tex = Content.Load<Texture2D>("MODELS/bottom");
            model = Content.Load<Model>("MODELS/bottommod");
            Texture2D sprite = Content.Load<Texture2D>("strip");
            Texture2D sprite2 = Content.Load<Texture2D>("strip2");
            Texture2D atk = Content.Load<Texture2D>("133");


            modelTransforms = new Matrix[model.Bones.Count];
            model.CopyBoneTransformsTo(modelTransforms);


            input.SubscribeToKey("Exit", delegate { SceneService.Exit(); });

            input.SubscribeToKey("Start", delegate { newanim.Play = true; });

            input.SubscribeToKey("Reset", delegate {
                objects[0].position.Y = 328 + 128;
                for (int i = 2; i < 10; i++) {
                    objects[i].tintColor = new Color(0, 0, 0, 0);
                }
            });

            for (int i = 0; i < 11; i++) {
                sprites.Add(new Sprite(textureAtlas));
            }

            sprites[0].textureSelection = textureData["OuterBattleOption"];
            sprites[1].textureSelection = textureData["InnerBattleOption"];
            sprites[9].textureSelection = textureData["OptionBattleBottom"];

            sprites[2].textureSelection = textureData["MoveLeft"];

            sprites[3].textureSelection = textureData["MoveRight"];

            sprites[4].textureSelection = textureData["BasicMoveLeft"];
            sprites[5].textureSelection = textureData["BasicMoveRight"];

            sprites[6].textureSelection = textureData["BattleHead"];
            sprites[7].textureSelection = textureData["BattleArm"];
            sprites[8].textureSelection = textureData["BattleBarBlue"];
            sprites[9].textureSelection = textureData["BattleBarRed"];
            sprites[10].textureSelection = textureData["BattleBarEmpty"];



            objects.Add(new Object2D(sprites[1], 248, 328 + 128));
            objects.Add(new Object2D(sprites[0], 248, 328 + 16 + 128));

            Text text = new Text(SceneService.font, 50, 30);
            text.SetText(battleEngine.Player1.ActiveCreature.GetAction(0).Name);
            Object2D temp = new Object2D(text, 20, 10);
            temp.tintColor = Color.Black;
            objects.Add(new Object2D(sprites[2], 88 + 50, 357, temp));
            objects.Add(new Object2D(sprites[2], 108 + 50, 416));
            objects.Add(new Object2D(sprites[2], 128 + 50, 475));

            objects.Add(new Object2D(sprites[3], 472 - 50, 357));
            objects.Add(new Object2D(sprites[3], 452 - 50, 416, temp));
            objects.Add(new Object2D(sprites[3], 432 - 50, 475));

            objects.Add(new Object2D(sprites[4], 5 + 50, 475));
            objects.Add(new Object2D(sprites[5], 587 - 50, 475));

            {
                Sprite sptem = new Sprite(atk);
                sptem.textureSelection = new TextureData();
                sptem.textureSelection.Main = new Rectangle2D();
                sptem.textureSelection.Main.X = 0;
                sptem.textureSelection.Main.Y = 0;
                sptem.textureSelection.Main.XSize = 40;
                sptem.textureSelection.Main.YSize = 40;
                sptem.animationFrames = 7;
                sptem.animationSpeed = 1;
                sptem.isAnimating = true;
                objects.Add(new Object2D(sptem, 128, 228));
                objects.Add(new Object2D(new Sprite(sptem), 592, 228));
            }
            {
                Sprite sptem = new Sprite(sprite);
                sptem.textureSelection = new TextureData();
                sptem.textureSelection.Main = new Rectangle2D();
                sptem.textureSelection.Main.X = 0;
                sptem.textureSelection.Main.Y = 0;
                sptem.textureSelection.Main.XSize = 256;
                sptem.textureSelection.Main.YSize = 256;
                sptem.animationFrames = 15;
                sptem.animationSpeed = 1;
                sptem.isAnimating = true;
                objects.Add(new Object2D(sptem, 0, 100));
                objects.Add(new Object2D(new Sprite(sptem), 464, 100));
            }
            /*
            input.SubscribeToKey("Select", objects[2].OnButtonPress);
            objects[2].PressedEvent += delegate {
                objects[0].position.Y = 328 + 128;
                for (int i = 2; i < 10; i++) {
                    objects[i].tintColor = new Color(0, 0, 0, 0);
                }
            };*/

            input.SubscribeToKey("Select", objects[2].OnButtonPress);
            objects[2].PressedEvent += delegate {
				if(battleEngine.Player1.SelectAction(0)) {
					
				} else {
					
				}
            };

            input.SubscribeToKey("Select", objects[3].OnButtonPress);
            objects[3].PressedEvent += delegate {
                Console.WriteLine("h");
                SceneService.OpenOverlay(new SettingsMenu(this));
            };

            input.SubscribeToKey("Select", objects[4].OnButtonPress);
            objects[4].PressedEvent += delegate {
                //objects[12].PlayAnimation(1);
                opponentAnim.Play = true;
            };

            input.SubscribeToKey("Select", objects[5].OnButtonPress);
            objects[5].PressedEvent += delegate {
                battleEngine.Player1.SelectAction(0);
            };

            input.SubscribeToKey("Select", objects[6].OnButtonPress);
            objects[6].PressedEvent += delegate {
                rot -= 2;
            };

            input.SubscribeToKey("Select", objects[7].OnButtonPress);
            objects[7].PressedEvent += delegate {
                decreaseTimer = true;
            };

            for (int i = 2; i < 10; i++) {
                objects[i].tintColor = new Color(0, 0, 0, 0);
            }

            newanim.AddTransition(objects[0], 0.35, 0, new DiscreteVector2(248, 328 + 128), new DiscreteVector2(248, 328));

            newanim.AddTransition(objects[2], 0.2, 0.2, -50, 0);
            newanim.AddTransition(objects[3], 0.2, 0.15, -50, 0);
            newanim.AddTransition(objects[4], 0.2, 0.1, -50, 0);
            newanim.AddTransition(objects[8], 0.2, 0.05, -50, 0);
            newanim.AddTint(objects[2], 0.2, 0.2, false);
            newanim.AddTint(objects[3], 0.2, 0.15, false);
            newanim.AddTint(objects[4], 0.2, 0.1, false);
            newanim.AddTint(objects[8], 0.2, 0.05, false);

            newanim.AddTransition(objects[5], 0.2, 0.2, 50, 0);
            newanim.AddTransition(objects[6], 0.2, 0.15, 50, 0);
            newanim.AddTransition(objects[7], 0.2, 0.1, 50, 0);
            newanim.AddTransition(objects[9], 0.2, 0.05, 50, 0);
            newanim.AddTint(objects[5], 0.2, 0.2, false);
            newanim.AddTint(objects[6], 0.2, 0.15, false);
            newanim.AddTint(objects[7], 0.2, 0.1, false);
            newanim.AddTint(objects[9], 0.2, 0.05, false);

            objects[0].animator.AddAnimation(newanim);

            userAnim.AddSprite(objects[12], 0, 1);
            userAnim.AddSprite(objects[11], 1, 1);

            objects[12].animator.AddAnimation(userAnim);

            opponentAnim.AddSprite(objects[13], 0, 1);
            opponentAnim.AddSprite(objects[10], 1, 1);

            objects[12].animator.AddAnimation(opponentAnim);

        }

        public void Rotate(int dr) {
            rot += dr;
        }

        int effectIndex = 0;
        int effectAmount = 0;

        double timerph = 0;


        public byte selectedTile = 0;

        public override void Update(double dt) {
            base.Update(dt);

            if (decreaseTimer & timer <= 1) {
                timer += dt;
                if (timer > 1) {
                    timer = 1;
                }
            }

            MouseState currentMouseState = Mouse.GetState();

            if (battleEffects == null) {
                battleEffects = battleEngine.Poll();
            } else {
                if (effectAmount == 0) {
                    timerph += dt;
                    if (timerph > 2) {
                        timerph = 0;
                        effectAmount = battleEffects.Count;
                    if (effectAmount == 0) {
                        battleEffects = null;
                    }
                }
                } else {
                    //if (battleEffects[effectIndex].SceneRun(this, dt)) {
                        effectIndex++;
                        if(effectIndex == effectAmount) {
                            battleEffects = null;
                            effectAmount = 0;
                            effectIndex = 0;
                        }
                    //}
                }
            }

            ///
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.LeftShift)) {
                if (state.IsKeyDown(Keys.D0)) {
                    selectedTile = 0;
                } else if (state.IsKeyDown(Keys.D1)) {
                    selectedTile = 1;
                } else if (state.IsKeyDown(Keys.D2)) {
                    selectedTile = 2;
                } else if (state.IsKeyDown(Keys.D3)) {
                    selectedTile = 3;
                } else if (state.IsKeyDown(Keys.D4)) {
                    selectedTile = 4;
                } else if (state.IsKeyDown(Keys.D5)) {
                    selectedTile = 5;
                } else if (state.IsKeyDown(Keys.D6)) {
                    selectedTile = 6;
                } else if (state.IsKeyDown(Keys.D7)) {
                    selectedTile = 7;
                } else if (state.IsKeyDown(Keys.D8)) {
                    selectedTile = 8;
                } else if (state.IsKeyDown(Keys.D9)) {
                    selectedTile = 9;
                } else if (state.IsKeyDown(Keys.OemMinus)) {
                    selectedTile = 10;
                }
            }
            ///

            foreach (Object2D ob2 in objects) {
                ob2.Update(dt);
                if (ob2.MouseOver(currentMouseState.X, currentMouseState.Y)) {
                }
            }
        }

        public override void Draw2D(SpriteBatch spriteBatch) {
            base.Draw2D(spriteBatch);
            //spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(200, 200, 200, 200);
            /*spriteBatch.Draw(SceneService.textureAtlas, new Rectangle(0, 125, 84, (int)(44 + 106 * Bezier(timer))), new Rectangle(224, (int)(362 + 106 * (1 - Bezier(timer))), 84, (int)(44 + 106 * Bezier(timer))), Color.White);

            spriteBatch.Draw(SceneService.textureAtlas, new Rectangle(75, 38, (int)(16 + 66 * Bezier(timer)), 20), new Rectangle((int)(308 + 66 * (1 - Bezier(timer))), 274, (int)(16 + 66 * Bezier(timer)), 18), Color.White);
            spriteBatch.Draw(SceneService.textureAtlas, new Rectangle(75, 61, (int)(16 + 66 * Bezier(timer)), 20), new Rectangle((int)(308 + 66 * (1 - Bezier(timer))), 274, (int)(16 + 66 * Bezier(timer)), 18), Color.White);

            spriteBatch.Draw(SceneService.textureAtlas, new Rectangle(75, 38, (int)(((16 + 66 * Bezier(timer)) * (1 - Bezier(timer2)))), 20), new Rectangle((int)(308 + 66 * (1 - Bezier(timer))), 292, (int)(((16 + 66 * Bezier(timer)) * (1 - Bezier(timer2)))), 18), Color.White);
            spriteBatch.Draw(SceneService.textureAtlas, new Rectangle(75, 61, (int)(((16 + 66 * Bezier(timer)) * (1 - Bezier(timer2)))), 20), new Rectangle((int)(308 + 66 * (1 - Bezier(timer))), 256, (int)(((16 + 66 * Bezier(timer)) * (1 - Bezier(timer2)))), 18), Color.White);

            sprites[6].Draw(spriteBatch, 0, 20, Color.White, false);*/

            spriteBatch.DrawString(SceneService.font, Convert.ToString(battleEngine.Player1.ActiveCreature.CreatureType.Name), new Vector2(5, 5), Color.Black);
            spriteBatch.DrawString(SceneService.font, Convert.ToString(battleEngine.Player1.ActiveCreature.Nickname), new Vector2(5, 15), Color.Black);

            for (int i = 0; i < 8; i++) {
				spriteBatch.DrawString(SceneService.font, ((CreatureStats) i).ToString() +": " + Convert.ToString(battleEngine.Player1.ActiveCreature.GetTotalStat((CreatureStats)i)), new Vector2(5, 50 +10 * i), Color.Black);
            }

			spriteBatch.DrawString(SceneService.font, ("BattleHealth: " + Convert.ToString(battleEngine.Player1.ActiveCreature.Health)), new Vector2(5, 150), Color.Black);
			spriteBatch.DrawString(SceneService.font, ("BattleKin: " + Convert.ToString(battleEngine.Player1.ActiveCreature.Kin)), new Vector2(5, 160), Color.Black);

			if(battleEngine.Player1.SelectedAction != null) {
				spriteBatch.DrawString(SceneService.font, ("ActionName: " + Convert.ToString(battleEngine.Player1.SelectedAction.Name)), new Vector2(5, 170), Color.Black);
				spriteBatch.DrawString(SceneService.font, ("ActionPower: " + Convert.ToString(battleEngine.Player1.SelectedAction.Power)), new Vector2(5, 180), Color.Black);
				spriteBatch.DrawString(SceneService.font, ("ActionMana: " + Convert.ToString(battleEngine.Player1.SelectedAction.Mana)), new Vector2(5, 190), Color.Black);
				spriteBatch.DrawString(SceneService.font, ("Amt Used: " + Convert.ToString(battleEngine.Player1.ActiveCreature.GetActionUsage(battleEngine.Player1.SelectedActionNumber)) +"/" + Convert.ToString(battleEngine.Player1.SelectedAction.Usage)), new Vector2(5, 200), Color.Black);
			}

			spriteBatch.DrawString(SceneService.font, Convert.ToString(battleEngine.Player2.ActiveCreature.CreatureType.Name), new Vector2(557, 5), Color.Black);
            spriteBatch.DrawString(SceneService.font, Convert.ToString(battleEngine.Player2.ActiveCreature.Nickname), new Vector2(557, 15), Color.Black);

            for (int i = 0; i < 8; i++) {
				spriteBatch.DrawString(SceneService.font, ((CreatureStats)i).ToString() + ": " + Convert.ToString(battleEngine.Player2.ActiveCreature.GetTotalStat((CreatureStats)i)), new Vector2(557, 50 + 10 * i), Color.Black);
            }

			spriteBatch.DrawString(SceneService.font, ("BattleHealth: " + Convert.ToString(battleEngine.Player2.ActiveCreature.Health)), new Vector2(557, 150), Color.Black);
			spriteBatch.DrawString(SceneService.font, ("BattleKin: " + Convert.ToString(battleEngine.Player2.ActiveCreature.Kin)), new Vector2(557, 160), Color.Black);

			if (battleEngine.Player2.SelectedAction != null) {
				spriteBatch.DrawString(SceneService.font, ("ActionName: " + Convert.ToString(battleEngine.Player2.SelectedAction.Name)), new Vector2(557, 170), Color.Black);
				spriteBatch.DrawString(SceneService.font, ("ActionPower: " + Convert.ToString(battleEngine.Player2.SelectedAction.Power)), new Vector2(557, 180), Color.Black);
				spriteBatch.DrawString(SceneService.font, ("ActionMana: " + Convert.ToString(battleEngine.Player2.SelectedAction.Mana)), new Vector2(557, 190), Color.Black);
				spriteBatch.DrawString(SceneService.font, ("Amt Used: " + Convert.ToString(battleEngine.Player2.ActiveCreature.GetActionUsage(battleEngine.Player2.SelectedActionNumber)) + "/" + Convert.ToString(battleEngine.Player2.SelectedAction.Usage)), new Vector2(557, 200), Color.Black);
			}


			for(int j = 0; j < 3; j++) {
                    if (battleEngine.AllEffects[selectedTile].effects[j] == null) {
                        spriteBatch.DrawString(SceneService.font, "-null-" , new Vector2(5, 250 + 10 * j), Color.Black);
                    } else {
                        spriteBatch.DrawString(SceneService.font, battleEngine.AllEffects[selectedTile].effects[j].Name, new Vector2(5, 250 + 10 * j), Color.Black);
                    }
                }

            spriteBatch.DrawString(SceneService.font, selectedTile.ToString(), new Vector2(5, 300), Color.Black);

            //spriteBatch.DrawString(SceneService.font, Convert.ToString(battleEngine.BattleMessage), new Vector2(300, 100), Color.Black);
            spriteBatch.DrawString(SceneService.font, Convert.ToString(battleEngine.pollIndex), new Vector2(360, 5), Color.Black);


        }

        public override void Draw3D(GameTime gameTime) {
            base.Draw3D(gameTime);
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {

                    effect.TextureEnabled = true;
                    effect.Texture = tex;
                    effect.World = Matrix.CreateFromAxisAngle(Vector3.Forward, (float)angle) * modelTransforms[mesh.ParentBone.Index];
                    effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.1f, -0.6f), new Vector3(0.0f, 0.0f, 0.8f), Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), 1.6f, 0.1f, 10000.0f);
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }

        public bool UpdateArenaRot(double dt) {
            if (rot != 0) {
                timer2 += dt;
                if (timer2 >= 1) {
                    timer2 = 0;
                    if (rot > 0) {
                        rot--;
                        pos++;
                        angle = pos * Math.PI / 4;
                    } else if (rot < 0) {
                        rot++;
                        pos--;
                        angle = pos * Math.PI / 4;
                    }
                    if (pos == 255) {
                        pos = 7;
                    } else if (pos == 8) {
                        pos = 0;
                    }
                }
                if (rot > 0) {
                    angle += dt * Math.PI / 4;
                } else if (rot < 0) {
                    angle -= dt * Math.PI / 4;
                }
                return false;
            } else {
                return true;
            }
        }


        private double Bezier(double t) {
            double u = 1 - t;
            double tt = t * t;
            double uu = u * u;
            double uuu = uu * u;
            double ttt = tt * t;

            double p = uuu * 0; //first term
            p += 3 * uu * t * 0.1; //second term
            p += 3 * u * tt * 1.0; //third term
            p += ttt * 1.0; //fourth term

            return p;
        }

    }
}