using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;

namespace HanselAndGretel
{
	public class SceneData
	{
		#region Properties

		public List<Rectangle> MoveArea;
		public List<Waypoint> Waypoints;

		public List<InteractiveObject> InteractiveObjects;
		public List<int> Events;

		public List<Collectable> Collectables;
		public List<Item> Items;

		public List<Enemy> Enemies;

		//public List<Light> Lights;
		//public List<Emitter> Emitter;
		//public List<SoundAreas> SoundAreas;

		#endregion

		#region Constructor

		#endregion

		#region OverrideMethods

		#endregion

		#region Methods

		public void LoadLevel(int pLevelId)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
