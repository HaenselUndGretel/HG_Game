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

		public static Vector2 LegUp_StartOffsetGretel = new Vector2(55, -20);
		public static Vector2 LegUp_OffsetGretel = new Vector2(-20, -255);

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

		public const int Scene_Amulet = 0;
		public const int Scene_Lantern = 4;
		public const int Scene_LanternDoor = 3;

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
		public const float Lantern_Radius = 8f;
		public const float Lantern_RadiusRaised = 15f;
		public const float Lantern_RaiseSteppingDuration = 0.3f;

		#endregion

		public const float End_FadingDuration = 3f;

		#region Animations
		/*
		Character Walk-Animations -> KryptonEngine/HG_Data/Character/Character.cs
		*/
		public const string Anim_RaiseLantern_Hansel = "raiseLantern";

		public const string Anim_Jump_Up_Hansel = "jump";
		public const string Anim_Jump_Down_Hansel = "jump";
		public const string Anim_Jump_Side_Hansel = "jump";

		public const string Anim_KnockOverTree_Up = "knockOverTree";
		public const string Anim_KnockOverTree_Down = "knockOverTree";
		public const string Anim_KnockOverTree_Side = "knockOverTree";

		public const string Anim_Balance_EnterUp = "balanceEnterUp";
		public const string Anim_Balance_EnterDown = "balanceEnterDown";
		public const string Anim_Balance_EnterSide = "balanceEnterSide";

		public const string Anim_Balance_Idle = "balanceSide";
		public const string Anim_Balance_Up = "balanceUp";
		public const string Anim_Balance_Down = "balanceDown";
		public const string Anim_Balance_Side = "balanceSide";
		/*						 
		public const string Anim_Balance_Up_Shiver = "balanceUpShiver";
		public const string Anim_Balance_Down_Shiver = "balanceDownShiver";
		public const string Anim_Balance_Side_Shiver = "balanceSideShiver";
		*/						 
		public const string Anim_Balance_LeaveUp = "balanceLeaveUp";
		public const string Anim_Balance_LeaveDown = "balanceLeaveDown";
		public const string Anim_Balance_LeaveSide = "balanceLeaveSide";

		public const string Anim_Tree_KnockOver_Up = "knockOver";
		public const string Anim_Tree_KnockOver_Down = "knockOver";
		public const string Anim_Tree_KnockOver_Side = "knockOver";

		public const string Anim_Tree_Falling_Up = "falling";
		public const string Anim_Tree_Falling_Down = "falling";
		public const string Anim_Tree_Falling_Side = "falling";

		public const string Anim_Tree_Fallen_Up = "fallen";
		public const string Anim_Tree_Fallen_Down = "fallen";
		public const string Anim_Tree_Fallen_Side = "fallen";

		public const string Anim_Lantern_Place = "lanternPlace";
		public const string Anim_Lantern_Grab = "lanternGrab";
								 
		public const string Anim_LegUp_Raise = "legUpRaise";
		public const string Anim_LegUp_Lower = "legUpLower";
		public const string Anim_LegUp_Lift_Gretel = "legUpLift";
		public const string Anim_LegUp_Grab_Gretel = "legUpGrab";

		public const string Anim_PushDoor_Up = "pushDoorUp";
		public const string Anim_PushDoor_Down = "pushDoorDown";
		public const string Anim_PushDoor_Side = "pushDoorSide";
								 
		public const string Anim_PullDoor_Up = "pullDoorUp";
		public const string Anim_PullDoor_Down = "pullDoorDown";
		public const string Anim_PullDoor_Side = "pullDoorSide";

		public const string Anim_Door_openUp = "openUpDown";
		public const string Anim_Door_openDown = "openUpDown";
		public const string Anim_Door_openSide = "openSide";

		public const string Anim_Door_closeUp = "closeUpDown";
		public const string Anim_Door_closeDown = "closeUpDown";
		public const string Anim_Door_closeSide = "closeSide";

		public const string Anim_Push_RockUp = "pushRockUp";
		public const string Anim_Push_RockDown = "pushRockDown";
		public const string Anim_Push_RockSide = "pushRockSide";

		public const string Anim_Rock_MovingUp = "moving";
		public const string Anim_Rock_MovingDown = "moving";
		public const string Anim_Rock_MovingSide = "moving";

		public const string Anim_SlipAway_Gretel = "slipAway";
		public const string Anim_SlipBack_Gretel = "slipBack";

		public const string Anim_Well_GrabWind_Hansel = "wellGrabWind";
		public const string Anim_Well_WindIdle_Hansel = "wellWindIdle";
		public const string Anim_Well_WindUp_Hansel = "wellWindUp";
		public const string Anim_Well_WindDown_Hansel = "wellWindDown";

		public const string Anim_Well_Enter_Gretel = "wellEnter";
		public const string Anim_Well_Leave_Gretel = "wellLeave";
		public const string Anim_Well_Hang_Gretel = "wellHang";
		public const string Anim_Well_Idle_Gretel = "wellIdle";

		public const string Anim_Amulet_Charge = "amuletCharge";
		public const string Anim_Amulet_Use = "amuletUse";

		#endregion

	}
}
