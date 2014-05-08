using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class GameScene : Scene
	{
		#region Properties

		#endregion

		#region Constructor

		public GameScene(String pSceneName)
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

		}

		public override void Update()
		{

		}

		public override void Draw()
		{
			
			
		}

		#endregion
	}
}
