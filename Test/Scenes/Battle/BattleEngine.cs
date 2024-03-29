﻿using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Battle {

    enum ActionCategory {
        PHYSICAL,
        MYSTICAL,
        DEFENSIVE,
        ADAPTIVE,
        STRUCTURED
    }

    enum MovementCategory {
        NULL,
        MOVE_LEFT,
        HOLD_GROUND,
        DO_NOTHING,
        MOVE_RIGHT
    }

    class BattleEngine {

        public Player Player1;
        public Player Player2;

        private MovementCategory PreviousMovement;
        private byte OldArenaOrientation;
        private byte ArenaOrientation;

        public bool IsDeathTurn;

        public int EffectGroups = 0;

        public BattleTile[] AllEffects = new BattleTile[11]; //0-7 tileeffects, 8 centre effects, 9-10 creature effects
                                                             //TODO: I THINK YOU SHOULD SCRAP ALLEFFECTS AND JUST GO WITH THE SORTEDEFFECTS
        private BattlePhase[] BattlePhases = new BattlePhase[9];

        public Dictionary<EffectTrigger, SortedList<byte, List<BattleEffect>>> SortedEffects;

        private EffectExecute Interpreter;
        private BattleNetwork Network;

        public Random randomGen;

        public BattleEngine() {
            Player1 = new Player(1, 0);
            Player2 = new Player(2, 4);
            Player1.setStart(1);
            Player2.setStart(3);

            Interpreter = new EffectHardExecute(this);
            Network = new BattleNetwork(null, 0);
            Network.StartOfflineClient();

            randomGen = new Random(0);

            Player1.playerServerNumber = 0;
            Player2.playerServerNumber = 1;

            for (int i = 0; i < 11; i++) {
                AllEffects[i] = new BattleTile();
            }

            for (int i = 0; i < 9; i++) {
                BattlePhases[i] = new BattlePhase(this, (byte)(i + 1));
            }

            pollIndex = 10;

            PreviousMovement = MovementCategory.DO_NOTHING;
            OldArenaOrientation = 0;
            ArenaOrientation = 0;

            SortedEffects = new Dictionary<EffectTrigger, SortedList<byte, List<BattleEffect>>>();
            foreach (EffectTrigger trigger in Enum.GetValues(typeof(EffectTrigger))) {
                SortedEffects[trigger] = new SortedList<byte, List<BattleEffect>>();
            }
        }

        public int pollIndex;

        public List<SceneAnimation> Poll() {
            if (Player1.Ready) {
                if (!Player1.SentData) {
                    var tmp = new byte[2];
                    tmp[0] = Player1.SelectedActionNumber;
                    tmp[1] = (byte)Player1.SelectedMovement;
                    Network.SendServer(tmp);
                }
                if (!Player2.Ready) {
                    //TODO: LOCK THE UI SO YOU CANT CHOOSE ANYTHING ELSE
                    var tmp = Network.ReadServer();

                } else {
                    if (IsDeathTurn) {
                        List<SceneAnimation> DeathTurn = new List<SceneAnimation>();

                        if (Player1.IsNotInArena()) {

                            Player1.forceSwitched = false;
                            Player1.InSwitchState = true;
                            DeathTurn.AddRange(Switch(Player1));
                            Player1.InSwitchState = false;
                        }
                        if (Player2.IsNotInArena()) {

                            Player2.forceSwitched = false;
                            Player2.InSwitchState = true;
                            DeathTurn.AddRange(Switch(Player2));
                            Player2.InSwitchState = false;
                        }

                        IsDeathTurn = false;
                        DeathTurn.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.END_TURN, null, "#END OF TURN#"));
                        return DeathTurn;
                    } else {
                        if (pollIndex == 10) {
                            foreach (EffectTrigger type in Enum.GetValues(typeof(EffectTrigger))) {
                                foreach (List<BattleEffect> Effects in SortedEffects[type].Values) {
                                    foreach (BattleEffect Effect in Effects) {
                                        if (Effect.Priority != 10) BattlePhases[Effect.Priority - 1].CurrentPhaseEffects.Add(Effect);
                                    }
                                }
                            }

                            int ran = randomGen.Next(0, 2);

                            // if (!Player1.HasActionDelay.Evaluate()) {
                            BattlePhases[CheckPace(Player1, Player2, ran) - 1 + Player1.SelectedAction.Priority - 4].Add(new ActionResolution(this, Player1, Player2));
                            // }
                            // if (!Player2.HasActionDelay.Evaluate()) {
                            BattlePhases[CheckPace(Player2, Player1, ran) - 1 + Player2.SelectedAction.Priority - 4].Add(new ActionResolution(this, Player2, Player1));
                            //  }
                            BattlePhases[CheckAwe(Player1, Player2, ran) - 1].Add(new MovementResolution(this, Player1, Player2));
                            BattlePhases[CheckAwe(Player2, Player1, ran) - 1].Add(new MovementResolution(this, Player2, Player1));

                            BattlePhases[8].Add(new EndResolution(this));

                            pollIndex = 0;
                        }
                        pollIndex++;
                        if (pollIndex == 10) {
                            Player1.Ready = false;
                            Player1.SelectedAction = null;
                            Player1.SelectedMovement = MovementCategory.NULL;
                            Player2.Ready = false;
                            Player2.SelectedAction = null;
                            Player2.SelectedMovement = MovementCategory.NULL;

                            for (int i = 0; i < 9; i++) {
                                BattlePhases[i] = new BattlePhase(this, (byte)(i + 1));
                                //TODO: DON'T DESTROY BATTLEPHASES JUST RESET THEM DEAR GOD
                            }

                            List<SceneAnimation> FinishTurn = new List<SceneAnimation>();
                            if (Player1.IsNotInArena()) {
                                IsDeathTurn = true;
                                FinishTurn.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.USER_DEATH_SELECT, null, "#USER DEATH TURN ACTIVATED#"));
                            }
                            if (Player2.IsNotInArena()) {
                                IsDeathTurn = true;
                                List<SceneAnimation> death = new List<SceneAnimation>();
                                FinishTurn.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.OPPONENT_DEATH_SELECT, null, "#OPPONENT DEATH TURN ACTIVATED#"));
                            }
                            FinishTurn.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.END_TURN, null, "#END OF TURN#"));
                            return FinishTurn;
                        }
                        return BattlePhases[pollIndex - 1].Run();
                    }
                }
            }
            return null;
        }

        public Player GetOpponent(Player User) {
            if (User == Player1) {
                return Player2;
            }
            return Player1;
        }

        public byte CheckPace(Player PlayerA, Player PlayerB, int ran) {
            Player P1 = PlayerA;
            Player P2 = PlayerB;

            if (P1.GetTotalStat(CreatureStats.PACE) > P2.GetTotalStat(CreatureStats.PACE)) {
                return 4;
            } else if (P1.GetTotalStat(CreatureStats.PACE) < P2.GetTotalStat(CreatureStats.PACE)) {
                return 5;
            } else {
                if (ran == PlayerA.playerServerNumber) {
                    return 4;
                } else {
                    return 5;
                }
            }
        }
        public byte CheckAwe(Player PlayerA, Player PlayerB, int ran) {
            Player P1 = PlayerA;
            Player P2 = PlayerB;

            if (P1.GetTotalStat(CreatureStats.AWE) > P2.GetTotalStat(CreatureStats.AWE)) {
                return 3;
            } else if (P1.GetTotalStat(CreatureStats.AWE) < P2.GetTotalStat(CreatureStats.AWE)) {
                return 6;
            } else {
                if (ran == PlayerA.playerServerNumber) {
                    return 3;
                } else {
                    return 6;
                }
            }
        }


        public List<SceneAnimation> AddEffect(BattleEffect OriginalEffect, Player User) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            foreach (byte Placement in OriginalEffect.Placement) {
                BattleEffect Effect = new BattleEffect(OriginalEffect, User);

                if (Placement < 8) {
                    if (Effect.User == Player1) {
                        Effect.CurrentPlacement = (byte)((Placement + ArenaOrientation) % 8);
                    } else {
                        Effect.CurrentPlacement = (byte)((Placement + ArenaOrientation + 4) % 8);
                    }
                }

                if (AllEffects[Effect.CurrentPlacement].CanAddEffect()) {
                    Effect.SetEffectGroup(EffectGroups);
                    foreach (EffectScript Script in Effect.Scripts) {
                        if (Script.Trigger == EffectTrigger.ON_EFFECT_ENTER) {
                            Animations.AddRange(InterpretEffect(Effect, Script, User, null, Effect.CurrentPlacement));
                        }
                    }
                    if (Placement < 8) {
                        Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_EFFECT, new double[] { Effect.CurrentPlacement, Effect.GetEffectGroup() }, Effect.Name));
                        Animations.AddRange(RunTriggerTypeEffect(EffectTrigger.ON_NEW_TILE_PLACED, User, true));
                        foreach (EffectScript Script in Effect.Scripts) {
                            if (Script.Trigger == EffectTrigger.ON_STAND_ENTER) {
                                if (Effect.CurrentPlacement == Player1.Placement && !AllEffects[Player1.Placement].HasEffectGroup(Effect.GetEffectGroup())) {
                                    if (Effect.EffectAnimation != null) {
                                        Animations.Add(Effect.EffectAnimation);
                                    }
                                    Animations.AddRange(InterpretEffect(Effect, Script, Effect.User, Player1, Effect.CurrentPlacement));

                                } else if (Effect.CurrentPlacement == Player2.Placement && !AllEffects[Player2.Placement].HasEffectGroup(Effect.GetEffectGroup())) {
                                    if (Effect.EffectAnimation != null) {
                                        Animations.Add(Effect.EffectAnimation);
                                    }
                                    Animations.AddRange(InterpretEffect(Effect, Script, Effect.User, Player2, Effect.CurrentPlacement));
                                }
                            }
                        }
                        if (Effect.User == Player1) {
                            AllEffects[(Placement + ArenaOrientation) % 8].AddEffect(Effect);
                            Animations.AddRange(RunTilePlacedOnTopEffect(Effect.CurrentPlacement, User));

                        } else {
                            AllEffects[(Placement + ArenaOrientation + 4) % 8].AddEffect(Effect);
                            Animations.AddRange(RunTilePlacedOnTopEffect(Effect.CurrentPlacement, User));
                        }

                    } else if (Placement == 8) {
                        //TODO: CENTRE EFFECTS
                    } else if (Placement == 9) {
                        if (Effect.User == Player1) {
                            AllEffects[9].AddEffect(Effect);
                            Effect.CurrentPlacement = 9;
                        } else {
                            AllEffects[10].AddEffect(Effect);
                            Effect.CurrentPlacement = 10;
                        }
                    } else {
                        if (Effect.User == Player1) {
                            AllEffects[10].AddEffect(Effect);
                            Effect.CurrentPlacement = 10;
                        } else {
                            AllEffects[9].AddEffect(Effect);
                            Effect.CurrentPlacement = 9;
                        }
                    }

                    Animations.AddRange(RunTriggerTypeEffect(EffectTrigger.ON_OTHER_EFFECT_ENTER, User, true));

                    foreach (EffectScript Script in Effect.Scripts) {
                        List<BattleEffect> Effects;
                        if (SortedEffects[Script.Trigger].ContainsKey(Script.Speed)) {
                            Effects = SortedEffects[Script.Trigger][Script.Speed];
                        } else {
                            Effects = new List<BattleEffect>();
                            SortedEffects[Script.Trigger].Add(Script.Speed, Effects);
                        }
                        Effects.Add(Effect);
                    }
                }
            }
            EffectGroups++;
            return Animations;
        }

        public List<SceneAnimation> RemoveEffect(BattleEffect Effect, bool NaturalRemoval) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            AllEffects[Effect.CurrentPlacement].RemoveEffect(Effect);
            foreach (EffectScript Script in Effect.Scripts) {
                if (NaturalRemoval) {
                    if (Script.Trigger == EffectTrigger.ON_EFFECT_TIMEOUT) {
                        Animations.AddRange(InterpretEffect(Effect, Script, Effect.User, null, Effect.CurrentPlacement));
                    }

                    if (Script.Trigger == EffectTrigger.ON_STAND_EXIT) {
                        if (Effect.CurrentPlacement == Player1.Placement && !AllEffects[Player1.Placement].HasEffectGroup(Effect.GetEffectGroup())) {
                            if (Effect.EffectAnimation != null) {
                                Animations.Add(Effect.EffectAnimation);
                            }
                            Animations.AddRange(InterpretEffect(Effect, Script, Effect.User, Player1, Effect.CurrentPlacement));

                        } else if (Effect.CurrentPlacement == Player2.Placement && !AllEffects[Player2.Placement].HasEffectGroup(Effect.GetEffectGroup())) {
                            if (Effect.EffectAnimation != null) {
                                Animations.Add(Effect.EffectAnimation);
                            }
                            Animations.AddRange(InterpretEffect(Effect, Script, Effect.User, Player2, Effect.CurrentPlacement));
                        }
                    }

                    SortedEffects[Script.Trigger][Script.Speed].Remove(Effect);

                    if (Effect.CurrentPlacement < 8) {
                        Interpreter.SetPrevElement(Effect.Element); //TODO: HIGHLY LIKELY A BETTER WAY OF DOING ALL THIS
                        Animations.AddRange(RunTriggerTypeEffect(EffectTrigger.ON_OTHER_EFFECT_TIMEOUT, Effect.User, true));
                    }

                } else {
                    if (Script.Trigger == EffectTrigger.ON_EFFECT_EXIT) {
                        Interpreter.SetPrevElement(Effect.Element);
                        Animations.AddRange(InterpretEffect(Effect, Script, Effect.User, null, Effect.CurrentPlacement));
                    }

                    if (Script.Trigger == EffectTrigger.ON_STAND_EXIT) {
                        if (Effect.CurrentPlacement == Player1.Placement && !AllEffects[Player1.Placement].HasEffectGroup(Effect.GetEffectGroup())) {
                            if (Effect.EffectAnimation != null) {
                                Animations.Add(Effect.EffectAnimation);
                            }
                            Animations.AddRange(InterpretEffect(Effect, Script, Effect.User, Player1, Effect.CurrentPlacement));

                        } else if (Effect.CurrentPlacement == Player2.Placement && !AllEffects[Player2.Placement].HasEffectGroup(Effect.GetEffectGroup())) {
                            if (Effect.EffectAnimation != null) {
                                Animations.Add(Effect.EffectAnimation);
                            }
                            Animations.AddRange(InterpretEffect(Effect, Script, Effect.User, Player2, Effect.CurrentPlacement));
                        }
                    }

                    SortedEffects[Script.Trigger][Script.Speed].Remove(Effect);

                    if (Effect.CurrentPlacement < 8) {
                        Interpreter.SetPrevElement(Effect.Element);
                        Animations.AddRange(RunTriggerTypeEffect(EffectTrigger.ON_OTHER_EFFECT_EXIT, Effect.User, true));
                    }
                }
            }
            if (Effect.CurrentPlacement < 8) Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.REMOVE_EFFECT, new double[] { Effect.CurrentPlacement, Effect.GetEffectGroup() }, Effect.Name));

            return Animations;
        }

        public List<SceneAnimation> RemoveEffectsOfElement(CreatureElement Element) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            for (int i = 0; i < 8; i++) {
                foreach (BattleEffect Effect in AllEffects[i].effects) {
                    if (Effect != null && Effect.Element == Element) {
                        Animations.AddRange(RemoveEffect(Effect, false));
                    }
                }
            }
            return Animations;
        }

        public List<SceneAnimation> RemoveAllEffectsExceptElement(CreatureElement Element) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 3; j++) {
                    if (AllEffects[i].effects[j] != null && AllEffects[i].effects[j].Element != Element) {
                        Animations.AddRange(RemoveEffect(AllEffects[i].effects[j], false));
                    }
                }
            }
            return Animations;
        }


        public List<SceneAnimation> Switch(Player Target) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            Animations.AddRange(
                RunTriggerTypeEffect(EffectTrigger.ON_YOUR_SWITCH, Target, false)
            );

            Target.ActiveCreature = Target.Team[Target.SelectedCreature];
            Target.Reset();
            Target.JustSwitchedIn = true;

            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.SWITCH_IN, new double[] {
                Target.playerNumber,
                Target.ActiveCreature.CreatureType.ID,
                Target.ActiveCreature.Health,
                Target.ActiveCreature.GetTotalStat(CreatureStats.HEALTH),
                Target.ActiveCreature.Kin,
                (double)Target.ActiveCreature.CreatureType.Elements[0],
                (double)Target.ActiveCreature.CreatureType.Elements[1],
                (double)Target.ActiveCreature.CreatureType.Elements[2], //TODO: ADD STATUS
            }, Target.ActiveCreature.Nickname));

            Animations.AddRange(
                RunCreatureChangeEffect(EffectTrigger.ON_STAND_ENTER, Target)
            );
            Target.JustSwitchedIn = false;

            Animations.AddRange(
                RunTriggerTypeEffect(EffectTrigger.ON_OPPONENT_SWITCH, GetOpponent(Target), false)
            );

            Target.RecentlySwitched = true;

            return Animations;
        }

        public List<SceneAnimation> Damage(Player Target, CreatureElement Element, ActionCategory Catagory, byte BaseAmount, byte AmountOfAttacks, SceneAnimation AttackAnimation, bool TriggersAttackFlags) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            if (TriggersAttackFlags) {
                Animations.AddRange(
                    RunTriggerTypeEffect(EffectTrigger.BEFORE_ATTACKED, Target, false)
                );
            }

            for (int i = 0; i < AmountOfAttacks; i++) {
                Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ATTACK, new double[] {
                GetOpponent(Target).playerNumber,
                (double)Catagory}, "#ATTACK ANIMATION#"));
                //Animations.Add(AttackAnimation); //TODO: DONT PASS AS ARGUMENT

                //Animations.AddRange(Target.GiveDamage(Element, Catagory, BaseAmount, GetOpponent(Target).GetTotalStat(CreatureStats.STRENGTH), GetOpponent(Target).GetTotalStat(CreatureStats.INTELLIGENCE), Target.GetTotalStat(CreatureStats.ENDURANCE), Target.GetTotalStat(CreatureStats.WISDOM), GetOpponent(Target).HasElement(Element) ? 1.5 : 1, Target.GetElementEffectiveness(Element)));
                Animations.AddRange(Target.GiveDamage(Element, Catagory, BaseAmount, GetOpponent(Target)));
				if (Target.IsNotInArena()) {
                    AmountOfAttacks = (byte)(i + 1);
                    break;
                }
            }
            if (AmountOfAttacks > 1) {
                if (AmountOfAttacks == 2) {
                    Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "Damage landed twice!"));
                } else {
                    Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "Damage landed " + AmountOfAttacks + " times!"));
                }
            }
            if (Target.IsKilled()) {
                Animations.AddRange(RemoveFromArena(Target, true));
            } else if (!Target.IsNotInArena()) {
                if (TriggersAttackFlags) {
                    Animations.AddRange(
                        RunTriggerTypeEffect(EffectTrigger.AFTER_ATTACKED, Target, false)
                    );
                }
            }
            return Animations;
        }

        public List<SceneAnimation> RemoveFromArena(Player Target, bool IsKilled) {
            List<SceneAnimation> Animations = new List<SceneAnimation>();
            if (IsKilled) {
                Target.ActiveCreature.killed = false;
                Animations.AddRange(
                    RunTriggerTypeEffect(EffectTrigger.ON_YOUR_DEATH, Target, false)
                );
                Target.ActiveCreature.killed = true;
            }

            Animations.AddRange(RemovePersonalEffects(Target.playerNumber));

            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.SWITCH_OUT, new double[] {
                Target.playerNumber,
                IsKilled ? 1 : 0
            }, "#REMOVAL ANIMATION#"));

            if (IsKilled) {
                Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, Target.ActiveCreature.Nickname + " collapses!"));

                Animations.AddRange(
                    RunTriggerTypeEffect(EffectTrigger.ON_OPPONENT_DEATH, GetOpponent(Target), false)
                );
            } else {
                Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, Target.ActiveCreature.Nickname + " has been forced out of the arena!"));
            }
            return Animations;
        }

        public List<SceneAnimation> SetArenaRotation(byte set) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();
            int RotationDelta = set - ArenaOrientation;
            if (RotationDelta > 4) RotationDelta -= 8;
            if (RotationDelta < -4) RotationDelta += 8;
            ArenaOrientation = set;
            OldArenaOrientation = set;

            byte NewPlacement1 = (byte)((ArenaOrientation + 8) % 8);
            byte NewPlacement2 = (byte)((ArenaOrientation + 4 + 8) % 8);

            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ARENA, new double[] { RotationDelta }, "#MOVEMENT ANIMATION#"));

            Animations.AddRange(
                RunRotationChangeEffect(EffectTrigger.ON_STAND_EXIT, Player1, Player1.Placement, NewPlacement1)
            );
            Animations.AddRange(
                RunRotationChangeEffect(EffectTrigger.ON_STAND_EXIT, Player2, Player2.Placement, NewPlacement2)
            );
            Animations.AddRange(
                RunRotationChangeEffect(EffectTrigger.ON_STAND_ENTER, Player1, NewPlacement1, Player1.Placement)
            );
            Animations.AddRange(
                RunRotationChangeEffect(EffectTrigger.ON_STAND_ENTER, Player2, NewPlacement2, Player2.Placement)
            );

            Player1.Placement = NewPlacement1;
            Player2.Placement = NewPlacement2;

            return Animations;
        }

        public short RotationFromMovement(MovementCategory Movement) {
            short RotationDelta = 0;
            OldArenaOrientation = ArenaOrientation;

            if (Movement == MovementCategory.HOLD_GROUND) {
                if (PreviousMovement == MovementCategory.MOVE_LEFT) {
                    Movement = MovementCategory.MOVE_RIGHT;
                } else if (PreviousMovement == MovementCategory.MOVE_RIGHT) {
                    Movement = MovementCategory.MOVE_LEFT;
                }
            }

            PreviousMovement = Movement;

            if (Movement == MovementCategory.MOVE_LEFT) {
                if (ArenaOrientation == 0) {
                    ArenaOrientation = 7;
                } else {
                    ArenaOrientation--;
                }
                //-1
                RotationDelta = -1;
            } else if (Movement == MovementCategory.MOVE_RIGHT) {
                if (ArenaOrientation == 7) {
                    ArenaOrientation = 0;
                } else {
                    ArenaOrientation++;
                }
                //1
                RotationDelta = 1;
            }

            return RotationDelta;
        }

        public List<SceneAnimation> DeltaRotate(short RotationDelta) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            byte NewPlacement1 = (byte)((Player1.Placement + RotationDelta + 8) % 8);
            byte NewPlacement2 = (byte)((Player2.Placement + RotationDelta + 8) % 8);

            Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ARENA, new double[] { RotationDelta }, "#MOVEMENT ANIMATION#"));

            Animations.AddRange(
                RunRotationChangeEffect(EffectTrigger.ON_STAND_EXIT, Player1, Player1.Placement, NewPlacement1)
            );
            Animations.AddRange(
                RunRotationChangeEffect(EffectTrigger.ON_STAND_EXIT, Player2, Player2.Placement, NewPlacement2)
            );
            Animations.AddRange(
                RunRotationChangeEffect(EffectTrigger.ON_STAND_ENTER, Player1, NewPlacement1, Player1.Placement)
            );
            Animations.AddRange(
                RunRotationChangeEffect(EffectTrigger.ON_STAND_ENTER, Player2, NewPlacement2, Player2.Placement)
            );

            Player1.Placement = NewPlacement1;
            Player2.Placement = NewPlacement2;

            return Animations;
        }

        public List<SceneAnimation> RunTilePlacedOnTopEffect(byte Position, Player Placer) {
            List<SceneAnimation> Animations = new List<SceneAnimation>();
            List<BattleEffect> DeadEffects = new List<BattleEffect>();

            bool u_nia = Placer.IsNotInArena();
            bool o_nia = GetOpponent(Placer).IsNotInArena();

            foreach (List<BattleEffect> Effects in SortedEffects[EffectTrigger.ON_NEW_TILE_HERE].Values) {
                foreach (BattleEffect Effect in Effects) {

                    if (Effect.CurrentPlacement == Position) {
                        if (Effect.EffectAnimation != null) {
                            Animations.Add(Effect.EffectAnimation);
                        }
                        Animations.AddRange(InterpretEffect(Effect, Effect.GetTriggeredEffect(EffectTrigger.ON_NEW_TILE_HERE), Effect.User, null, Position));
                    }
                }
            }
            foreach (BattleEffect Effect in DeadEffects) {
                Animations.AddRange(RemoveEffect(Effect, true));
            }

            Animations.AddRange(CheckIfRemoved(Placer, GetOpponent(Placer), u_nia, o_nia));

            return Animations;
        }


        public List<SceneAnimation> RunRotationChangeEffect(EffectTrigger Trigger, Player Target, byte NewPosition, byte OldPosition) {
            List<SceneAnimation> Animations = new List<SceneAnimation>();
            List<BattleEffect> DeadEffects = new List<BattleEffect>();

            bool u_nia = Target.IsNotInArena();
            bool o_nia = GetOpponent(Target).IsNotInArena();

            foreach (List<BattleEffect> Effects in SortedEffects[Trigger].Values) {
                foreach (BattleEffect Effect in Effects) {
                    if (Effect.CurrentPlacement == NewPosition && !AllEffects[OldPosition].HasEffectGroup(Effect.GetEffectGroup())) {
                        if (Effect.EffectAnimation != null) {
                            Animations.Add(Effect.EffectAnimation);
                        }
                        Animations.AddRange(InterpretEffect(Effect, Effect.GetTriggeredEffect(Trigger), Effect.User, Target, Effect.CurrentPlacement));
                    }
                }
            }
            foreach (BattleEffect Effect in DeadEffects) {
                Animations.AddRange(RemoveEffect(Effect, true));
            }

            Animations.AddRange(CheckIfRemoved(Target, GetOpponent(Target), u_nia, o_nia));

            return Animations;
        }

        public List<SceneAnimation> RunCreatureChangeEffect(EffectTrigger Trigger, Player Target) {
            List<SceneAnimation> Animations = new List<SceneAnimation>();
            List<BattleEffect> DeadEffects = new List<BattleEffect>();

            bool u_nia = Target.IsNotInArena();
            bool o_nia = GetOpponent(Target).IsNotInArena();

            if (!Target.IsNotInArena()) {
                foreach (List<BattleEffect> Effects in SortedEffects[Trigger].Values) {
                    foreach (BattleEffect Effect in Effects) {
                        if (Effect.CurrentPlacement == Target.Placement) {
                            if (Effect.EffectAnimation != null) {
                                Animations.Add(Effect.EffectAnimation);
                            }
                            Animations.AddRange(InterpretEffect(Effect, Effect.GetTriggeredEffect(Trigger), Effect.User, Target, Effect.CurrentPlacement));
                        }
                    }
                }
                foreach (BattleEffect Effect in DeadEffects) {
                    Animations.AddRange(RemoveEffect(Effect, true));
                }
            }

            Animations.AddRange(CheckIfRemoved(Target, GetOpponent(Target), u_nia, o_nia));

            return Animations;
        }


        public List<SceneAnimation> RunTriggerTypeEffect(EffectTrigger Trigger, Player TargetUser, bool RunIfKilled) {
            //If TargetUser is parsed it will only run if they are not killed and own the effect
            //TODO: PLEASE UNDERSTAND THIS SO THAT DEATH IN THIS IS DETERMINISTIC
            List<SceneAnimation> Animations = new List<SceneAnimation>();
            List<BattleEffect> DeadEffects = new List<BattleEffect>();

            bool u_nia = TargetUser.IsNotInArena();
            bool o_nia = GetOpponent(TargetUser).IsNotInArena();


            if (RunIfKilled || !TargetUser.IsNotInArena()) {
                foreach (List<BattleEffect> Effects in SortedEffects[Trigger].Values) {
                    foreach (BattleEffect Effect in Effects) {
                        if (RunIfKilled || Effect.User == TargetUser) {
                            if (Effect.EffectAnimation != null) {
                                Animations.Add(Effect.EffectAnimation);
                            }
                            Animations.AddRange(InterpretEffect(Effect, Effect.GetTriggeredEffect(Trigger), Effect.User, null, 0));
                            if (Effect.IsLifespanDepleted()) DeadEffects.Add(Effect);
                        }
                    }
                }
                foreach (BattleEffect Effect in DeadEffects) {
                    Animations.AddRange(RemoveEffect(Effect, true));
                }

            }

            Animations.AddRange(CheckIfRemoved(TargetUser, GetOpponent(TargetUser), u_nia, o_nia));

            return Animations;
        }


        public void RemovePersonalEffect(BattleEffect Effect) {

            AllEffects[Effect.CurrentPlacement].RemoveEffect(Effect);
            foreach (EffectScript Script in Effect.Scripts) {

                    SortedEffects[Script.Trigger][Script.Speed].Remove(Effect);

            }
        }

        private List<SceneAnimation> RemovePersonalEffects(byte PlayerNumber) {
            List<SceneAnimation> Animations = new List<SceneAnimation>();//NOT NEEDED

            if (AllEffects[8 + PlayerNumber].effects[0] != null) RemovePersonalEffect(AllEffects[8 + PlayerNumber].effects[0]);
            if (AllEffects[8 + PlayerNumber].effects[0] != null) RemovePersonalEffect(AllEffects[8 + PlayerNumber].effects[0]);
            if (AllEffects[8 + PlayerNumber].effects[0] != null) RemovePersonalEffect(AllEffects[8 + PlayerNumber].effects[0]);

            return Animations;
        }

        private List<SceneAnimation> CheckIfRemoved(Player User, Player Opponent, bool u_nia, bool o_nia) {
            List<SceneAnimation> Animations = new List<SceneAnimation>();

            Player Target = User;
            if (!u_nia && Target.IsNotInArena()) {
                Animations.AddRange(RemoveFromArena(Target, !Target.forceSwitched));
            } else if(Target.RecentlySwitched) {
                Animations.AddRange(RemovePersonalEffects(Target.playerNumber));
                Target.RecentlySwitched = false;
            }

            Target = Opponent;
            if (!o_nia && Target.IsNotInArena()) {
                Animations.AddRange(RemoveFromArena(Target, !Target.forceSwitched));
            } else if (Target.RecentlySwitched) {
                Animations.AddRange(RemovePersonalEffects(Target.playerNumber));
                Target.RecentlySwitched = false;
            }

            return Animations;
        }

        public List<SceneAnimation> InterpretEffect(BattleEffect Effect, EffectScript EffectScripts, Player User, Player Trigger, byte TriggerPosition) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();
            Interpreter.SetNewFrame(Effect, User, GetOpponent(User), Trigger, TriggerPosition);
            //Interpreter.SetTargets(User, GetOpponent(User), Trigger, TriggerPosition);
            //Interpreter.SetEffect(Effect);
            Animations.AddRange(Interpreter.ExecuteEffect(EffectScripts));

            return Animations; //TODO: GET ANIMATIONS FROM SCRIPT
        }
    }
}