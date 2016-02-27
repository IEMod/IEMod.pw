using Patchwork.Attributes;

namespace IEMod.Mods.Options {
	[ModifiesType]
	public abstract class mod_UIWindowManager :  UIWindowManager
	{
		[NewMember]
		[DuplicatesBody("WindowHidden")]
		public void WindowHiddenOrig(UIHudWindow window, bool unsuspend) {
			
		}

		[ModifiesMember("WindowHidden")]
		public void WindowHiddenNew(UIHudWindow window, bool unsuspend)
		{
			if (unsuspend && window.IAmSuspending != null) {
				foreach (UIHudWindow window2 in window.IAmSuspending) {
					window2.Unsuspend();
				}
				window.IAmSuspending.Clear();
			}
					if (UIAbilityTooltipManager.Instance != null)
		{
			UIAbilityTooltipManager.Instance.HideAll ();
		}
		UIActionBarTooltip.GlobalHide ();
		if ((UIWindowSwitcher.Instance != null) && (UIWindowSwitcher.Instance.Anchor.widgetContainer == window.SwitcherAnchor))
		{
			UIWindowSwitcher.Instance.Hide ();
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		foreach (UIHudWindow window3 in this.m_Windows)
		{
			if (window3.WindowActive () && (window3 != window))
			{
				flag |= window3.HidesHud;
				flag3 = window3.EatsMouseInput || flag3;
				flag2 = window3.PausesGame || flag2;
				flag4 = (window3.DimsBackground || window3.DimsBackgroundTemp) || flag4;
				flag5 |= window3.ClickOffCloses && !window3.DimsBackground;
			}
		}
		if (window.EatsKeyInput && (CameraControl.Instance != null))
		{
			CameraControl.Instance.EnablePlayerControl(true);
		}
		if (!flag && (InGameHUD.Instance != null))
		{
			InGameHUD.Instance.HidePause = false;
		}
		if (this.m_WindowHasGamePaused && !flag2)
		{
			this.m_WindowHasGamePaused = false;
			if (TimeController.Instance != null)
			{
				TimeController.Instance.UiPaused = false;
			}
		}
		if (this.m_WindowHasBgDimmed && !flag4)
		{
			this.m_WindowHasBgDimmed = false;
			this.DimBackgroundTween.Play(false);
		}
		if (!flag5)
		{
			this.NonDimBackgroundObject.SetActive(false);
		}
		if (this.OnWindowHidden != null)
		{
			this.OnWindowHidden(window);
		}
		}
	}
}