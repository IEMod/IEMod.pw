using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Tooltips {
	[ModifiesType]
	public class mod_UICamera : UICamera
	{
		[ModifiesMember("Update")]
		private void UpdateNew()
		{
			if (Application.isPlaying && this.handlesEvents)
			{
				current = this;
				if (this.useMouse || (this.useTouch && this.mIsEditor))
				{
					this.ProcessMouse();
				}
				if (this.useTouch)
				{
					this.ProcessTouches();
				}
				if (onCustomInput != null)
				{
					onCustomInput();
				}
				if ((this.useMouse && (mSel != null)) && (((this.cancelKey0 != KeyCode.None) && Input.GetKeyDown(this.cancelKey0)) || ((this.cancelKey1 != KeyCode.None) && Input.GetKeyDown(this.cancelKey1))))
				{
					selectedObject = null;
				}
				if (mSel != null)
				{
					string inputString = Input.inputString;
					if (this.useKeyboard && Input.GetKeyDown(KeyCode.Delete))
					{
						inputString = inputString + "\b";
					}
					if (inputString.Length > 0)
					{
						if (!this.stickyTooltip && (this.mTooltip != null))
						{
							this.ShowTooltip(false);
						}
						Notify(mSel, "OnInput", inputString);
					}
				}
				else
				{
					inputHasFocus = false;
				}
				if (mSel != null)
				{
					this.ProcessOthers();
				}
				if (this.useMouse && (mHover != null))
				{
					float axis = Input.GetAxis(this.scrollAxisName);
					if (axis != 0f)
					{
						Notify(mHover, "OnScroll", axis);
					}
					if ((showTooltips && (this.mTooltipTime != 0f)) && (((this.mTooltipTime < Time.realtimeSinceStartup) || Input.GetKey(KeyCode.LeftShift)) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.Tab))) // added || Input.GetKey(KeyCode.Tab)
					{
						this.mTooltip = mHover;
						this.ShowTooltip(true);
					}
				}
				current = null;
			}
		}
	}
}