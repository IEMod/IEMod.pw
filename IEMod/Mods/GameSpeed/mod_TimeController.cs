using IEMod.Mods.Options;
using Patchwork.Attributes;


namespace IEMod.Mods.GameSpeed {
	[ModifiesType]
	public class mod_TimeController : TimeController
	{
		[ModifiesMember("ToggleFast")]
		public void ToggleFastNew()
		{
			if (GameInput.GetControlkey() && IEModOptions.GameSpeedMod) {
				this.TimeScale = this.TimeScale == 5.0f ? this.NormalTime : 5.0f;
				this.UpdateTimeScale();
			} else if (!GameInput.GetControlkey()) //default behavior is that ctrl+d does not trigger fast mode toggle
			{
				this.TimeScale = this.TimeScale == this.FastTime ? this.NormalTime : this.FastTime;
				this.UpdateTimeScale();
			}
		}

		[ModifiesMember("ToggleSlow")]
		public void ToggleSlowNew()
		{
			if (GameInput.GetControlkey() && IEModOptions.GameSpeedMod) {
				this.TimeScale = this.TimeScale == 0.16f ? this.NormalTime : 0.16f;
				this.UpdateTimeScale();
			} else if (!GameInput.GetControlkey()) {
				this.TimeScale = this.TimeScale == this.SlowTime ? this.NormalTime : this.SlowTime;
				this.UpdateTimeScale();
			}
		}

		[ModifiesMember("Update")]
		private void UpdateNew()
		{
			if (GameState.InCombat && this.TimeScale > this.NormalTime)
			{
				this.TimeScale = 1f;
			}
			if (!GameState.IsLoading)
			{
				this.UpdateTimeScale();
			}
			if (UIWindowManager.KeyInputAvailable)
			{
				if (GameInput.GetControlDown(MappedControl.RESTORE_SPEED, true))
				{
					this.TimeScale = this.NormalTime;
				}
				else if (GameInput.GetControlDownWithoutModifiers(MappedControl.SLOW_TOGGLE))
				{
					this.ToggleSlow();
				}
				else if (!GameState.InCombat && GameInput.GetControlDownWithoutModifiers(MappedControl.FAST_TOGGLE))
				{
					this.ToggleFast();
				}
			}
		}
	}
}
