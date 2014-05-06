using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.SceneManagement;
using KryptonEngine.Entities;

namespace HanselAndGretel
{
	public class MenuScene : Scene
	{
		#region Constructor

		public MenuScene(String pSceneName)
            : base(pSceneName)
        {

        }

		#endregion

		#region Override Methods

		public override void Initialize()
		{
			mCamera = new Camera();
		}

		public override void LoadContent()
		{
			throw new System.NotImplementedException();
		}

		public override void Update()
		{
			throw new System.NotImplementedException();
		}

		public override void Draw()
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
