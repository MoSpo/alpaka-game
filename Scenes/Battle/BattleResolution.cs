using System;
using System.Collections.Generic;

namespace Alpaka.Scenes.Battle {
	abstract class BattleResolution {
		protected BattleEngine Battle;
		protected Player User;
		protected Player Opponent;
		public abstract List<SceneAnimation> Resolve();

		public Player GetUser() {
			return User;
		}
	}

	class MovementResolution : BattleResolution {

		short movementDirection;

		public MovementResolution(BattleEngine Battle, Player User, Player Opponent) {
			this.Battle = Battle;
			this.User = User;
			this.Opponent = Opponent;
		}

		public override List<SceneAnimation> Resolve() {

			List<SceneAnimation> Animations = new List<SceneAnimation>();

			short RotationDelta = Battle.RotationFromMovement(User.SelectedMovement);

			if (RotationDelta != 0) {

                    Animations.AddRange(Battle.RunTriggerTypeEffect(EffectTrigger.BEFORE_MOVEMENT, User, true));

                    if (RotationDelta > 0) {
                        Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, User.ActiveCreature.Nickname + " moves right..."));
                    } else if (RotationDelta < 0) {
                        Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, User.ActiveCreature.Nickname + " moves left..."));
                    }
                //Do Movement

                if (!Opponent.CanBeMoved.Evaluate()) {
                    Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "But " + Opponent.ActiveCreature.Nickname + " cannot be moved!"));
                } else {
                    Animations.AddRange(Battle.DeltaRotate(RotationDelta));

                    Animations.AddRange(Battle.RunTriggerTypeEffect(EffectTrigger.AFTER_MOVEMENT, User, true));
                }
			} else {
				Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, User.ActiveCreature.Nickname + " stays still..."));
			}
			return Animations;
		}
	}


	class ActionResolution : BattleResolution {
		private CreatureAction Action;
		public ActionResolution(BattleEngine Battle, Player User, Player Opponent) {
			this.Battle = Battle;
			this.User = User;
			this.Opponent = Opponent;
			Action = User.SelectedAction;
		}

		public override List<SceneAnimation> Resolve() {

			List<SceneAnimation> Animations = new List<SceneAnimation>();

			Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, User.ActiveCreature.Nickname + " " + Action.Style + " with " + Action.Name + "!"));

			if (User.CanUseActionThisTurn.Evaluate()) {

				Animations.AddRange(Battle.RunTriggerTypeEffect(EffectTrigger.BEFORE_ACTION, User, false));

				if (Opponent.CanBeAttacked.Evaluate() || !(Action.Catagory == ActionCategory.MYSTICAL || Action.Catagory == ActionCategory.PHYSICAL)) {
					if (Action.Mana > 0) {
						Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.KIN_BAR, new double[] {
							User.playerNumber,
						User.ActiveCreature.Kin,
						User.ActiveCreature.Kin - Action.Mana*100,
						-1*Action.Mana*100,
						}, "#KIN COST#"));
						User.ActiveCreature.Kin -= Action.Mana * 100;
					}

					if (Action.ActionEffect != null) {
						Action.ActionEffect.SetBaseAttacks(User.GetTotalStat(CreatureStats.STRENGTH), User.GetTotalStat(CreatureStats.INTELLIGENCE));
						Animations.AddRange(Battle.AddEffect(Action.ActionEffect, User)); //TODO???: PROPER ADDING OF ACTION EFFECTS TO CURRENT BATTLE STATE
					}
				}
				if ((Action.Catagory == ActionCategory.MYSTICAL || Action.Catagory == ActionCategory.PHYSICAL)) { //TODO: ADD ADAPTIVES AND DEFENSIVES

					if (Opponent.CanBeAttacked.Evaluate()) {
						Animations.AddRange(Battle.RunTriggerTypeEffect(EffectTrigger.BEFORE_ATTACKING, User, false));

						//Do Attack
						Animations.AddRange(Battle.Damage(Opponent, Action.Element, Action.Catagory, Action.Power, User.amountOfAttacks, Action.Animation, User.TriggersAttackFlags.Evaluate())); //BEOFRE_ATTACKED migrated into here

						User.amountOfAttacks = 1; //Needed for if any following effects inflict damage

						Animations.AddRange(Battle.RunTriggerTypeEffect(EffectTrigger.AFTER_ATTACKING, User, false));
					} else {
						Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, Opponent.ActiveCreature.Nickname + " cannot be attacked!"));
					}
				} else {
					Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ATTACK, new double[] {
				Battle.GetOpponent(User).playerNumber, 
						(double)Action.Catagory}, "#ATTACK ANIMATION#"));
					//Animations.Add(Action.Animation); //TODO: DONT PASS AS ARGUMENT
					User.amountOfAttacks = 1;
				}

				Animations.AddRange(Battle.RunTriggerTypeEffect(EffectTrigger.AFTER_ACTION, User, false));

			} else {
				Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "But the Action failed!"));
			}

			User.amountOfAttacks = 1; //Needed for if user can't attack this turn

			return Animations;
		}
	}


	class EndResolution : BattleResolution {

		public EndResolution(BattleEngine Battle) {
			this.Battle = Battle;
		}

		public override List<SceneAnimation> Resolve() {

			List<SceneAnimation> Animations = new List<SceneAnimation>();

			if (!Battle.Player1.IsNotInArena()) {
                if (Battle.Player1.RestoreHealthInseadOfMana.Evaluate()) {
					Animations.AddRange(Battle.Player1.Heal((int)Math.Floor(1.0 * Battle.Player1.ActiveCreature.GetTotalStat(CreatureStats.KIN) / 60)));
                } else if (Battle.Player1.RestoreMana.Evaluate()) {
					Animations.AddRange(Battle.Player1.GainKin((int)Math.Floor(1.0 * Battle.Player1.ActiveCreature.GetTotalStat(CreatureStats.KIN) / 6)));
                }
				Battle.Player1.DecreaseFlags();
			}

            if (!Battle.Player2.IsNotInArena()) {
                if (Battle.Player2.RestoreHealthInseadOfMana.Evaluate()) {
					Animations.AddRange(Battle.Player2.Heal((int)Math.Floor(1.0 * Battle.Player2.ActiveCreature.GetTotalStat(CreatureStats.KIN) / 60)));
				} else if (Battle.Player2.RestoreMana.Evaluate()) {
					Animations.AddRange(Battle.Player2.GainKin((int)Math.Floor(1.0 * Battle.Player2.ActiveCreature.GetTotalStat(CreatureStats.KIN) / 6)));
                }
                Battle.Player2.DecreaseFlags();
            }

            return Animations;
		}
	}
}