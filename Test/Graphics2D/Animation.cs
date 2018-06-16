using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D {

    public class Animation {

        private List<AnimationComponent> animationSequence;
        public bool Play = false;
        public bool isBackwards = false;
        private double timeElapsed;
        private short finishCount = 0;

        public Animation() {
            animationSequence = new List<AnimationComponent>();
        }

        public void AddTint(Object2D objectReference, double animationLength, double startTime, Color startColor, Color endColor) {
            animationSequence.Add(new TintAC(objectReference, animationLength, startTime, startColor, endColor));
        }

        public void AddTint(Object2D objectReference, double animationLength, double startTime, bool fadeOut) {
            animationSequence.Add(new TintAC(objectReference, animationLength, startTime, new Color(0,0,0,0), new Color(255, 255, 255, 255)));
        }

        public void AddSprite(Object2D objectReference, double startTime, byte animationIndex) {
            animationSequence.Add(new SpriteAC(objectReference, startTime, animationIndex));
        }

        public void AddTransition(Object2D objectReference, double animationLength, double startTime, DiscreteVector2 start, DiscreteVector2 finish) {
            animationSequence.Add(new TranslationAC(objectReference, animationLength, startTime, start, finish));
        }

        public void AddTransition(Object2D objectReference, double animationLength, double startTime, int x, int y) {
            animationSequence.Add(new TranslationAC(objectReference, animationLength, startTime, new DiscreteVector2(objectReference.position), new DiscreteVector2(objectReference.position.X + x, objectReference.position.Y + y)));
        }

        public void Order() {
            //add counting sort
        }

        public void Update(double dt) {
            timeElapsed += dt;
            if (Play) {
                foreach (AnimationComponent AC in animationSequence) {
                    if (timeElapsed >= AC.animationStartTime) {
                        double relativeTime = timeElapsed - AC.animationStartTime;
                        if (relativeTime >= AC.animationLength && !AC.Stopped) {
                            AC.Finish();
                            finishCount++;
                            if(finishCount >= animationSequence.Count) {
                                Play = false;
                                timeElapsed = 0;
                                finishCount = 0;
                                foreach (AnimationComponent endAC in animationSequence) {
                                    endAC.Stopped = false;
                                }
                                break;
                            }
                        } else {
                            if (!AC.Stopped) {
                                AC.Update((relativeTime));
                            }
                        }
                    } else {
                        continue;
                    }
                }
            }
        }
    }
}
