using HanselAndGretel.Data;
using KryptonEngine.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public static class Cheats
	{
		#region Methods

		public static void Update(Savegame pSavegame, SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			List<Player> UpdateList = new List<Player>();
			if (InputHelper.Player1.KeyPressed(Keys.NumPad1))
				UpdateList.Add(pHansel);
			if (InputHelper.Player1.KeyPressed(Keys.NumPad2))
				UpdateList.Add(pGretel);

			float Speed = 10f;

			foreach (Player p in UpdateList)
			{
				if (InputHelper.Player1.KeyPressed(Keys.NumPad4))
					p.MoveInteractiveObject(new Vector2(-Speed, 0));
				if (InputHelper.Player1.KeyPressed(Keys.NumPad8))
					p.MoveInteractiveObject(new Vector2(0, -Speed));
				if (InputHelper.Player1.KeyPressed(Keys.NumPad6))
					p.MoveInteractiveObject(new Vector2(Speed, 0));
				if (InputHelper.Player1.KeyPressed(Keys.NumPad5))
					p.MoveInteractiveObject(new Vector2(0, Speed));
			}
		}

		#endregion

	}
}
