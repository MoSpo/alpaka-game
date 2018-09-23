using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D {
    public delegate void OnMouseOver();
    public delegate void PressedEventHandler(int Button);

    abstract class GUIComponent {

        public bool IsPressable;

        public OnMouseOver onMouseOver;
        public event PressedEventHandler PressedEvent;

        public abstract void MouseOver(int x, int y);

        public void Pressed() {
            if (IsPressable) {
               onMouseOver();
               PressedEvent(1);
            }
        }
    }
}
