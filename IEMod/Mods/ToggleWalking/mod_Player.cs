using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.ToggleWalking {
	[ModifiesType]
	public class mod_Player : Player
	{
		[NewMember]
		public static bool WalkMode = false;

		[NewMember]
		public static void ToggleWalkMode()
		{
			if (UIWindowManager.KeyInputAvailable)
			{
				if (GameInput.GetControlUp(MappedControl.Deprecated_CHANT_EDITOR))
				{
					WalkMode = !WalkMode;
					string msg;
					Color color;
					if (WalkMode)
					{
						msg = "It is a nice day for a walk";
						color = Color.green;
					}
					else
					{
						msg = "Time to move.  We can sight see later!";
						color = Color.red;
					}
					Console.AddMessage(msg, color);
				}
			}
		}

        [NewMember]
        [DuplicatesBody("UpdateCursor")]
        protected void UpdateCursorOriginal()
        {

        }

        [ModifiesMember("UpdateCursor")]
        protected void UpdateCursorNew()
        {
            mod_Player.ToggleWalkMode();
            UpdateCursorOriginal();            
        }


	}
}