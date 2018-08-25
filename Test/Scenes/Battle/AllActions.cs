	using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Battle {
	class AllActions {
		List<CreatureAction> AllCreatureActionList;

		public CreatureAction GetAction(short ActionID) {
			return AllCreatureActionList[ActionID];
		}

		public AllActions() {
			AllCreatureActionList = HardCodedActions();
		}

		public List<CreatureAction> HardCodedActions() {
			List<CreatureAction> c = new List<CreatureAction>();

			//1//
			c.Add(new CreatureAction(
				"Cosmic Sting", //NAME
				CreatureElement.COSMIC,
				 ActionCategory.PHYSICAL,
				50,          //SPEED
				0,           //PRIORITY MODIFIER
				35,          //POWER
				4,          //USAGE
				2,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.BEFORE_ATTACKING,
														   100, //SPEED
					                                       new byte[1] { 0x00 }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Speed Slash", //NAME
				CreatureElement.WIND,
				 ActionCategory.PHYSICAL,
				90,          //SPEED
				-2,           //PRIORITY MODIFIER
				70,          //POWER
				5,          //USAGE
				2,           //MANA
				null
				//new BattleEffect(
				//	10, //PRIORITY
				//	0, //LIFESPAN
				//    new byte[1] { 9 }, //PLACEMENT
				//	new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
				//										   100, //SPEED
				//	                                       new byte[1] { 0x01 }) }
				//	)
				)
			);

			c.Add(new CreatureAction(
				"Slingshot", //NAME
				CreatureElement.ROCK,
				 ActionCategory.PHYSICAL,
				60,          //SPEED
				0,           //PRIORITY MODIFIER
				50,          //POWER
				3,          //USAGE
				2,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   100, //SPEED
					                                       new byte[1] { 0x02 }) }
					)
				)
			);


			c.Add(new CreatureAction(
				"Metamorph", //NAME
				CreatureElement.WOOD,
				ActionCategory.DEFENSIVE,
				50,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				8,          //USAGE
				0,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   70, //SPEED
					                                       new byte[1] { 0x15 })}
					)
				)
			);

			c.Add(new CreatureAction(
				"Asteroids", //NAME
				CreatureElement.COSMIC,
				ActionCategory.ADAPTIVE,
				50,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				3,           //MANA
				new BattleEffect(
					9, //PRIORITY
					5, //LIFESPAN
					new byte[2] { 2, 5 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.ON_STAND_ENTER,
														   70, //SPEED
					                                       new byte[1] { 0x16 })}
					)
				)
			);

			c.Add(new CreatureAction(
				"Comet Launcher", //NAME
				CreatureElement.COSMIC,
				ActionCategory.ADAPTIVE,
				50,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				3,          //USAGE
				2,           //MANA
				new BattleEffect(
					9, //PRIORITY
					8, //LIFESPAN
					new byte[1] { 0 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.ON_OTHER_EFFECT_TIMEOUT,
														   70, //SPEED
					                                       new byte[1] { 0x17 })}
					)
				)
			);

			//2//

			c.Add(new CreatureAction(
				"Flash Freeze", //NAME
				CreatureElement.ICE,
				ActionCategory.MYSTICAL,
				110,          //SPEED
				0,           //PRIORITY MODIFIER
				240,          //POWER
				3,          //USAGE
				5,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   120, //SPEED
					                                       new byte[1] { 0x06 }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Mind Strike", //NAME
				CreatureElement.MAGIC,
				ActionCategory.MYSTICAL,
				80,          //SPEED
				0,           //PRIORITY MODIFIER
				80,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   90, //SPEED
					                                       new byte[1] { 0x19 }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Water Stream", //NAME
				CreatureElement.WATER,
				ActionCategory.MYSTICAL,
				75,          //SPEED
				0,           //PRIORITY MODIFIER
				80,          //POWER
				6,          //USAGE
				0,           //MANA
				null
				)
			);

			c.Add(new CreatureAction(
				"Ceasefire", //NAME
				CreatureElement.ETHER,
				ActionCategory.DEFENSIVE,
				50,          //SPEED
				3,           //PRIORITY MODIFIER
				0,          //POWER
				2,          //USAGE
				0,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   50, //SPEED
					                                       new byte[1] { 0x08 }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Cold Storage", //NAME
				CreatureElement.ICE,
				ActionCategory.ADAPTIVE,
				50,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				2,           //MANA
				new BattleEffect(
					9, //PRIORITY
					10, //LIFESPAN
					new byte[3] { 3, 4, 5 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.ON_STAND_ENTER,
														   50, //SPEED
					                                       new byte[1] { 0x09 }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Healing Bell", //NAME
				CreatureElement.ETHER,
				ActionCategory.DEFENSIVE,
				55,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				2,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   55, //SPEED
					                                       new byte[1] { 0x0A }) }
					)
				)
			);

			//3//
			c.Add(new CreatureAction(
				"Smoulder Smash", //NAME
				CreatureElement.FIRE,
				ActionCategory.PHYSICAL,
				80,          //SPEED
				0,           //PRIORITY MODIFIER
				90,          //POWER
				10,          //USAGE
				0,//4,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   55, //SPEED
					                                       new byte[1] { 0x0C }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Clobber", //NAME
				CreatureElement.ROCK,
				ActionCategory.PHYSICAL,
				55,          //SPEED
				0,           //PRIORITY MODIFIER
				60,          //POWER
				10,          //USAGE
				0,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   55, //SPEED
					                                       new byte[1] { 0x0D }) }
					)
				)
			);
			c.Add(new CreatureAction(
				"Ancient Roar", //NAME
				CreatureElement.SOUND,
				ActionCategory.PHYSICAL,
				85,          //SPEED
				1,           //PRIORITY MODIFIER
				60,          //POWER
				5,          //USAGE
				0,//2,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   55, //SPEED
					                                       new byte[1] { 0x0E }) }
					)
				)
			);
			c.Add(new CreatureAction(
				"Polish", //NAME
				CreatureElement.STEEL,
				ActionCategory.DEFENSIVE,
				55,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				0,//2,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   55, //SPEED
					                                       new byte[1] { 0x0F }) }
					)
				)
			);
			c.Add(new CreatureAction(
				"Kindle", //NAME
				CreatureElement.FIRE,
				ActionCategory.DEFENSIVE,
				80,          //SPEED
				0,           //PRIORITY
				0,          //POWER
				3,          //USAGE
				0,//1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   55, //SPEED
					                                       new byte[1] { 0x10 }) }
					)
				)
			);
			c.Add(new CreatureAction(
				"Deter", //NAME
				CreatureElement.LIGHT,
				ActionCategory.DEFENSIVE,
				55,          //SPEED
				-3,           //PRIORITY MODIFIER
				0,          //POWER
				2,          //USAGE
				0,//3,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   55, //SPEED
					                                       new byte[1] { 0x11 }) }
					)
				)
			);

			//4//
			c.Add(new CreatureAction(
				"Sleet Hammer", //NAME
				CreatureElement.ICE,
				ActionCategory.PHYSICAL,
				70,          //SPEED
				-1,           //PRIORITY MODIFIER
				100,          //POWER
				5,          //USAGE
				2,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   55, //SPEED
					                                       new byte[1] { 0x12 }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Kamakazi", //NAME
				CreatureElement.FIRE,
				ActionCategory.PHYSICAL,
				70,          //SPEED
				3,           //PRIORITY MODIFIER
				255,          //POWER
				1,          //USAGE
				3,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   55, //SPEED
					                                       new byte[1] { 0x14 }) }
					)
				)
			);
			c.Add(new CreatureAction(
				"Earth Grind", //NAME
				CreatureElement.EARTH,
				ActionCategory.PHYSICAL,
				70,          //SPEED
				0,           //PRIORITY MODIFIER
				100,          //POWER
				5,          //USAGE
				1,           //MANA
				null
				)
			);

			c.Add(new CreatureAction(
				"Windtunnel", //NAME
				CreatureElement.WIND,
				ActionCategory.ADAPTIVE,
				40,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				3,          //USAGE
				1,           //MANA
				new BattleEffect(
					9, //PRIORITY
					7, //LIFESPAN
					new byte[4] { 1, 2, 6, 7 }, //PLACEMENT
					new EffectScript[2] { new EffectScript(EffectTrigger.ON_STAND_ENTER,
														   70, //SPEED
					                                       new byte[1] { 0x13 }),
						new EffectScript(EffectTrigger.ON_STAND_EXIT,
														   68, //SPEED
														   new byte[1] { 0x1A }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Spring Of Illusion", //NAME
				CreatureElement.WATER,
				ActionCategory.ADAPTIVE,
				40,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				1,          //USAGE
				1,           //MANA
				new BattleEffect(
					9, //PRIORITY
					5, //LIFESPAN
					new byte[1] { 0 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   60, //SPEED
														   new byte[1] { 0x1B }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Motivator", //NAME
				CreatureElement.ETHER,
				ActionCategory.ADAPTIVE,
				40,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				4,          //USAGE
				0,           //MANA
				new BattleEffect(
					9, //PRIORITY
					5, //LIFESPAN
					new byte[3] { 0, 1, 7 }, //PLACEMENT
					new EffectScript[2] { new EffectScript(EffectTrigger.ON_STAND_ENTER,
														   70, //SPEED
					                                       new byte[1] { 0x1C }),
						new EffectScript(EffectTrigger.ON_STAND_EXIT,
														   68, //SPEED
														   new byte[1] { 0x1D }) }
					)
				)
			);

			//5//
			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.FIRE,
				ActionCategory.MYSTICAL,
				70,          //SPEED
				0,           //PRIORITY MODIFIER
				80,          //POWER
				5,          //USAGE
				0,           //MANA
				null
				)
		   );

			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.NUCLEAR,
				ActionCategory.PHYSICAL,
				90,          //SPEED
				0,           //PRIORITY MODIFIER
				85,          //POWER
				6,          //USAGE
				0,           //MANA
				null
				)
			);

			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.DARK,
				ActionCategory.PHYSICAL,
				70,          //SPEED
				0,           //PRIORITY MODIFIER
				100,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   55, //SPEED
					                                       new byte[1] { 0x1E}) }
					)
				)
			);

			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.MAGIC,
				ActionCategory.DEFENSIVE,
				40,          //SPEED
				2,           //PRIORITY MODIFIER
				0,          //POWER
				2,          //USAGE
				1,           //MANA
				new BattleEffect(
					8, //PRIORITY
					2, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.ON_EFFECT_TIMEOUT,
														   70, //SPEED
														   new byte[1] { 0x1F }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.MAGIC,
				ActionCategory.DEFENSIVE,
				40,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   60, //SPEED
														   new byte[1] { 0x20 }) }
					)
				)
			);
			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.DARK,
				ActionCategory.DEFENSIVE,
				40,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   60, //SPEED
														   new byte[1] { 0x21 }) }
					)
				)
			);
			//6//
			c.Add(new CreatureAction(
				"Fireball", //NAME
				CreatureElement.FIRE,
				ActionCategory.MYSTICAL,
				70,          //SPEED
				0,           //PRIORITY MODIFIER
				80,          //POWER
				5,          //USAGE
				0,           //MANA
				null
				)
		   );

			c.Add(new CreatureAction(
				"Nuclear Burst", //NAME
				CreatureElement.NUCLEAR,
				ActionCategory.PHYSICAL,
				90,          //SPEED
				0,           //PRIORITY MODIFIER
				85,          //POWER
				6,          //USAGE
				0,           //MANA
				null
				)
			);

			c.Add(new CreatureAction(
				"Soul Crush", //NAME
				CreatureElement.DARK,
				ActionCategory.PHYSICAL,
				70,          //SPEED
				0,           //PRIORITY MODIFIER
				100,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   55, //SPEED
					                                       new byte[1] { 0x1E}) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Future Zoom", //NAME
				CreatureElement.MAGIC,
				ActionCategory.DEFENSIVE,
				40,          //SPEED
				2,           //PRIORITY MODIFIER
				0,          //POWER
				2,          //USAGE
				1,           //MANA
				new BattleEffect(
					8, //PRIORITY
					2, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.ON_EFFECT_TIMEOUT,
														   70, //SPEED
														   new byte[1] { 0x1F }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Sharp Mind", //NAME
				CreatureElement.MAGIC,
				ActionCategory.DEFENSIVE,
				40,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   60, //SPEED
														   new byte[1] { 0x20 }) }
					)
				)
			);
			c.Add(new CreatureAction(
				"Intimidate", //NAME
				CreatureElement.DARK,
				ActionCategory.DEFENSIVE,
				40,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   60, //SPEED
														   new byte[1] { 0x21 }) }
					)
				)
			);

			//7//
			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.FIRE,
				ActionCategory.MYSTICAL,
				70,          //SPEED
				0,           //PRIORITY MODIFIER
				80,          //POWER
				5,          //USAGE
				0,           //MANA
				null
				)
		   );

			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.NUCLEAR,
				ActionCategory.PHYSICAL,
				90,          //SPEED
				0,           //PRIORITY MODIFIER
				85,          //POWER
				6,          //USAGE
				0,           //MANA
				null
				)
			);

			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.DARK,
				ActionCategory.PHYSICAL,
				70,          //SPEED
				0,           //PRIORITY MODIFIER
				100,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   55, //SPEED
					                                       new byte[1] { 0x1E}) }
					)
				)
			);

			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.MAGIC,
				ActionCategory.DEFENSIVE,
				40,          //SPEED
				2,           //PRIORITY MODIFIER
				0,          //POWER
				2,          //USAGE
				1,           //MANA
				new BattleEffect(
					8, //PRIORITY
					2, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.ON_EFFECT_TIMEOUT,
														   70, //SPEED
														   new byte[1] { 0x1F }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.MAGIC,
				ActionCategory.DEFENSIVE,
				40,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   60, //SPEED
														   new byte[1] { 0x20 }) }
					)
				)
			);
			c.Add(new CreatureAction(
				"---", //NAME
				CreatureElement.DARK,
				ActionCategory.DEFENSIVE,
				40,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   60, //SPEED
														   new byte[1] { 0x21 }) }
					)
				)
			);

			//8//
			c.Add(new CreatureAction(
				"Piston Punch", //NAME
				CreatureElement.STEEL,
				ActionCategory.PHYSICAL,
				70,          //SPEED
				-2,           //PRIORITY MODIFIER
				60,          //POWER
				8,          //USAGE
				0,           //MANA
				null
				)
		   );

			c.Add(new CreatureAction(
				"Sea Scatter", //NAME
				CreatureElement.WATER,
				ActionCategory.PHYSICAL,
				90,          //SPEED
				0,           //PRIORITY MODIFIER
				85,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   60, //SPEED
														   new byte[1] { 0x22 }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Isobind", //NAME
				CreatureElement.NUCLEAR,
				ActionCategory.MYSTICAL,
				70,          //SPEED
				0,           //PRIORITY MODIFIER
				100,          //POWER
				6,          //USAGE
				2,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
														   55, //SPEED
					                                       new byte[1] { 0x23 }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Spikes", //NAME
				CreatureElement.ROCK,
				ActionCategory.ADAPTIVE,
				40,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				10,          //USAGE
				0,           //MANA
				new BattleEffect(
					9, //PRIORITY
					6, //LIFESPAN
					new byte[3] { 3,4,5 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.ON_STAND_ENTER,
														   70, //SPEED
														   new byte[1] { 0x24 }) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Detox", //NAME
				CreatureElement.NUCLEAR,
				ActionCategory.DEFENSIVE,
				40,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				0,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   60, //SPEED
														   new byte[1] { 0x25 }) }
					)
				)
			);
			c.Add(new CreatureAction(
				"Weapon Recall", //NAME
				CreatureElement.STEEL,
				ActionCategory.DEFENSIVE,
				40,          //SPEED
				-2,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   60, //SPEED
														   new byte[1] { 0x26 }) }
					)
				)
			);
            c.Last().IsSwitch = true;

            c.Add(new CreatureAction( //TODO: AFTER SCORCHED EARTH
				"Cyclone", //NAME
				CreatureElement.ETHER,
				ActionCategory.DEFENSIVE,
				55,          //SPEED
				0,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				2,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   55, //SPEED
					                                       new byte[1] { 0x0A }) }
					)
				)
			);
			c.Add(new CreatureAction(
				"Pressurise", //NAME
				CreatureElement.WIND,
				ActionCategory.DEFENSIVE,
				55,          //SPEED
				-1,           //PRIORITY MODIFIER
				0,          //POWER
				5,          //USAGE
				1,           //MANA
				new BattleEffect(
					10, //PRIORITY
					0, //LIFESPAN
					new byte[1] { 9 }, //PLACEMENT
					new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
														   55, //SPEED
					                                       new byte[1] { 0x14 }) }
					)
				)
			);


            c.Add(new CreatureAction(
                "Hover", //NAME
                CreatureElement.WIND,
                ActionCategory.DEFENSIVE,
                40,          //SPEED
                0,           //PRIORITY MODIFIER
                0,          //POWER
                5,          //USAGE
                1,           //MANA
                new BattleEffect(
                    10, //PRIORITY
                    0, //LIFESPAN
                    new byte[1] { 9 }, //PLACEMENT
                    new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
                                                           60, //SPEED
					                                       new byte[1] { 0x04 }) }
                    )
                )
            );

            c.Add(new CreatureAction(
                "Transport Rip", //NAME
                CreatureElement.COSMIC,
                ActionCategory.ADAPTIVE,
                50,          //SPEED
                0,           //PRIORITY MODIFIER
                0,          //POWER
                8,          //USAGE
                0,           //MANA
                new BattleEffect(
                    9, //PRIORITY
                    3, //LIFESPAN
                    new byte[1] { 0 }, //PLACEMENT
                    new EffectScript[2] { new EffectScript(EffectTrigger.ON_STAND_ENTER,
                                                           70, //SPEED
					                                       new byte[1] { 0x03 }),
                        new EffectScript(EffectTrigger.ON_STAND_EXIT,
                                                           68, //SPEED
														   new byte[1] { 0x0B }) }
                    )
                )
            );

            c.Add(new CreatureAction(
                "Bend Presence", //NAME
                CreatureElement.COSMIC,
                ActionCategory.ADAPTIVE,
                30,          //SPEED
                0,           //PRIORITY MODIFIER
                0,          //POWER
                10,          //USAGE
                0,           //MANA
                new BattleEffect(
                    5, //PRIORITY
                    3, //LIFESPAN
                    new byte[1] { 2 }, //PLACEMENT
                    new EffectScript[1] { new EffectScript(EffectTrigger.ON_EFFECT_TIMEOUT,
                                                           120, //SPEED
					                                       new byte[1] { 0x05 }) }
                    )
                )
            );

            c.Add(new CreatureAction(
                "Solidify", //NAME
                CreatureElement.ICE,
                ActionCategory.MYSTICAL,
                80,          //SPEED
                0,           //PRIORITY MODIFIER
                120,          //POWER
                6,          //USAGE
                2,           //MANA
                new BattleEffect(
                    10, //PRIORITY
                    0, //LIFESPAN
                    new byte[1] { 9 }, //PLACEMENT
                    new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
                                                           80, //SPEED
					                                       new byte[1] { 0x07 }) }
                    )
                )
            );

            c.Add(new CreatureAction(
                "Mach Speed", //NAME
                CreatureElement.WIND,
                ActionCategory.PHYSICAL,
                70,          //SPEED
                -2,           //PRIORITY MODIFIER
                100,          //POWER
                50,          //USAGE
                2,           //MANA
                new BattleEffect(
                    10, //PRIORITY
                    0, //LIFESPAN
                    new byte[1] { 9 }, //PLACEMENT
                    new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ATTACKING,
                                                           55, //SPEED
					                                       new byte[1] { 0x12 }) }
                    )
                )
            );
            c.Add(new CreatureAction(
                "Air Cutter", //NAME
                CreatureElement.WIND,
                ActionCategory.PHYSICAL,
                55,          //SPEED
                0,           //PRIORITY MODIFIER
                80,          //POWER
                5,          //USAGE
                0,           //MANA
                null
                )
            );

            c.Add(new CreatureAction(
                "Earth Wave", //NAME
                CreatureElement.ETHER,
                ActionCategory.DEFENSIVE,
                55,          //SPEED
                0,           //PRIORITY MODIFIER
                0,          //POWER
                5,          //USAGE
                2,           //MANA
                new BattleEffect(
                    10, //PRIORITY
                    0, //LIFESPAN
                    new byte[1] { 9 }, //PLACEMENT
                    new EffectScript[1] { new EffectScript(EffectTrigger.AFTER_ACTION,
                                                           55, //SPEED
					                                       new byte[1] { 0x0A }) }
                    )
                )
            );

            return c;
		}

		/*
		public List<CreatureAction> Actions() {
			List<CreatureAction> c = new List<CreatureAction>();

			c.Add(new CreatureAction(
				"Grill", //NAME
				CreatureElement.FIRE,
				ActionCategory.PHYSICAL,
				80,          //SPEED
				1,           //PRIORITY
				50,          //POWER
				10,          //USAGE
				20,           //MANA
				new BattleEffect(
					new EffectTrigger[1] { EffectTrigger.AFTER_ATTACKING },
					new byte[1] { 80 }, //SPEEDS
					10, //PRIORITY
					1, //LIFESPAN
					0, //PLACEMENT
					new EffectScript[1] { new EffectScript(new byte[3] { 0x00, 0x02, 0x02 } ) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Smoulder Smash", //NAME
				CreatureElement.FIRE,
				ActionCategory.PHYSICAL,
				80,          //SPEED
				0,           //PRIORITY
				90,          //POWER
				10,          //USAGE
				40,           //MANA
				new BattleEffect(
					new EffectTrigger[1] { EffectTrigger.AFTER_ATTACKING },
					new byte[1] { 80 }, //SPEEDS
					10, //PRIORITY
					1, //LIFESPAN
					0, //PLACEMENT
					new EffectScript[1] { new EffectScript(new byte[3] { 0x00, 0x02, 0x04 } ) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Fireball", //NAME
				CreatureElement.FIRE,
				ActionCategory.MYSTICAL,
				80,          //SPEED
				0,           //PRIORITY
				80,          //POWER
				15,          //USAGE
				0,           //MANA
				null
				)
			);

			c.Add(new CreatureAction(
				"Flame Horizon", //NAME
				CreatureElement.FIRE,
				ActionCategory.MYSTICAL,
				80,          //SPEED
				-3,          //PRIORITY
				140,         //POWER
				5,           //USAGE
				50,          //MANA
				null
				)
			);

			c.Add(new CreatureAction(
				"Magma", //NAME
				CreatureElement.FIRE,
				ActionCategory.MYSTICAL,
				80,          //SPEED
				0,           //PRIORITY
				50,          //POWER
				10,          //USAGE
				0,           //MANA
				new BattleEffect(
					new EffectTrigger[1] { EffectTrigger.AFTER_ATTACKING },
					new byte[1] { 80 }, //SPEEDS
					10, //PRIORITY
					1, //LIFESPAN
					0, //PLACEMENT
					new EffectScript[1] { new EffectScript(new byte[4] { 0x02, 0x06, 0x01, 0X01 } ) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Flash Flare", //NAME
				CreatureElement.FIRE,
				ActionCategory.MYSTICAL,
				80,          //SPEED
				-1,          //PRIORITY
				90,         //POWER
				7,           //USAGE
				30,          //MANA
				null
				)
			);

			c.Add(new CreatureAction(
				"Shed Flame", //NAME
				CreatureElement.FIRE,
				ActionCategory.MYSTICAL,
				80,          //SPEED
				0,           //PRIORITY
				120,          //POWER
				6,          //USAGE
				0,           //MANA
				new BattleEffect(
					new EffectTrigger[1] { EffectTrigger.AFTER_ATTACKING },
					new byte[1] { 80 }, //SPEEDS
					10, //PRIORITY
					1, //LIFESPAN
					0, //PLACEMENT
					new EffectScript[1] { new EffectScript(new byte[4] { 0x02, 0x03, 0x01, 0X02 } ) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Kindle", //NAME
				CreatureElement.FIRE,
				ActionCategory.DEFENSIVE,
				80,          //SPEED
				0,           //PRIORITY
				0,          //POWER
				5,          //USAGE
				10,           //MANA
				new BattleEffect(
					new EffectTrigger[1] { EffectTrigger.AFTER_ATTACKING },
					new byte[1] { 80 }, //SPEEDS
					10, //PRIORITY
					1, //LIFESPAN
					0, //PLACEMENT
					new EffectScript[1] { new EffectScript(new byte[3] { 0x00, 0x02, 0x02 } ) }
					)
				)
			);

			c.Add(new CreatureAction(
				"Molten Mold", //NAME
				CreatureElement.FIRE,
				ActionCategory.DEFENSIVE,
				80,          //SPEED
				0,           //PRIORITY
				0,          //POWER
				5,          //USAGE
				20,           //MANA
				new BattleEffect(
					new EffectTrigger[2] { EffectTrigger.ON_EFFECT_ENTER, EffectTrigger.ON_EFFECT_EXIT },
					new byte[2] { 80, 80 }, //SPEEDS
					10, //PRIORITY
					1, //LIFESPAN
					0, //PLACEMENT
					new EffectScript[2] { new EffectScript(new byte[3] { 0x14, 0x01, 0x06 } ),
						new EffectScript(new byte[2] { 0x15, 0x01 } )
					    }
					)
				)
			);

			c.Add(new CreatureAction(
				"Fire Pact", //NAME
				CreatureElement.FIRE,
				ActionCategory.DEFENSIVE,
				80,          //SPEED
				0,           //PRIORITY
				0,          //POWER
				10,          //USAGE
				0,           //MANA
				new BattleEffect(
					new EffectTrigger[1] { EffectTrigger.ON_EFFECT_ENTER },
					new byte[1] { 80 }, //SPEEDS
					10, //PRIORITY
					1, //LIFESPAN
					0, //PLACEMENT
					new EffectScript[1] { new EffectScript(new byte[5] { 0x18, 0x01, 0x00, 0x00, 0x01 } )
						}
					)
				)
			);

			c.Add(new CreatureAction(
				"Melt", //NAME
				CreatureElement.FIRE,
				ActionCategory.DEFENSIVE,
				80,          //SPEED
				0,           //PRIORITY
				0,          //POWER
				10,          //USAGE
				10,           //MANA
				new BattleEffect(
					new EffectTrigger[1] { EffectTrigger.ON_EFFECT_ENTER },
					new byte[1] { 80 }, //SPEEDS
					10, //PRIORITY
					1, //LIFESPAN
					0, //PLACEMENT
					new EffectScript[1] { new EffectScript(new byte[3] { 0x16, 0x07, 0x01 } )
						}
					)
				)
			);

			c.Add(new CreatureAction(
				"Blaze Up", //NAME
				CreatureElement.FIRE,
				ActionCategory.DEFENSIVE,
				80,          //SPEED
				0,           //PRIORITY
				0,          //POWER
				15,          //USAGE
				10,           //MANA
				new BattleEffect(
					new EffectTrigger[1] { EffectTrigger.ON_EFFECT_ENTER },
					new byte[1] { 80 }, //SPEEDS
					10, //PRIORITY
					1, //LIFESPAN
					0, //PLACEMENT
					new EffectScript[1] { new EffectScript(new byte[3] { 0x16, 0x07, 0x01 } )
						}
					)
				)
			);
			return c;
		}*/
	}
}
