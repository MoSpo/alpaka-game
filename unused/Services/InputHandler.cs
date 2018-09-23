using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Services {

    public delegate void KeyEventHandler(int x);

    public class InputHandler {

        private Dictionary<string, KeyEventHandler> bindings;

        public InputHandler() {
            bindings = new Dictionary<string, KeyEventHandler>();
        }

        public void HandleInput(string inputName) {
            if (bindings.ContainsKey(inputName)) {
                bindings[inputName](1);
            }
        }

        public void SubscribeToKey(string keyName, KeyEventHandler keyHandler) {
            if (!bindings.ContainsKey(keyName)) {
                bindings[keyName] = keyHandler;

            } else {
            bindings[keyName] += keyHandler;
        }
        }
    }
}
