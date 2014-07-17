using HanselAndGretel.Data;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class ActivityInstruction
	{
		#region Properties

		public enum ThumbstickDirection
		{
			None,
			Up,
			Down,
			Left,
			Right,
			Rotate
		};

		#region Ressources

		//Button
		protected Texture2D ButtonX;

		//Thumbstick
		protected Texture2D ThumbstickUp;
		protected Texture2D ThumbstickDown;
		protected Texture2D ThumbstickLeft;
		protected Texture2D ThumbstickRight;
		protected Texture2D ThumbstickRotate;

		//Offsets
		protected Vector2 OffsetButton;
		protected Vector2 OffsetThumbstick;

		#endregion

		//Fading
		public HudFading Fading;

		//Specific States
		public ThumbstickDirection ThumbstickDirHansel;
		public ThumbstickDirection ThumbstickDirGretel;

		#endregion

		#region Constructor

		public ActivityInstruction()
		{
			LoadContent();
			Fading = new HudFading();
			OffsetButton = new Vector2(0, -200);
			OffsetThumbstick = new Vector2(0, -140);
			ThumbstickDirHansel = ThumbstickDirection.None;
			ThumbstickDirGretel = ThumbstickDirection.None;
		}

		#endregion

		#region Methods

		protected void LoadContent()
		{
			ButtonX = TextureManager.Instance.GetElementByString("button_x");

			ThumbstickUp = TextureManager.Instance.GetElementByString("ThumbstickUp");
			ThumbstickDown = TextureManager.Instance.GetElementByString("ThumbstickDown");
			ThumbstickLeft = TextureManager.Instance.GetElementByString("ThumbstickLeft");
			ThumbstickRight = TextureManager.Instance.GetElementByString("ThumbstickRight");
			ThumbstickRotate = TextureManager.Instance.GetElementByString("ThumbstickRotate");
		}

		public void Update()
		{
			Fading.Update();
		}

		public void Draw(SpriteBatch pSpriteBatch, Hansel pHansel, Gretel pGretel)
		{
			//Buttons
			pSpriteBatch.Draw(ButtonX, pHansel.SkeletonPosition + OffsetButton, Color.White * Fading.VisibilityHansel);
			pSpriteBatch.Draw(ButtonX, pGretel.SkeletonPosition + OffsetButton, Color.White * Fading.VisibilityGretel);
			//Thumbsticks
			DrawThumbstick(pSpriteBatch, ThumbstickDirHansel, pHansel, Fading.VisibilityHansel);
			DrawThumbstick(pSpriteBatch, ThumbstickDirGretel, pGretel, Fading.VisibilityGretel);
			
		}

		protected void DrawThumbstick(SpriteBatch pSpriteBatch, ThumbstickDirection pDirection, Player pPlayer, float pAlpha)
		{
			Texture2D Thumbstick;
			switch (pDirection)
			{
				case ThumbstickDirection.Up:
					Thumbstick = ThumbstickUp;
					break;
				case ThumbstickDirection.Down:
					Thumbstick = ThumbstickDown;
					break;
				case ThumbstickDirection.Left:
					Thumbstick = ThumbstickLeft;
					break;
				case ThumbstickDirection.Right:
					Thumbstick = ThumbstickRight;
					break;
				case ThumbstickDirection.Rotate:
					Thumbstick = ThumbstickRotate;
					break;
				default:
					return;
			}
			pSpriteBatch.Draw(Thumbstick, pPlayer.SkeletonPosition + OffsetButton, Color.White * pAlpha);
		}

		public void SetFadingState(Player pPlayer, bool pShow, bool pInstant = true)
		{
			bool IsHansel = true;
			if (pPlayer.GetType() == typeof(Gretel))
				IsHansel = false;
			Fading.SetState(IsHansel, pShow, pInstant);
		}

		public void SetThumbstickDir(Player pPlayer, ThumbstickDirection pDir)
		{
			if (pPlayer.GetType() == typeof(Hansel))
				ThumbstickDirHansel = pDir;
			else
				ThumbstickDirGretel = pDir;
		}

		public void SetThumbstickDirBoth(ThumbstickDirection pDir)
		{
			ThumbstickDirHansel = pDir;
			ThumbstickDirGretel = pDir;
		}

		#endregion

	}
}
