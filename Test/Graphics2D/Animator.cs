using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D {
    public class Animator {
        private List<Animation> animations;

        public Animator() {
            animations = new List<Animation>();
        }

        public void Update(double dt) {
            foreach(Animation anim in animations) {
                if(anim.Play) {
                    anim.Update(dt);
                }
            }
        }

        public void AddAnimation(Animation anim) {
            animations.Add(anim);
        }
    }
}
