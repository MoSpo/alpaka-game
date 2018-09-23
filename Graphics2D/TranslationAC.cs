using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D {
    class TranslationAC : AnimationComponent {

        private DiscreteVector2 start;
        private DiscreteVector2 finish;

        public TranslationAC(Object2D objectReference, double animationLength, double startTime, DiscreteVector2 start, DiscreteVector2 finish) {
            this.objectReference = objectReference;
            this.animationLength = animationLength;
            this.animationStartTime = startTime;
            Stopped = false;
            this.start = start;
            this.finish = finish;
        }

        public override void Update(double t) {
            objectReference.position.X = (int)(start.X + (finish.X - start.X) * Bezier(t / animationLength));
            objectReference.position.Y = (int)(start.Y + (finish.Y - start.Y) * Bezier(t / animationLength));
        }

        public override void Finish() {
            objectReference.position.X = finish.X;
            objectReference.position.Y = finish.Y;
            Stopped = true;
        }

        private double Linear(double t) {
            return t;
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
