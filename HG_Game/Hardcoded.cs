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

		public const float KnockOverTree_EnterBalanceDistance = 74f;
		public const float KnockOverTree_BalanceSpeedFactor = 0.6f;

		public static Vector2 LegUp_StartOffsetGretel = new Vector2(0, 30);
		public static Vector2[] LegUp_OffsetGretel = {new Vector2(0, -275), //000
													 Vector2.Zero, //001
													 Vector2.Zero, //002
													 Vector2.Zero, //003
													 new Vector2(0, -275), //004
													 Vector2.Zero, //005
													 Vector2.Zero, //006
													 new Vector2(0, -275), //007
													 Vector2.Zero, //008
													 Vector2.Zero, //009
													 new Vector2(0, -275), //010
													 Vector2.Zero, //011
													 Vector2.Zero, //012
													 new Vector2(0, -275), //013
													 Vector2.Zero, //014
													 Vector2.Zero, //015
													 Vector2.Zero, //016
													 Vector2.Zero //017
													 };

		public const int PushRock_RockMoveDistance = 256;
		public const float PushRock_SteppingDuration = 7f;

		public const float UseWell_ProgressPerRotation = 0.1f;
		public const float UseWell_UpRotationFrictionFactor = 0.75f;
		public const float UseWell_ShowOverlayProgressBarrier = 0.1f;

		#endregion
		
		#region ActivityHud

		public static Vector2 ActI_OffsetButton = new Vector2(0, -200);
		public static Vector2 ActI_OffsetThumbstick = new Vector2(0, -140);

		public static Vector2 ActionInfo_Offset = new Vector2(-100, -200);
		public static Vector2 ActionInfo_OffsetButton = new Vector2(-50, -300);

		#endregion

		#region Special Scenes

		public const int Scene_Amulet = 16;
		public const int Scene_Lantern = 4;
		public const int Scene_LanternDoor = 3;
		public static int[] Scene_Waystone = { 3, 6, 12, 15 };
		public const string Scene_Waystone_IObjName = "waystone";

		#endregion

		#region Frost

		public const float Temp_SteppingDuration = 6f;
		public const float Temp_Distance = 300f;
		public const float Temp_MinBodyTemperature = 0.7f;

		#endregion

		#region Lantern

		public const float Lantern_Intensity = 10f;
		public static Vector3 Lantern_LightColor = new Vector3(1f, 1f, 1f);
		public const float Lantern_MaxSwapDistance = 200f;
		public const float Lantern_Height = 0.08f;
		public const float Lantern_HeightRaised = 0.15f;
		public const float Lantern_Radius = 200.0f;
		public const float Lantern_RadiusRaised = 250.0f;
		public const float Lantern_RaiseSteppingDuration = 0.3f;

		#endregion

		public const float End_FadingDuration = 3f;

		#region Animations

		//SetupInteractiveObjectsFromDeserialization
		public const string Anim_Tree_Fallen_Up = "idle";//"fallen";
		public const string Anim_Tree_Fallen_Down = "idle";//"fallen";
		public const string Anim_Tree_Fallen_Side = "idle";//"fallen";

		/*
		Character Walk-Animations -> KryptonEngine/HG_Data/Character/Character.cs
		*/
		public const string Anim_RaiseLantern_Hansel = "attack";//"raiseLantern";

		public const string Anim_Jump_Up_Hansel = "attack";//"jump";
		public const string Anim_Jump_Down_Hansel = "attack";//"jump";
		public const string Anim_Jump_Side_Hansel = "attack";//"jump";

		public const string Anim_KnockOverTree_Up = "attack";//"knockOverTree";
		public const string Anim_KnockOverTree_Down = "attack";//"knockOverTree";
		public const string Anim_KnockOverTree_Side = "attack";//"knockOverTree";

		public const string Anim_Balance_Enter_Up = "attack";//"balanceEnterUp";
		public const string Anim_Balance_Enter_Down = "attack";//"balanceEnterDown";
		public const string Anim_Balance_Enter_Side = "attack";//"balanceEnterSide";

		public const string Anim_Balance_Idle = "attack";//"balanceSide";
		public const string Anim_Balance_Up = "attack";//"balanceUp";
		public const string Anim_Balance_Down = "attack";//"balanceDown";
		public const string Anim_Balance_Side = "attack";//"balanceSide";

		public const string Anim_Balance_Leave_Up = "attack";//"balanceLeaveUp";
		public const string Anim_Balance_Leave_Down = "attack";//"balanceLeaveDown";
		public const string Anim_Balance_Leave_Side = "attack";//"balanceLeaveSide";

		public const string Anim_Tree_KnockOver_Up = "wiggle";//"knockOver";
		public const string Anim_Tree_KnockOver_Down = "wiggle";//"knockOver";
		public const string Anim_Tree_KnockOver_Side = "wiggle";//"knockOver";

		public const string Anim_Tree_Falling_Up = "falling_back";//"falling";
		public const string Anim_Tree_Falling_Down = "falling_front";//"falling";
		public const string Anim_Tree_Falling_Side = "fall_side";//"falling";

		public const string Anim_Lantern_Place = "attack";//"lanternPlace";
		public const string Anim_Lantern_Grab = "attack";//"lanternGrab";
								 
		public const string Anim_LegUp_Raise = "attack";//"legUpRaise";
		public const string Anim_LegUp_Lower = "attack";//"legUpLower";
		public const string Anim_LegUp_Lift_Gretel = "attack";//"legUpLift";
		public const string Anim_LegUp_Grab_Gretel = "attack";//"legUpGrab";

		public const string Anim_PushDoor = "attack";//"pushDoorDown";
		public const string Anim_Door_Open = "attack";//"openUpDown";

		public const string Anim_PushRock_Up = "attack";//"pushRockUp";
		public const string Anim_PushRock_Down = "attack";//"pushRockDown";
		public const string Anim_PushRock_Side = "attack";//"pushRockSide";

		public const string Anim_SlipAway_Gretel = "attack";//"slipAway";
		public const string Anim_SlipBack_Gretel = "attack";//"slipBack";

		public const string Anim_Well_GrabWind_Hansel = "attack";//"wellGrabWind";
		public const string Anim_Well_Wind_Idle_Hansel = "attack";//"wellWindIdle";
		public const string Anim_Well_Wind_Up_Hansel = "attack";//"wellWindUp";
		public const string Anim_Well_Wind_Down_Hansel = "attack";//"wellWindDown";

		public const string Anim_Well_Enter_Gretel = "attack";//"wellEnter";
		public const string Anim_Well_Leave_Gretel = "attack";//"wellLeave";
		public const string Anim_Well_Hang_Gretel = "attack";//"wellHang";
		public const string Anim_Well_Idle_Gretel = "attack";//"wellIdle";

		public const string Anim_Amulet_Charge = "attack";//"amuletCharge";
		public const string Anim_Amulet_Use = "attack";//"amuletUse";

		#endregion

	}
}
