using Alpaka.Graphics2D;
using Alpaka.Scenes.Battle;
using Alpaka.Scenes.Map;
using Alpaka.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Menus {
    class SettingsMenu : Overlay {

        public SettingsMenu(Overlay previousOverlay) : base(previousOverlay) {
        }

        public SettingsMenu(BattleScene battleScene) : base(battleScene) {
        }

        public SettingsMenu(MapScene mapScene) : base(mapScene) {
        }

        protected override void Initialise() {
            input.SubscribeToKey("Exit", delegate { SceneService.Exit(); });
        }
    }
}
