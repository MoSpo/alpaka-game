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

                Animations.AddRange(Battle.RunEffectType(EffectTrigger.BEFORE_MOVEMENT, null));

                if (RotationDelta > 0) {
                    Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, User.ActiveCreature.Nickname + " moves right..."));
                } else if (RotationDelta < 0) {
                    Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, User.ActiveCreature.Nickname + " moves left..."));
                }
                //Do Movement
                Animations.AddRange(Battle.DeltaRotate(RotationDelta));

                Animations.AddRange(Battle.RunEffectType(EffectTrigger.AFTER_MOVEMENT, null));
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

                Animations.AddRange(Battle.RunEffectType(EffectTrigger.BEFORE_ACTION, User));

                if (Action.ActionEffect != null && Opponent.CanBeAttacked.Evaluate()) {
                    Animations.AddRange(Battle.AddEffect(Action.ActionEffect, User)); //TODO???: PROPER ADDING OF ACTION EFFECTS TO CURRENT BATTLE STATE
                }

                if ((Action.Catagory == ActionCategory.MYSTICAL || Action.Catagory == ActionCategory.PHYSICAL)) { //TODO: ADD ADAPTIVES AND DEFENSIVES

                    if (Opponent.CanBeAttacked.Evaluate()) {
                        Animations.AddRange(Battle.RunEffectType(EffectTrigger.BEFORE_ATTACKING, User));

                        //Do Attack
                        Animations.AddRange(Battle.Damage(Opponent, Action.Element, Action.Catagory, Action.Power, User.amountOfAttacks, Action.Animation, User.TriggersAttackFlags.Evaluate())); //BEOFRE_ATTACKED migrated into here

                        User.amountOfAttacks = 1; //Needed for if any following effects inflict damage

                        Animations.AddRange(Battle.RunEffectType(EffectTrigger.AFTER_ATTACKING, User));
                    } else {
                        Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, Opponent.ActiveCreature.Nickname + " cannot be attacked!"));
                    }
                } else {
                    Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ATTACK, new double[] {
                Battle.GetOpponent(User).playerNumber }, "#ATTACK ANIMATION#"));
                    //Animations.Add(Action.Animation); //TODO: DONT PASS AS ARGUMENT
                    User.amountOfAttacks = 1;
                }

                Animations.AddRange(Battle.RunEffectType(EffectTrigger.AFTER_ACTION, User));

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

            if (!Battle.Player1.IsKilled()) {
                if (Battle.Player1.RestoreHealthInseadOfMana.Evaluate()) {
                    short Increase = (short)Math.Floor(Battle.Player1.actionCombo * Battle.Player1.ActiveCreature.GetTotalStat(CreatureStats.KIN) / 10);
                    Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.HEALTH_BAR, new double[] {
                        1,
                        Battle.Player1.ActiveCreature.GetTotalStat(CreatureStats.HEALTH),
                        Battle.Player1.ActiveCreature.Health + Increase,
                        Increase,
                        1,1}, "#END PHASE KIN [ADDED TO HEALTH]#"));
                    Battle.Player1.ActiveCreature.Health += Increase;
                } else {
                    short Increase = (short)Math.Floor(Battle.Player1.actionCombo * Battle.Player1.ActiveCreature.GetTotalStat(CreatureStats.KIN) / 5);
                    Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.KIN_BAR, new double[] {
                        1,
                        Battle.Player1.ActiveCreature.Kin,
                        Battle.Player1.ActiveCreature.Kin + Increase,
                        Increase,
                        }, "#END PHASE KIN#"));
                    Battle.Player1.ActiveCreature.Kin += Increase;
                }
                Battle.Player1.DecreaseFlags();
            }
            if (!Battle.Player2.IsKilled()) {
                if (Battle.Player2.RestoreHealthInseadOfMana.Evaluate()) {
                    short Increase = (short)Math.Floor(Battle.Player2.actionCombo * Battle.Player2.ActiveCreature.GetTotalStat(CreatureStats.KIN) / 10);
                    Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.HEALTH_BAR, new double[] {
                        2,
                        Battle.Player2.ActiveCreature.GetTotalStat(CreatureStats.HEALTH),
                        Battle.Player2.ActiveCreature.Health + Increase,
                        Increase,
                        1,1}, "#END PHASE KIN [ADDED TO HEALTH]#"));
                    Battle.Player2.ActiveCreature.Health += Increase;
                } else {
                    short Increase = (short)Math.Floor(Battle.Player2.actionCombo * Battle.Player2.ActiveCreature.GetTotalStat(CreatureStats.KIN) / 5);
                    Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.KIN_BAR, new double[] {
                2,
                Battle.Player2.ActiveCreature.Kin,
                Battle.Player2.ActiveCreature.Kin + Increase,
                Increase,
                }, "#END PHASE KIN#"));
                    Battle.Player2.ActiveCreature.Kin += Increase;
                }
                Battle.Player2.DecreaseFlags();

            }

            return Animations;
        }
    }
}