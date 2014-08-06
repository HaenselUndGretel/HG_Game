using HanselAndGretel.Data;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class ActivityHandler
	{
		#region Properties

		public static None None = new None();
		
		protected HudFading ActionInfoFading;

		protected int ActionInfoHansel;
		protected int ActionInfoGretel;
		protected Texture2D ActionInfoButton;

		protected bool AmuletCharged = false;
		protected List<ChargeAmulet> AmuletStates;
		protected UseAmulet Amulet;

		protected PushDoor Door;

		public static bool AmuletBlocksWaypoints;
		public static bool DoorBlocksWaypoint;

		#endregion

		#region Constructor

		public ActivityHandler()
		{
			ActionInfoFading = new HudFading();
			AmuletStates = new List<ChargeAmulet>(4);
		}

		#endregion

		#region Methods

		public void LoadContent(Hansel pHansel, Gretel pGretel)
		{
			ActionInfoButton = TextureManager.Instance.GetElementByString("button_x");
			Amulet = new UseAmulet(pHansel, pGretel, null);
		}

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel, Savegame pSavegame)
		{
			//-----Ist Player verfügbar für eine Activity?-----
			bool TestHansel = false;
			bool TestGretel = false;
			if (pHansel.mCurrentActivity == None)
				TestHansel = true;
			if (pGretel.mCurrentActivity == None)
				TestGretel = true;

			//-----Wenn nicht getestet wird keine ActionInfo anzeigen-----
			ActionInfoFading.ShowHudHansel = false;
			ActionInfoFading.ShowHudGretel = false;
			//-----Activity ggf starten-----
			if (TestHansel || TestGretel)
			{
				//Betretene InteractiveObjects bestimmen
				InteractiveObject IObjIntersectsHansel = null;
				InteractiveObject IObjIntersectsGretel = null;
				Activity PossibleActivityHansel = Activity.None;
				Activity PossibleActivityGretel = Activity.None;
				foreach (InteractiveObject iObj in pScene.InteractiveObjects)
				{
					foreach (Rectangle rect in iObj.ActionRectList)
					{
						if (TestHansel && rect.Contains(new Point((int)pHansel.SkeletonPosition.X, (int)pHansel.SkeletonPosition.Y)))
						{
							IObjIntersectsHansel = iObj;
							PossibleActivityHansel = iObj.ActivityState.GetPossibleActivity(pHansel, pGretel);
						}
						if (TestGretel && rect.Contains(new Point((int)pGretel.SkeletonPosition.X, (int)pGretel.SkeletonPosition.Y)))
						{
							IObjIntersectsGretel = iObj;
							PossibleActivityGretel = iObj.ActivityState.GetPossibleActivity(pGretel, pHansel);
						}
					}
				}

				//-----Tür erst offen wenn Laterne eingesammelt wurde-----
				if (pSavegame.SceneId == Hardcoded.Scene_LanternDoor)
				{ 
					foreach (Collectable col in pSavegame.Scenes[Hardcoded.Scene_LanternDoor].Collectables)
					{
						if (col.GetType() == typeof(Lantern))
						{
							if (PossibleActivityHansel == Activity.PushDoor)
								PossibleActivityHansel = Activity.None;
							if (PossibleActivityGretel == Activity.PushDoor)
								PossibleActivityGretel = Activity.None;
						}
					}
				}

				//-----UseAmulet-----
				if (pSavegame.SceneId == Hardcoded.Scene_Amulet_Boss && AmuletCharged)
				{
					if (TestHansel && PossibleActivityHansel == Activity.None)
					{
						PossibleActivityHansel = Amulet.GetPossibleActivity(pHansel, pGretel);
						if (PossibleActivityHansel != Activity.None && Conditions.ActionPressed(pHansel))
						{
							pHansel.mCurrentActivity = Amulet;
							PossibleActivityHansel = Activity.None;
						}
					}
					if (TestGretel && PossibleActivityGretel == Activity.None)
					{
						PossibleActivityGretel = Amulet.GetPossibleActivity(pGretel, pHansel);
						if (PossibleActivityGretel != Activity.None && Conditions.ActionPressed(pGretel))
						{
							pGretel.mCurrentActivity = Amulet;
							PossibleActivityGretel = Activity.None;
						}
					}
				}

				//-----Activity aufgrund von Spielereingabe starten?-----
				if (TestHansel &&
					IObjIntersectsHansel != null &&
					Conditions.ActionPressed(pHansel) &&
					PossibleActivityHansel != Activity.None)
				{
					pHansel.mCurrentActivity = IObjIntersectsHansel.ActivityState;
				}
				if (TestGretel &&
					IObjIntersectsGretel != null &&
					Conditions.ActionPressed(pGretel) &&
					PossibleActivityGretel != Activity.None)
				{
					pGretel.mCurrentActivity = IObjIntersectsGretel.ActivityState;
				}

				//-----Update ActionInfoState-----
				if (PossibleActivityHansel != Activity.None)
				{
					ActionInfoFading.ShowHudHansel = true;
					ActionInfoHansel = (int)PossibleActivityHansel;
				}
				if (PossibleActivityGretel != Activity.None)
				{
					ActionInfoFading.ShowHudGretel = true;
					ActionInfoGretel = (int)PossibleActivityGretel;
				}
			}

			//-----Update Activities-----
			pHansel.mCurrentActivity.Update(pHansel, pGretel);
			pGretel.mCurrentActivity.Update(pGretel, pHansel);

			//Brunnen Overlay updaten
			if (pHansel.mCurrentActivity.GetType() == typeof(UseWell) && pHansel.mCurrentState == 4)
				((UseWell)pHansel.mCurrentActivity).UpdateOverlay(ref pScene.RenderList);

			//AmuletStates updaten
			UpdateAmulet(pSavegame, pScene);
			UpdateDoor(pSavegame);
			

			ActionInfoFading.Update();
		}

		protected void UpdateAmulet(Savegame pSavegame, SceneData pScene)
		{
			if (pSavegame.SceneId != Hardcoded.Scene_Amulet_Boss || Amulet.m2ndState)
			{
				AmuletBlocksWaypoints = false;
				return;
			}
			AmuletBlocksWaypoints = true;
			AmuletCharged = true;
			foreach(ChargeAmulet a in AmuletStates)
			{
				if (a.m2ndState == false)
				{
					AmuletCharged = false;
					return;
				}
			}
			/*
			foreach (ChargeAmulet a in AmuletStates)
				a.m2ndState = false;
			Witch w = null;
			foreach (Enemy e in pScene.Enemies)
				if (e.GetType() == typeof(Witch))
				{
					w = (Witch)e;
				}
			if (w == null) throw new Exception("Witch nicht gefunden!");
			pScene.Enemies.Remove(w);
			*/
		}

		protected void UpdateDoor(Savegame pSavegame)
		{
			if (pSavegame.SceneId == Hardcoded.Scene_LanternDoor && !Door.m2ndState)
			{
				DoorBlocksWaypoint = true;
				return;
			}
			DoorBlocksWaypoint = false;
		}

		#region Draw

		public void DrawActionInfo(SpriteBatch pSpriteBatch, Hansel pHansel, Gretel pGretel)
		{
			//ButtonX
			pSpriteBatch.Draw(ActionInfoButton, pHansel.SkeletonPosition + Hardcoded.ActionInfo_OffsetButton, Color.White * ActionInfoFading.VisibilityHansel);
			pSpriteBatch.Draw(ActionInfoButton, pGretel.SkeletonPosition + Hardcoded.ActionInfo_OffsetButton, Color.White * ActionInfoFading.VisibilityGretel);
		}

		public void DrawActivityInstruction(SpriteBatch pSpriteBatch, Hansel pHansel, Gretel pGretel)
		{
			if (pHansel.mCurrentActivity != null)
			{
				pHansel.mCurrentActivity.Draw(pSpriteBatch, pHansel, pGretel);
			}
			if (pGretel.mCurrentActivity != null)
			{
				if (pHansel.mCurrentActivity != null && pGretel.mCurrentActivity == pHansel.mCurrentActivity)
					return; //Nicht doppelt zeichnen
				pGretel.mCurrentActivity.Draw(pSpriteBatch, pGretel, pHansel);
			}
		}

		#endregion

		#region Setup InteractiveObjects.ActivityState

		//-----Workaround für linerare Abhängigkeit HG_Game -> HG_Data -> KryptonEngine-----
		public void SetupInteractiveObjectsFromDeserialization(Savegame pSavegame, Hansel pHansel, Gretel pGretel)
		{
			for (int i = 0; i < pSavegame.Scenes.Length; i++)
			{
				foreach (InteractiveObject iObj in pSavegame.Scenes[i].InteractiveObjects)
				{
					switch (iObj.ActivityId)
					{
						case Activity.None:
							iObj.ActivityState = None;
							break;
						case Activity.KnockOverTree:
							iObj.ActivityState = new KnockOverTree(pHansel, pGretel, iObj);
							break;
						case Activity.BalanceOverTree:
							iObj.ActivityState = new KnockOverTree(pHansel, pGretel, iObj);
							Sequences.AnimateAccordingToDirection(iObj, iObj.ActionPosition2 - iObj.ActionPosition1, Hardcoded.Anim_Tree_Fallen_Up, Hardcoded.Anim_Tree_Fallen_Down, Hardcoded.Anim_Tree_Fallen_Side);
							iObj.ActivityState.m2ndState = true;
							break;
						case Activity.PushRock:
							iObj.ActivityState = new PushRock(pHansel, pGretel, iObj);
							break;
						case Activity.SlipThroughRock:
							iObj.ActivityState = new SlipThroughRock(pHansel, pGretel, iObj);
							break;
						case Activity.JumpOverGap:
							iObj.ActivityState = new JumpOverGap(pHansel, pGretel, iObj);
							break;
						case Activity.LegUp:
							iObj.ActivityState = new LegUp(pHansel, pGretel, iObj);
							break;
						case Activity.LegUpGrab:
							iObj.ActivityState = new LegUp(pHansel, pGretel, iObj);
							iObj.ActivityState.m2ndState = true;
							break;
						case Activity.PushDoor:
							iObj.ActivityState = new PushDoor(pHansel, pGretel, iObj);
							Door = (PushDoor)iObj.ActivityState;
							break;
						case Activity.PullDoor:
							throw new Exception("Es gibt keine Tür mehr die von den Spielern geschlossen werden soll");
							/*iObj.ActivityState = new PushDoor(pHansel, pGretel, iObj);
							Sequences.AnimateAccordingToDirection(iObj, new Vector2(iObj.CollisionRectList[0].X - iObj.ActionRectList[0].X, iObj.CollisionRectList[0].Y - iObj.ActionRectList[0].Y), Hardcoded.Anim_Door_Open_Up, Hardcoded.Anim_Door_Open_Down, Hardcoded.Anim_Door_Open_Side);
							iObj.ActivityState.m2ndState = true;*/
							break;
						case Activity.UseWell:
							iObj.ActivityState = new UseWell(pHansel, pGretel, iObj);
							break;
						case Activity.ChargeAmulet:
							iObj.ActivityState = new ChargeAmulet(pHansel, pGretel, iObj);
							AmuletStates.Add((ChargeAmulet)iObj.ActivityState);
							break;
						default:
							throw new Exception("Im InteractiveObject " + iObj.ObjectId.ToString() + " in Scene " + i.ToString() + " ist eine ungültige Action angegeben!");
					}
				}
			}
		}

		#endregion


		#endregion
	}
}
