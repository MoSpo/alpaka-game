using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D {
    class TintAC : AnimationComponent {

        Color startColor;
        Color endColor;

        public TintAC(Object2D objectReference, double animationLength, double startTime, Color startColor, Color endColor) {
            this.objectReference = objectReference;
            this.animationLength = animationLength;
            this.animationStartTime = startTime;
            this.startColor = startColor;
            this.endColor = endColor;
        }

        public override void Update(double t) {
            objectReference.tintColor.R = (byte)(startColor.R + (endColor.R - startColor.R) * Bezier(t / animationLength));
            objectReference.tintColor.B = (byte)(startColor.B + (endColor.B - startColor.B) * Bezier(t / animationLength));
            objectReference.tintColor.G = (byte)(startColor.G + (endColor.G - startColor.G) * Bezier(t / animationLength));
            objectReference.tintColor.A = (byte)(startColor.A + (endColor.A - startColor.A) * Bezier(t / animationLength));
        }

        public override void Finish() {
            objectReference.tintColor.R = endColor.R;
            objectReference.tintColor.B = endColor.B;
            objectReference.tintColor.G = endColor.G;
            objectReference.tintColor.A = endColor.A;
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
