using System.Collections.Generic;
using System.Linq;

namespace Alpaka.Scenes.Battle {
    class EffectScript {

		public EffectTrigger Trigger;
		public byte Speed;
        public CreatureElement Element;
        public List<byte> Script;

		public EffectScript(EffectTrigger Trigger, byte Speed, byte[] Script) {
			this.Trigger = Trigger;
			this.Speed = Speed;
            this.Script = Script.ToList();
        }
    }

	enum EffectTrigger {
		ON_EFFECT_ENTER, // when this effect enters play run this effect script
		ON_EFFECT_EXIT, // when this effect leaves play run this effect script
		ON_EFFECT_TIMEOUT, // when this effect leaves play naturally after timing out run this effect script
		ON_OTHER_EFFECT_ENTER, // when another effect enters play run this effect script
		ON_OTHER_EFFECT_EXIT, // when another effect leaves play run this effect script
		ON_OTHER_EFFECT_TIMEOUT, // when another effect leaves play naturally after timing out run this effect script
		ON_STAND_ENTER, // when someone stands on the tile this effect is on run this effect script
		ON_STAND_EXIT, // when someone leaves the tile this effect is on run this effect script
		ON_NEW_TILE_PLACED, // when a new tile is placed in the arena run this effect script
		ON_NEW_TILE_HERE,  // when a new tile is placed on the tile this effect is on run this effect script
		BEFORE_MOVEMENT,
		AFTER_MOVEMENT,
		ON_YOUR_SWITCH,
		ON_OPPONENT_SWITCH,
        ON_YOUR_DEATH,
		ON_OPPONENT_DEATH,
        BEFORE_ACTION,
        AFTER_ACTION,
        BEFORE_ATTACKING,
        AFTER_ATTACKING,
        BEFORE_ATTACKED,
        AFTER_ATTACKED,
		AFTER_EFFECT_TRIGGER, // when an effect is triggered run this effect script
		EACHTURN_UNTIL_EFFECT_END

    }
}
