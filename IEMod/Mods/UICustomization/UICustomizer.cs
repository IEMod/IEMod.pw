using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IEMod.Helpers;
using IEMod.Mods.ConsoleMod;
using IEMod.Mods.Options;
using IEMod.Mods.PartyBar;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace IEMod.Mods.UICustomization {
	[NewType]
	public static class UICustomizer
	{

		#region Controls

		private static GameObject DragPartyBar;


		private static GameObject DragHugBgr;


		private static GameObject DragLog;


		private static GameObject DragAbilitiesBar;

		
		private  static GameObject ToggleLogButtons;

		
		private  static GameObject SaveBtn;

		
		private  static GameObject CancelBtn;

		
		private  static GameObject UseDefaultUIBtn;

		private  static GameObject ToggleHudOrientation;

		
		private  static GameObject DragHudPanelLeft;

		
		private  static GameObject DragHudPanelRight;

		
		private  static GameObject DragClock;

		
		private  static GameObject ToggleHudBgr;

		
		private  static GameObject TogglePartyOrientation;

		
		private  static GameObject ToggleBuffsSide;

		
		private  static GameObject DragFormationBar;

		
		private  static GameObject ToggleButtonsBgr;

		
		private  static GameObject TogglePortraitHighlights;

		
		private  static GameObject ToggleCustomTextures;


		private static GameObject FrameDropdown;
		#endregion

		#region GameObjects

		public static GameObject
			_uiCamera;

		
		private static GameObject
			_hud;

		
		private static GameObject
			_bottom;

		
		private static GameObject
			_partyBarWindow;

		
		private static GameObject
			_partyBar;

		
		private static GameObject
			_partyPortraitBar;

		
		private static GameObject
			_actionBarWindow;

		
		private static GameObject
			_actionBar;

		
		private static GameObject
			_buttonsLeft;

		
		private static GameObject
			_01attack;

		
		private static GameObject
			_buttonsRight;

		
		private static GameObject
			_bigMapTooltip;

		
		private static GameObject
			_consoleWindow;

		
		private static GameObject
			_console;

		
		private static GameObject
			_radioGroup;

		
		private static GameObject
			_formationButtonSet;

		
		private static GameObject
			_partySolidHud;

		
		private static GameObject
			_abilitiesBar;

		
		private static GameObject
			_timeWidget;

		
		private static GameObject
			_actionBarTrimB;
		#endregion

	
		private  static Texture DefaultActionBarAtlas;

		public static string SelectedFrame;
	
		private  static Texture2D AlternateActionBarAtlas;

		private  static Texture DefaultLeftCornerTexture;

		private static bool IsInitialized;

		private static bool IsInterfaceCreated;

		private static IEModOptions.LayoutOptions DefaultLayout;
		private static GameObject _hudTrim;
		private static GameObject _customizeButton;

		public static void Initialize() {
			AddCustomizeButton();
			if (IsInitialized) return;
			//these may break in future version changes, but I doubt they'll break much.
			//since changing them will probably break some of the developer's code as well
			//to fix them, dump the UICamera hierarchy to file and see which names changed.
			//using Child(string) also tells you which names are broken because it throws an exception if it can't find something,
			//instead of silently returning null and letting you figure out where the problem came from.
			_uiCamera = UIOptionsManager.Instance.transform.parent.gameObject;
			_hud = _uiCamera.Child("HUD");
			_bottom = _hud.Child("Bottom");
			_partyBarWindow = _bottom.Child("PartyBarWindow");
			_partyBar = _partyBarWindow.Child("PartyBar");
			_partyPortraitBar = _partyBar.Child("PartyPortraitBar");
			_actionBarWindow = _bottom.Child("ActionBarWindow");
			_actionBar = _actionBarWindow.Child("ActionBar");
			_buttonsLeft = _actionBar.Child("ButtonsLeft");
			_01attack = _buttonsLeft.Child("01.Attack");
			_buttonsRight = _actionBar.Child("ButtonsRight");
			_bigMapTooltip = _uiCamera.ChildPath("Overlay/MapTooltips/BigTooltip");
			_consoleWindow = _bottom.Child("ConsoleWindow");
			_console = _consoleWindow.Child("Console");
			_radioGroup = _console.Child("RadioGroup");
			_formationButtonSet = _actionBar.Child("FormationButtonSet");
			_partySolidHud = _partyBarWindow.Child("SolidHud");
			_abilitiesBar = _partyBarWindow.Child("AbilitiesBar");
			_timeWidget = _actionBar.Child("TimeWidget");
			_actionBarTrimB = _actionBar.Child("trimB");
			_hudTrim = _hud.Child("Trim");
			if (DefaultLayout == null) {
				DefaultLayout = new IEModOptions.LayoutOptions();
				SaveLayout(DefaultLayout);
			}

			//FixPortraitHoles ();    
			UICustomizer.DefaultActionBarAtlas =
				UICustomizer._01attack.Child(0).Component<UISprite>().atlas.spriteMaterial.mainTexture;

			// turning off BB-version label in the upper right corner, cause it's annoying when you want to move portraits there
			UICustomizer._uiCamera.Child("BBVersion").gameObject.SetActive(false);

			// UIAnchors on the ActionBarWindow that prevent it from being moved... (it's related to the partybar somehow?)
			// HUD -> Bottom -> ActionBarWindow -> destroy 3 UIAnchors
			foreach (UIAnchor comp in UICustomizer._actionBarWindow.Components<UIAnchor>()) {
				GameUtilities.DestroyComponent(comp);
			}

			var component = UICustomizer._consoleWindow.Component<UIAnchor>();
			if (component) {
				GameUtilities.DestroyComponent(component);
			}

			// disable the minimize buttons for the log and the actionbar
			UICustomizer._bottom.Child("ConsoleMinimize").SetActive(false);
			UICustomizer._bottom.Child("ActionBarMinimize").gameObject.SetActive(false);

			// this UIPanel used to hide the clock when it was moved from far away from its original position
			// HUD -> Bottom -> ActionBarWindow -> ActionBarExpandedAnchor -> UIPanel
			UICustomizer._actionBarWindow.Component<UIPanel>().clipping = UIDrawCall.Clipping.None;

			// detaches the "GAME PAUSED" and "SLOW MO" from the Clock panel, to which it was attached for some reason...
			var gamePausedAnchors = UICustomizer._uiCamera.Child("GamePaused").Components<UIAnchor>();
			gamePausedAnchors[0].widgetContainer = UICustomizer._hud.Component<UIPanel>().widgets[0];
			gamePausedAnchors[1].DisableY = true;
			var slowMoAnchors = UICustomizer._uiCamera.Child("GameSpeed").Components<UIAnchor>();
			slowMoAnchors[0].widgetContainer = UICustomizer._hud.Component<UIPanel>().widgets[0];
			slowMoAnchors[1].DisableY = true;
			IsInitialized = true;
		}

		private static void AddCustomizeButton() {
			if (_customizeButton != null) return;
			_customizeButton = new GameObject {
				name = "CustomizeButton"
			};
			_customizeButton.transform.parent = UIPartyPortraitBar.Instance.transform.parent;
			_customizeButton.transform.localScale = new Vector3(1f, 1f, 1f);
			UIMultiSpriteImageButton dpbImageButton = NGUITools.AddChild(_customizeButton.gameObject, UIOptionsManager.Instance.PageButtonPrefab.gameObject).Component<UIMultiSpriteImageButton>();
			IEDebug.Log(UnityPrinter.HierarchyPrinter.Print(dpbImageButton));
			if (dpbImageButton.transform.childCount == 5) // sometimes the pagebuttonprefab seems to already have a collider, sometimes not. if it has one, we destroy it and just always manually create one.
				Object.DestroyImmediate(_customizeButton.transform.GetChild(0).GetChild(4).gameObject);

			GameObject bx = new GameObject("Collider");

			bx.transform.parent = dpbImageButton.transform;
			bx.transform.localScale = new Vector3(269f, 56f, 1f);
			bx.transform.localPosition = new Vector3(0f, 0, -2f);
			bx.layer = 14;
			//without a BoxCollider + UINoClick, clicks will go through the control.
			bx.AddComponent<BoxCollider>().size = new Vector3(1, 1, 1);
			bx.AddComponent<UINoClick>().BlockClicking = true;
			bx.AddComponent<UIEventListener>();
			dpbImageButton.Label.Component<GUIStringLabel>().FormatString = "Customize UI";
			bx.AddComponent<UIDragObject>().target = _customizeButton.transform;
			_customizeButton.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
			_customizeButton.transform.localPosition = new Vector3(1742.5f, 1010.2f, -6.0f);
			dpbImageButton.onClick += o => {
				if (IsInterfaceVisible) {
					SaveUi(IEModOptions.Layout);
					dpbImageButton.Label.Component<GUIStringLabel>().FormatString = "Customize UI";
					IEDebug.Log(_customizeButton.transform.localPosition);

					IEDebug.Log(UnityPrinter.HierarchyPrinter.Print(_customizeButton));
				} else {
					ShowInterface();
					dpbImageButton.Label.Component<GUIStringLabel>().FormatString = "Save & Close";
					dpbImageButton.Label.Component<GUIStringLabel>().RefreshText();
				}
				dpbImageButton.Label.Component<GUIStringLabel>().RefreshText();
			};
		}

		private static void ReplaceAtlas(bool state) {
			/*			// THIS IS WHAT YOU WOULD DO if you wanted to use a custom atlas
			//
			//			UIAtlas copy = (UIAtlas) UIAtlas.Instantiate (attackspr.atlas);
			//			copy.name = "Pizda";
			//			attackspr.atlas = copy;
			//			copy.spriteMaterial =  new Material(UIAtlasManager.Instance.Inventory.spriteMaterial.shader);
			//			copy.spriteMaterial.mainTexture = atlasTexture;
			//
			//			copy.spriteList.Clear ();
			//
			//			UIAtlas.Sprite sprite1 = new UIAtlas.Sprite ();
			//			sprite1.inner = new Rect (283, 18, 42, 42);
			//			sprite1.outer = new Rect (0, 0, 0, 0);
			//			sprite1.paddingLeft = 0;
			//			sprite1.paddingTop = 0;
			//			sprite1.paddingRight = 0;
			//			sprite1.paddingBottom = 0;
			//			sprite1.name = "test1";
			//
			//			copy.spriteList.Add (sprite1);
			//
			*/ // END
			var icon = _01attack.Child("Icon");
			if (state) {
				if (AlternateActionBarAtlas == null) {
					string alternateAtlas = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/buttons"),
						"ActionBarAlternate.png");

					if (File.Exists(alternateAtlas)) {
						Texture2D atlasTexture = new Texture2D(512, 256, TextureFormat.RGB24, false);
						byte[] bytes = File.ReadAllBytes(alternateAtlas);
						atlasTexture.LoadImage(bytes);
						atlasTexture.name = "AlternateAtlas";
						AlternateActionBarAtlas = atlasTexture;
						icon.Component<UISprite>().atlas.spriteMaterial.mainTexture = atlasTexture; //attack icon

					} else {
						Console.AddMessage("Couldn't read file at path: " + alternateAtlas, Color.red);
					}
				} else {
					icon.Component<UISprite>().atlas.spriteMaterial.mainTexture = AlternateActionBarAtlas;
				}
			} else {
				icon.Component<UISprite>().atlas.spriteMaterial.mainTexture = DefaultActionBarAtlas;
			}
		}

		public static void DumpOptionsManager() {
			var printer = new UnityPrinter() {
				ComponentFilter = x => false,
			};
			IEDebug.Log(printer.Print(_uiCamera));
		}

		public static IEnumerable<GameObject> GetAllPortraits() {
			return from child in _partyPortraitBar.Children()
				where child.name.Contains("PartyPortrait")
				select child;
		}

		public  static void SetPortraitHighlight(bool? state = null)
		{
			foreach (var portrait in GetAllPortraits()) {
				// "StupidPanelBack"... someone was working overtime :)
				var isActive = !portrait.Child("StupidPanelBack").gameObject.activeSelf;
				portrait.Child("StupidPanelBack").gameObject.SetActive(state ?? isActive);
			}
		}

		private static void UseNoFrame()
		{
			_bigMapTooltip.Component<UIAnchor>().pixelOffset = new Vector2(25f, -25f);

			_hudTrim.Child("Right").gameObject.SetActive(true);

			var hudTrimLeft = _hudTrim.Child("Left");
			if (DefaultLeftCornerTexture != null)
				hudTrimLeft.Component<UITexture>().mainTexture = DefaultLeftCornerTexture;

			hudTrimLeft.Children().ToList().ForEach(x => x.SetActive(false));

			if (hudTrimLeft.GetComponent<UIResolutionScaler>() != null) {
				Object.Destroy(hudTrimLeft.Component<UIResolutionScaler>());
			}
				
			hudTrimLeft.transform.localScale = new Vector3(248f, 160f, 1);
		}
		 

		[NewMember]
		private static void UseUframe(string uframePath)
		{
			if (string.IsNullOrEmpty(uframePath)) {
				UseNoFrame();
				return;
			}
			if (PlayerPrefs.GetInt("DisableTooltipOffset", 0) != 1)
				_bigMapTooltip.Component<UIAnchor>().pixelOffset = new Vector2(150f, -25f); // enemy tooltip in the left upper corner

			string textpath = Path.ChangeExtension(uframePath, ".txt");

			float leftBarWidthPixels = 0f;
			float bottomBarHeightPixels = 0f;
			float rightBarPixels = 0f;

			float leftBarWidth = 0f;
			float bottomBarHeight = 0f;
			float rightBarWidth = 0f;

			if (File.Exists(textpath))
			{
				string[] threeLines = File.ReadAllLines(textpath);
				leftBarWidthPixels = (float)int.Parse(threeLines[0]);
				bottomBarHeightPixels = (float)int.Parse(threeLines[1]);
				rightBarPixels = (float)int.Parse(threeLines[2]);

				leftBarWidth = (leftBarWidthPixels * 2) / Screen.width;
				bottomBarHeight = (bottomBarHeightPixels * 2) / Screen.height;
				rightBarWidth = (rightBarPixels * 2) / Screen.width;
			}
			else
			{
				global::Console.AddMessage("Couldn't read file at path: " + textpath, Color.red);
			}

			if (File.Exists(uframePath))
			{
				var SolidUFrame = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
				byte[] bytes = File.ReadAllBytes(uframePath);
				SolidUFrame.LoadImage(bytes);
				SolidUFrame.name = uframePath;
				//Console.AddMessage ("Texture loaded: "+uframeName);


				// Displaying U-frame
				_hudTrim.Child("Right").gameObject.SetActive(false);

				// we load it only once
				var hudTrimLeft = _hudTrim.Child("Left");
				if (DefaultLeftCornerTexture == null) {
					DefaultLeftCornerTexture = hudTrimLeft.Component<UITexture>().mainTexture;
				}
					
				hudTrimLeft.Component<UITexture>().mainTexture = SolidUFrame;
				float width = Screen.width;
				float height = Screen.height;
				hudTrimLeft.transform.localScale = new Vector3(width, height, 1f);

				var scaler = _hudTrim.gameObject.GetComponent<UIResolutionScaler>()
					?? _hudTrim.gameObject.AddComponent<UIResolutionScaler>();
				scaler.DesignedWidth = (int)width;
				scaler.DesignedHeight = (int)height;
				scaler.UseMaximumScale = true;
				scaler.Apply();
				hudTrimLeft.Component<UITexture>().MakePixelPerfect();
				// end of U-frame

				// destroying the 3 colliders?

				hudTrimLeft.Children().ToList().ForEach(Object.Destroy);

				// if we hadn't previously created 3 colliders, we create them, otherwise we just activate them

				// no-click collider for the left bar
				UITexture leftBarTexture = NGUITools.AddWidget<UITexture>(hudTrimLeft.gameObject);
				GameObject leftbar = leftBarTexture.gameObject;
				leftbar.transform.localScale = new Vector3(leftBarWidth, 2f, 1f); // i seriously have no idea why those values have to be used, i just guessed them after a few hours of trying... you'd think that you need to use (126, 1080, 1), but apperently not... WHY???
				leftBarTexture.mainTexture = new Texture2D((int)leftBarWidthPixels, Screen.height);
				BoxCollider box = NGUITools.AddWidgetCollider(leftbar.gameObject); // adding a box collider, it's required for the UINoClick component
				box.gameObject.AddComponent<UINoClick>(); // this prevents clicks from going through the U-frame
				UIAnchor ank = leftbar.gameObject.AddComponent<UIAnchor>();
				ank.side = UIAnchor.Side.BottomLeft;
				leftBarTexture.depth = 1;
				// end of left bar collider

				// no click collider for the right bar
				UITexture rightBarTexture = NGUITools.AddWidget<UITexture>(hudTrimLeft.gameObject);
				GameObject righttbar = rightBarTexture.gameObject;
				righttbar.transform.localScale = new Vector3(rightBarWidth, 2f, 1f); // i seriously have no idea why those values have to be used, i just guessed them after a few hours of trying... you'd think that you need to use (126, 1080, 1), but apperently not... WHY???
				rightBarTexture.mainTexture = new Texture2D((int)rightBarPixels, Screen.height);
				BoxCollider boxRight = NGUITools.AddWidgetCollider(righttbar.gameObject); // adding a box collider, it's required for the UINoClick component
				boxRight.gameObject.AddComponent<UINoClick>(); // this prevents clicks from going through the U-frame
				UIAnchor ankRight = righttbar.gameObject.AddComponent<UIAnchor>();
				ankRight.side = UIAnchor.Side.BottomRight;
				rightBarTexture.depth = 1;
				// end of right bar collider

				// no click collider for the bottom
				UITexture bottomBarTexture = NGUITools.AddWidget<UITexture>(hudTrimLeft.gameObject);
				GameObject bottombar = bottomBarTexture.gameObject;
				bottombar.transform.localScale = new Vector3(1f, bottomBarHeight, 1f); // i seriously have no idea why those values have to be used, i just guessed them after a few hours of trying... you'd think that you need to use (126, 1080, 1), but apperently not... WHY???
				bottomBarTexture.mainTexture = new Texture2D(Screen.width, (int)bottomBarHeightPixels);
				BoxCollider boxBottom = NGUITools.AddWidgetCollider(bottombar.gameObject); // adding a box collider, it's required for the UINoClick component
				boxBottom.gameObject.AddComponent<UINoClick>(); // this prevents clicks from going through the U-frame
				UIAnchor ankBottom = bottombar.gameObject.AddComponent<UIAnchor>();
				ankBottom.side = UIAnchor.Side.Bottom;
				bottomBarTexture.depth = 1;
				// end of bottom bar collider

			}
			else global::Console.AddMessage("Couldn't read file at path: " + uframePath, Color.red);
		}

		public  static void DisableTooltipOffset(bool state)
		{
			if (state)
			{
				_bigMapTooltip.Component<UIAnchor>().pixelOffset = new Vector2(25f, -25f);
				PlayerPrefs.SetInt("DisableTooltipOffset", 1);
			}
			else
			{
				PlayerPrefs.SetInt("DisableTooltipOffset", 0);
				if (PlayerPrefs.GetString("Uframe", "FRM_botCornerLft") != "FRM_botCornerLft")
					_bigMapTooltip.Component<UIAnchor>().pixelOffset = new Vector2(150f, -25f);
			}
		}


		private static void SetButtonsBackgroundActive(bool? isInactive = null)
		{		
			var btnsLeftBgs =
				from button in _buttonsLeft.Children().Concat(_buttonsRight.Children())
				where button.HasChild("Background")
				select button.Child("Background");

			foreach (var bg in btnsLeftBgs) {
				bg.SetActive(isInactive ?? !bg.activeSelf);
			}
		}

		private static void SetCustomTexturesActive(bool? isActive = null) {
			ReplaceAtlas(isActive ?? _01attack.Child("Icon").Component<UISprite>().atlas.spriteMaterial.mainTexture == DefaultActionBarAtlas);
		}

		private static void SetLogButtonsAlignment(bool left) // toggles log buttons position between topright and topleft
		{
			var tabCombat = _radioGroup.Child("TabCombat");
			var tabDialog = _radioGroup.Child("TabDialog");
			var radioGroupAnchor = _radioGroup.Component<UIAnchor>();

			radioGroupAnchor.side = left ? UIAnchor.Side.TopLeft : UIAnchor.Side.TopRight;
			tabCombat.transform.localPosition = new Vector3(left ? 144 : 0, 0, 0);
			tabDialog.transform.localPosition = new Vector3(left ? 200 : -56, 0, 0);
		}


		private static void SetHudOrientation(bool horizontal) {
			var buttonsLeft_grid = _buttonsLeft.Component<UIGrid>();
			buttonsLeft_grid.arrangement = horizontal ? UIGrid.Arrangement.Horizontal : UIGrid.Arrangement.Vertical;
			buttonsLeft_grid.Reposition();

			//rather amusingly, the right hud already has orientation by default... it just looks horizontal. :)
			var buttonsRight_grid = _buttonsRight.Component<UIGrid>();
			buttonsRight_grid.arrangement = horizontal ? UIGrid.Arrangement.Vertical : UIGrid.Arrangement.Horizontal;
			buttonsRight_grid.Reposition();
		}

		/// <summary>
		/// Null means toggle it.
		/// </summary>
		/// <param name="isTopLeft"></param>
		private static void SetBuffSide(bool? isTopLeft = null)
		{
			foreach (var portrait in GetAllPortraits()) {
				var statusEffects = portrait.Child("StatusEffects");
				var uiAnchor = statusEffects.Component<UIAnchor>();
				var isTopLeftReal = isTopLeft ?? uiAnchor.side == UIAnchor.Side.TopLeft;
				uiAnchor.side = isTopLeftReal ? UIAnchor.Side.TopRight : UIAnchor.Side.TopLeft;
				uiAnchor.pixelOffset = new Vector2(isTopLeftReal ? 3f : -27f, 0f); // default is (3,0)
				statusEffects.Component<UIGrid>().Reposition();
			}
		}


		private static void SetDraggableUiActive(bool state)
		{
			DragPartyBar.SetActive(state);
			DragHugBgr.SetActive(state);
			DragLog.SetActive(state);
			DragAbilitiesBar.SetActive(state);
			ToggleLogButtons.SetActive(state);
			ToggleHudOrientation.SetActive(state);
			DragHudPanelLeft.SetActive(state);
			DragHudPanelRight.SetActive(state);
			DragClock.SetActive(state);
			ToggleHudBgr.SetActive(state);
			TogglePartyOrientation.SetActive(state);
			ToggleBuffsSide.SetActive(state);
			DragFormationBar.SetActive(state);
			FrameDropdown.SetActive(state);
			ToggleButtonsBgr.SetActive(state);
			TogglePortraitHighlights.SetActive(state);
			ToggleCustomTextures.SetActive(state);
			SaveBtn.SetActive(state);
			CancelBtn.SetActive(state);
			UseDefaultUIBtn.SetActive(state);
		}


		private static void SaveLayout(IEModOptions.LayoutOptions layout) {
			layout.CustomizeButtonPosition = _customizeButton.transform.localPosition;
			layout.FormationPosition = _formationButtonSet.transform.localPosition;
			layout.BuffsSideLeft = GetAllPortraits().First().Child("StatusEffects").Component<UIAnchor>().side
				!= UIAnchor.Side.TopRight;
			layout.PartyBarPosition = _partyPortraitBar.transform.localPosition;
			layout.PartySolidHudPosition = _partySolidHud.transform.localPosition;
			layout.LogButtonsLeft = _radioGroup.Component<UIAnchor>().side != UIAnchor.Side.TopRight;
			layout.HudPosition = _actionBarTrimB.transform.localPosition;
			layout.AbilitiesBarPosition = _abilitiesBar.Child("ButtonSet").transform.localPosition;
			layout.LeftHudBarPosition = _buttonsLeft.transform.localPosition;
			layout.RightHudBarPosition = _buttonsRight.transform.localPosition;
			layout.ClockPosition = _timeWidget.transform.localPosition;
			layout.AbilitiesHorizontal = _buttonsLeft.Component<UIGrid>().arrangement != UIGrid.Arrangement.Vertical;
			layout.UsingCustomTextures = _01attack.Child("Icon").Component<UISprite>().atlas.spriteMaterial.mainTexture
				!= DefaultActionBarAtlas;
			layout.PortraitHighlightsDisabled = !GetAllPortraits().First().Child("StupidPanelBack").activeSelf;
			layout.ButtonsBackground = _buttonsLeft.ChildPath("#0/Background").activeSelf;
			layout.HudTextureHidden = !_actionBarTrimB.activeSelf;
			layout.LogPosition = _consoleWindow.transform.localPosition;
			layout.PartyBarHorizontal = mod_UIPartyPortrait.IsHorizontal;
			layout.FramePath = SelectedFrame;
		}

		private static void SaveUi(IEModOptions.LayoutOptions saveToLayout)
		{
			saveToLayout.UseCustomUI = true;
			SaveLayout(saveToLayout);
			Console.AddMessage("UI Layout Saved.", Color.green);
			SetDraggableUiActive(false);
		}

		static string GetFrameNameFromPath(string path) {
			var name = Path.GetFileNameWithoutExtension(path);
			//remove resolution
			var noResolution = Regex.Replace(name, @"\d+x\d+$", "");
			//turn dashes into spaces, make Title Case, and join with a space separator.
			return noResolution.Split('-').Select(x => x.SentenceCase()).Join(" ");
		}


		internal static void LoadLayout(IEModOptions.LayoutOptions newLayout) {
			Initialize();
			
			var buffsChanged = newLayout.BuffsSideLeft;
			foreach (var portrait in GetAllPortraits()) {
				var statusEffects = portrait.Child("StatusEffects");
				var uiAnchor = statusEffects.Component<UIAnchor>();
				uiAnchor.side = buffsChanged ? UIAnchor.Side.TopLeft : UIAnchor.Side.TopRight;
				uiAnchor.pixelOffset = new Vector2(buffsChanged ? -27f : 3f, 0f); // default is (3,0)
				statusEffects.Component<UIGrid>().Reposition();
			}

			_formationButtonSet.transform.localPosition = newLayout.FormationPosition;
			_partyPortraitBar.transform.localPosition = newLayout.PartyBarPosition;
			_partySolidHud.transform.localPosition = newLayout.PartySolidHudPosition;
			_customizeButton.transform.localPosition = newLayout.CustomizeButtonPosition;
			if (newLayout.BuffsSideLeft != (_radioGroup.Component<UIAnchor>().side == UIAnchor.Side.TopRight))
			{
				GameObject blop = new GameObject();
				SetLogButtonsAlignment(blop);
			}
			_actionBar.Child("trimB").transform.localPosition = newLayout.HudPosition;
			_abilitiesBar.Child("ButtonSet").transform.localPosition = newLayout.AbilitiesBarPosition;
			_consoleWindow.transform.localPosition = newLayout.LogPosition;

			SetLogButtonsAlignment(newLayout.LogButtonsLeft);
			_buttonsLeft.transform.localPosition = newLayout.LeftHudBarPosition;
			_buttonsRight.transform.localPosition = newLayout.RightHudBarPosition;
			_timeWidget.transform.localPosition = newLayout.ClockPosition;

			var leftUiGrid = _buttonsLeft.Component<UIGrid>();
			leftUiGrid.arrangement = newLayout.AbilitiesHorizontal ? UIGrid.Arrangement.Horizontal : UIGrid.Arrangement.Vertical;
			leftUiGrid.Reposition();

			var rightUiGrid = _buttonsRight.Component<UIGrid>();
			rightUiGrid.arrangement = newLayout.AbilitiesHorizontal ? UIGrid.Arrangement.Vertical : UIGrid.Arrangement.Horizontal;
			rightUiGrid.Reposition();

			_actionBarTrimB.gameObject.SetActive(!newLayout.HudTextureHidden);

			mod_UIPartyPortrait.IsHorizontal = newLayout.PartyBarHorizontal;

			if (_buttonsLeft.ChildPath("#0/Background").gameObject.activeSelf != newLayout.ButtonsBackground)
				SetButtonsBackgroundActive();

			ReplaceAtlas(newLayout.UsingCustomTextures);

			SetPortraitHighlight(!newLayout.PortraitHighlightsDisabled);

			SelectedFrame = newLayout.FramePath;
			UseUframe(SelectedFrame);
			if (FrameDropdown != null) {
				FrameDropdown.ComponentInDescendants<UIDropdownMenu>().SetSelectedValue(newLayout.FramePath);
			}
		}

		public static bool IsInterfaceVisible {
			get {
				return IsInterfaceCreated && DragPartyBar.activeSelf;
			}
		}


		public static void ShowInterface() {
			Initialize();
			if (IsInterfaceCreated) {
				SetDraggableUiActive(true);
				return;
			}
			DragPartyBar = new GameObject {
				name = "DragPartyBar"
			};
			DragPartyBar.transform.parent = UIPartyPortraitBar.Instance.transform.parent;
			DragPartyBar.transform.localScale = new Vector3(1f, 1f, 1f);
			UIMultiSpriteImageButton dpbImageButton = NGUITools.AddChild(DragPartyBar.gameObject, UIOptionsManager.Instance.PageButtonPrefab.gameObject).Component<UIMultiSpriteImageButton>();
			IEDebug.Log(UnityPrinter.HierarchyPrinter.Print(dpbImageButton));
			if (dpbImageButton.transform.childCount == 5) // sometimes the pagebuttonprefab seems to already have a collider, sometimes not. if it has one, we destroy it and just always manually create one.
				Object.DestroyImmediate(DragPartyBar.transform.GetChild(0).GetChild(4).gameObject);

			GameObject bx = new GameObject("Collider");
			bx.transform.parent = dpbImageButton.transform;
			bx.transform.localScale = new Vector3(269f, 56f, 1f);
			bx.transform.localPosition = new Vector3(0f, 0, -2f);
			bx.layer = 14;
			//without a BoxCollider + UINoClick, clicks will go through the control.
			bx.AddComponent<BoxCollider>().size = new Vector3(1, 1, 1);
			bx.AddComponent<UINoClick>().BlockClicking = true;
			bx.AddComponent<UIEventListener>();
			//bx.AddComponent<UIDragObject>().target = UIPartyPortraitBar.Instance.transform;
			dpbImageButton.Label.Component<GUIStringLabel>().FormatString = "Drag Party Bar";
			// this button will have a second component UIDragObject to drag the party solid background, but since it would also be duplicated on other buttons, we'll add this component at the very end of the method

			var controlFactory = new IEControlFactory() {
				CurrentParent = DragPartyBar.transform.parent,
				ExampleButton = DragPartyBar,
				ExampleDropdown = UIOptionsManager.Instance.ResolutionDropdown.transform.parent.Component<UIOptionsTag>()
			};
			
			var initialPos = DragPartyBar.transform.localPosition;
			Action<GameObject, GameObject> addDragObject = (o,target) => {
				var collider = o.Descendant("Collider");
				collider.AddComponent<UIDragObject>().target = target.transform;
			};
			Action<GameObject, UIEventListener.VoidDelegate> addOnClick = (o, handler) => {
				o.ComponentInDescendants<UIMultiSpriteImageButton>().onClick += handler;
			};

			var choices =
				Directory.GetFiles(@"PillarsOfEternity_Data\Managed\iemod\frames", string.Format("*{0}x{1}.png", Screen.width, Screen.height)).Select(
					path => new IEDropdownChoice(path, GetFrameNameFromPath(path))).ToList();

			choices.Insert(0, new IEDropdownChoice("", "(none)"));

			FrameDropdown = controlFactory.Dropdown(() => SelectedFrame, choices, 300, 150, label:"Custom Frame: ");
			var dif = new Vector3(392.6f, 622.7f, -6.0f) - initialPos;
			FrameDropdown.layer = DragPartyBar.layer;
			FrameDropdown.transform.localPosition = initialPos + dif;
			//this makes the combobox block clicks
			FrameDropdown.Descendant("Background").AddComponent<UINoClick>().BlockClicking = true;
			//this max the BG between the dropdown options block clicks
			FrameDropdown.Descendant("BackgroundDropdown").AddComponent<UINoClick>().BlockClicking = true;

			//this makes the options themselves block clicks. There *has* to be a better way to do this. I just don't know what it is.
			FrameDropdown.ComponentsInDescendants<UILabel>().ToList().ForEach(x => {
				if (!x.gameObject.HasComponent<BoxCollider>()) {
					x.gameObject.AddComponent<BoxCollider>().size = Vector3.one;
				}
				x.gameObject.AddComponent<UINoClick>().BlockClicking = true;
			});

			FrameDropdown.ComponentInDescendants<UIDropdownMenu>().OnDropdownOptionChangedEvent += newValue => {
				var choice = (IEDropdownChoice) newValue;
				UseUframe((string) choice.Value);
			};

			DragHugBgr = controlFactory.Button("Drag Hud Bgr", localPos:initialPos.Plus(y:50f));
			addDragObject(DragHugBgr, _actionBarTrimB);

			DragLog = controlFactory.Button("Drag Log", localPos:initialPos.Plus(y:100f));
			addDragObject(DragLog, _consoleWindow);

			DragAbilitiesBar = controlFactory.Button("Drag Abilities", localPos:initialPos.Plus(y:-50f));
			addDragObject(DragAbilitiesBar, _abilitiesBar);

			ToggleLogButtons = controlFactory.Button("T. Log Buttons Side", localPos:initialPos.Plus(x:400f, y:-50f));
			addOnClick(ToggleLogButtons, _ => SetLogButtonsAlignment(_radioGroup.Component<UIAnchor>().side != UIAnchor.Side.TopLeft));

			ToggleHudOrientation = controlFactory.Button("T. Hud Horiz/Vert", localPos:initialPos.Plus(x:400f));
			addOnClick(ToggleHudOrientation, _ => SetHudOrientation(_buttonsLeft.Component<UIGrid>().arrangement == UIGrid.Arrangement.Vertical));

			DragHudPanelLeft = controlFactory.Button("Drag Hud Panel 1", localPos:initialPos.Plus(y:-150f));
			addDragObject(DragHudPanelLeft, _buttonsLeft);

			DragHudPanelRight = controlFactory.Button("Drag Hud Panel 2", localPos:initialPos.Plus(y:-200f));
			addDragObject(DragHudPanelRight, _buttonsRight);

			DragClock = controlFactory.Button("Drag Clock", localPos:initialPos.Plus(y:-100f));
			addDragObject(DragClock, _timeWidget);

			ToggleHudBgr = controlFactory.Button("T. Hud Background", localPos:initialPos.Plus(x:400f, y:50f));
			addOnClick(ToggleHudBgr, _ => _actionBarTrimB.SetActive(!_actionBarTrimB.gameObject.activeSelf));

			TogglePartyOrientation = controlFactory.Button("T. Party Horiz/Vert", localPos:initialPos.Plus(x:400f, y:100f));
			addOnClick(TogglePartyOrientation, _ => mod_UIPartyPortrait.IsHorizontal = !mod_UIPartyPortrait.IsHorizontal);

			ToggleBuffsSide = controlFactory.Button("Toggle Buffs Side", localPos:initialPos.Plus(x:400f, y:150f));
			addOnClick(ToggleBuffsSide, x => SetBuffSide());

			DragFormationBar = controlFactory.Button("Drag Formation Bar", localPos:initialPos.Plus(y:-250f));
			addDragObject(DragFormationBar, _formationButtonSet);

			ToggleButtonsBgr = controlFactory.Button("T. Buttons Background", localPos: initialPos.Plus(x: 400f, y: -100f));
			addOnClick(ToggleButtonsBgr, x => SetButtonsBackgroundActive());

			TogglePortraitHighlights = controlFactory.Button("T. Portrait Highlight", localPos: initialPos.Plus(400f, -150f));
			addOnClick(TogglePortraitHighlights, x => SetPortraitHighlight());

			ToggleCustomTextures = controlFactory.Button("T. Custom Textures", localPos: initialPos.Plus(400f, -200f));
			addOnClick(ToggleCustomTextures, x => SetCustomTexturesActive());

			SaveBtn = controlFactory.Button("Save UI & Close", localPos:initialPos.Plus(x:-140f, y:250f));
			addOnClick(SaveBtn, x => {
				SaveUi(IEModOptions.Layout);
				IEDebug.Log(FrameDropdown.transform.localPosition);
			});

			CancelBtn = controlFactory.Button("Reset to Prev", localPos:initialPos.Plus(x:140f, y:250f));
			addOnClick(CancelBtn, x => {
				LoadLayout(IEModOptions.Layout);
			});

			UseDefaultUIBtn = controlFactory.Button("Reset to Default", localPos:initialPos.Plus(400f, 350f));
			addOnClick(UseDefaultUIBtn, x => {
				LoadLayout(DefaultLayout);
			});

			// adding the second drag component to the DragPartyBar button that will drag the partybar solid background
			addDragObject(DragPartyBar, UIPartyPortraitBar.Instance.gameObject);
			addDragObject(DragPartyBar, _partySolidHud);
			IsInterfaceCreated = true;
		}
	}
}
