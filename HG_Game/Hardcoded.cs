using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public static class Hardcoded
	{
		#region Activities

		public static const float KnockOverTree_EnterBalanceDistance = 74f;
		public static const float KnockOverTree_BalanceSpeedFactor = 0.6f;

		public static const Vector2 LegUp_StartOffsetGretel = new Vector2(55, -20);
		public static const Vector2 LegUp_OffsetGretel = new Vector2(-20, -255);

		public static const int PushRock_RockMoveDistance = 256;
		public static const float PushRock_SteppingDuration = 7f;

		public static const float UseWell_ProgressPerRotation = 0.1f;
		public static const float UseWell_UpRotationFrictionFactor = 0.75f;
		public static const float UseWell_ShowOverlayProgressBarrier = 0.1f;

		#endregion
		
		#region ActivityHud

		public static const Vector2 ActI_OffsetButton = new Vector2(0, -200);
		public static const Vector2 ActI_OffsetThumbstick = new Vector2(0, -140);

		public static const Vector2 ActionInfo_Offset = new Vector2(-100, -200);
		public static const Vector2 ActionInfo_OffsetButton = new Vector2(-50, -300);

		#endregion

		#region Special Scenes

		public static const int Scene_Amulet = 0;
		public static const int Scene_Lantern = 4;
		public static const int Scene_LanternDoor = 3;

		#endregion

		#region Frost

		public static const float Temp_SteppingDuration = 6f;
		public static const float Temp_Distance = 300f;
		public static const float Temp_MinBodyTemperature = 0.7f;

		#endregion

		#region Lantern

		public static const float Lantern_Intensity = 10f;
		public static const Vector3 Lantern_LightColor = new Vector3(1f, 1f, 1f);
		public static const float Lantern_MaxSwapDistance = 200f;
		public static const float Lantern_Height = 0.08f;
		public static const float Lantern_HeightRaised = 0.15f;
		public static const float Lantern_Radius = 8f;
		public static const float Lantern_RadiusRaised = 15f;
		public static const float Lantern_RaiseSteppingDuration = 0.3f;

		#endregion

		public static const float End_FadingDuration = 3f;

		#region Animations

		#endregion

	}
}
