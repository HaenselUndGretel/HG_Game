using HanselAndGretel.Data;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using KryptonEngine.Rendering;
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

		protected string ActionInfoHansel;
		protected string ActionInfoGretel;
		protected Sprite ActionInfoButton;

		#endregion

		#region Constructor

		public ActivityHandler()
		{
			ActionInfoFading = new HudFading();
			ActionInfoButton = new Sprite(Vector2.Zero, "ButtonX");
		}

		#endregion

		#region Methods

		public void LoadContent()
		{
			ActionInfoButton.LoadTextures();
		}

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			//-----Ist Player verfügbar für eine Activity?-----
			bool TestHansel = false;
			bool TestGretel = false;
			if (pHansel.mCurrentActivity == None)
				TestHansel = true;
			if (pGretel.mCurrentActivity == None)
				TestGretel = true;

			//-----Activity ggf starten-----
			if (TestHansel || TestGretel)
			{
				//Betretene InteractiveObjects bestimmen
				InteractiveObject IObjIntersectsHansel = null;
				InteractiveObject IObjIntersectsGretel = null;
				Activity ActivityHansel = Activity.None;
				Activity ActivityGretel = Activity.None;
				foreach (InteractiveObject iObj in pScene.InteractiveObjects)
				{
					foreach (Rectangle rect in iObj.ActionRectList)
					{
						if (TestHansel && rect.Intersects(pHansel.CollisionBox))
						{
							IObjIntersectsHansel = iObj;
							ActivityHansel = iObj.ActivityState.GetPossibleActivity(pHansel, pGretel);
						}
						if (TestGretel && rect.Intersects(pGretel.CollisionBox))
						{
							IObjIntersectsGretel = iObj;
							ActivityGretel = iObj.ActivityState.GetPossibleActivity(pGretel, pHansel);
						}
					}
				}

				//Activity aufgrund von Spielereingabe starten?
				if (TestHansel &&
					IObjIntersectsHansel != null &&
					Conditions.ActionPressed(pHansel) &&
					ActivityHansel != Activity.None)
				{
					pHansel.mCurrentActivity = IObjIntersectsHansel.ActivityState;
				}
				if (TestGretel &&
					IObjIntersectsGretel != null &&
					Conditions.ActionPressed(pGretel) &&
					ActivityGretel != Activity.None)
				{
					pHansel.mCurrentActivity = IObjIntersectsGretel.ActivityState;
				}

				//-----Update ActionInfoState-----
				ActionInfoFading.ShowHudHansel = false;
				ActionInfoFading.ShowHudGretel = false;
				if (ActivityHansel != Activity.None)
				{
					ActionInfoFading.ShowHudHansel = true;
					UpdateActionInfo(true, ActivityHansel);
				}
				if (ActivityGretel != Activity.None)
				{
					ActionInfoFading.ShowHudGretel = true;
					UpdateActionInfo(false, ActivityGretel);
				}
			}

			//-----Update Activities-----
			pHansel.mCurrentActivity.Update(pHansel, pGretel);
			pGretel.mCurrentActivity.Update(pGretel, pHansel);


			ActionInfoFading.Update();
		}

		/// <summary>
		/// Updated die ActionInfo zur möglichen Activity
		/// </summary>
		/// <param name="pHansel">true = Hansel, false = Gretel</param>
		/// <param name="pActivity">Activity zu der die ActionInfo gesetzt werden soll</param>
		protected void UpdateActionInfo(bool pHansel, Activity pActivity)
		{
			if (pHansel)
			{
				ActionInfoHansel = ActivityState.ActivityInfo[pActivity];
				return;
			}
			ActionInfoGretel = ActivityState.ActivityInfo[pActivity];
		}

		public void DrawActionInfo(TwoDRenderer pRenderer, Hansel pHansel, Gretel pGretel)
		{
			pRenderer.Draw(ActionInfoButton, pHansel.Position);//Alpha = ActionInfoFading.VisibilityHansel
			//Draw ActionInfoText Hansel
			pRenderer.Draw(ActionInfoButton, pGretel.Position);//Alpha = ActionInfoFading.VisibilityGretel
			//Draw ActionInfoText Gretel
		}

		#endregion
	}
}
