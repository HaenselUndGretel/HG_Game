using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.FModAudio;
using KryptonEngine.HG_Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class ItemHandler
	{
		#region Properties

		public PointLight LanternLight;
		SteppingProgress LanternRaiseProgress;

		#endregion

		#region Constructor

		public ItemHandler()
		{
			LanternLight = new PointLight();
			LanternLight.Intensity = Hardcoded.Lantern_Intensity;
			LanternLight.LightColor = Hardcoded.Lantern_LightColor;
			LanternLight.Depth = Hardcoded.Lantern_Height;
			LanternLight.Radius = Hardcoded.Lantern_Radius;
			LanternRaiseProgress = new SteppingProgress(Hardcoded.Lantern_RaiseSteppingDuration);
		}

		#endregion

		#region Methods

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel, Savegame pSavegame, ref GameScene.GameState pGameState)
		{
			UpdateLantern(pHansel, pGretel);
			UpdateVisibility(pScene, pHansel, pGretel);
			CollectCollectables(pSavegame, pScene, pHansel, pGretel, ref pGameState);
		}

		protected void UpdateLantern(Hansel pHansel, Gretel pGretel)
		{
			//Lantern
			if ((pHansel.SkeletonPosition - pGretel.SkeletonPosition).Length() <=  Hardcoded.Lantern_MaxSwapDistance)
			{
				if (pHansel.Lantern && pHansel.mCurrentActivity == ActivityHandler.None && pHansel.Input.SwitchItemJustPressed)
				{
					pGretel.Lantern = true;
					pHansel.Lantern = false;
				}
				else if (pGretel.Lantern && pGretel.mCurrentActivity == ActivityHandler.None && pGretel.Input.SwitchItemJustPressed)
				{
					pHansel.Lantern = true;
					pGretel.Lantern = false;
				}
			}

			if (pHansel.Lantern)
			{
				LanternLight.Position = pHansel.SkeletonPosition;
				if (pHansel.Input.UseItemIsPressed)
					LanternRaiseProgress.StepForward();
				else
					LanternRaiseProgress.StepBackward();
				if (LanternRaiseProgress.Complete)
					pHansel.IsLanternRaised = true;
				else
					pHansel.IsLanternRaised = false;
			}
			else if (pGretel.Lantern)
			{
				LanternLight.Position = pGretel.SkeletonPosition;
				LanternRaiseProgress.Reset();
			}
			LanternLight.Depth = Hardcoded.Lantern_Height + ((Hardcoded.Lantern_HeightRaised - Hardcoded.Lantern_Height) * LanternRaiseProgress.Progress);
			LanternLight.Radius = Hardcoded.Lantern_Radius + ((Hardcoded.Lantern_RadiusRaised - Hardcoded.Lantern_Radius) * LanternRaiseProgress.Progress);
		}

		protected void UpdateVisibility(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			//Update Collectables
			foreach (Collectable col in pScene.Collectables)
			{
				col.IsVisible = false;
				if (!col.IsHidden || InLanternLight(col, pHansel, pGretel)) //Collectable befindet sich im Lichtkreis
				{
					col.IsVisible = true;
				}
			}
		}

		protected bool InLanternLight(Collectable pCollectable, Hansel pHansel, Gretel pGretel)
		{
			if (!pGretel.Lantern)
				return false;
			return ((pGretel.SkeletonPosition - pCollectable.SkeletonPosition).Length() < GameReferenzes.LIGHT_RADIUS) ? true : false;
		}

		protected void CollectCollectables(Savegame pSavegame, SceneData pScene, Hansel pHansel, Gretel pGretel, ref GameScene.GameState pGameState)
		{
			bool isCollected = false;
			Collectable delCol = null;

			foreach (Collectable col in pScene.Collectables)
			{
				if (col.IsVisible)
				{
					if (col.CollisionBox.Intersects(pHansel.CollisionBox))
					{
						isCollected = true;
						delCol = col;
						//Laterne einsammeln
						if (col.GetType() == typeof(Lantern))
							pHansel.Lantern = true;
						else if(col.GetType() != typeof(Amulet))
						{
							pGameState = GameScene.GameState.CollectableInfo;
							FmodMediaPlayer.Instance.AddSong("collectable" + col.CollectableId, 0.8f);
						}
					}
					else if (col.CollisionBox.Intersects(pGretel.CollisionBox) ||
						( //Collectable bei LegUpGrab einsammeln
						pGretel.mCurrentActivity != null &&
						pGretel.mCurrentActivity.GetType() == typeof(LegUp) &&
						pGretel.mCurrentActivity.m2ndState &&
						pGretel.mCurrentState == 4 &&
						col.CollisionBox.Intersects(new Rectangle((int)(pGretel.SkeletonPosition.X + Hardcoded.LegUp_OffsetGretel[pSavegame.SceneId].X - 10), (int)(pGretel.SkeletonPosition.Y + Hardcoded.LegUp_OffsetGretel[pSavegame.SceneId].Y - 10), 20, 20))
						)
						)
					{
						delCol = col;
						isCollected = true;
						//Laterne einsammeln
						if (col.GetType() == typeof(Lantern))
							pGretel.Lantern = true;
						else if (col.GetType() != typeof(Amulet))
						{
							pGameState = GameScene.GameState.CollectableInfo;
							FmodMediaPlayer.Instance.AddSong("collectable" + col.CollectableId, 0.8f);
						}
					}
				}
			}
			if (isCollected)
			{
				pSavegame.Collectables.Add(delCol);
				pScene.Collectables.Remove(delCol);
				pScene.RenderList.Remove(delCol);
			}
		}

		#endregion
	}
}
