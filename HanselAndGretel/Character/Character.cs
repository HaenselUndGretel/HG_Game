using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;

namespace HanselAndGretel
{
	public class Character : GameObject
	{
		#region Properties

		public SpineObject Model;

		#endregion

		#region Override Methods

		public override void Initialize()
		{
			throw new System.NotImplementedException();
		}

		public override void LoadContent()
		{
			throw new System.NotImplementedException();
		}

		public override void Update()
		{
			throw new System.NotImplementedException();
		}

		public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
		{
			throw new System.NotImplementedException();
		}

		#endregion

		#region Methods

		public void Move(Vector2 pDelta)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
