using IEMod.Helpers;
using Patchwork.Attributes;

namespace IEMod.Mods.Options {

	/// <summary>
	/// This mod the GameMode class to save IEMod-specific preferences along with the normal preferences.
	/// </summary>
	[ModifiesType]
	public class mod_GameMode : GameMode
	{
		//Note that IEModOptions are just kept in a static class.
		[NewMember]
		[DuplicatesBody(".ctor")]
		private void CtorOrig() {
			throw new DeadEndException("CtorOrig");
		}

		[ModifiesMember(".ctor")]
		public void CtorNew() {
			CtorOrig();
			IEModOptions.LoadFromPrefs();
		}

		[NewMember]
		[DuplicatesBody("LoadFromPrefs")]
		private void LoadFromPrefsOrig() {
			throw new DeadEndException("LoadFromPrefsOrig");
		}

		[ModifiesMember("LoadFromPrefs")]
		public void LoadFromPrefsNew() {
			LoadFromPrefsOrig();
			IEModOptions.LoadFromPrefs();
		}

		[NewMember]
		[DuplicatesBody("SaveToPrefs")]
		private void SaveToPrefsOrig() {
			throw new DeadEndException("SaveToPrefsOrig");
		}

		[ModifiesMember("SaveToPrefs")]
		public void SaveToPrefsNew()
		{
			SaveToPrefsOrig();
			IEModOptions.SaveToPrefs();
		}

		[NewMember]
		[DuplicatesBody("Matches")]
		private bool MatchesOrig(GameMode other) {
			throw new DeadEndException("MatchesOrig");
		}

		[ModifiesMember("Matches")]
		public bool MatchesNew(GameMode other) {
			return MatchesOrig(other) && IEModOptions.IsIdenticalToPrefs();
		}
	}
}