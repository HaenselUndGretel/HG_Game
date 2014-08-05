using HanselAndGretel.Data;
using KryptonEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class EnemyHandler
	{

		public EnemyHandler()
		{

		}

		public void Update(Savegame pSavegame)
		{
			if (GameReferenzes.SceneID == 9)
			{
				if (pSavegame.Scenes[8].Collectables.Count == 0)
				{
					bool WitchAvidable = false;
					foreach (Enemy e in GameReferenzes.Level.Enemies)
						if (e.GetType() == typeof(Witch))
							WitchAvidable = true;
					if (!WitchAvidable)
					{
						Witch witch = new Witch("witch");
						witch.LoadContent();
						witch.Position = new Vector2(675, 650);
						witch.ApplySettings();
						GameReferenzes.Level.Enemies.Add(witch);
						GameReferenzes.Level.RenderList.Add(witch);
					}
				}
			}

			foreach (Enemy e in GameReferenzes.Level.Enemies)
			{
				e.Update();
				if (Vector2.Distance(GameReferenzes.ReferenzHansel.Position, e.Position) < GameReferenzes.DEATH_ZONE)
					GameScene.End = true;
				if (Vector2.Distance(GameReferenzes.ReferenzGretel.Position, e.Position) < GameReferenzes.DEATH_ZONE)
					GameScene.End = true;
			}
		}
	}
}
