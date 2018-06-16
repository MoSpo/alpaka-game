using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D {

    abstract class AnimationComponent {
        protected Object2D objectReference;
        public double animationLength;
        public double animationStartTime;
        public bool Stopped;

        abstract public void Update(double t);

        abstract public void Finish();

    }
}
