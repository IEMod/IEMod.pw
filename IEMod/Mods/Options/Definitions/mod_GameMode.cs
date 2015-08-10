using IEMod.Helpers;
using Patchwork.Attributes;

namespace IEMod.Mods.Options {

	/// <summary>
	/// This mod the GameMode class to save IEMod-specific preferences along with the normal preferences.
	/// </summary>
	[ModifiesType("GameMode")]
	public class mod_GameMode : GameMode
	{

		[MemberAlias(".ctor", typeof(object))]
		private void object_ctor() {
			
		}
		//Note that IEModOptions are just kept in a static class.
		[NewMember]
		[DuplicatesBody(".ctor", typeof(GameMode))]
		private void CtorOrig() {
			throw new DeadEndException("CtorOrig");
		}

		[ModifiesMember(".ctor")]
		public void CtorNew() {
			object_ctor();
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