using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Services {
    static class ServiceLocator {

        private static AudioService audio;
        private static InputService input;
        //private static SceneService scene;

        public static AudioService getAudioService() {
            return audio;
        }

        public static InputService getInputService() {
            return input;
        }

        /*public static SceneService getSceneService() {
            return scene;
        }*/

    }
}
