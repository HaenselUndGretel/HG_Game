using KryptonEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework.Graphics;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using KryptonEngine.Controls;
using Microsoft.Xna.Framework.Input;
using KryptonEngine;

namespace HG_Game
{
    public class SplashScene : Scene
    {
        #region Properties

        bool isBehindert = false;

        #endregion

        #region Constructor

        public SplashScene(String pSceneName)
            : base(pSceneName)
        {

        }

        #endregion

        #region Override Methods

        public override void Initialize()
        {
        }

        public override void LoadContent()
        {
        }

        public override void Update()
        {
            if(!isBehindert)
            {
                CutScenePlayer.Play("Intro");
                isBehindert = true;
            }
            if (InputHelper.Player1.InputJustPressed(InputHelper.mDebug))
            {
                CutScenePlayer.Stop();
                SceneManager.Instance.SetCurrentSceneTo("Menu");
            }
            if (!CutScenePlayer.isPlaying()) SceneManager.Instance.SetCurrentSceneTo("Menu");
        }

        public override void Draw()
        {
            
            mSpriteBatch.Begin();
            CutScenePlayer.Draw(mSpriteBatch);
            mSpriteBatch.End();
        }

        #endregion
    }
}
