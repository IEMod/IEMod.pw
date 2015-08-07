using IEMod.Helpers;
using IEMod.Mods.Options;
using Patchwork.Attributes;


namespace IEMod.Mods.AlwaysShowCircles {
	/// <summary>
	/// Always shows Selection Circles if such option is enabled. Also show circles when game is paused like in IE games.
	/// </summary>
	[ModifiesType]
	public abstract class Mod_AlwaysShowSelectionCircles_Faction : Faction
	{
		[NewMember]
		[DuplicatesBody("ShowSelectionCircle")]
		private bool ShowSelectionCircleOrig(bool elevate) {
			throw new DeadEndException("ShowSelectionCircleOrig");
		}

		[ModifiesMember("ShowSelectionCircle")]	
		public bool ShowSelectionCircleNew(bool elevate) {
		if (InGameHUD.Instance == null) {
			return false;
		}
		//fine
		
		Health component = base.GetComponent<Health>();
		bool flag = this.DrawSelectionCircle && InGameHUD.Instance.ShowHUD;
		bool flag2 = ((component == null) || !component.ShowDead) || GameInput.SelectDead;
		
		bool flag3 = !GameState.Option.GetOption(GameOption.BoolOption.HIDE_CIRCLES) || GameState.Paused;
		//fine
		
		bool flag4 = ((this.RelationshipToPlayer == Relationship.Hostile) || this.isPartyMember) || elevate;

		
		bool alwaysShow = IEModOptions.AlwaysShowCircles; // added this line
		
		
		return (((flag && flag2) && this.isFowVisible) && (((flag3 && flag4) || this.MousedOver) || InGameHUD.Instance.HighlightActive || alwaysShow || GameState.Paused)); // added alwaysShow and GameState.Paused
		}
	}
}
