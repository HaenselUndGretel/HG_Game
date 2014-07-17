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

		protected enum LanternOwnership { None, Hansel, Gretel };

		protected const float MaxSwapDistance = 200f;
		protected LanternOwnership Lantern;
		protected Vector2 LanternOffset;
		protected PointLight LanternLight;

		#endregion

		#region Constructor

		public ItemHandler()
		{
			Lantern = LanternOwnership.None;
			LanternOffset = new Vector2(0, -100);
			LanternLight = new PointLight(); 
		}

		#endregion

		#region Methods

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel, Savegame pSavegame)
		{
			//SwapItem
			if (
				((pHansel.mCurrentActivity == ActivityHandler.None && pHansel.Input.SwitchItemJustPressed) ||
				(pGretel.mCurrentActivity == ActivityHandler.None && pGretel.Input.SwitchItemJustPressed))
				&&
				((pHansel.SkeletonPosition - pGretel.SkeletonPosition).Length() <= MaxSwapDistance)
				)
			{
				switch (Lantern)
				{
					case LanternOwnership.Hansel:
						Lantern = LanternOwnership.Gretel;
						break;
					case LanternOwnership.Gretel:
						Lantern = LanternOwnership.Hansel;
						break;
				}
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
			if (Lantern == LanternOwnership.None)
				return false;
			Vector2 Position;
			float Radius = 100f;
			switch (Lantern)
			{
				case LanternOwnership.Hansel:
					Position = pHansel.SkeletonPosition + LanternOffset;
					if (pHansel.Input.UseItemIsPressed)
						Radius *= 1.8f;
					break;
				case LanternOwnership.Gretel:
					Position = pGretel.SkeletonPosition + LanternOffset;
					break;
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
