using Alpaka.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D {

    public delegate void ObjectEventHandler(Object2D Object);

    public class Object2D {

        private List<Object2D> childObjects;
        public Animator animator;
        private bool IsMouseOver = false;

        public event ObjectEventHandler SelectedEvent;
        public event ObjectEventHandler DeselectedEvent;
        public event ObjectEventHandler PressedEvent;

        public DiscreteVector2 position;
        private Drawable drawable;
        public Color tintColor;

        public bool visable = true;
        public bool selectable = true;
        //private bool drawLater = false;

        public Object2D(Drawable drawable, int x, int y, Object2D child = null) {
            position = new DiscreteVector2(x, y);
            animator = new Animator();
            childObjects = new List<Object2D>();
            if(child != null) {
                childObjects.Add(child);
            }
            this.drawable = drawable;
            tintColor = Color.White;
        }

        public void Update(double dt) {
            animator.Update(dt);
            drawable.Update(dt);
        }


        public void Draw(SpriteBatch spriteBatch) {
            if (visable) {
                drawable.Draw(spriteBatch, position.X, position.Y, tintColor, selectable & IsMouseOver);
                foreach(Object2D oj2 in childObjects) {
                    oj2.Draw(spriteBatch, position.X, position.Y, tintColor);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int x, int y, Color ParentColor) {
            Color newColor = new Color(ParentColor.ToVector4() * tintColor.ToVector4());
            if (visable) {
                drawable.Draw(spriteBatch, x + position.X, y + position.Y, newColor, selectable & IsMouseOver);
                foreach (Object2D oj2 in childObjects) {
                    oj2.Draw(spriteBatch, x + position.X, y + position.Y, newColor);
                }
            }
        }

        public void OnButtonPress(int x) {
            if (selectable & IsMouseOver) {
                PressedEvent(this);
            }
        }

        public void PlayAnimation(byte AnimationIndex) {
            drawable.Animate(AnimationIndex);
        }

        public bool MouseOver(int x, int y) {
            int localX = x - position.X;
            int localY = y - position.Y;

            if (drawable.MouseOver(localX, localY)) {
                if (IsMouseOver) {
                    //DO STUFF HERE
                } else {
                    IsMouseOver = true;
                    if (SelectedEvent != null) {
                        SelectedEvent(this);
                    }
                }
                return true;
            } else {
                if (IsMouseOver) {
                    IsMouseOver = false;
                    if (DeselectedEvent != null) {
                        DeselectedEvent(this);
                    }
                }
                return false;
            }
        }

        public void AddChild(Object2D child) {
            childObjects.Add(child);
        }

        public void AddTextureChild(Object2D child, int index) {
            child.position = drawable.GetChildPosition(index);
            childObjects.Add(child);
        }
    }
}
