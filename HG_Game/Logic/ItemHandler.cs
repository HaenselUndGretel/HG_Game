using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
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
		protected const float LanternHeight = 3f;
		protected const float LanternHeightRaised = 5f;

		#endregion

		#region Constructor

		public ItemHandler()
		{
			LanternOffset = new Vector2(0, -100);
			LanternLight = new PointLight();
			LanternLight.Depth = 0.5f;
			LanternLight.Intensity = 1f;
			LanternLight.LightColor = new Vector3(1f, 1f, 1f);
			LanternLight.Radius = 10f;
		}

		#endregion

		#region Methods

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel, Savegame pSavegame)
		{
			//Lantern
			if (
				((pHansel.mCurrentActivity == ActivityHandler.None && pHansel.Input.SwitchItemJustPressed) ||
				(pGretel.mCurrentActivity == ActivityHandler.None && pGretel.Input.SwitchItemJustPressed))
				&&
				((pHansel.SkeletonPosition - pGretel.SkeletonPosition).Length() <= MaxSwapDistance)
				)
			{
				if (pHansel.Lantern)
				{
					pGretel.Lantern = true;
					pHansel.Lantern = false;
				}
				else if (pGretel.Lantern)
				{
					pHansel.Lantern = true;
					pGretel.Lantern = false;
				}
			}

			if (pHansel.Lantern)
			{
				LanternLight.Position = pHansel.SkeletonPosition + LanternOffset;
				if (pHansel.Input.UseItemIsPressed)
					LanternLight.NormalZ = LanternHeightRaised;
				else
					LanternLight.NormalZ = LanternHeight;
			}
			else if (pGretel.Lantern)
			{
				LanternLight.Position = pHansel.SkeletonPosition + LanternOffset;
				LanternLight.NormalZ = LanternHeight;
			}
			UpdateVisibility(pScene, pHansel, pGretel);
			CollectCollectables(pSavegame, pScene, pHansel, pGretel);
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

		protected void CollectCollectables(Savegame pSavegame, SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			foreach (Collectable col in pScene.Collectables)
			{
				if (col.IsVisible)
				{
					if (col.CollisionBox.Intersects(pHansel.CollisionBox))
					{
						pSavegame.Collectables.Add(col);
						pScene.Collectables.Remove(col);
					}
					else if (col.CollisionBox.Intersects(pGretel.CollisionBox))
					{
						//Bei LegUpGrab Item erst im richtigen Moment einsammeln
						if (pGretel.mCurrentActivity != null && pGretel.mCurrentActivity.GetType() == typeof(LegUp) && pGretel.mCurrentActivity.m2ndState && pGretel.mCurrentState < 4)
							return;
						pSavegame.Collectables.Add(col);
						pScene.Collectables.Remove(col);
					}
				}
			}
		}

		#endregion
	}
}
