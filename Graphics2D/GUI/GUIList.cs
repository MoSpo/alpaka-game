using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Graphics2D.GUI {
    class GUIList : GUIComponent {

   // object.scrollable = true
        int scrollIndex;
        int scrollMax;

        int displayAmount;// = 3

        //object.selectList = nil
        //object.setnil = false

        List<Object2D> list;

        public override void MouseOver(int x, int y) {
        }
    }
}
