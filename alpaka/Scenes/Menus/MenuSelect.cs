using Alpaka.Graphics2D;
using Alpaka.Scenes.Battle;
using Alpaka.Scenes.Map;
using Alpaka.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Menus {
    class MenuSelect : Overlay {

        public MenuSelect(Overlay previousOverlay) : base(previousOverlay) {
        }

        public MenuSelect(BattleScene battleScene) : base(battleScene) {
        }

        public MenuSelect(MapScene mapScene) : base(mapScene) {
        }


        protected override void Initialise() {

        Texture2D textureAtlas = SceneService.textureAtlas;
        Dictionary<string, TextureData> textureData = SceneService.textureData;

        Animation newanim = new Animation();

            input.SubscribeToKey("Exit", delegate { SceneService.Exit(); });

            input.SubscribeToKey("Start", delegate { newanim.Play = true; });

            input.SubscribeToKey("Reset", delegate {
                objects[0].position.Y = 328 + 128;
                for (int i = 2; i < 10; i++) {
                    objects[i].tintColor = new Color(0, 0, 0, 0);
                }
            });

            for (int i = 0; i < 6; i++) {
                sprites.Add(new Sprite(textureAtlas));
            }

            sprites[0].textureSelection = textureData["OuterBattleOption"];
            sprites[1].textureSelection = textureData["InnerBattleOption"];

            sprites[2].textureSelection = textureData["MoveLeft"];
            sprites[2].animationFrames = 1;
            sprites[2].animationSpeed = 100;
            sprites[3].textureSelection = textureData["MoveRight"];

            sprites[4].textureSelection = textureData["BasicMoveLeft"];
            sprites[5].textureSelection = textureData["BasicMoveRight"];

            objects.Add(new Object2D(sprites[1], 248, 328 + 128));
            objects.Add(new Object2D(sprites[0], 248, 328 + 16 + 128));

            objects.Add(new Object2D(sprites[2], 88 + 50, 357));
            objects.Add(new Object2D(sprites[2], 108 + 50, 416));
            objects.Add(new Object2D(sprites[2], 128 + 50, 475));

            objects.Add(new Object2D(sprites[3], 472 - 50, 357));
            objects.Add(new Object2D(sprites[3], 452 - 50, 416));
            objects.Add(new Object2D(sprites[3], 432 - 50, 475));

            objects.Add(new Object2D(sprites[4], 5 + 50, 475));
            objects.Add(new Object2D(sprites[5], 587 - 50, 475));

            input.SubscribeToKey("Select", objects[2].OnButtonPress);
            objects[2].PressedEvent += delegate {
                objects[0].position.Y = 328 + 128;
                for (int i = 2; i < 10; i++) {
                    objects[i].tintColor = new Color(0, 0, 0, 0);
                }
            };

            input.SubscribeToKey("Select", objects[3].OnButtonPress);
            objects[3].PressedEvent += delegate {
                SceneService.OpenOverlay(new SettingsMenu(this));
            };

            input.SubscribeToKey("Select", objects[4].OnButtonPress);
            objects[4].PressedEvent += delegate {
                //objects[4].Animate();
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
           
        }

        public override void Update(double dt) {

            base.Update(dt);
            MouseState currentMouseState = Mouse.GetState();

            foreach (Object2D ob2 in objects) {
                ob2.Update(dt);
                if (ob2.MouseOver(currentMouseState.X, currentMouseState.Y)) {
                    //Hello
                }
            }

        }

        public override void Draw(SpriteBatch spriteBatch) {
            base.Draw(spriteBatch);
        }
    }
}
