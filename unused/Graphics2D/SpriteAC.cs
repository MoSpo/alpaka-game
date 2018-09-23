using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D {
    class SpriteAC : AnimationComponent{

        //animates texture
        private byte animationIndex;
        private byte animationFrames;
        private float animationSpeed;

        public SpriteAC(Object2D objectReference, double startTime, byte animationIndex) {
            this.objectReference = objectReference;
            this.animationIndex = animationIndex;
            this.animationLength = 0;
            this.animationStartTime = startTime;
        }


        public SpriteAC(Object2D objectReference, double startTime, byte animationIndex, byte animationFrames, float animationSpeed) {
            this.objectReference = objectReference;
            this.animationIndex = animationIndex;
            this.animationFrames = animationFrames;
            this.animationSpeed = animationSpeed;
            this.animationLength = 0;
            this.animationStartTime = startTime;
        }

        public override void Update(double t) {
        }

        public override void Finish() {
            objectReference.PlayAnimation(animationIndex);
            Stopped = true;
        }
    }
}
