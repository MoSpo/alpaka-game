using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Battle {

	abstract class EffectExecute {

		public List<SceneAnimation> Animations;
		public List<List<SceneAnimation>> AnimationStack;

		public abstract List<SceneAnimation> ExecuteEffect(EffectScript Script);

		public abstract void SetTargets(Player User, Player Opponent, Player Trigger, byte TriggerPosition);

		public abstract void SetEffect(BattleEffect Effect);

        public abstract void SetElement(CreatureElement Element);

		public abstract void SetPrevElement(CreatureElement Element);

		public abstract void SetBaseAttacks(int BasePhysical, int BaseMystical);
    }

    class EffectHardExecute : EffectExecute {

		BattleEngine Battle;
		Player User;
		Player Opponent;
		Player Trigger;
		byte TriggerPosition;
		byte PlayersEffected;
		BattleEffect CurrentEffect;

		public int BasePhysical;
		public int BaseMystical;

		CreatureElement PrevElement;
		CreatureElement Element;

		OP_[] OP_Decode;

		public override List<SceneAnimation> ExecuteEffect(EffectScript Script) {

			Animations = new List<SceneAnimation>();
			AnimationStack.Add(Animations);

			if (!(Element == CreatureElement.EARTH && Opponent.NotEffectedByEarth.Evaluate())) {

				List<byte> Memory = Script.Script;
				for (int i = 0; i < Memory.Count; i++) {
					OP_Decode[Memory[i] >> 4]((byte)(Memory[i] & 0x0F))();
				}
			}
			AnimationStack.Remove(Animations);
			List<SceneAnimation> Finished = Animations;
			if (AnimationStack.Count > 0) {
				Animations = AnimationStack.Last();
			} else {
				AnimationStack.Clear();
				Animations = null;
			}
			return Finished;
		}

		public override void SetTargets(Player User, Player Opponent, Player Trigger, byte TriggerPosition) {
			this.User = User;
			this.Opponent = Opponent;
			this.Trigger = Trigger;
			this.TriggerPosition = TriggerPosition;
		}

		public override void SetEffect(BattleEffect Effect) {
			CurrentEffect = Effect;
			SetElement(Effect.Element);
			SetBaseAttacks(Effect.BasePhysical, Effect.BaseMystical);
		}


		public override void SetElement(CreatureElement Element) {
            this.Element = Element;
        }

		public override void SetPrevElement(CreatureElement Element) {
			PrevElement = Element;
		}

		public override void SetBaseAttacks(int BasePhysical, int BaseMystical) {
			this.BasePhysical = BasePhysical;
			this.BaseMystical = BaseMystical;
		}

		private string OpponentText(Player Target) {
			if (Battle.Player2 == Target) {
				return "The Opponent's ";
			} else {
				return "";
			}
		}

		public EffectHardExecute(BattleEngine Battle) {
			this.Battle = Battle;
			AnimationStack = new List<List<SceneAnimation>>();

			OP_[] fun = {OP_0_, OP_1_, OP_2_, OP_3_};

			OP_Decode = fun;
		}

		delegate Action OP_(byte OP);

		Action OP_0_(byte OP) {
			Action[] OP_ = {
				new Action(COSMIC_STING), new Action(SPEED_SLASH), new Action(SLINGSHOT), new Action(TRANSPORT_RIP), new Action(HOVER), new Action(BEND_PRESENCE), new Action(FLASH_FREEZE), new Action(SOLIDIFY),
				new Action(CEASEFIRE), new Action(COLD_STORAGE), new Action(HEALING_BELL), new Action(TRANSPORT_RIP2), new Action(SMOULDER_SMASH), new Action(CLOBBER), new Action(ANCIENT_ROAR), new Action(POLISH)
			};
			return OP_[OP];
		}

		Action OP_1_(byte OP) {
			Action[] OP_ = {
				new Action(KINDLE), new Action(DETER), new Action(SLEET_HAMMER), new Action(WINDTUNNEL), new Action(KAMAKAZI), new Action(METAMORPH), new Action(ASTEROIDS), new Action(COMET_LAUNCHER),
				new Action(SWITCH), new Action(MIND_STRIKE), new Action(WINDTUNNEL2), new Action(SPRING_OF_ILLUSION), new Action(MOTIVATOR), new Action(MOTIVATOR2), new Action(SOUL_CRUSH), new Action(FUTURE_ZOOM)
			};
			return OP_[OP];
		}
		Action OP_2_(byte OP) {
			 Action[] OP_ = {
					new Action(SHARP_MIND), new Action(INTIMIDATE), new Action(OP_22), new Action(OP_23), new Action(OP_24), new Action(OP_25), new Action(OP_26), new Action(OP_27),
					new Action(OP_28), new Action(OP_29), new Action(OP_2A), new Action(OP_2B), new Action(OP_2C), new Action(OP_2D), new Action(OP_2E), new Action(OP_2F)
				};
				return OP_[OP];
		}
		Action OP_3_(byte OP) {
			/*
			Action[] OP_ = {
				new Action(OP_30), new Action(OP_31), new Action(OP_32), new Action(OP_33), new Action(OP_34), new Action(OP_35), new Action(OP_36), new Action(OP_37),
				new Action(OP_38), new Action(OP_39), new Action(OP_3A), new Action(OP_3B), new Action(OP_3C), new Action(OP_3D), new Action(OP_3E), new Action(OP_3F)
			};
			return OP_[OP]; */
			return null;

		}

		void COSMIC_STING() {
			for (int i = 0; i < 9; i++) {
				BattleEffect[] eff = Battle.AllEffects[i].effects;
				for (int j = 0; j < eff.Length; j++) {
					if(eff[j] != null && eff[j].Element == CreatureElement.COSMIC) {
						User.amountOfAttacks += 1;
					}
				}
   			}
		}

		void SPEED_SLASH() {
                Animations.Add(Opponent.GiveCondition((byte)CreatureCondition.CUT));
                Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " is Cut!"));
		}

		void SLINGSHOT() {
				//Animations.Add(Opponent.GiveCondition((byte)CreatureCondition.BLIND));
				//Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " is Blind!"));
			Animations.AddRange(Opponent.GiveStatBoost(CreatureStats.STRENGTH, false));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " Strength is lowered!"));

		}

		void TRANSPORT_RIP() {
			Trigger.HasActionDelay.SetFlag(255);

		}
		void TRANSPORT_RIP2() {
			Trigger.HasActionDelay.RemoveFlag();
		}
		void HOVER() {
			User.NotEffectedByEarth.SetFlag(3);
		}
		void BEND_PRESENCE() {
			Battle.SetArenaRotation(TriggerPosition);
		}
		void FLASH_FREEZE() {
            Animations.Add(Opponent.GiveCondition((byte)CreatureCondition.FROZEN));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE,null, OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " is Frozen!"));
            Animations.AddRange(Battle.RemoveAllEffectsExceptElement(CreatureElement.ICE));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE,null, "All but Ice Effects have been removed from the Arena!"));
		}
		void SOLIDIFY() {
            Animations.Add(Opponent.SetNewElement(CreatureElement.ICE));
            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " has been changed to Ice!"));
        }
        void CEASEFIRE() {
			User.CanBeAttacked.SetFlag(2);
			Opponent.CanBeAttacked.SetFlag(2);
            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "Neither Player can directly attack the other next turn!"));
        }
        void COLD_STORAGE() {
			if (Trigger.JustSwitchedIn) {
                Animations.Add(Trigger.GiveCondition((byte)CreatureCondition.FROZEN));
				Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE,null, OpponentText(Trigger) + Trigger.ActiveCreature.Nickname + " is Frozen!"));
			}
		}
		void HEALING_BELL() {
			User.RestoreHealthInseadOfMana.SetFlag(4);
		}
		void SMOULDER_SMASH() {
			Animations.Add(Opponent.GiveCondition((byte)CreatureCondition.BLIND));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " is Blind!"));
		}

		void CLOBBER() {
			Animations.AddRange(Opponent.GiveStatBoost(CreatureStats.ENDURANCE, false));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " Endurance is lowered!"));
		}

		void ANCIENT_ROAR() {
            Opponent.forceSwitched = true;
        }

        void POLISH() {
            Animations.AddRange(User.Heal(20));
            Animations.AddRange(User.GiveStatBoost(CreatureStats.ENDURANCE, true));
            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "The Endurance of " + OpponentText(User) + User.ActiveCreature.Nickname + " increased!"));
        }

        void KINDLE() {
            Animations.Add(Opponent.GiveCondition((byte)CreatureCondition.BURNED));
            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " is Burned!"));
        }

        void DETER() {
            User.CanBeMoved.SetFlag(3);
            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(User) + User.ActiveCreature.Nickname + " cannot be moved for 3 turns!"));
        }

        void SLEET_HAMMER() {
            Animations.AddRange(Opponent.GiveStatBoost(CreatureStats.AWE, false));
            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "The Awe of " + OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " decreased!"));

        }
        void WINDTUNNEL() {
			Animations.Add(Trigger.GiveElementBuff(CreatureElement.WIND, true));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Trigger) + Trigger.ActiveCreature.Nickname + " became more proficient with Wind Actions!"));
			Animations.AddRange(Trigger.GiveStatBoost(CreatureStats.PACE, true));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "The Pace of " + OpponentText(Trigger) + Trigger.ActiveCreature.Nickname + " increased!"));
		}

		void KAMAKAZI() {
			Animations.AddRange(User.GiveDamage(CreatureElement.FIRE, ActionCategory.PHYSICAL, 255, BasePhysical, BaseMystical, User.GetTotalStat(CreatureStats.ENDURANCE)/4, User.GetTotalStat(CreatureStats.WISDOM)/4, 1, User.GetElementEffectiveness(Element)));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(User) + User.ActiveCreature.Nickname + " takes damage from Kamakazi!"));
		}

		void METAMORPH() {
			Animations.AddRange(User.GiveStatBoost(CreatureStats.STRENGTH, true));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "The Strength of " + OpponentText(User) + User.ActiveCreature.Nickname + " increased!"));
		}

		void ASTEROIDS() {
			Animations.AddRange(Trigger.GiveDamage(CreatureElement.ROCK, ActionCategory.PHYSICAL, 60, BasePhysical, BaseMystical, Trigger.GetTotalStat(CreatureStats.ENDURANCE), Trigger.GetTotalStat(CreatureStats.WISDOM), 1, Trigger.GetElementEffectiveness(Element)));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Trigger) + Trigger.ActiveCreature.Nickname + " takes damage from Asteroids!"));
		}

		void COMET_LAUNCHER() {
			if (PrevElement == CreatureElement.COSMIC) {
				Animations.AddRange(Opponent.GiveDamage(CreatureElement.ICE, ActionCategory.PHYSICAL, 60, BasePhysical, BaseMystical, Opponent.GetTotalStat(CreatureStats.ENDURANCE), Opponent.GetTotalStat(CreatureStats.WISDOM), 1, Opponent.GetElementEffectiveness(Element)));
				Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " takes damage from Comet Launcher!"));
			}
		}

		void SWITCH() {
            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(User) + User.ActiveCreature.Nickname + " is exiting the arena!"));
            Animations.AddRange(Battle.Switch(User));
            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(User) + User.ActiveCreature.Nickname + " has taken it's place!"));
        }

        void MIND_STRIKE() {
            Animations.AddRange(User.GiveStatBoost(CreatureStats.WISDOM, true));
            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "The Wisdom of " + OpponentText(User) + User.ActiveCreature.Nickname + " increased!"));
        }

        void WINDTUNNEL2() {
			Animations.Add(Trigger.GiveElementBuff(CreatureElement.WIND, false));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Trigger) + Trigger.ActiveCreature.Nickname + " became less proficient with Wind Actions!"));
			Animations.AddRange(Trigger.GiveStatBoost(CreatureStats.PACE, false));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "The Pace of " + OpponentText(Trigger) + Trigger.ActiveCreature.Nickname + " decreased!"));
		}

		void SPRING_OF_ILLUSION() {
			byte sm = 1;
			for (int i = 0; i < 3; i++) {
				if (Battle.AllEffects[Opponent.Placement].effects[i] != null) {
					if(sm > Battle.AllEffects[Opponent.Placement].effects[i].CurrentLifespan) sm = Battle.AllEffects[Opponent.Placement].effects[i].CurrentLifespan;
					CurrentEffect.Scripts.AddRange(Battle.AllEffects[Opponent.Placement].effects[i].Scripts);
				}
			}
			CurrentEffect.CurrentLifespan = sm;
		}

		void MOTIVATOR() {
			Trigger.AttackGainOnStatBoost.SetFlag(255);
		}

		void MOTIVATOR2() {
			Trigger.AttackGainOnStatBoost.RemoveFlag();

		}

		void SOUL_CRUSH() {
			Opponent.RestoreMana.SetFlag(1);
		}

		void FUTURE_ZOOM() {
			Animations.AddRange(User.GiveStatBoost(CreatureStats.PACE, true));
			Animations.AddRange(User.GiveStatBoost(CreatureStats.PACE, true));
			Animations.AddRange(User.GiveStatBoost(CreatureStats.PACE, true));
			Animations.AddRange(User.GiveStatBoost(CreatureStats.PACE, true));
			Animations.AddRange(User.GiveStatBoost(CreatureStats.PACE, true));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "The Pace of " + OpponentText(User) + User.ActiveCreature.Nickname + " increased drastically from Future Zoom!"));
		}
		void SHARP_MIND() {
			Animations.AddRange(User.GiveStatBoost(CreatureStats.WISDOM, true));
			Animations.AddRange(User.GiveStatBoost(CreatureStats.WISDOM, true));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "The Wisdom of " + OpponentText(User) + User.ActiveCreature.Nickname + " increased!"));

		}
		void INTIMIDATE() {
			Animations.AddRange(Opponent.GiveStatBoost(CreatureStats.AWE, false));
			Animations.AddRange(Opponent.GiveStatBoost(CreatureStats.AWE, false));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "The Awe of " + OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " decreased!"));

		}
		void OP_22() {
			Animations.Add(Opponent.GiveElementBuff(CreatureElement.FIRE, false));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " became less proficient with Fire Actions!"));
		}
		void OP_23() {
			Animations.Add(User.GiveElementBuff(CreatureElement.NUCLEAR, true));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(User) + User.ActiveCreature.Nickname + " became more proficient with Nuclear Actions!"));
		}
		void OP_24() {
			if (Trigger.JustSwitchedIn) {
				Animations.AddRange(Trigger.GiveDamage(CreatureElement.ROCK, ActionCategory.PHYSICAL, 60, BasePhysical, BaseMystical, Trigger.GetTotalStat(CreatureStats.ENDURANCE), Trigger.GetTotalStat(CreatureStats.WISDOM), 1, Trigger.GetElementEffectiveness(Element)));
				Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(Trigger) + Trigger.ActiveCreature.Nickname + " takes damage from Spikes!"));
			}
		}
		void OP_25() {
			Animations.AddRange(Opponent.GiveStatBoost(CreatureStats.WISDOM, false));
			Animations.AddRange(Opponent.GiveStatBoost(CreatureStats.WISDOM, false));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "The Wisdom of " + OpponentText(Opponent) + Opponent.ActiveCreature.Nickname + " decreased!"));

		}
		void OP_26() {
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(User) + User.ActiveCreature.Nickname + " is exiting the arena!"));
			Animations.AddRange(Battle.Switch(User));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, OpponentText(User) + User.ActiveCreature.Nickname + " has taken it's place!"));
			Animations.AddRange(User.GiveStatBoost(CreatureStats.STRENGTH, true));
			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "The Strength of " + OpponentText(User) + User.ActiveCreature.Nickname + " increased!"));
		}
		void OP_27() {

		}
		void OP_28() {

		}
		void OP_29() {

		}
		void OP_2A() {

		}
		void OP_2B() {

		}
		void OP_2C() {

		}
		void OP_2D() {

		}
		void OP_2E() {

		}
		void OP_2F() {

		}
	}
	/*
	class EffectInterpreter : EffectExecute {

        BattleEngine Battle;
        Player User;
        Player Opponent;
        Player Trigger;
        List<byte> Memory;
		byte PlayersEffected;
        short Counter = 0;

        OP_[] OP_Decode;

        public override List<SceneAnimation> ExecuteEffect(EffectScript Script) {
           Memory = Script.Script;
           while (Counter < Memory.Count) {
                Console.WriteLine(Memory[Counter]);
                OP_Decode[Memory[Counter] >> 4]((byte)(Memory[Counter] & 0x0F))();
                Counter++;          
            }
            Counter = 0;
			return null;
        }

        public override void SetTargets(Player User, Player Opponent, Player Trigger, byte TriggerPosition) {
            this.User = User;
            this.Opponent = Opponent;
            this.Trigger = Trigger;
        }

		private string OpponentText(Player Trigger) {
            Console.WriteLine(Battle.Player2 == Trigger);
            if (Battle.Player2 == Trigger) {
                return "The Opponent's ";
            } else {
                return "";
            }
		}


        public EffectInterpreter(BattleEngine Battle) {
            this.Battle = Battle;

            OP_[] fun = {
                OP_0_, OP_1_, OP_2_, OP_3_, OP_4_, OP_5_, OP_6_ };//, OP_7_,
               // OP_8_, OP_9_, OP_A_, OP_B_, OP_C_, OP_D_, OP_E_, OP_F_
            //};

            OP_Decode = fun;
        }

       delegate Action OP_(byte OP);

        Action OP_0_(byte OP) {
            Action[] OP_ = {
                new Action(OP_00), new Action(OP_01), new Action(OP_02), new Action(OP_03), new Action(OP_04), new Action(OP_05), new Action(OP_06), new Action(OP_07),
                new Action(OP_08), new Action(OP_09), new Action(OP_0A), new Action(OP_0B), new Action(OP_0C), new Action(OP_0D), new Action(OP_0E), new Action(OP_0F)
            };
            return OP_[OP];
        }

        Action OP_1_(byte OP) {

            Action[] OP_ = {
                new Action(OP_10), new Action(OP_11), new Action(OP_12), new Action(OP_13), new Action(OP_14), new Action(OP_15), new Action(OP_16), new Action(OP_17),
                new Action(OP_18), new Action(OP_19), new Action(OP_1A), new Action(OP_1B), new Action(OP_1C), new Action(OP_1D), new Action(OP_1E), new Action(OP_1F)
            };
            return OP_[OP];
        }
        Action OP_2_(byte OP) {

            Action[] OP_ = {
                new Action(OP_20), new Action(OP_21), new Action(OP_22), new Action(OP_23), new Action(OP_24), new Action(OP_25), new Action(OP_26), new Action(OP_27),
                new Action(OP_28), new Action(OP_29), new Action(OP_2A), new Action(OP_2B), new Action(OP_2C), new Action(OP_2D), new Action(OP_2E), new Action(OP_2F)
            };
            return OP_[OP];
        }
        Action OP_3_(byte OP) {

            Action[] OP_ = {
                new Action(OP_30), new Action(OP_31), new Action(OP_32), new Action(OP_33), new Action(OP_34), new Action(OP_35), new Action(OP_36), new Action(OP_37),
                new Action(OP_38), new Action(OP_39), new Action(OP_3A), new Action(OP_3B), new Action(OP_3C), new Action(OP_3D), new Action(OP_3E), new Action(OP_3F)
            };
            return OP_[OP];
        }
        Action OP_4_(byte OP) {

            Action[] OP_ = {
                new Action(OP_40), new Action(OP_41), new Action(OP_42), new Action(OP_43), new Action(OP_44), new Action(OP_45), new Action(OP_46), new Action(OP_47),
                new Action(OP_48), new Action(OP_49), new Action(OP_4A), new Action(OP_4B), new Action(OP_4C), new Action(OP_4D), new Action(OP_4E), new Action(OP_4F)
            };
            return OP_[OP];
        }
        Action OP_5_(byte OP) {

            Action[] OP_ = {
                new Action(OP_50), new Action(OP_51), new Action(OP_52), new Action(OP_53), new Action(OP_54), new Action(OP_55), new Action(OP_56), new Action(OP_57),
                new Action(OP_58), new Action(OP_59), new Action(OP_5A), new Action(OP_5B), new Action(OP_5C), new Action(OP_5D), new Action(OP_5E), new Action(OP_5F)
            };
            return OP_[OP];
        }
        Action OP_6_(byte OP) {

            Action[] OP_ = {
                new Action(OP_60), new Action(OP_61), new Action(OP_62), new Action(OP_63), new Action(OP_64), new Action(OP_65), new Action(OP_66), new Action(OP_67),
                new Action(OP_68), new Action(OP_69), new Action(OP_6A), new Action(OP_6B), new Action(OP_6C), new Action(OP_6D), new Action(OP_6E), new Action(OP_6F)
            };
            return OP_[OP];
        }

		/// /////////////////////////////////////////////// ///

		Player[] GetActingPlayers() {
			Player[] actingPlayers = new Player[2];
			switch (Memory[Counter + 1]) {
				case 0:
					actingPlayers[0] = User;
					actingPlayers[1] = Opponent;
					PlayersEffected = 2;
					break;
				case 1:
					actingPlayers[0] = User;
					PlayersEffected = 1;
					break;
				case 2:
					actingPlayers[0] = Opponent;
					PlayersEffected = 1;
					break;
				case 3:
					actingPlayers[0] = Trigger;
					PlayersEffected = 1;
					break;
			}
			return actingPlayers;
		}

	void OP_00() {
			//inflict condition
			//[ALL/USR/PNT/TGR] [CONDITION]
			Player[] actingPlayers = GetActingPlayers();

			for(int i = 0; i< PlayersEffected; i++) {
                if(!actingPlayers[i].ActiveCreature.HasCondition(Memory[Counter + 2])) {
                    actingPlayers[i].ActiveCreature.GiveCondition(Memory[Counter + 2]);
                    Battle.AddMessage(OpponentText(actingPlayers[i]) + actingPlayers[i].ActiveCreature.Nickname + " is " + (CreatureCondition)Memory[Counter + 2] + "!");
                }
            }
            Counter += 2;
        }

        void OP_01() {
            //remove all conditions
			//[ALL/USR/PNT/TGR]
			Player[] actingPlayers = GetActingPlayers();

			for (int i = 0; i < PlayersEffected; i++) {
                actingPlayers[i].ActiveCreature.RemoveConditions();
                Battle.AddMessage(OpponentText(actingPlayers[i]) + actingPlayers[i].ActiveCreature.Nickname + " is cured of Battle Conditions!");
            }
            Counter += 1;
        }

        void OP_02() {
            //give stat boost
			//[ALL/USR/PNT/TGR] [STAT] [POS/NEG] [NUMBER]
			Player[] actingPlayers = GetActingPlayers();

			for (int i = 0; i < PlayersEffected; i++) {
				if (Memory[Counter + 3] == 0) {
                    actingPlayers[i].Statboosts.IncreaseStat(Memory[Counter + 4], Memory[Counter + 2]);
                    Battle.AddMessage(OpponentText(actingPlayers[i]) + actingPlayers[i].ActiveCreature.Nickname + " increased its " + (CreatureStatName)Memory[Counter + 2] + " by " + Memory[Counter + 4] + " stages!");
                } else {
                    actingPlayers[i].Statboosts.DecreaseStat(Memory[Counter + 4], Memory[Counter + 2]);
                    Battle.AddMessage(OpponentText(actingPlayers[i]) + actingPlayers[i].ActiveCreature.Nickname + " decreased its " + (CreatureStatName)Memory[Counter + 2] + " by " + Memory[Counter + 4] + " stages!");
                }
            }
            Counter += 4;
        }

        void OP_03() {
            //take stat boost
			//[ALL/USR/PNT/TGR] [STAT] [POS/NEG] [NUMBER]
        }

        void OP_04() {
            //reset stat boosts
			//[ALL/USR/PNT/TGR]
			Player[] actingPlayers = GetActingPlayers();

			for (int i = 0; i < PlayersEffected; i++) {
				for (byte j = 0; i < 8; j++) {
                    actingPlayers[i].Statboosts[j] = 0;
                }
                Battle.AddMessage(OpponentText(actingPlayers[i]) + actingPlayers[i].ActiveCreature.Nickname + " reset its stat changes!");
            }
            Counter += 1;

        }

        void OP_05() {
            //switch one stat with another
			//[ALL/USR/PNT/TGR] [STAT] [STAT]

        }

        void OP_06() {
			//modify stat by percent
			//[ALL/USR/PNT/TGR] [STAT] [NUMBER]
        }

        void OP_07() {
            //deal [NUMBER] [ELEMENT] damage to [USR/PNT/ALL]
			Player[] actingPlayers = GetActingPlayers();

			for (int i = 0; i < PlayersEffected; i++) {
				actingPlayers[i].GiveDamage((CreatureElement)Memory[Counter + 2], (ActionCategory)Memory[Counter + 3], Memory[Counter + 4]);
            }
            Counter += 4;
        }

        void OP_08() {
            //move damage is double the last move
            User.GiveDamage();
            Counter += 1;

        }

        void OP_09() {
            //increase next damage by [POS/NEG] [NUMBER]%

        }

        void OP_0A() {
            //increase next damage by [POS/NEG] [STAT] -- NOT NEEDED ANYMORE

        }

        void OP_0B() {
            //reset stat switches

        }

        void OP_0C() {
            //remove [POS/NEG] [NUMBER] pp from [USR/PNT/ALL] last move

        }

        void OP_0D() {
            //remove [POS/NEG] [NUMBER] pp from [USR/PNT/ALL] moves

        }

        void OP_0E() {
            //heal [POS/NEG] [NUMBER]% of [USR/PNT/ALL] health from damage

        }

        void OP_0F() {
            //heal [POS/NEG] [NUMBER]% of [USR/PNT/ALL] health

        }

        void OP_10() {
			//gain [POS/NEG] [NUMBER] of [USR/PNT/ALL] health

        }

        void OP_11() {
			//heal [POS/NEG] [NUMBER] % of[USR / PNT / ALL] kin

        }
        void OP_12() {
			//gain [POS/NEG] [NUMBER] of [USR/PNT/ALL] kin

        }
        void OP_13() {
			//heal [POS/NEG] [NUMBER]% of [USR/PNT/ALL] kin from damage

        }

        void OP_14() {
			//change all elements to singular element
			//[ALL/USR/PNT/TGR] [ELEMENT]
			Player[] actingPlayers = GetActingPlayers();

			for (int i = 0; i < PlayersEffected; i++) {
				actingPlayers[i].newElement = (CreatureElement)Memory[Counter + 2];
				Battle.AddMessage(OpponentText(actingPlayers[i]) + actingPlayers[i].ActiveCreature.Nickname + "'s element affinity changed to " + (CreatureElement)Memory[Counter + 2] + "!");
			}
			Counter += 2;
		}

        void OP_15() {
			//reset [USR/PNT/ALL] elements
			//[ALL/USR/PNT/TGR]
			Player[] actingPlayers = GetActingPlayers();

			for (int i = 0; i < PlayersEffected; i++) {
				actingPlayers[i].newElement = CreatureElement.NULL;
				Battle.AddMessage(OpponentText(actingPlayers[i]) + actingPlayers[i].ActiveCreature.Nickname + " reset its element changes!");
			}
			Counter += 1;
		}

        void OP_16() {
			//replace [USR/PNT/ALL] [ELEMENT] with [ELEMENT]
			//[ALL/USR/PNT/TGR] [ELEMENT] [ELEMENT]
			Player[] actingPlayers = GetActingPlayers();

			for (int i = 0; i < PlayersEffected; i++) {
				int l = actingPlayers[i].actionElementMaskTo.Count;
				actingPlayers[i].actionElementMaskFrom.Add((CreatureElement)Memory[Counter + 2]);
				actingPlayers[i].actionElementMaskTo.Add((CreatureElement)Memory[Counter + 3]);
				Battle.AddMessage(OpponentText(actingPlayers[i]) + actingPlayers[i].ActiveCreature.Nickname + "'s Actions changed from " + (CreatureElement)Memory[Counter + 2]  + " to " + (CreatureElement)Memory[Counter + 3] + "!");
			}
			Counter += 3;
		}

        void OP_17() {
			//add [ELEMENT] to [USR/PNT/ALL]

        }

        void OP_18() {
			//give [ELEMENT] [POS/NEG] buff to [USR/PNT/ALL]
			//[ALL/USR/PNT/TGR] [ELEMENT] [POS/NEG] [NUMBER]
			Player[] actingPlayers = GetActingPlayers();

			for (int i = 0; i < PlayersEffected; i++) {
				actingPlayers[i].ElementBoosts[Memory[Counter + 2]] += (byte)((Memory[Counter + 3]*2 - 1)*Memory[Counter + 4]);
				Battle.AddMessage(OpponentText(actingPlayers[i]) + actingPlayers[i].ActiveCreature.Nickname + " has had its element buffed!");
			}
			Counter += 4;

		}

        void OP_19() {
			//remove all buffs [USR/PNT/ALL]

        }

        void OP_1A() {
			//give [ELEMENT] [NUMBER] [POS/NEG] priority to [USR/PNT/ALL]

        }

        void OP_1B() {
			//give [NUMBER] [POS/NEG] priority to [USR/PNT/ALL]

        }

        void OP_1C() {
			//increase mana cost of [USR/PNT/ALL] [ELEMENT] moves by [NUMBER]
        }

        void OP_1D() {
			//change [USR/PNT/ALL] move elements all to [ELEMENT]

        }

        void OP_1E() {
			//replace [USR/PNT/ALL] [ELEMENT] move elements with [ELEMENT]

        }

        void OP_1F() {
			//reset last move element changes

        }

        void OP_20() {

        }
        void OP_21() {

        }
        void OP_22() {

        }
        void OP_23() {

        }
        void OP_24() {

        }
        void OP_25() {

        }
        void OP_26() {

        }
        void OP_27() {

        }
        void OP_28() {

        }
        void OP_29() {

        }
        void OP_2A() {

        }
        void OP_2B() {

        }
        void OP_2C() {

        }
        void OP_2D() {

        }
        void OP_2E() {

        }
        void OP_2F() {

        }
        void OP_30() {

        }
        void OP_31() {

        }
        void OP_32() {

        }
        void OP_33() {

        }
        void OP_34() {

        }
        void OP_35() {

        }
        void OP_36() {

        }
        void OP_37() {

        }
        void OP_38() {

        }
        void OP_39() {

        }
        void OP_3A() {

        }
        void OP_3B() {

        }
        void OP_3C() {

        }
        void OP_3D() {

        }
        void OP_3E() {

        }
        void OP_3F() {

        }
        void OP_40() {

        }
        void OP_41() {

        }
        void OP_42() {

        }
        void OP_43() {

        }
        void OP_44() {

        }
        void OP_45() {

        }
        void OP_46() {

        }
        void OP_47() {

        }
        void OP_48() {

        }
        void OP_49() {

        }
        void OP_4A() {

        }
        void OP_4B() {

        }
        void OP_4C() {

        }
        void OP_4D() {

        }
        void OP_4E() {

        }
        void OP_4F() {

        }
        void OP_50() {

        }
        void OP_51() {

        }
        void OP_52() {

        }
        void OP_53() {

        }
        void OP_54() {

        }
        void OP_55() {

        }
        void OP_56() {

        }
        void OP_57() {

        }
        void OP_58() {

        }
        void OP_59() {

        }
        void OP_5A() {

        }
        void OP_5B() {

        }
        void OP_5C() {

        }
        void OP_5D() {

        }
        void OP_5E() {

        }
        void OP_5F() {

        }
        void OP_60() {

        }
        void OP_61() {

        }
        void OP_62() {

        }
        void OP_63() {

        }
        void OP_64() {

        }
        void OP_65() {

        }
        void OP_66() {

        }
        void OP_67() {

        }
        void OP_68() {

        }
        void OP_69() {

        }
        void OP_6A() {

        }
        void OP_6B() {

        }
        void OP_6C() {

        }
        void OP_6D() {

        }
        void OP_6E() {

        }
        void OP_6F() {

        }

        public void RunEffectScript(List<byte> EffectScript) {
            Memory = EffectScript;
        }


    }*/
}
