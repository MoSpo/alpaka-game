using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka {
	public class Arena {

        private Model model;
        private Texture2D tex;

        Matrix[] modelTransforms;

        double angle;

        public int rot;
        byte pos;

        double timer;

        bool rotating = false;

        public Arena(ContentManager Content)
        {
            tex = Content.Load<Texture2D>("bottom");
            model = Content.Load<Model>("bottommod");

            modelTransforms = new Matrix[model.Bones.Count];
            model.CopyBoneTransformsTo(modelTransforms);

            pos = 0;
            angle = 0;
            rot = 0;
            timer = 0;
        }

        public bool UpdateArenaRot(double dt, Game1 g)
        {
            if (rot != 0)
            {
                timer += dt;
                if (timer >= 1)
                {
                    timer = 0;
                    if (rot > 0)
                    {
                        rot--;
                        pos++;
                        angle = pos * Math.PI / 4;
                    }
                    else if (rot < 0)
                    {
                        rot++;
                        pos--;
                        angle = pos * Math.PI / 4;
                    }
                    if (pos == 255)
                    {
                        pos = 7;
                    }
                    else if (pos == 8)
                    {
                        pos = 0;
                    }
                }
                if (rot > 0)
                {
                    angle += dt * Math.PI / 4;
                }
                else if (rot < 0)
                {
                    angle -= dt * Math.PI / 4;
                }
                rotating = true;
                return false;
            }
            else
            {
                if (rotating) {
                    rotating = false;
                    g.nextAnim = true;
                }
                return true;
            }
        }


        public void Draw()
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {

                    effect.TextureEnabled = true;
                    effect.Texture = tex;
                    effect.World = Matrix.CreateFromAxisAngle(Vector3.Forward, (float)angle) * modelTransforms[mesh.ParentBone.Index];
                    effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.1f, -0.6f), new Vector3(0.0f, 0.0f, 0.8f), Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), 1.6f, 0.1f, 10000.0f);

                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f);
                    effect.DirectionalLight0.Direction = new Vector3(0, -1, 0);
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                }

                mesh.Draw();
            }

        }
    }
}
