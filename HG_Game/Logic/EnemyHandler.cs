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

		public void Update()
		{
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
