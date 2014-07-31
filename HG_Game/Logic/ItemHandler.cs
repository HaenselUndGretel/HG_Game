using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.FModAudio;
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

		protected const float MaxSwapDistance = 200f;
		protected Vector2 LanternOffset;
		public PointLight LanternLight;
		protected const float LanternHeight = 0.08f;
		protected const float LanternHeightRaised = 0.15f;
		protected const float LanternRadius = 8f;
		protected const float LanternRadiusRaised = 15f;
		SteppingProgress LanternRaiseProgress;

		#endregion

		#region Constructor

		public ItemHandler()
		{
			LanternOffset = new Vector2(0, -5);
			LanternLight = new PointLight();
			LanternLight.Intensity = 10f;
			LanternLight.LightColor = new Vector3(1f, 1f, 1f);
			LanternLight.Depth = LanternHeight;
			LanternLight.Radius = LanternRadius;
			LanternRaiseProgress = new SteppingProgress(0.3f);
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
			if ((pHansel.SkeletonPosition - pGretel.SkeletonPosition).Length() <= MaxSwapDistance)
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
				LanternLight.Position = pHansel.SkeletonPosition + LanternOffset;
				if (pHansel.Input.UseItemIsPressed)
					LanternRaiseProgress.StepForward();
				else
					LanternRaiseProgress.StepBackward();
			}
			else if (pGretel.Lantern)
			{
				LanternLight.Position = pGretel.SkeletonPosition + LanternOffset;
				LanternRaiseProgress.Reset();
			}
			LanternLight.Depth = LanternHeight + ((LanternHeightRaised - LanternHeight) * LanternRaiseProgress.Progress);
			LanternLight.Radius = LanternRadius + ((LanternRadiusRaised - LanternRadius) * LanternRaiseProgress.Progress);
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
			if (!pHansel.Lantern && !pGretel.Lantern)
				return false;
			Vector2 Position;
			float Radius = 100f;
			if (pHansel.Lantern)
			{
				Position = pHansel.SkeletonPosition + LanternOffset;
				if (pHansel.Input.UseItemIsPressed)
					Radius *= 1.8f;
			}
			else if (pGretel.Lantern)
			{
				Position = pGretel.SkeletonPosition + LanternOffset;
			}
			
			//Eigentlicher Test

			return true;
		}

		protected void CollectCollectables(Savegame pSavegame, SceneData pScene, Hansel pHansel, Gretel pGretel, ref GameScene.GameState pGameState)
		{
			foreach (Collectable col in pScene.Collectables)
			{
				if (col.IsVisible)
				{
					if (col.CollisionBox.Intersects(pHansel.CollisionBox))
					{
						pSavegame.Collectables.Add(col);
						pScene.Collectables.Remove(col);
						//Laterne einsammeln
						if (col.GetType() == typeof(Lantern))
							pHansel.Lantern = true;
						pGameState = GameScene.GameState.CollectableInfo;
						FmodMediaPlayer.Instance.AddSong("Collectable0" + col.CollectableId);
						return;
					}
					else if (col.CollisionBox.Intersects(pGretel.CollisionBox))
					{
						//Bei LegUpGrab Item erst im richtigen Moment einsammeln
						if (pGretel.mCurrentActivity != null && pGretel.mCurrentActivity.GetType() == typeof(LegUp) && pGretel.mCurrentActivity.m2ndState && pGretel.mCurrentState < 4)
							return;
						pSavegame.Collectables.Add(col);
						pScene.Collectables.Remove(col);
						//Laterne einsammeln
						if (col.GetType() == typeof(Lantern))
							pGretel.Lantern = true;
						pGameState = GameScene.GameState.CollectableInfo;
						FmodMediaPlayer.Instance.AddSong("Collectable0" + col.CollectableId);
						return;
					}
				}
			}
		}

		#endregion
	}
}
