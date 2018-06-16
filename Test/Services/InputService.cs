using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Services {

    public delegate void InputEventHandler(string inputName);

    public class KeyBinding {
        public bool mouseKey;
        public Keys keyboardKey;
        public Buttons controllerKey;
        public bool keyDown;

        public KeyBinding(bool mouseKey, Keys keyboardKey, Buttons controllerKey) {
            this.mouseKey = mouseKey;
            this.keyboardKey = keyboardKey;
            this.controllerKey = controllerKey;
            this.keyDown = false;
        }
    }

    public class InputService {

        private Dictionary<string, KeyBinding> bindings;
        private MouseState previousMouseState;
        public bool IsController = false;
        public InputEventHandler inputHandler;

        public InputService() {
            bindings = new Dictionary<string, KeyBinding>();
        }

        public InputService(Dictionary<string, KeyBinding> bindings) {
            this.bindings = bindings;
        }

        public void PollInput() {
            if (IsController) {
                GamePadState state = GamePad.GetState(PlayerIndex.One);
                foreach (KeyValuePair<string, KeyBinding> keys in bindings) {
                    if (state.IsButtonDown(keys.Value.controllerKey)) {
                        if (!keys.Value.keyDown) {
                            inputHandler(keys.Key);
                            keys.Value.keyDown = true;
                        }
                    } else {
                        if (keys.Value.keyDown) {
                            keys.Value.keyDown = false;
                        }
                    }
                }
            } else {
                KeyboardState state = Keyboard.GetState();
                MouseState mouseState = Mouse.GetState();
                foreach (KeyValuePair<string, KeyBinding> keys in bindings) {
                    if (state.IsKeyDown(keys.Value.keyboardKey) || (keys.Value.mouseKey && mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed)) {
                        if (!keys.Value.keyDown) {
                            inputHandler(keys.Key);
                            keys.Value.keyDown = true;
                        }
                    } else {
                        if (keys.Value.keyDown) {
                            keys.Value.keyDown = false;
                        }
                    }
                }
                previousMouseState = mouseState;
            }
        }

        public void AddKey(string keyName, bool mouseKey, Keys keyboardKey, Buttons controllerKey) {
            bindings.Add(keyName, new KeyBinding(mouseKey, keyboardKey, controllerKey));
        }

        public void ChangeKey(string keyName, bool mouseKey, Keys keyboardKey, Buttons controllerKey) {
            bindings[keyName].mouseKey = mouseKey;
            bindings[keyName].keyboardKey = keyboardKey;
            bindings[keyName].controllerKey = controllerKey;
        }

        public void ChangeKeyboardKey(string keyName, Keys keyboardKey) {
            bindings[keyName].keyboardKey = keyboardKey;
        }

        public void ChangecontrollerKey(string keyName, Buttons controllerKey) {
            bindings[keyName].controllerKey = controllerKey;
        }

    }
}
