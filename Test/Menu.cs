using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka {
    public class Menu {

        Texture2D outershell;
        Texture2D outershellbuttons;
        Texture2D shell;
        Texture2D normalactions;
        Texture2D smallactions;
        Texture2D unusableactions;
        Texture2D disc;
        Texture2D[] discTextures;
        Texture2D arrows;
		Texture2D cross;

        Thing[] middle;

        DiscThing[] discParts;

        ActionThing[] actions;

        public Game1 g;

        public enum MenuMode {
            CLOSED,
            CLOSED_CHOICE,
            MESSAGE,
            ACTION,
            ACTION_CHOICE,
            MOVEMENT
        }

        MenuMode currentMode;
        //MenuMode newMode;
        public MenuMode newMode = MenuMode.ACTION;

        double timer;
        double rotateTimer;

        public bool useTimer = true;
        bool creatureChoose = false;

        public int rot;
        private int currentRot;
        public bool keepRot = false;
        byte spriteRot = 0;


        public byte chosenAction = 0;
        public byte chosenMovement = 0;
        public byte chosenCreature = 0;


        public Menu(ContentManager Content) {
            currentMode = MenuMode.CLOSED;

            timer = 0;
            rotateTimer = 0;
            outershell = Content.Load<Texture2D>("outershell");
            outershellbuttons = Content.Load<Texture2D>("outershellbuttons");
            shell = Content.Load<Texture2D>("shell");
            normalactions = Content.Load<Texture2D>("actions");
            smallactions = Content.Load<Texture2D>("smallactions");
            unusableactions = Content.Load<Texture2D>("noactions");
            disc = Content.Load<Texture2D>("disc");

			arrows = Content.Load<Texture2D>("arrows");
			cross = Content.Load<Texture2D>("cross");

            Texture2D chooser = Content.Load<Texture2D>("chooser");
            Texture2D pointers = Content.Load<Texture2D>("pointers");

            discTextures = new Texture2D[] {
                Content.Load<Texture2D>("discparts/disc0"),
                Content.Load<Texture2D>("discparts/disc1"),
                Content.Load<Texture2D>("discparts/disc2"),
                Content.Load<Texture2D>("discparts/disc3"),
                Content.Load<Texture2D>("discparts/disc4"),
                Content.Load<Texture2D>("discparts/disc5"),
                Content.Load<Texture2D>("discparts/disc6"),
                Content.Load<Texture2D>("discparts/disc7"),
            };

            discParts = new DiscThing[] {
                new DiscThing(discTextures[0], 0, 0, 222, 74),
                new DiscThing(discTextures[1], 0, 0, 222, 74),
                new DiscThing(discTextures[2], 0, 0, 222, 74),
                new DiscThing(discTextures[3], 0, 0, 222, 74),
                new DiscThing(discTextures[4], 0, 0, 222, 74),
                new DiscThing(discTextures[5], 0, 0, 222, 74),
                new DiscThing(discTextures[6], 0, 0, 222, 74),
                new DiscThing(discTextures[7], 0, 0, 222, 74),
            };

            middle = new Thing[] {
                new Thing(shell, 0, 0, 224, 192),
                new Thing(outershell, 0, 0, 224, 224),
                new Thing(disc, 0, 0, 222, 74),

                ////
                new ActionThing(normalactions, 0, 192, 180, 64, true),
                new ActionThing(normalactions, 0, 192, 180, 64, true),
                new ActionThing(normalactions, 0, 192, 180, 64, true),

                new ActionThing(normalactions, 0, 128, 180, 64, false),
                new ActionThing(normalactions, 0, 128, 180, 64, false),
                new ActionThing(normalactions, 0, 128, 180, 64, false),

                new ActionThing(smallactions, 0, 64, 140, 64, true),
                new ActionThing(smallactions, 0, 0, 140, 64, false),
                ////

                new Thing(chooser, 0, 0, 720, 122),
                new Thing(pointers, 64, 0, 86, 50),

                new Thing(outershell, 0, 0, 49, 224),
                new Thing(outershell, 47, 0, 66, 224),
                new Thing(outershell, 111, 0, 66, 224),
                new Thing(outershell, 174, 0, 49, 224),

				new Thing(arrows,0,0, 222, 74),
				new Thing(cross, 0,0,122,122),
				new Thing(cross, 0,0,122,122),
				new Thing(cross, 0,0,122,122),
				new Thing(cross, 0,0,122,122),
				new Thing(cross, 0,0,122,122),
				new Thing(cross, 0,0,122,122),
            };

            actions = new ActionThing[] {
                new ActionThing(normalactions, 0, 192, 180, 64, true),
                new ActionThing(normalactions, 0, 192, 180, 64, true),
                new ActionThing(normalactions, 0, 192, 180, 64, true),

                new ActionThing(normalactions, 0, 128, 180, 64, false),
                new ActionThing(normalactions, 0, 128, 180, 64, false),
                new ActionThing(normalactions, 0, 128, 180, 64, false),

                new ActionThing(smallactions, 0, 64, 130, 64, true),
                new ActionThing(smallactions, 0, 0, 130, 64, false),
            };

            for (int i = 3; i < 11; i++) {
                middle[i] = actions[i - 3];
                middle[i].color = new Color(0, 0, 0, 0);
            }
        }

        public void Update(double dt, bool clicked, int x, int y) {

            if (useTimer) {
                timer += dt;
            }

			if (clicked && !useTimer) {
				if (currentMode == MenuMode.CLOSED) {
					//newMode = MenuMode.ACTION;
					//useTimer = true;

				} else if (currentMode == MenuMode.MESSAGE) {

				} else if (currentMode == MenuMode.ACTION) {

					for (int i = 3; i < 11; i++) {
						if (middle[i].x + middle[i].width - 15 >= x && middle[i].y + middle[i].height - 6 >= y && middle[i].x + 15 <= x && middle[i].y + 6 <= y) {
							if (g.getCanUse((byte)(i - 3))) {
								chosenAction = (byte)(i - 2);
                                if (g.getSwitch((byte)(chosenAction - 1))) {
                                    newMode = MenuMode.ACTION_CHOICE;
                                } else {
                                    newMode = MenuMode.MOVEMENT;
                                }
                                useTimer = true;
                            } else {
								chosenAction = 0;
							}
						} else {
                            if (i < 9) middle[i].texture = normalactions;
                            else middle[i].texture = smallactions;
                        }
					}
				} else if (currentMode == MenuMode.CLOSED_CHOICE) {
					if (y > 430) {
						byte choose = (byte)(Math.Floor((double)x * 6 / 720));
						if (choose < 6 && g.getCanUseTeam(choose)) {
							chosenCreature = (byte)(choose + 1);
							newMode = MenuMode.MESSAGE;
							useTimer = true;
						}
					}
				} else if (currentMode == MenuMode.ACTION_CHOICE) {
					if (y > 430) {
						byte choose = (byte)(Math.Floor((double)x * 6 / 720));
						if (choose < 6 && g.getCanUseTeam(choose)) {
							chosenCreature = (byte)(choose + 1);
							newMode = MenuMode.MOVEMENT;
							useTimer = true;
						}
					} else if (middle[12].x + middle[12].width - 15 >= x && middle[12].y + middle[12].height - 15 >= y && middle[12].x + 15 <= x && middle[12].y + 15 <= y) {
						newMode = MenuMode.ACTION;
						useTimer = true;
					}

				} else if (currentMode == MenuMode.MOVEMENT) {
					if (y > 435 && y < 490) {
						for (int i = 13; i < 17; i++) {
							if (middle[i].x + middle[i].width - 2 >= x && middle[i].x + 2 <= x) {
								middle[i].texture = outershellbuttons;
								chosenMovement = (byte)(i - 12);
								newMode = MenuMode.MESSAGE;
								useTimer = true;
							} else {
								middle[i].texture = outershell;
							}
						}
					} else if (middle[12].x + middle[12].width - 15 >= x && middle[12].y + middle[12].height - 15 >= y && middle[12].x + 15 <= x && middle[12].y + 15 <= y) {
						newMode = MenuMode.ACTION;
						useTimer = true;
					}
				}
			} else if (!clicked && !useTimer) {
				if (currentMode == MenuMode.MOVEMENT) {
					if (y > 435 && y < 490) {
						byte u = 0;
						for (int i = 13; i < 17; i++) {
							if (middle[i].x + middle[i].width - 2 >= x && middle[i].x + 2 <= x) {
								middle[17].sourcey = 74 * (i - 12);
                                if (i == 13 && rot == 0) rot--;
                                if (i == 16 && rot == 0) rot++;
                            } else {
								u++;
                            }
                        }
						if(u >=4) middle[17].sourcey = 0;
						u = 0;
					}
				}
			}
        }


        public void Rotate(double dt) {
            if (rot != 0) {
                rotateTimer += dt;
                if (rotateTimer >= 1) {

                    rotateTimer = 0;
                    if (rot > 0) {
                        rot--;
                        if (keepRot) {
                            currentRot--;
                            if (currentRot < 0) currentRot = 7;
                            RotateEffects(false);
                            keepRot = false;
                        }

                    } else if (rot < 0) {
                        rot++;
                        if (keepRot) {
                            currentRot++;
                            if (currentRot > 7) currentRot = 0;
                            RotateEffects(true);
                            keepRot = false;
                        }
                    }
                    spriteRot = 0;
                }

                if (rot > 0) {
                    spriteRot = (byte)Math.Floor(rotateTimer * 8);
                } else if (rot < 0) {
                    spriteRot = (byte)(7 - Math.Floor(rotateTimer * 8));
                }

            }

        }

        public void AddEffect(int pos) {
            discParts[(8-pos) % 8].IncreaseAmount();
        }
        public void RemoveEffect(int pos) {
            discParts[(8-pos) % 8].DecreaseAmount();
        }

        private void RotateEffects(bool IsRight) {
            if(!IsRight) {
                Texture2D t = discParts[0].texture;
                for (int i = 0; i < 7;  i++) {
                    discParts[i].texture = discParts[i + 1].texture;
                }
                discParts[7].texture = t;
            } else {
                Texture2D t = discParts[7].texture;
                for (int i = 7; i > 0; i--) {
                    discParts[i].texture = discParts[i - 1].texture;
                }
                discParts[0].texture = t;
            }
        }

        public void blend(Thing t, double ti, Color startColor, Color endColor) {
            t.color.R = (byte)(startColor.R + (endColor.R - startColor.R) * ti);
            t.color.B = (byte)(startColor.B + (endColor.B - startColor.B) * ti);
            t.color.G = (byte)(startColor.G + (endColor.G - startColor.G) * ti);
            t.color.A = (byte)(startColor.A + (endColor.A - startColor.A) * ti);
        }


        public void Draw(SpriteBatch spriteBatch) {
            if (useTimer) {
                if (newMode == MenuMode.ACTION) {
                    if (currentMode == MenuMode.CLOSED) {

                        for (int i = 0; i < 8; i++) {
                            actions[i].element = g.getActionElement((byte)i)-1;
                            actions[i].type = g.getActionType((byte)i);
                        }


                        middle[0].Draw(248, 505 - (int)((505 - 368) * Bezier(timer / 0.35)), spriteBatch);
                        middle[1].Draw(248, 475 - (int)((475 - 475) * Bezier(timer / 0.35)), spriteBatch);
                        middle[2].Draw(249, 464 - (int)((464 - 328) * Bezier(timer / 0.35)), spriteBatch);

                        foreach(DiscThing d in discParts) {
                            d.Draw(249, 464 - (int)((464 - 328) * Bezier(timer / 0.35)), spriteBatch);
                        }

                        blend(middle[3], Bezier((timer - 0.2) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[4], Bezier((timer - 0.15) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[5], Bezier((timer - 0.1) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        blend(middle[6], Bezier((timer - 0.2) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[7], Bezier((timer - 0.15) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[8], Bezier((timer - 0.1) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        blend(middle[9], Bezier((timer - 0.05) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[10], Bezier((timer - 0.05) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        middle[3].Draw(80 + 50 - (int)(50 * Bezier((timer - 0.2) / 0.2)), 357, spriteBatch);
                        middle[4].Draw(100 + 50 - (int)(50 * Bezier((timer - 0.15) / 0.2)), 416, spriteBatch);
                        middle[5].Draw(120 + 50 - (int)(50 * Bezier((timer - 0.1) / 0.2)), 475, spriteBatch);

                        middle[6].Draw(464 - 50 + (int)(50 * Bezier((timer - 0.2) / 0.2)), 357, spriteBatch);
                        middle[7].Draw(444 - 50 + (int)(50 * Bezier((timer - 0.15) / 0.2)), 416, spriteBatch);
                        middle[8].Draw(424 - 50 + (int)(50 * Bezier((timer - 0.1) / 0.2)), 475, spriteBatch);

                        middle[9].Draw(50 - (int)(50 * Bezier((timer - 0.05) / 0.2)), 475, spriteBatch);
                        middle[10].Draw(590 - 50 + (int)(50 * Bezier((timer - 0.05) / 0.2)), 475, spriteBatch);
                        if (timer > 1 * 0.2 + 0.2) {
                            timer = 0;
                            currentMode = MenuMode.ACTION;
                            useTimer = false;
                        }
                    } else if (currentMode == MenuMode.ACTION_CHOICE) {
                        middle[0].Draw(248, 368, spriteBatch);
                        middle[1].Draw(248, 475, spriteBatch);
                        middle[2].Draw(249, 328, spriteBatch);

                        foreach (DiscThing d in discParts) {
                            d.Draw(249, 328, spriteBatch);
                        }

                        middle[3].Draw(80, 357, spriteBatch);
                        middle[4].Draw(100, 416, spriteBatch);
                        middle[5].Draw(120, 475, spriteBatch);

                        middle[6].Draw(464, 357, spriteBatch);
                        middle[7].Draw(444, 416, spriteBatch);
                        middle[8].Draw(424, 475, spriteBatch);

                        middle[9].Draw(0, 475, spriteBatch);
                        middle[10].Draw(590, 475, spriteBatch);


                        middle[11].Draw(0, 540 - (int)(122 * Bezier(1 - (timer / 0.2))), spriteBatch);
                        blend(middle[12], Bezier(1 - (timer / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        middle[12].Draw(317, 510 - (int)(122 * Bezier(1 - (timer / 0.2))), spriteBatch);
                        if (timer > 1 * 0.2) {
                            timer = 0;
                            currentMode = MenuMode.ACTION;
                            useTimer = false;
                        }
                    } else if (currentMode == MenuMode.MOVEMENT) {
                        middle[0].Draw(248, 368, spriteBatch);
                        middle[1].Draw(248, (int)(475 - 80 + 80 * Bezier(timer / 0.2)), spriteBatch);
                        middle[2].Draw(249, 328, spriteBatch);

                        middle[2].sourcey = 0;

                        foreach (DiscThing d in discParts) {
                            d.sourcey = 0;
                            d.Draw(249, 328, spriteBatch);
                        }

                        blend(middle[3], Bezier((timer - 0.2) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[4], Bezier((timer - 0.15) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[5], Bezier((timer - 0.1) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        blend(middle[6], Bezier((timer - 0.2) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[7], Bezier((timer - 0.15) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[8], Bezier((timer - 0.1) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        blend(middle[9], Bezier((timer - 0.05) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[10], Bezier((timer - 0.05) / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        middle[3].Draw(80 + 50 - (int)(50 * Bezier((timer - 0.2) / 0.2)), 357, spriteBatch);
                        middle[4].Draw(100 + 50 - (int)(50 * Bezier((timer - 0.15) / 0.2)), 416, spriteBatch);
                        middle[5].Draw(120 + 50 - (int)(50 * Bezier((timer - 0.1) / 0.2)), 475, spriteBatch);

                        middle[6].Draw(464 - 50 + (int)(50 * Bezier((timer - 0.2) / 0.2)), 357, spriteBatch);
                        middle[7].Draw(444 - 50 + (int)(50 * Bezier((timer - 0.15) / 0.2)), 416, spriteBatch);
                        middle[8].Draw(424 - 50 + (int)(50 * Bezier((timer - 0.1) / 0.2)), 475, spriteBatch);

                        middle[9].Draw(50 - (int)(50 * Bezier((timer - 0.05) / 0.2)), 475, spriteBatch);
                        middle[10].Draw(590 - 50 + (int)(50 * Bezier((timer - 0.05) / 0.2)), 475, spriteBatch);

                        blend(middle[12], Bezier(timer / 0.3), new Color(255, 255, 255, 255), new Color(0, 0, 0, 0));
                        middle[12].Draw(317, 510 - 18 + (int)(18 * Bezier(timer / 0.3)), spriteBatch);

                        if (timer > 1 * 0.2 + 0.2) {
                            timer = 0;
                            currentMode = MenuMode.ACTION;
                            useTimer = false;
                        }
                    }
                } else if (newMode == MenuMode.ACTION_CHOICE) {

                    middle[0].Draw(248, 368, spriteBatch);
                    middle[1].Draw(248, 475, spriteBatch);
                    middle[2].Draw(249, 328, spriteBatch);

                    foreach (DiscThing d in discParts) {
                        d.Draw(249, 328, spriteBatch);
                    }

                    middle[3].Draw(80, 357, spriteBatch);
                    middle[4].Draw(100, 416, spriteBatch);
                    middle[5].Draw(120, 475, spriteBatch);

                    middle[6].Draw(464, 357, spriteBatch);
                    middle[7].Draw(444, 416, spriteBatch);
                    middle[8].Draw(424, 475, spriteBatch);

                    middle[9].Draw(0, 475, spriteBatch);
                    middle[10].Draw(590, 475, spriteBatch);


                    middle[11].Draw(0, 540 - (int)(122 * Bezier(timer / 0.2)), spriteBatch);
                    blend(middle[12], Bezier(timer / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                    middle[12].Draw(317, 510 - (int)(122 * Bezier(timer / 0.2)), spriteBatch);
                    if (timer > 1 * 0.2) {
                        timer = 0;
                        currentMode = MenuMode.ACTION_CHOICE;
                        useTimer = false;
                    }
                } else if (newMode == MenuMode.CLOSED_CHOICE) {

                    middle[0].Draw(248, 505, spriteBatch);
                    middle[1].Draw(248, 475, spriteBatch);
                    middle[2].Draw(249, 464, spriteBatch);

                    foreach (DiscThing d in discParts) {
                        d.Draw(249, 464, spriteBatch);
                    }

                    middle[11].Draw(0, 540 - (int)(122 * Bezier(timer / 0.2)), spriteBatch);
                    blend(middle[12], Bezier(timer / 0.2), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                    middle[12].Draw(317, 510 - (int)(122 * Bezier(timer / 0.2)), spriteBatch);
                    if (timer > 1 * 0.2) {
                        timer = 0;
                        currentMode = MenuMode.CLOSED_CHOICE;
                        useTimer = false;
                    }
                } else if (newMode == MenuMode.MOVEMENT) {
                    if (currentMode == MenuMode.ACTION) {
                        middle[0].Draw(248, 368, spriteBatch);
                        middle[1].Draw(248, (int)(475 - 80 * Bezier(timer / 0.2)), spriteBatch);
                        middle[2].Draw(249, 328, spriteBatch);

                        foreach (DiscThing d in discParts) {
                            d.Draw(249, 328, spriteBatch);
                        }

                        blend(middle[3], Bezier(1 - ((timer - 0.2) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[4], Bezier(1 - ((timer - 0.15) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[5], Bezier(1 - ((timer - 0.1) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        blend(middle[6], Bezier(1 - ((timer - 0.2) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[7], Bezier(1 - ((timer - 0.15) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[8], Bezier(1 - ((timer - 0.1) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        blend(middle[9], Bezier(1 - ((timer - 0.05) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[10], Bezier(1 - ((timer - 0.05) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        middle[3].Draw(80 + 50 - (int)(50 * Bezier(1 - ((timer - 0.2) / 0.2))), 357, spriteBatch);
                        middle[4].Draw(100 + 50 - (int)(50 * Bezier(1 - ((timer - 0.15) / 0.2))), 416, spriteBatch);
                        middle[5].Draw(120 + 50 - (int)(50 * Bezier(1 - ((timer - 0.1) / 0.2))), 475, spriteBatch);

                        middle[6].Draw(464 - 50 + (int)(50 * Bezier(1 - ((timer - 0.2) / 0.2))), 357, spriteBatch);
                        middle[7].Draw(444 - 50 + (int)(50 * Bezier(1 - ((timer - 0.15) / 0.2))), 416, spriteBatch);
                        middle[8].Draw(424 - 50 + (int)(50 * Bezier(1 - ((timer - 0.1) / 0.2))), 475, spriteBatch);

                        middle[9].Draw(50 - (int)(50 * Bezier(1 - ((timer - 0.05) / 0.2))), 475, spriteBatch);
                        middle[10].Draw(590 - 50 + (int)(50 * Bezier(1 - ((timer - 0.05) / 0.2))), 475, spriteBatch);

                        blend(middle[12], Bezier(timer / 0.3), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        middle[12].Draw(317, 510 - (int)(18 * Bezier(timer / 0.3)), spriteBatch);

                        if (timer > 1 * 0.2 + 0.2) {
                            timer = 0;
                            currentMode = MenuMode.MOVEMENT;
                            useTimer = false;
                        }
                    } else if (currentMode == MenuMode.ACTION_CHOICE) {
                        middle[0].Draw(248, 368, spriteBatch);
                        middle[1].Draw(248, (int)(475 - 80 * Bezier(timer / 0.2)), spriteBatch);
                        middle[2].Draw(249, 328, spriteBatch);

                        foreach (DiscThing d in discParts) {
                            d.Draw(249, 328, spriteBatch);
                        }

                        blend(middle[3], Bezier(1 - ((timer - 0.2) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[4], Bezier(1 - ((timer - 0.15) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[5], Bezier(1 - ((timer - 0.1) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        blend(middle[6], Bezier(1 - ((timer - 0.2) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[7], Bezier(1 - ((timer - 0.15) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[8], Bezier(1 - ((timer - 0.1) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        blend(middle[9], Bezier(1 - ((timer - 0.05) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        blend(middle[10], Bezier(1 - ((timer - 0.05) / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));

                        middle[3].Draw(80 + 50 - (int)(50 * Bezier(1 - ((timer - 0.2) / 0.2))), 357, spriteBatch);
                        middle[4].Draw(100 + 50 - (int)(50 * Bezier(1 - ((timer - 0.15) / 0.2))), 416, spriteBatch);
                        middle[5].Draw(120 + 50 - (int)(50 * Bezier(1 - ((timer - 0.1) / 0.2))), 475, spriteBatch);

                        middle[6].Draw(464 - 50 + (int)(50 * Bezier(1 - ((timer - 0.2) / 0.2))), 357, spriteBatch);
                        middle[7].Draw(444 - 50 + (int)(50 * Bezier(1 - ((timer - 0.15) / 0.2))), 416, spriteBatch);
                        middle[8].Draw(424 - 50 + (int)(50 * Bezier(1 - ((timer - 0.1) / 0.2))), 475, spriteBatch);

                        middle[9].Draw(50 - (int)(50 * Bezier(1 - ((timer - 0.05) / 0.2))), 475, spriteBatch);
                        middle[10].Draw(590 - 50 + (int)(50 * Bezier(1 - ((timer - 0.05) / 0.2))), 475, spriteBatch);


                        middle[11].Draw(0, 540 - (int)(122 * Bezier(1 - (timer / 0.2))), spriteBatch);
                        middle[12].Draw(317, 492 - (int)(122 * Bezier(1 - (timer / 0.2))), spriteBatch);
                        if (timer > 1 * 0.2 + 0.2) {
                            timer = 0;
                            currentMode = MenuMode.MOVEMENT;
                            useTimer = false;
                        }
                    }
                } else if (newMode == MenuMode.MESSAGE) {
                    if (currentMode == MenuMode.MOVEMENT) {

                        middle[0].Draw(248, 368 + (int)(137 * Bezier(timer / 0.2)), spriteBatch);
                        middle[1].Draw(248, 475 - 80 + (int)(80 * Bezier(timer / 0.2)), spriteBatch);
                        middle[2].Draw(249, 328 + (int)(136 * Bezier(timer / 0.2)), spriteBatch);

                        middle[2].sourcey = 0;

                        foreach (DiscThing d in discParts) {
                            d.sourcey = 0;
                            d.Draw(249, 328 + (int)(136 * Bezier(timer / 0.2)), spriteBatch);
                        }

                        if (timer > 1 * 0.2) {
                            timer = 0;
                            currentMode = MenuMode.MESSAGE;
                            useTimer = false;
                        }
                    } else if(currentMode == MenuMode.CLOSED_CHOICE) {
                        middle[0].Draw(248, 505, spriteBatch);
                        middle[1].Draw(248, 475, spriteBatch);
                        middle[2].Draw(249, 464, spriteBatch);

                        foreach (DiscThing d in discParts) {
                            d.Draw(249, 464, spriteBatch);
                        }

                        middle[11].Draw(0, 540 - (int)(122 * Bezier(1 - (timer / 0.2))), spriteBatch);
                        blend(middle[12], Bezier(1 - (timer / 0.2)), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255));
                        middle[12].Draw(317, 510 - (int)(122 * Bezier(1 - (timer / 0.2))), spriteBatch);
                        if (timer > 1 * 0.2) {
                            timer = 0;
                            currentMode = MenuMode.MESSAGE;
                            useTimer = false;
                        }

                    }
                }
            } else {
                if (currentMode == MenuMode.CLOSED) {

                    middle[0].Draw(248, 505, spriteBatch);
                    middle[1].Draw(248, 475, spriteBatch);
                    middle[2].sourcey = 74 * spriteRot;
                    middle[2].Draw(249, 464, spriteBatch);

                    foreach (DiscThing d in discParts) {
                        d.sourcey = 74 * spriteRot;
                        d.Draw(249, 464, spriteBatch);
                    }

                } else if (currentMode == MenuMode.MESSAGE) {
                    middle[0].Draw(248, 505, spriteBatch);
                    middle[1].Draw(248, 475, spriteBatch);
                    middle[2].Draw(249, 464, spriteBatch);

                    foreach (DiscThing d in discParts) {
                        d.Draw(249, 464, spriteBatch);
                    }

                    g.chosen = true;
                    currentMode = MenuMode.CLOSED;

                } else if (currentMode == MenuMode.ACTION) {

                    for (int i = 3; i < 11; i++) {
                        middle[i].color = new Color(255, 255, 255, 255);
                    }

					for (int i = 3; i < 9; i++) {
						if (!g.getCanUse((byte)(i-3))) {
							middle[i].texture = unusableactions;
						} else {
							middle[i].texture = normalactions;
						}
					}
					for (int i = 0; i < 11; i++) {
                        middle[i].Draw(spriteBatch);
                    }

                    foreach (DiscThing d in discParts) {
                        d.Draw(spriteBatch);
                    }

                    spriteBatch.DrawString(g.font, g.getAction(0), new Vector2(100, 367), Color.Fuchsia);
					spriteBatch.DrawString(g.font, g.getAction(1), new Vector2(120, 426), Color.Fuchsia);
					spriteBatch.DrawString(g.font, g.getAction(2), new Vector2(140, 485), Color.Fuchsia);
					spriteBatch.DrawString(g.font, g.getAction(3), new Vector2(504, 367), Color.Fuchsia);
					spriteBatch.DrawString(g.font, g.getAction(4), new Vector2(484, 426), Color.Fuchsia);
					spriteBatch.DrawString(g.font, g.getAction(5), new Vector2(464, 485), Color.Fuchsia);

					spriteBatch.DrawString(g.font, "Attack", new Vector2(30, 485), Color.Fuchsia);
					spriteBatch.DrawString(g.font, "Switch", new Vector2(624, 485), Color.Fuchsia);

					spriteBatch.DrawString(g.font, g.getCanUseAction(0), new Vector2(203, 381), Color.Blue);
					spriteBatch.DrawString(g.font, g.getCanUseAction(1), new Vector2(223, 440), Color.Blue);
					spriteBatch.DrawString(g.font, g.getCanUseAction(2), new Vector2(243, 499), Color.Blue);
					spriteBatch.DrawString(g.font, g.getCanUseAction(3), new Vector2(502, 381), Color.Blue);
					spriteBatch.DrawString(g.font, g.getCanUseAction(4), new Vector2(482, 440), Color.Blue);
					spriteBatch.DrawString(g.font, g.getCanUseAction(5), new Vector2(462, 499), Color.Blue);


                } else if (currentMode == MenuMode.ACTION_CHOICE) {
                    for (int i = 0; i < 13; i++) {
                        middle[i].Draw(spriteBatch);
                    }

                    foreach (DiscThing d in discParts) {
                        d.Draw(spriteBatch);
                    }

                    for (int i = 18; i < 24; i++) {
                        if (!g.getCanUseTeam((byte)(i - 18))) {
                            middle[i].Draw(120 * (i - 18), 418, spriteBatch);
                        }
                    }

                    spriteBatch.DrawString(g.font, g.getTeam(0), new Vector2(10, 500), Color.Black);
					spriteBatch.DrawString(g.font, g.getTeam(1), new Vector2(130, 500), Color.Black);
					spriteBatch.DrawString(g.font, g.getTeam(2), new Vector2(250, 500), Color.Black);
					spriteBatch.DrawString(g.font, g.getTeam(3), new Vector2(370, 500), Color.Black);
					spriteBatch.DrawString(g.font, g.getTeam(4), new Vector2(490, 500), Color.Black);
					spriteBatch.DrawString(g.font, g.getTeam(5), new Vector2(610, 500), Color.Black);

                } else if (currentMode == MenuMode.CLOSED_CHOICE) {
                    middle[0].Draw(248, 505, spriteBatch);
                    middle[1].Draw(248, 475, spriteBatch);
                    middle[2].Draw(249, 464, spriteBatch);

                    foreach (DiscThing d in discParts) {
                        d.Draw(249, 464, spriteBatch);
                    }

                    middle[11].Draw(spriteBatch);
                    middle[12].Draw(spriteBatch);

					for (int i = 18; i < 24; i++) {
						if (!g.getCanUseTeam((byte)(i - 18))) {
							middle[i].Draw(120 * (i - 18), 418, spriteBatch);
						}
					}

					spriteBatch.DrawString(g.font, g.getTeam(0), new Vector2(10, 500), Color.Black);
					spriteBatch.DrawString(g.font, g.getTeam(1), new Vector2(130, 500), Color.Black);
					spriteBatch.DrawString(g.font, g.getTeam(2), new Vector2(250, 500), Color.Black);
					spriteBatch.DrawString(g.font, g.getTeam(3), new Vector2(370, 500), Color.Black);
					spriteBatch.DrawString(g.font, g.getTeam(4), new Vector2(490, 500), Color.Black);
					spriteBatch.DrawString(g.font, g.getTeam(5), new Vector2(610, 500), Color.Black);

                } else if (currentMode == MenuMode.MOVEMENT) {
                    for (int i = 0; i < 2; i++) {
                        middle[i].Draw(spriteBatch);
                    }
                    middle[2].sourcey = 74 * spriteRot;
                    middle[2].Draw(spriteBatch);

                    foreach (DiscThing d in discParts) {
                        d.sourcey = 74 * spriteRot;
                        d.Draw(spriteBatch);
                    }

                    middle[13].Draw(248, 475 - 80, spriteBatch);
                    middle[14].Draw(248 + 47, 475 - 80, spriteBatch);
                    middle[15].Draw(248 + 111, 475 - 80, spriteBatch);
                    middle[16].Draw(248 + 174, 475 - 80, spriteBatch);

					middle[17].Draw(249, 328, spriteBatch);

                    middle[12].Draw(spriteBatch);

                }
            }
        }

        private double Bezier(double t) {
            if (t < 0) t = 0;
            if (t > 1) t = 1;

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

    public class Thing {

        public Texture2D texture;
        public int sourcex;
        public int sourcey;
        public int width;
        public int height;

        public int x;
        public int y;

        public Color color;

        public Thing(Texture2D texture, int sourcex, int sourcey, int width, int height) {
            this.texture = texture;
            this.sourcex = sourcex;
            this.sourcey = sourcey;
            this.width = width;
            this.height = height;
            color = new Color(255, 255, 255, 255);
        }

        public virtual void Draw(int x, int y, SpriteBatch spriteBatch) {
            this.x = x;
            this.y = y;
            spriteBatch.Draw(texture, new Rectangle(x, y, width, height), new Rectangle(sourcex, sourcey, width, height), color);
        }
        public virtual void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(texture, new Rectangle(x, y, width, height), new Rectangle(sourcex, sourcey, width, height), color);
        }


    }

    public class DiscThing : Thing {

        private int amt;

        public DiscThing(Texture2D texture, int sourcex, int sourcey, int width, int height) : base(texture, sourcex, sourcey, width, height) {
            amt = 0;
        }
        public void IncreaseAmount() {
            if(amt < 3) {
                amt++;
                color = new Color(255 - 5 * amt * amt, 255/(amt+1), 255 *(3-amt)/3, 255);
            }
        }
        public void DecreaseAmount() {
            if (amt > 0) {
                amt--;
                color = new Color(255 - 5 * amt* amt, 255 / (amt + 1), 255 * (3 - amt) / 3, 255);
            }
        }
    }

    public class ActionThing : Thing {

        public int element = 0;
        public int type = 0;
        public bool isRight;

        public ActionThing(Texture2D texture, int sourcex, int sourcey, int width, int height, bool isRight) : base(texture, sourcex, sourcey, width, height) {
            this.isRight = isRight;
        }

        public override void Draw(int x, int y, SpriteBatch spriteBatch) {
            this.x = x;
            this.y = y;
            int width = this.width - 45;
            if (isRight) {
                spriteBatch.Draw(texture, new Rectangle(x + 9, y+4, width, 55), new Rectangle(width + 23 * 4, element * 55, width, 55), color);
                spriteBatch.Draw(texture, new Rectangle(x + 9 + width, y+4, 23, 55), new Rectangle(width * 2 + 23 * 4 + 23 * (3-type), element * 55, 23, 55), color);
            } else {
                spriteBatch.Draw(texture, new Rectangle(x + 9 + 23, y+4, width, 55), new Rectangle(23 * 4, element * 55, width, 55), color);
                spriteBatch.Draw(texture, new Rectangle(x + 9, y+4, 23, 55), new Rectangle(23 * type, element * 55, 23, 55), color);
            }
        }
        public override void Draw(SpriteBatch spriteBatch) {
            int width = this.width - 45;
            if (isRight) {
                spriteBatch.Draw(texture, new Rectangle(x+9, y+4, width, 55), new Rectangle(width + 23 * 4, element * 55, width, 55), color);
                spriteBatch.Draw(texture, new Rectangle(x + 9 + width, y+4, 23, 55), new Rectangle(width*2 + 23*4 + 23 * (3-type), element * 55, 23, 55), color);
            } else {
                spriteBatch.Draw(texture, new Rectangle(x + 9 + 23, y+4, width, 55), new Rectangle(23 * 4, element * 55, width, 55), color);
                spriteBatch.Draw(texture, new Rectangle(x + 9, y+4, 23, 55), new Rectangle(23 * type, element * 55, 23, 55), color);
            }
        }

    }
}