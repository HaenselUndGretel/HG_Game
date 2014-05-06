using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class GameGraphics
	{
		#region Methods

		public void Update(float GIP)
		{
			//GraphicsInterpolationProgress:
			//Interpolieren über einen LogicTick hinweg:
			//OldLogic -> Graphics -> NewLogic
			//z.B.:
			//X = (newX - oldX) * GIP
		}

		public void Draw()
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
