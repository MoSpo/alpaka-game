using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaka.Scenes.Battle {
	class Player {
		public CreatureInstance[] Team = new CreatureInstance[6];
		public CreatureInstance ActiveCreature;
		public bool Ready;

		public byte SelectedActionNumber = 0;
		public CreatureAction SelectedAction = null;
		private CreatureAction OldAction = null;

		public MovementCategory SelectedMovement = MovementCategory.NULL;
		public byte Placement;

		public byte SelectedCreature;
		public bool JustSwitchedIn;

		public float actionCombo = 1;

		public byte amountOfAttacks = 1;

		public byte playerNumber;
		public byte playerServerNumber;

		private Dictionary<CreatureStats, byte> Statboosts = new Dictionary<CreatureStats, byte>();

		public byte[] ElementBoosts = new byte[Enum.GetNames(typeof(CreatureElement)).Length];

		public byte[] SwitchedStats = new byte[8];
		public bool HasNewElement = false;
		private CreatureElement newElement;

		private List<CreatureElement> actionElementMaskFrom;
		private List<CreatureElement> actionElementMaskTo;

		public bool SentData = false;

		public bool tempNotKilled = false; //used for switching only

		//FLAGS
		public Flag CanUseActionThisTurn = new Flag(true);
		public Flag TriggersAttackFlags = new Flag(true);
		public Flag CanBeMovedBackwards = new Flag(true);
		public Flag CanBeMovedByEffects = new Flag(true);
		public Flag MovingTwiceAmount = new Flag(false);
		public Flag HasActionDelay = new Flag(false);
		public Flag CanBeAttacked = new Flag(true);
		public Flag LosesStatsOnSwitch = new Flag(true);
		public Flag ConditionsPassedOnSwitch = new Flag(false);
		public Flag NotEffectedByEarth = new Flag(false);
		public Flag SkillDisabled = new Flag(false);
		public Flag AbilityDisabled = new Flag(false);
		public Flag CanSwitch = new Flag(true);
		public Flag CanMove = new Flag(true);
		public Flag CanMoveLeft = new Flag(true);
		public Flag CanMoveRight = new Flag(true);
		public Flag CanChooseAction = new Flag(true);
		public Flag IsJumping = new Flag(false);
		public Flag RestoreHealthInseadOfMana = new Flag(false);
		//

		public byte[][] ElementEffectiveness = new byte[][] {
			new byte[] {2,1,2,2,2,4,1,4,4,2,1,1,2,4,2,2,2},	//FIRE
			new byte[] {4,2,1,2,2,1,4,1,2,4,2,2,2,2,1,2,2}, //WATER
			new byte[] {2,4,2,1,4,2,4,2,2,2,1,2,2,2,2,2,2}, //EARTH
			new byte[] {4,2,4,2,2,2,2,2,1,2,0,2,2,2,4,2,2}, //WIND
			new byte[] {2,4,0,2,2,2,1,2,4,1,2,2,2,4,1,2,2}, //ELECTRIC
			new byte[] {1,4,4,2,2,4,2,2,1,2,2,2,2,2,2,2,2}, //WOOD
			new byte[] {4,2,1,2,2,2,1,4,1,2,4,2,2,2,1,2,2}, //ROCK
			new byte[] {1,4,2,2,2,2,4,1,1,2,4,4,2,2,1,2,2}, //ICE
			new byte[] {1,1,2,2,1,2,2,2,4,4,2,2,2,2,2,2,2}, //STEEL
			new byte[] {2,2,4,4,2,4,4,4,4,2,1,2,1,2,1,2,2}, //NUCLEAR
			new byte[] {4,2,2,4,2,2,2,1,2,4,2,4,1,2,4,2,2}, //COSMIC
			new byte[] {2,2,2,2,2,1,2,2,2,4,2,2,4,1,2,2,2}, //VOID
			new byte[] {2,2,2,2,2,1,2,2,1,4,2,4,2,4,2,2,2}, //ETHER
			new byte[] {4,4,2,2,1,2,2,1,1,2,2,4,0,1,2,2,2}, //MAGIC
			new byte[] {2,4,2,2,1,1,1,1,4,2,2,2,4,4,2,2,2}, //SOUND
			new byte[] {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,4}, //DARK
			new byte[] {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,4,1}  //LIGHT
		};

		/*public byte[][] ElementEffectiveness = new byte[][] {
            new byte[] {2,1,2,2,2,3,1,3,3,2,1,1,2,3,2,2,2},	//FIRE
			new byte[] {3,2,1,2,2,1,3,1,2,3,2,2,2,2,1,2,2}, //WATER
			new byte[] {2,3,2,1,3,2,3,2,2,2,1,2,2,2,2,2,2}, //EARTH
			new byte[] {3,2,3,2,2,2,2,2,1,2,0,2,2,2,3,2,2}, //WIND
			new byte[] {2,3,0,2,2,2,1,2,3,1,2,2,2,3,1,2,2}, //ELECTRIC
			new byte[] {1,3,3,2,2,3,2,2,1,2,2,2,2,2,2,2,2}, //WOOD
			new byte[] {3,2,1,2,2,2,1,3,1,2,3,2,2,2,1,2,2}, //ROCK
			new byte[] {1,3,2,2,2,2,3,1,1,2,3,3,2,2,1,2,2}, //ICE
			new byte[] {1,1,2,2,1,2,2,2,3,3,2,2,2,2,2,2,2}, //STEEL
			new byte[] {2,2,3,3,2,3,3,3,3,2,1,2,1,2,1,2,2}, //NUCLEAR
			new byte[] {3,2,2,3,2,2,2,1,2,3,2,3,1,2,3,2,2}, //COSMIC
			new byte[] {2,2,2,2,2,1,2,2,2,3,2,2,3,1,2,2,2}, //VOID
			new byte[] {2,2,2,2,2,1,2,2,1,3,2,3,2,3,2,2,2}, //ETHER
			new byte[] {3,3,2,2,1,2,2,1,1,2,2,3,0,1,2,2,2}, //MAGIC
			new byte[] {2,3,2,2,1,1,1,1,3,2,2,2,3,3,2,2,2}, //SOUND
			new byte[] {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,3}, //DARK
			new byte[] {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,3,1}  //LIGHT
		};*/

		public Player(byte Number, byte Placement) {

			this.Placement = Placement;
			playerNumber = Number;
			Ready = false;

			foreach (CreatureStats Stat in Enum.GetValues(typeof(CreatureStats))) {
				Statboosts[Stat] = 5;
			}
		}


		/// //////////////////

		public void setStart(byte s) {  //STAND IN CODE
			AllCreatures temp = new AllCreatures();
			if (playerNumber == 1) {
				Team[0] = new CreatureInstance(temp.GetCreature(1));
				Team[1] = new CreatureInstance(temp.GetCreature(0));
				Team[2] = new CreatureInstance(temp.GetCreature(2));
				Team[3] = new CreatureInstance(temp.GetCreature(3));
				Team[4] = new CreatureInstance(temp.GetCreature(5));
				Team[5] = new CreatureInstance(temp.GetCreature(7));

			} else {
				Team[0] = new CreatureInstance(temp.GetCreature(8));
				Team[1] = new CreatureInstance(temp.GetCreature(6));
				Team[2] = new CreatureInstance(temp.GetCreature(4));
				Team[3] = new CreatureInstance(temp.GetCreature(10));
				Team[4] = new CreatureInstance(temp.GetCreature(9));
				Team[5] = new CreatureInstance(temp.GetCreature(11));

			}
			ActiveCreature = Team[0];
		}

		/// //////////////////


		public List<SceneAnimation> GiveDamage(CreatureElement Element, ActionCategory Category, byte BaseAmount, Player Attacked, Player Attacker) {

			List<SceneAnimation> Animations = new List<SceneAnimation>();

			int Damage = 0;
			double ElementBonus = 1.0;
			for (int i = 0; i < 16; i++) if (Attacked.HasElement((CreatureElement)(i + 1))) ElementBonus *= (double)ElementEffectiveness[((int)Element) - 1][i] / 2;
			double SameElementBonus = 1.0;
			if (Attacker.HasElement(Element)) SameElementBonus *= 1.5;
			double Bonuses = ElementBonus * SameElementBonus;

			if (Bonuses > 0 && !(Element == CreatureElement.EARTH && NotEffectedByEarth.Evaluate())) {
				if (Category == ActionCategory.PHYSICAL) {
					Damage = (int)Math.Floor((Attacker.ActiveCreature.GetTotalStat(CreatureStats.STRENGTH) * BaseAmount * Bonuses /
										 Attacked.ActiveCreature.GetTotalStat(CreatureStats.ENDURANCE) + 2.0));

				} else if (Category == ActionCategory.MYSTICAL) {
					Damage = (int)Math.Floor((Attacker.ActiveCreature.GetTotalStat(CreatureStats.INTELLIGENCE) * BaseAmount * Bonuses /
												Attacked.ActiveCreature.GetTotalStat(CreatureStats.WISDOM) + 2.0));
				}

				int nh = ActiveCreature.Health - Damage;
				if (nh < 0) nh = 0;

				Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.HEALTH_BAR, new double[] {
				playerNumber,
				ActiveCreature.GetTotalStat(CreatureStats.HEALTH),
				nh,
				-1*Damage,
				ElementBonus,
				SameElementBonus}, "#DAMAGE GIVEN#"));


				ActiveCreature.Health -= Damage;

				if (ActiveCreature.Health <= 0) {
					ActiveCreature.Health = 0;
					ActiveCreature.killed = true;
				}
			} else {
				Animations.Add(new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, ActiveCreature.Nickname + " was unaffected by the damage!"));
			}
			return Animations;
		}

		public void GainKin() {
			//TODO GAIN KIN
		}

		public int GetTotalStat(CreatureStats Stat) {
			int s = Statboosts[Stat] - 5;
			double boost = 1;
			if (s > 0) {
				boost = (s + 2) / 2;
			} else {
				boost = 2 / (2 - s);
			}
			return (int)(boost * ActiveCreature.GetTotalStat(Stat)); //TODO: THIS IS USED WAYYY TOO MUCH PLEASE FIX
		}

		public SceneAnimation SetNewElement(CreatureElement Element) {
			newElement = Element;
			return new SceneAnimation(SceneAnimation.SceneAnimationType.ELEMENT_CHANGE, new double[] { playerNumber, (double)Element, 0, 0 }, "#SET NEW ELEMENT#");
		}

		public bool HasElement(CreatureElement Element) {
			if (newElement == CreatureElement.NULL) return ActiveCreature.HasElement(Element);
			return newElement == Element;

		}

		public SceneAnimation GiveCondition(byte Condition) {
			ActiveCreature.GiveCondition(Condition);
			return new SceneAnimation(SceneAnimation.SceneAnimationType.CONDITION, new double[] { playerNumber, Condition }, "#CONDITION ANIMATION#");
		}

		public SceneAnimation GiveStatBoost(CreatureStats Stat, bool IsPositiveBoost) {

			if (Stat == CreatureStats.HEALTH || Stat == CreatureStats.KIN) {
				return new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "#ERROR#");

			}
			if (IsPositiveBoost) {
				if (Statboosts[Stat] < 10) {
					Statboosts[Stat]++;
					return new SceneAnimation(SceneAnimation.SceneAnimationType.STAT_BOOST, new double[] { playerNumber, (double)Stat, 1 }, "#STAT BOOST ANIMATION#");

				}
				return new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "");

			}
			if (Statboosts[Stat] > 0) {
				Statboosts[Stat]--;
				return new SceneAnimation(SceneAnimation.SceneAnimationType.STAT_BOOST, new double[] { playerNumber, (double)Stat, -1 }, "#STAT BOOST ANIMATION#");

			}

			return new SceneAnimation(SceneAnimation.SceneAnimationType.ADD_MESSAGE, null, "");
		}

		public bool SelectAction(byte ActionNumber) {
			if (ActionNumber < 6) {
				if (ActiveCreature.GetActionUsage(ActionNumber) == 0) {
					return false;
				}
				SelectedActionNumber = ActionNumber;
				SelectedAction = ActiveCreature.GetAction(ActionNumber);
			} else {
				//TODO: BASIC ACTIONS HERE
			}
			if (OldAction == null) {
				actionCombo = 1;
			} else {
				if (OldAction != SelectedAction) {
					actionCombo = 1.5f;
				} else {
					actionCombo = 1;
				}
			}
			OldAction = SelectedAction;
			if (SelectedMovement != MovementCategory.NULL) {
				Ready = true;
			}
			return true;
		}

		public void SelectMovement(MovementCategory Movement) {
			SelectedMovement = Movement;

			if (SelectedAction != null) {
				Ready = true;
			}
		}

		public void SelectMovement(byte MovementNumber) {
			SelectedMovement = (MovementCategory)MovementNumber;

			if (SelectedAction != null) {
				Ready = true;
			}
		}

		public bool SelectCreature(byte CreatureNumber) {
			if (Team[CreatureNumber].killed) return false;
			SelectedCreature = CreatureNumber;
			return true;
		}

		public void Reset() {
			CanUseActionThisTurn.RemoveFlag();
			TriggersAttackFlags.RemoveFlag();
			CanBeMovedBackwards.RemoveFlag();
			CanBeMovedByEffects.RemoveFlag();
			MovingTwiceAmount.RemoveFlag();
			HasActionDelay.RemoveFlag();
			CanBeAttacked.RemoveFlag();
			LosesStatsOnSwitch.RemoveFlag();
			ConditionsPassedOnSwitch.RemoveFlag();
			NotEffectedByEarth.RemoveFlag();
			SkillDisabled.RemoveFlag();
			AbilityDisabled.RemoveFlag();
			CanSwitch.RemoveFlag();
			CanMove.RemoveFlag();
			CanMoveLeft.RemoveFlag();
			CanMoveRight.RemoveFlag();
			CanChooseAction.RemoveFlag();
			IsJumping.RemoveFlag();
			RestoreHealthInseadOfMana.RemoveFlag();

			actionCombo = 1;

			amountOfAttacks = 1;

			Statboosts = new Dictionary<CreatureStats, byte>();

			ElementBoosts = new byte[Enum.GetNames(typeof(CreatureElement)).Length];

			SwitchedStats = new byte[8];
			HasNewElement = false;

			ActiveCreature.Kin = 0;

			actionElementMaskFrom = new List<CreatureElement>();
			actionElementMaskTo = new List<CreatureElement>();
		}

		public void DecreaseFlags() {
			CanUseActionThisTurn.DecreaseLifespan();
			TriggersAttackFlags.DecreaseLifespan();
			CanBeMovedBackwards.DecreaseLifespan();
			CanBeMovedByEffects.DecreaseLifespan();
			MovingTwiceAmount.DecreaseLifespan();
			HasActionDelay.DecreaseLifespan();
			CanBeAttacked.DecreaseLifespan();
			LosesStatsOnSwitch.DecreaseLifespan();
			ConditionsPassedOnSwitch.DecreaseLifespan();
			NotEffectedByEarth.DecreaseLifespan();
			SkillDisabled.DecreaseLifespan();
			AbilityDisabled.DecreaseLifespan();
			CanSwitch.DecreaseLifespan();
			CanMove.DecreaseLifespan();
			CanMoveLeft.DecreaseLifespan();
			CanMoveRight.DecreaseLifespan();
			CanChooseAction.DecreaseLifespan();
			IsJumping.DecreaseLifespan();
			RestoreHealthInseadOfMana.DecreaseLifespan();
		}

		public bool IsKilled() {

			return ActiveCreature.killed && !tempNotKilled;
		}

	}

	public class Flag {

		private bool IsInfinate;
		private bool NegativeFlag;
		private bool Bool;
		private byte Timer;

		public Flag(bool StartValue) {
			NegativeFlag = StartValue;
			Bool = false;
			Timer = 0;
		}

		public void SetFlag(byte Duration) {
			IsInfinate = (Duration == 255);
			Timer = Duration;
			Bool = true;
		}

		public void RemoveFlag() {
			Timer = 0;
			Bool = false;
		}

		public void DecreaseLifespan() {
			if (Bool && !IsInfinate) {
				Timer--;
				Bool = !(Timer <= 0);
			}
		}

		public bool Evaluate() {
			return (NegativeFlag ^ Bool);
		}
	}
}
