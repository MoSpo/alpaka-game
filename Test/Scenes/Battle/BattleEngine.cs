using Microsoft.Xna.Framework.Input;
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

        private bool IsPreviousMovementLeft;
        private byte OldArenaOrientation;
        private byte ArenaOrientation;

        public BattleTile[] AllEffects = new BattleTile[11]; //0-7 tileeffects, 8 centre effects, 9-10 creature effects

        private BattlePhase[] BattlePhases = new BattlePhase[9];

        public Dictionary<EffectTrigger, SortedList<byte, List<BattleEffect>>> SortedEffects;

        private EffectExecute Interpreter;
        private BattleNetwork Network;

        private Random randomGen;

        public BattleEngine() {
            Player1 = new Player();
            Player2 = new Player();

            Interpreter = new EffectHardExecute(this);
            Network = new BattleNetwork(null, 0);
            Network.StartOfflineClient();

            randomGen = new Random(0);

            Player1.playerNumber = 1;
            Player2.playerNumber = 2;

            Player1.playerServerNumber = 0;
            Player2.playerServerNumber = 1;

            for (int i = 0; i < 11; i++) {
                AllEffects[i] = new BattleTile();
            }

            for (int i = 0; i < 9; i++) {
                BattlePhases[i] = new BattlePhase(this, (byte)(i + 1));
            }

            pollIndex = 10;

            IsPreviousMovementLeft = false;
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
                    //Player2.SelectAction(0);
                    //Player2.SelectMovement(MovementCategory.HOLD_GROUND);
                    var tmp = new byte[2];
                    tmp[0] = 0;
                    tmp[1] = (byte)MovementCategory.HOLD_GROUND;
                    Network.SendAsOfflineServer(tmp);
                }
                if (!Player2.Ready) {
                    //TODO: LOCK THE UI SO YOU CANT CHOOSE ANYTHING ELSE
                    var tmp = Network.ReadServer();
                    Player2.SelectAction(tmp[0]);
                    Player2.SelectMovement(tmp[1]);
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

                        BattlePhases[CheckPace(Player1, Player2, ran) - 1 + Player1.SelectedAction.Priority - 4].Add(new ActionResolution(this, Player1, Player2));
                        BattlePhases[CheckPace(Player2, Player1, ran) - 1 + Player2.SelectedAction.Priority - 4].Add(new ActionResolution(this, Player2, Player1));

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
                        return null;
                    }
                    return BattlePhases[pollIndex - 1].Run();
                }
            } else {
                //TODO: REMOVE ALL THIS HACKY CODE
                KeyboardState state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.Up)) {
                    Player1.SelectMovement(MovementCategory.DO_NOTHING);
                } else if (state.IsKeyDown(Keys.Down)) {
                    Player1.SelectMovement(MovementCategory.HOLD_GROUND);
                } else if (state.IsKeyDown(Keys.Left)) {
                    Player1.SelectMovement(MovementCategory.MOVE_LEFT);
                } else if (state.IsKeyDown(Keys.Right)) {
                    Player1.SelectMovement(MovementCategory.MOVE_RIGHT);
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
            CreatureInstance P1 = PlayerA.ActiveCreature;
            CreatureInstance P2 = PlayerB.ActiveCreature;

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
            CreatureInstance P1 = PlayerA.ActiveCreature;
            CreatureInstance P2 = PlayerB.ActiveCreature;


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

        public bool CheckDeath() {
            return (Player1.ActiveCreature.killed || Player2.ActiveCreature.killed);
        }



        public List<SceneAnimation> AddEffect(BattleEffect OriginalEffect, Player User) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            foreach (byte Placement in OriginalEffect.Placement) {
                BattleEffect Effect = new BattleEffect(OriginalEffect, User);

                foreach (EffectScript Script in Effect.Scripts) {
                    List<BattleEffect> Effects;
                    if (SortedEffects[Script.Trigger].ContainsKey(Script.Speed)) {
                        Effects = SortedEffects[Script.Trigger][Script.Speed];
                    } else {
                        Effects = new List<BattleEffect>();
                        SortedEffects[Script.Trigger].Add(Script.Speed, Effects);
                    }
                    Effects.Add(Effect);

                    if (Script.Trigger == EffectTrigger.ON_EFFECT_ENTER) {
                        Animations.AddRange(InterpretEffect(Script, User, null, Effect.CurrentPlacement));
                    }

                    Animations.AddRange(RunEffectType(EffectTrigger.ON_OTHER_EFFECT_ENTER, null));
                }

                if (Placement < 8) {
                    if (Effect.User == Player1) {
                        Effect.CurrentPlacement = (byte)((Placement + ArenaOrientation) % 8);
                        AllEffects[(Placement + ArenaOrientation) % 8].AddEffect(Effect);
                        Animations.AddRange(RunSameTilePlacedEffect((byte)((Placement + ArenaOrientation) % 8)));

                    } else {
                        Effect.CurrentPlacement = (byte)((Placement + ArenaOrientation + 4) % 8);
                        AllEffects[(Placement + ArenaOrientation + 4) % 8].AddEffect(Effect);
                        Animations.AddRange(RunSameTilePlacedEffect((byte)((Placement + ArenaOrientation + 4) % 8)));

                    }
                    Animations.AddRange(RunEffectType(EffectTrigger.ON_NEW_TILE_PLACED, null));
                    Animations.AddRange(RunTriggerEffectType(EffectTrigger.ON_STAND_ENTER, null));

                } else if (Placement == 8) {
                    //TODO: CENTRE EFFECTS
                } else if (Placement == 9) {
                    if (Effect.User == Player1) {
                        AllEffects[9].AddEffect(Effect);
                    } else {
                        AllEffects[10].AddEffect(Effect);
                    }
                } else {
                    if (Effect.User == Player1) {
                        AllEffects[10].AddEffect(Effect);
                    } else {
                        AllEffects[9].AddEffect(Effect);
                    }
                }

            }
            return Animations;
        }

        public List<SceneAnimation> RemoveEffect(BattleEffect Effect, bool NaturalRemoval) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            AllEffects[Effect.CurrentPlacement].RemoveEffect(Effect);
            foreach (EffectScript Script in Effect.Scripts) {
                if (NaturalRemoval) {
                    if (Script.Trigger == EffectTrigger.ON_EFFECT_TIMEOUT) {
                        Animations.AddRange(InterpretEffect(Script, Effect.User, null, Effect.CurrentPlacement));
                    }

                    Animations.AddRange(RunEffectType(EffectTrigger.ON_OTHER_EFFECT_TIMEOUT, null));
                    if (Script.Trigger == EffectTrigger.ON_STAND_EXIT) {
                        Animations.AddRange(RunTriggerEffectType(EffectTrigger.ON_STAND_EXIT, null));
                    }
                    SortedEffects[Script.Trigger][Script.Speed].Remove(Effect);

                } else {
                    if (Script.Trigger == EffectTrigger.ON_EFFECT_EXIT) {
                        Animations.AddRange(InterpretEffect(Script, Effect.User, null, Effect.CurrentPlacement));
                    }

                    Animations.AddRange(RunEffectType(EffectTrigger.ON_OTHER_EFFECT_EXIT, null));
                    if (Script.Trigger == EffectTrigger.ON_STAND_EXIT) {
                        Animations.AddRange(RunTriggerEffectType(EffectTrigger.ON_STAND_EXIT, null));
                    }

                    SortedEffects[Script.Trigger][Script.Speed].Remove(Effect);
                }
            }
            return Animations;
        }

        public List<SceneAnimation> RemoveEffectsOfElement(CreatureElement Element) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            for (int i = 0; i < 8; i++) {
                foreach (BattleEffect Effect in AllEffects[i].effects) {
                    if (Effect.Element == Element) {
                        Animations.AddRange(RemoveEffect(Effect, false));
                    }
                }
            }
            return Animations;
        }

        public List<SceneAnimation> RemoveAllEffectsExceptElement(CreatureElement Element) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            for (int i = 0; i < 8; i++) {
                foreach (BattleEffect Effect in AllEffects[i].effects) {
                    if (Effect.Element != Element) {
                        Animations.AddRange(RemoveEffect(Effect, false));
                    }
                }
            }
            return Animations;
        }


        public List<SceneAnimation> Switch(Player Target) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            Animations.AddRange(
                RunEffectType(EffectTrigger.ON_YOUR_SWITCH, Target)
            );

            Target.ActiveCreature = Target.SelectedSwitchCreature;
            Target.Reset();
            Target.JustSwitchedIn = true;
            Animations.AddRange(
                RunTriggerEffectType(EffectTrigger.ON_STAND_ENTER, Target)
            );
            Target.JustSwitchedIn = false;

            Animations.AddRange(
                RunEffectType(EffectTrigger.ON_OPPONENT_SWITCH, GetOpponent(Target))
            );

            return Animations;
        }

        public List<SceneAnimation> Damage(Player Target, CreatureElement Element, ActionCategory Catagory, byte BaseAmount, byte AmountOfAttacks, SceneAnimation AttackAnimation, bool TriggersAttackFlags) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            if (Target.CanBeAttacked.Evaluate()) {

                if (TriggersAttackFlags) {
                    Animations.AddRange(
                        RunEffectType(EffectTrigger.BEFORE_ATTACKED, Target)
                    );
                }

                for (int i = 0; i < AmountOfAttacks; i++) {
                    Animations.Add(new SceneAnimation(2, new double[] {
                GetOpponent(Target).playerNumber }, "#ATTACK ANIMATION#"));
                    //Animations.Add(AttackAnimation); //TODO: DONT PASS AS ARGUMENT
                    Animations.AddRange(Target.GiveDamage(Element, Catagory, BaseAmount, Target, GetOpponent(Target)));
                    if (Target.IsKilled()) {
                        AmountOfAttacks = (byte)(i + 1);
                        break;
                    }
                }
                if (AmountOfAttacks > 1) {
                    if (AmountOfAttacks == 2) {
                        Animations.Add(new SceneAnimation(0, null, "Damage landed twice!"));
                    } else {
                        Animations.Add(new SceneAnimation(0, null, "Damage landed " + AmountOfAttacks + " times!"));
                    }
                }
                if (Target.IsKilled()) {
                    Target.ActiveCreature.killed = false;
                    Animations.AddRange(
                        RunEffectType(EffectTrigger.ON_YOUR_DEATH, Target)
                    );
                    Target.ActiveCreature.killed = true;

                    Animations.Add(new SceneAnimation(5, new double[] {
                Target.playerNumber }, "#DEATH ANIMATION#"));

                    Animations.Add(new SceneAnimation(0, null, Target.ActiveCreature.Nickname + " collapses!"));

                    Animations.AddRange(
                        RunEffectType(EffectTrigger.ON_OPPONENT_DEATH, GetOpponent(Target))
                    );
                } else {
                    if (TriggersAttackFlags) {
                        Animations.AddRange(
                            RunEffectType(EffectTrigger.AFTER_ATTACKED, Target)
                        );
                    }
                }
            } else {
                //TODO: OPPONENT CANNOT BE ATTACKED - THE ATTACK FAILED
            }
            return Animations;
        }


        public List<SceneAnimation> RunEffectType(EffectTrigger Trigger, Player TargetUser) {
            List<SceneAnimation> Animations = new List<SceneAnimation>();
            List<BattleEffect> DeadEffects = new List<BattleEffect>();

            if (TargetUser == null || !TargetUser.IsKilled()) {
                foreach (List<BattleEffect> Effects in SortedEffects[Trigger].Values) {
                    foreach (BattleEffect Effect in Effects) {
                        if (Effect.User == TargetUser || TargetUser == null) {
                            if (Effect.EffectAnimation != null) {
                                Animations.Add(Effect.EffectAnimation);
                            }
                            Animations.AddRange(InterpretEffect(Effect.GetTriggeredEffect(Trigger), Effect.User, null, 0));
                            if (Effect.IsLifespanDepleted()) DeadEffects.Add(Effect);
                        }
                    }
                }
                foreach (BattleEffect Effect in DeadEffects) {
                    Animations.AddRange(RemoveEffect(Effect, true));
                }

            }
            return Animations;
        }

        public List<SceneAnimation> InterpretEffect(EffectScript EffectScripts, Player User, Player Trigger, byte TriggerPosition) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();
            Interpreter.SetTargets(User, GetOpponent(User), Trigger, TriggerPosition);

            Animations.AddRange(Interpreter.ExecuteEffect(EffectScripts));

            return Animations; //TODO: GET ANIMATIONS FROM SCRIPT
        }


        public List<SceneAnimation> SetArenaRotation(byte set) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();
            int RotationDelta = set - ArenaOrientation;
            if (RotationDelta > 4) RotationDelta -= 8;
            if (RotationDelta < -4) RotationDelta += 8;
            ArenaOrientation = set;
            OldArenaOrientation = set;

            Animations.AddRange(
                RunTriggerEffectType(EffectTrigger.ON_STAND_EXIT, null)
            );

            Player1.Placement = (byte)((ArenaOrientation) % 8);
            Player2.Placement = (byte)((ArenaOrientation + 4) % 8);

            Animations.Add(new SceneAnimation(3, new double[] { RotationDelta }, "#MOVEMENT#"));

            Animations.AddRange(
                RunTriggerEffectType(EffectTrigger.ON_STAND_ENTER, null)
            );

            return Animations;
        }

        public short RotationFromMovement(MovementCategory Movement) {
            short RotationDelta = 0;

            if (Movement == MovementCategory.HOLD_GROUND) {
                if (ArenaOrientation != OldArenaOrientation) {
                    ArenaOrientation = OldArenaOrientation;
                    if (IsPreviousMovementLeft) {
                        IsPreviousMovementLeft = false;
                        //1
                        RotationDelta = 1;
                    } else {
                        IsPreviousMovementLeft = true;
                        //-1
                        RotationDelta = -1;
                    }
                } //else 0
            } else if (Movement == MovementCategory.MOVE_LEFT) {
                IsPreviousMovementLeft = true;
                OldArenaOrientation = ArenaOrientation;
                if (ArenaOrientation == 0) {
                    ArenaOrientation = 7;
                } else {
                    ArenaOrientation--;
                }
                //-1
                RotationDelta = -1;
            } else if (Movement == MovementCategory.MOVE_RIGHT) {
                IsPreviousMovementLeft = false;
                if (ArenaOrientation == 7) {
                    ArenaOrientation = 0;
                } else {
                    ArenaOrientation++;
                }
                //1
                RotationDelta = 1;
            }//else 0
            return RotationDelta;
        }

        public List<SceneAnimation> DeltaRotate(short RotationDelta) {

            List<SceneAnimation> Animations = new List<SceneAnimation>();

            Animations.AddRange(
                RunTriggerEffectType(EffectTrigger.ON_STAND_EXIT, null)
            );

            Player1.Placement = (byte)((Player1.Placement + RotationDelta) % 8);
            Player2.Placement = (byte)((Player1.Placement + RotationDelta) % 8);

            Animations.Add(new SceneAnimation(3, new double[] { RotationDelta }, "#MOVEMENT ANIMATION#"));

            Animations.AddRange(
                RunTriggerEffectType(EffectTrigger.ON_STAND_ENTER, null)
            );

            return Animations;
        }

        public List<SceneAnimation> RunSameTilePlacedEffect(byte Position) {
            List<SceneAnimation> Animations = new List<SceneAnimation>();
            List<BattleEffect> DeadEffects = new List<BattleEffect>();

            foreach (List<BattleEffect> Effects in SortedEffects[EffectTrigger.ON_NEW_TILE_HERE].Values) {
                foreach (BattleEffect Effect in Effects) {

                    if (Effect.CurrentPlacement == Position) {
                        if (Effect.EffectAnimation != null) {
                            Animations.Add(Effect.EffectAnimation);
                        }
                        Animations.AddRange(InterpretEffect(Effect.GetTriggeredEffect(EffectTrigger.ON_NEW_TILE_HERE), Effect.User, null, Position));
                    }
                }
            }
            foreach (BattleEffect Effect in DeadEffects) {
                Animations.AddRange(RemoveEffect(Effect, true));
            }
            return Animations;
        }


        public List<SceneAnimation> RunTriggerEffectType(EffectTrigger Trigger, Player Target) {
            List<SceneAnimation> Animations = new List<SceneAnimation>();
            List<BattleEffect> DeadEffects = new List<BattleEffect>();

            if (Target == null) {
                foreach (List<BattleEffect> Effects in SortedEffects[Trigger].Values) {
                    foreach (BattleEffect Effect in Effects) {
                        if (Effect.CurrentPlacement == Player1.Placement) {
                            if (Effect.EffectAnimation != null) {
                                Animations.Add(Effect.EffectAnimation);
                            }
                            Animations.AddRange(InterpretEffect(Effect.GetTriggeredEffect(Trigger), Effect.User, Player1, Effect.CurrentPlacement));

                        } else if (Effect.CurrentPlacement == Player2.Placement) {
                            if (Effect.EffectAnimation != null) {
                                Animations.Add(Effect.EffectAnimation);
                            }
                            Animations.AddRange(InterpretEffect(Effect.GetTriggeredEffect(Trigger), Effect.User, Player2, Effect.CurrentPlacement));
                        }
                    }
                }
                foreach (BattleEffect Effect in DeadEffects) {
                    Animations.AddRange(RemoveEffect(Effect, true));
                }
            } else if (!Target.IsKilled()) {
                foreach (List<BattleEffect> Effects in SortedEffects[Trigger].Values) {
                    foreach (BattleEffect Effect in Effects) {
                        if (Effect.CurrentPlacement == Target.Placement) {
                            if (Effect.EffectAnimation != null) {
                                Animations.Add(Effect.EffectAnimation);
                            }
                            Animations.AddRange(InterpretEffect(Effect.GetTriggeredEffect(Trigger), Effect.User, Target, Effect.CurrentPlacement));
                        }
                    }
                }
                foreach (BattleEffect Effect in DeadEffects) {
                    Animations.AddRange(RemoveEffect(Effect, true));
                }
            }
            return Animations;
        }
    }
}