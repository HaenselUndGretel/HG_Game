using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using KryptonEngine;
using KryptonEngine.SceneManagement;

namespace HG_Game
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : EngineGame
	{

		public Game1()
			: base()
		{
			EngineSettings.OnWindows = true;
			EngineSettings.IsDebug = false;
			this.IsMouseVisible = false;
			EngineSettings.DisplayHeight = 720;
			EngineSettings.DisplayWidth = 1280;
			EngineSettings.SetResolution(1280, 720);
			//EngineSettings.SetToFullScreen();
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			SceneManager.Instance.AddScene(new SplashScene("Splash"));
			SceneManager.Instance.AddScene(new MenuScene("Menu"));
			SceneManager.Instance.AddScene(new GameScene("Game"));
			SceneManager.Instance.AddScene(new CreditsScene("Credits"));
			SceneManager.Instance.SetStartSceneTo("Game");
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			base.LoadContent();
			SceneManager.Instance.GetScene("Game").Background = "pixel";
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}
	}
}
