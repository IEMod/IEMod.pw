using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using IEMod.Helpers;
using IEMod.Mods.Options;
using IEMod.Mods.PartyBar;
using IEMod.QuickControls;
using IEMod.QuickControls.Behaviors;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IEMod.Mods.UICustomization {
	[NewType]
	public static class UICustomizer {

		public static readonly IBindingValue<string> SelectedFrame =
			BindingValue.Variable("", nameof(SelectedFrame)).OnChange(OnFrameChanged);

		public static readonly IBindingValue<bool> TooltipOffset =
			BindingValue.Variable(false, nameof(TooltipOffset)).OnChange(TooltipOffsetChanged);

		private static Texture DefaultActionBarAtlas;
		private static Texture2D AlternateActionBarAtlas;
		private static Texture DefaultLeftCornerTexture;
		public static IEModOptions.LayoutOptions DefaultLayout;
		private static QuickButton _customizeButton;
		public static void Initialize() {
			
			//Note that the game will destroy the UI when you go to the main menu, so we'll have to rebuild it.
			//The best way to check if we need to initialize everything seem to be the following, though it's strange messy.
			if (IsInitialized) { return; }

			//This is the 'Customize UI' button that lets you customize the UI.
			_customizeButton = new QuickButton(UIPartyPortraitBar.Instance.transform.parent) {
				Caption = "Customize UI",
				Name = "CustomizeUI",
				LocalPosition = new Vector3(903.5f, 76.2f, 0f),
				LocalScale = new Vector3(0.7f, 0.7f, 1f)
			};

			_customizeButton.Click += x => {
				SaveLayout(IEModOptions.Layout);
				ShowInterface(!IsInterfaceVisible);
			};

			//these may break in future version changes, but I doubt they'll break much.
			//since changing them will probably break some of the developer's code as well
			//to fix them, dump the UICamera hierarchy to file and see which names changed.
			//using Child(string) also tells you which names are broken because it throws an exception if it can't find something,
			//instead of silently returning null and letting you figure out where the problem came from.
			if (DefaultLayout == null) {
				DefaultLayout = new IEModOptions.LayoutOptions();
				SaveLayout(DefaultLayout);
			}

			DefaultActionBarAtlas =
				Attack.Child(0).Component<UISprite>().atlas.spriteMaterial.mainTexture;

			// turning off BB-version label in the upper right corner, cause it's annoying when you want to move portraits there
			UiCamera.Child("BBVersion").gameObject.SetActive(false);

			// UIAnchors on the ActionBarWindow that prevent it from being moved... (it's related to the partybar somehow?)
			// HUD -> Bottom -> ActionBarWindow -> destroy 3 UIAnchors
			foreach (var comp in ActionBarWindow.Components<UIAnchor>()) {
				GameUtilities.DestroyComponent(comp);
			}

			var component = ConsoleWindow.Component<UIAnchor>();
			if (component) {
				GameUtilities.DestroyComponent(component);
			}

			// disable the minimize buttons for the log and the actionbar
			Bottom.Child("ConsoleMinimize").SetActive(false);
			Bottom.Child("ActionBarMinimize").gameObject.SetActive(false);

			// this UIPanel used to hide the clock when it was moved from far away from its original position
			// HUD -> Bottom -> ActionBarWindow -> ActionBarExpandedAnchor -> UIPanel
			ActionBarWindow.Component<UIPanel>().clipping = UIDrawCall.Clipping.None;
			
			// detaches the "GAME PAUSED" and "SLOW MO" from the Clock panel, to which it was attached for some reason...
			var gamePausedAnchors = UiCamera.Child("GamePaused").Components<UIAnchor>();
			gamePausedAnchors[0].widgetContainer = Hud.Component<UIPanel>().widgets[0];
			gamePausedAnchors[1].DisableY = true;
			var slowMoAnchors = UiCamera.Child("GameSpeed").Components<UIAnchor>();
			slowMoAnchors[0].widgetContainer = Hud.Component<UIPanel>().widgets[0];
			slowMoAnchors[1].DisableY = true;
			PartyBar.AddChild(new GameObject("IsInitialized"));
		}

		public static bool IsInitialized {
			get {

				return PartyBar.HasChild("IsInitialized");
			}
		}

		public static void DumpOptionsManager() {
			var printer = new UnityPrinter {
				ComponentFilter = x => false
			};
			printer.Print(UiCamera);
		}

		public static IEnumerable<GameObject> GetAllPortraits() {
			return from child in PartyPortraitBar.Children()
				where child.name.Contains("PartyPortrait")
				select child;
		}

		public static void SetPortraitHighlight(bool? state = null) {
			foreach (var portrait in GetAllPortraits()) {
				// "StupidPanelBack"... someone was working overtime :)
				var isActive = !portrait.Child("StupidPanelBack").gameObject.activeSelf;
				portrait.Child("StupidPanelBack").gameObject.SetActive(state ?? isActive);
			}
		}

		private static void TooltipOffsetChanged(IBindingValue<bool> source) {
			BigMapTooltip.Component<UIAnchor>().pixelOffset = source.Value ? new Vector2(150f, -25f) : new Vector2(25f, -25f);
		}

		public static bool IsInterfaceVisible {
			get {
				return DragPartyBar.IsAlive() && DragPartyBar.ActiveSelf;
			}
		}

		public static GameObject UiCamera {
			get {
				return UIOptionsManager.Instance.transform.parent.gameObject;
			}
		}

		public static GameObject Hud {
			get {
				return UiCamera.Child("HUD");
			}
		}

		public static GameObject Bottom {
			get {
				return Hud.Child("Bottom");
			}
		}

		public static GameObject PartyBarWindow {
			get {
				return Bottom.Child("PartyBarWindow");
			}
		}

		public static GameObject PartyBar {
			get {
				return PartyBarWindow.Child("PartyBar");
			}
		}

		public static GameObject PartyPortraitBar {
			get {
				return PartyBar.Child("PartyPortraitBar");
			}
		}

		public static GameObject ActionBarWindow {
			get {
				return Bottom.Child("ActionBarWindow");
			}
		}

		public static GameObject ActionBar {
			get {
				return ActionBarWindow.Child("ActionBar");
			}
		}

		public static GameObject ButtonsLeft {
			get {
				return ActionBar.Child("ButtonsLeft");
			}
		}

		public static GameObject Attack {
			get {
				return ButtonsLeft.Child("01.Attack");
			}
		}

		public static GameObject ButtonsRight {
			get {
				return ActionBar.Child("ButtonsRight");
			}
		}

		public static GameObject BigMapTooltip {
			get {
				return UiCamera.ChildPath("Overlay/MapTooltips/BigTooltip");
			}
		}

		public static GameObject ConsoleWindow {
			get {
				return Bottom.Child("ConsoleWindow");
			}
		}

		public static GameObject Console1 {
			get {
				return ConsoleWindow.Child("Console");
			}
		}

		public static GameObject RadioGroup {
			get {
				return Console1.Child("RadioGroup");
			}
		}

		public static GameObject FormationButtonSet {
			get {
				return ActionBar.Child("FormationButtonSet");
			}
		}

		public static GameObject PartySolidHud {
			get {
				return PartyBarWindow.Child("SolidHud");
			}
		}

		public static GameObject AbilitiesBar {
			get {
				return PartyBarWindow.Child("AbilitiesBar");
			}
		}

		public static GameObject TimeWidget {
			get {
				return ActionBar.Child("TimeWidget");
			}
		}

		public static GameObject ActionBarTrimB {
			get {
				return ActionBar.Child("trimB");
			}
		}

		public static GameObject HudTrim {
			get {
				return Hud.Child("Trim");
			}
		}

		private static void CreateInterface() {
		
			var quickFactory = new QuickFactory(UIPartyPortraitBar.Instance.transform.parent);
			Action<QuickButton, GameObject> addDragObject = (o, target) => {
				var collider = o.Collider;
				collider.AddComponent<UIDragObject>().target = target.transform;
			};

			var initialPos = new Vector3(905, 487, -6);
			DragPartyBar = quickFactory.Button("Drag Party Bar", localPos: initialPos);

			addDragObject(DragPartyBar, UIPartyPortraitBar.Instance.gameObject);
			addDragObject(DragPartyBar, PartySolidHud);

			var choices =
				Directory.GetFiles(@"PillarsOfEternity_Data\Managed\iemod\frames",
					string.Format("*{0}x{1}.png", Screen.width, Screen.height)).Select(
						path => new DropdownChoice<string>(GetFrameNameFromPath(path), path)).ToList();

			choices.Insert(0, new DropdownChoice<string>("(none)",""));

			var controlFactory = quickFactory;
			var layer = 14;
			FrameDropdown =  new QuickDropdown<string>(quickFactory.CurrentParent) {
				Layer = layer,
				LocalPosition = new Vector3(392.6f, 622.7f, -6.0f),
				LabelText = "Custom Frame:",
				LabelWidth = 150,
				Width = 300,
				Options = choices
			};
			FrameDropdown.SelectedValue.Bind(SelectedFrame);

			BlockClicking.Apply(FrameDropdown);

			DragHugBgr = controlFactory.Button("Drag Hud Bgr", localPos: initialPos.Plus(y: 50f));
			DragHugBgr.LocalPosition = initialPos.Plus(y: 50f);
			
			DragHugBgr.Layer = layer;
			addDragObject(DragHugBgr, ActionBarTrimB);

			DragLog = controlFactory.Button("Drag Log", localPos: initialPos.Plus(y: 100f));
			DragLog.Layer = layer;
			addDragObject(DragLog, ConsoleWindow);

			DragAbilitiesBar = controlFactory.Button("Drag Abilities", localPos: initialPos.Plus(y: -50f));
			DragAbilitiesBar.Layer = layer;
			addDragObject(DragAbilitiesBar, AbilitiesBar);

			ToggleLogButtons = controlFactory.Button("T. Log Buttons Side", localPos: initialPos.Plus(x:400f, y:-50f));
			ToggleLogButtons.Layer = layer;
			ToggleLogButtons.Click +=
				x => SetLogButtonsAlignment(RadioGroup.Component<UIAnchor>().side != UIAnchor.Side.TopLeft);

			ToggleHudOrientation = controlFactory.Button("T. Hud Horiz/Vert", localPos: initialPos.Plus(400f));
			ToggleHudOrientation.Layer = layer;
			ToggleHudOrientation.Click +=
				x => SetHudOrientation(ButtonsLeft.Component<UIGrid>().arrangement == UIGrid.Arrangement.Vertical);

			DragHudPanelLeft = controlFactory.Button("Drag Hud Panel 1", localPos: initialPos.Plus(y: -150f));
			DragHudPanelLeft.Layer = layer;
			addDragObject(DragHudPanelLeft, ButtonsLeft);

			DragHudPanelRight = controlFactory.Button("Drag Hud Panel 2", localPos: initialPos.Plus(y: -200f));
			DragHudPanelRight.Layer = layer;
			addDragObject(DragHudPanelRight, ButtonsRight);

			DragClock = controlFactory.Button("Drag Clock", localPos: initialPos.Plus(y: -100f));
			DragClock.Layer = layer;
			addDragObject(DragClock, TimeWidget);

			ToggleHudBgr = controlFactory.Button("T. Hud Background", localPos: initialPos.Plus(400f, 50f));
			ToggleHudBgr.Layer = layer;
			ToggleHudBgr.Click += x => ActionBarTrimB.SetActive(!ActionBarTrimB.gameObject.activeSelf);

			TooltipOffsetCheckbox = new QuickCheckbox("TooltipOffsetChbox", controlFactory.CurrentParent) {
				Label = "Tooltip Offset",
				Layer = layer,
				LocalPosition = FrameDropdown.LocalPosition.Plus(x:-110, y:-50)
			};
			
			//UnityPrinter.HierarchyPrinter.Print(TooltipOffsetCheckbox.GameObject);
			TooltipOffsetCheckbox.IsChecked.Bind(TooltipOffset);

			TogglePartyOrientation = controlFactory.Button("T. Party Horiz/Vert", localPos: initialPos.Plus(400f, 100f));
			TogglePartyOrientation.Layer = layer;
			TogglePartyOrientation.Click += _ => mod_UIPartyPortrait.IsVertical = !mod_UIPartyPortrait.IsVertical;

			ToggleBuffsSide = controlFactory.Button("Toggle Buffs Side", localPos: initialPos.Plus(400f, 150f));
			ToggleBuffsSide.Layer = layer;
			ToggleBuffsSide.Click += x => SetBuffSide();

			DragFormationBar = controlFactory.Button("Drag Formation Bar", localPos: initialPos.Plus(y: -250f));
			DragFormationBar.Layer = layer;
			addDragObject(DragFormationBar, FormationButtonSet);

			ToggleButtonsBgr = controlFactory.Button("T. Buttons Background", localPos: initialPos.Plus(400f, -100f));
			ToggleButtonsBgr.Click += x => SetButtonsBackgroundActive();
			ToggleButtonsBgr.Layer = layer;

			TogglePortraitHighlights = controlFactory.Button("T. Portrait Highlight", localPos: initialPos.Plus(400f, -150f));
			TogglePortraitHighlights.Click += x => SetPortraitHighlight();
			TogglePortraitHighlights.Layer = layer;

			ToggleCustomTextures = controlFactory.Button("T. Custom Textures", localPos: initialPos.Plus(400f, -200f));
			ToggleCustomTextures.Click += x => SetCustomTexturesActive();
			ToggleCustomTextures.Layer = layer;

			SaveBtn = controlFactory.Button("Save & Close", localPos: initialPos.Plus(-140f, 250f));
			SaveBtn.Layer = layer;
			SaveBtn.Click += x => {
				SaveLayout(IEModOptions.Layout);
				ShowInterface(false);
			};

			CancelBtn = controlFactory.Button("Reset to Prev", localPos: initialPos.Plus(140f, 250f));
			CancelBtn.Layer = layer;
			CancelBtn.Click += x => {
				LoadLayout(IEModOptions.Layout);
			};
			

			UseDefaultUIBtn = controlFactory.Button("Reset to Default", localPos: initialPos.Plus(400f, 350f));
			UseDefaultUIBtn.Layer = layer;
			UseDefaultUIBtn.Click += x => {
				LoadLayout(DefaultLayout);
			};
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
			FormationButtonSet.transform.localPosition = newLayout.FormationPosition;
			PartyPortraitBar.transform.localPosition = newLayout.PartyBarPosition;
			PartySolidHud.transform.localPosition = newLayout.PartySolidHudPosition;
			_customizeButton.LocalPosition = newLayout.CustomizeButtonPosition;
			if (newLayout.BuffsSideLeft != (RadioGroup.Component<UIAnchor>().side == UIAnchor.Side.TopRight)) {
				var blop = new GameObject();
				SetLogButtonsAlignment(blop);
			}
			ActionBar.Child("trimB").transform.localPosition = newLayout.HudPosition;
			AbilitiesBar.transform.localPosition = newLayout.AbilitiesBarPosition;
			ConsoleWindow.transform.localPosition = newLayout.LogPosition;

			SetLogButtonsAlignment(newLayout.LogButtonsLeft);
			ButtonsLeft.transform.localPosition = newLayout.LeftHudBarPosition;
			ButtonsRight.transform.localPosition = newLayout.RightHudBarPosition;
			TimeWidget.transform.localPosition = newLayout.ClockPosition;
			
			var leftUiGrid = ButtonsLeft.Component<UIGrid>();
			leftUiGrid.arrangement = newLayout.HudHorizontal ? UIGrid.Arrangement.Horizontal : UIGrid.Arrangement.Vertical;
			leftUiGrid.Reposition();

			var rightUiGrid = ButtonsRight.Component<UIGrid>();
			rightUiGrid.arrangement = newLayout.HudHorizontal ? UIGrid.Arrangement.Vertical : UIGrid.Arrangement.Horizontal;
			rightUiGrid.Reposition();

			ActionBarTrimB.gameObject.SetActive(!newLayout.HudTextureHidden);

			mod_UIPartyPortrait.IsVertical = newLayout.PartyBarHorizontal;

			if (ButtonsLeft.ChildPath("#0/Background").gameObject.activeSelf != newLayout.ButtonsBackground) {
				SetButtonsBackgroundActive();
			}

			ReplaceAtlas(newLayout.UsingCustomTextures);

			SetPortraitHighlight(!newLayout.PortraitHighlightsDisabled);

			SelectedFrame.Value = newLayout.FramePath;
			//not sure why, but the tooltip offset only updates correctly if we do this last.
			TooltipOffset.Value = newLayout.TooltipOffset;
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
			var icon = Attack.Child("Icon");
			if (state) {
				if (AlternateActionBarAtlas == null) {
					var alternateAtlas = Path.Combine(Path.Combine(Application.dataPath, "Managed/iemod/buttons"),
						"ActionBarAlternate.png");

					if (File.Exists(alternateAtlas)) {
						var atlasTexture = new Texture2D(512, 256, TextureFormat.RGB24, false);
						var bytes = File.ReadAllBytes(alternateAtlas);
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

		private static void UseNoFrame() {
			BigMapTooltip.Component<UIAnchor>().pixelOffset = new Vector2(25f, -25f);

			HudTrim.Child("Right").gameObject.SetActive(true);

			var hudTrimLeft = HudTrim.Child("Left");
			if (DefaultLeftCornerTexture != null) {
				hudTrimLeft.Component<UITexture>().mainTexture = DefaultLeftCornerTexture;
			}

			hudTrimLeft.Children().ToList().ForEach(x => x.SetActive(false));

			if (hudTrimLeft.GetComponent<UIResolutionScaler>() != null) {
				Object.Destroy(hudTrimLeft.Component<UIResolutionScaler>());
			}

			hudTrimLeft.transform.localScale = new Vector3(248f, 160f, 1);
		}

		[NewMember]
		private static void OnFrameChanged(IBindingValue<string> uframePaths) {
			var uframePath = uframePaths.Value;
			if (string.IsNullOrEmpty(uframePath)) {
				UseNoFrame();
				return;
			}
			if (!TooltipOffset.Value) {
				BigMapTooltip.Component<UIAnchor>().pixelOffset = new Vector2(150f, -25f);
					// enemy tooltip in the left upper corner
			}

			var textpath = Path.ChangeExtension(uframePath, ".txt");

			var leftBarWidthPixels = 0f;
			var bottomBarHeightPixels = 0f;
			var rightBarPixels = 0f;

			var leftBarWidth = 0f;
			var bottomBarHeight = 0f;
			var rightBarWidth = 0f;

			if (File.Exists(textpath)) {
				var threeLines = File.ReadAllLines(textpath);
				leftBarWidthPixels = int.Parse(threeLines[0]);
				bottomBarHeightPixels = int.Parse(threeLines[1]);
				rightBarPixels = int.Parse(threeLines[2]);

				leftBarWidth = (leftBarWidthPixels * 2) / Screen.width;
				bottomBarHeight = (bottomBarHeightPixels * 2) / Screen.height;
				rightBarWidth = (rightBarPixels * 2) / Screen.width;
			} else {
				Console.AddMessage("Couldn't read file at path: " + textpath, Color.red);
			}

			if (File.Exists(uframePath)) {
				var SolidUFrame = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
				var bytes = File.ReadAllBytes(uframePath);
				SolidUFrame.LoadImage(bytes);
				SolidUFrame.name = uframePath;
				//Console.AddMessage ("Texture loaded: "+uframeName);


				// Displaying U-frame
				HudTrim.Child("Right").gameObject.SetActive(false);

				// we load it only once
				var hudTrimLeft = HudTrim.Child("Left");
				if (DefaultLeftCornerTexture == null) {
					DefaultLeftCornerTexture = hudTrimLeft.Component<UITexture>().mainTexture;
				}

				hudTrimLeft.Component<UITexture>().mainTexture = SolidUFrame;
				float width = Screen.width;
				float height = Screen.height;
				hudTrimLeft.transform.localScale = new Vector3(width, height, 1f);

				var scaler = HudTrim.gameObject.GetComponent<UIResolutionScaler>()
					?? HudTrim.gameObject.AddComponent<UIResolutionScaler>();
				scaler.DesignedWidth = (int) width;
				scaler.DesignedHeight = (int) height;
				scaler.UseMaximumScale = true;
				scaler.Apply();
				hudTrimLeft.Component<UITexture>().MakePixelPerfect();
				// end of U-frame

				// destroying the 3 colliders?

				hudTrimLeft.Children().ToList().ForEach(Object.Destroy);

				// if we hadn't previously created 3 colliders, we create them, otherwise we just activate them

				// no-click collider for the left bar
				var leftBarTexture = NGUITools.AddWidget<UITexture>(hudTrimLeft.gameObject);
				var leftbar = leftBarTexture.gameObject;
				leftbar.transform.localScale = new Vector3(leftBarWidth, 2f, 1f);
					// i seriously have no idea why those values have to be used, i just guessed them after a few hours of trying... you'd think that you need to use (126, 1080, 1), but apperently not... WHY???
				leftBarTexture.mainTexture = new Texture2D((int) leftBarWidthPixels, Screen.height);
				var box = NGUITools.AddWidgetCollider(leftbar.gameObject);
					// adding a box collider, it's required for the UINoClick component
				box.gameObject.AddComponent<UINoClick>(); // this prevents clicks from going through the U-frame
				var ank = leftbar.gameObject.AddComponent<UIAnchor>();
				ank.side = UIAnchor.Side.BottomLeft;
				leftBarTexture.depth = 1;
				// end of left bar collider

				// no click collider for the right bar
				var rightBarTexture = NGUITools.AddWidget<UITexture>(hudTrimLeft.gameObject);
				var righttbar = rightBarTexture.gameObject;
				righttbar.transform.localScale = new Vector3(rightBarWidth, 2f, 1f);
					// i seriously have no idea why those values have to be used, i just guessed them after a few hours of trying... you'd think that you need to use (126, 1080, 1), but apperently not... WHY???
				rightBarTexture.mainTexture = new Texture2D((int) rightBarPixels, Screen.height);
				var boxRight = NGUITools.AddWidgetCollider(righttbar.gameObject);
					// adding a box collider, it's required for the UINoClick component
				boxRight.gameObject.AddComponent<UINoClick>(); // this prevents clicks from going through the U-frame
				var ankRight = righttbar.gameObject.AddComponent<UIAnchor>();
				ankRight.side = UIAnchor.Side.BottomRight;
				rightBarTexture.depth = 1;
				// end of right bar collider

				// no click collider for the bottom
				var bottomBarTexture = NGUITools.AddWidget<UITexture>(hudTrimLeft.gameObject);
				var bottombar = bottomBarTexture.gameObject;
				bottombar.transform.localScale = new Vector3(1f, bottomBarHeight, 1f);
					// i seriously have no idea why those values have to be used, i just guessed them after a few hours of trying... you'd think that you need to use (126, 1080, 1), but apperently not... WHY???
				bottomBarTexture.mainTexture = new Texture2D(Screen.width, (int) bottomBarHeightPixels);
				var boxBottom = NGUITools.AddWidgetCollider(bottombar.gameObject);
					// adding a box collider, it's required for the UINoClick component
				boxBottom.gameObject.AddComponent<UINoClick>(); // this prevents clicks from going through the U-frame
				var ankBottom = bottombar.gameObject.AddComponent<UIAnchor>();
				ankBottom.side = UIAnchor.Side.Bottom;
				bottomBarTexture.depth = 1;
				// end of bottom bar collider

			} else {
				Console.AddMessage("Couldn't read file at path: " + uframePath, Color.red);
			}
		}

		private static void SetButtonsBackgroundActive(bool? isInactive = null) {
			var btnsLeftBgs =
				from button in ButtonsLeft.Children().Concat(ButtonsRight.Children())
				where button.HasChild("Background")
				select button.Child("Background");

			foreach (var bg in btnsLeftBgs) {
				bg.SetActive(isInactive ?? !bg.activeSelf);
			}
		}

		private static void SetCustomTexturesActive(bool? isActive = null) {
			ReplaceAtlas(isActive
				?? Attack.Child("Icon").Component<UISprite>().atlas.spriteMaterial.mainTexture == DefaultActionBarAtlas);
		}

		private static void SetLogButtonsAlignment(bool left) // toggles log buttons position between topright and topleft
		{
			var tabCombat = RadioGroup.Child("TabCombat");
			var tabDialog = RadioGroup.Child("TabDialog");
			var radioGroupAnchor = RadioGroup.Component<UIAnchor>();

			radioGroupAnchor.side = left ? UIAnchor.Side.TopLeft : UIAnchor.Side.TopRight;
			tabCombat.transform.localPosition = new Vector3(left ? 144 : 0, 0, 0);
			tabDialog.transform.localPosition = new Vector3(left ? 200 : -56, 0, 0);
		}

		private static void SetHudOrientation(bool horizontal) {
			var buttonsLeft_grid = ButtonsLeft.Component<UIGrid>();
			buttonsLeft_grid.arrangement = horizontal ? UIGrid.Arrangement.Horizontal : UIGrid.Arrangement.Vertical;
			buttonsLeft_grid.Reposition();

			//rather amusingly, the right hud already has orientation by default... it just looks horizontal. :)
			var buttonsRight_grid = ButtonsRight.Component<UIGrid>();
			buttonsRight_grid.arrangement = horizontal ? UIGrid.Arrangement.Vertical : UIGrid.Arrangement.Horizontal;
			buttonsRight_grid.Reposition();
		}

		///     Null means togg
		/// <summary>le it.
		/// </summary>
		/// <param name="isTopLeft"></param>
		private static void SetBuffSide(bool? isTopLeft = null) {
			foreach (var portrait in GetAllPortraits()) {
				var statusEffects = portrait.Child("StatusEffects");
				var uiAnchor = statusEffects.Component<UIAnchor>();
				var isTopLeftReal = isTopLeft ?? uiAnchor.side == UIAnchor.Side.TopLeft;
				uiAnchor.side = isTopLeftReal ? UIAnchor.Side.TopRight : UIAnchor.Side.TopLeft;
				uiAnchor.pixelOffset = new Vector2(isTopLeftReal ? 3f : -27f, 0f); // default is (3,0)
				statusEffects.Component<UIGrid>().Reposition();
			}
		}

		public static bool IsInterfaceCreated {
			get {
				return DragPartyBar.IsAlive();
			}
		}

		public static void ShowInterface(bool state) {
			Initialize();
			if (IsInterfaceVisible == state) return;

			_customizeButton.Caption = state ? "Save & Close" : "Customize UI";
			//destroying a Component and then adding the same Component can crash the game.
			var collider = _customizeButton.Child("Collider");
			if (state) {
				if (_customizeButton.HasComponent<UIDragObject>()) {
					
					collider.SetBehaviors<UIDragObject>(true);
				} else {
					collider.AddComponent<UIDragObject>().target = _customizeButton.GameObject.transform;	
				}
			} else {
				collider.SetBehaviors<UIDragObject>(false);
			}
			if (state && !IsInterfaceCreated) {
				CreateInterface();
				return;
			}

			TooltipOffsetCheckbox.SetActive(state);
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
			layout.TooltipOffset = TooltipOffset.Value;
			layout.CustomizeButtonPosition = _customizeButton.LocalPosition;
			layout.FormationPosition = FormationButtonSet.transform.localPosition;
			layout.BuffsSideLeft = GetAllPortraits().First().Child("StatusEffects").Component<UIAnchor>().side
				!= UIAnchor.Side.TopRight;
			layout.PartyBarPosition = PartyPortraitBar.transform.localPosition;
			layout.PartySolidHudPosition = PartySolidHud.transform.localPosition;
			layout.LogButtonsLeft = RadioGroup.Component<UIAnchor>().side != UIAnchor.Side.TopRight;
			layout.HudPosition = ActionBarTrimB.transform.localPosition;
			layout.AbilitiesBarPosition = AbilitiesBar.transform.localPosition;
			layout.LeftHudBarPosition = ButtonsLeft.transform.localPosition;
			layout.RightHudBarPosition = ButtonsRight.transform.localPosition;
			layout.ClockPosition = TimeWidget.transform.localPosition;
			layout.HudHorizontal = ButtonsLeft.Component<UIGrid>().arrangement != UIGrid.Arrangement.Vertical;
			layout.UsingCustomTextures = Attack.Child("Icon").Component<UISprite>().atlas.spriteMaterial.mainTexture
				!= DefaultActionBarAtlas;
			layout.PortraitHighlightsDisabled = !GetAllPortraits().First().Child("StupidPanelBack").activeSelf;
			layout.ButtonsBackground = ButtonsLeft.ChildPath("#0/Background").activeSelf;
			layout.HudTextureHidden = !ActionBarTrimB.activeSelf;
			layout.LogPosition = ConsoleWindow.transform.localPosition;
			layout.PartyBarHorizontal = mod_UIPartyPortrait.IsVertical;
			layout.FramePath = SelectedFrame.Value;
		}

		private static string GetFrameNameFromPath(string path) {
			var name = Path.GetFileNameWithoutExtension(path);
			//remove resolution
			var noResolution = Regex.Replace(name, @"\d+x\d+$", "");
			//turn dashes into spaces, make Title Case, and join with a space separator.
			return noResolution.Split('-').Select(x => x.SentenceCase()).Join(" ");
		}

		#region Controls

		private static QuickButton DragPartyBar;


		private static QuickButton DragHugBgr;


		private static QuickButton DragLog;


		private static QuickButton DragAbilitiesBar;


		private static QuickButton ToggleLogButtons;


		private static QuickButton SaveBtn;


		private static QuickButton CancelBtn;


		private static QuickButton UseDefaultUIBtn;


		private static QuickButton ToggleHudOrientation;


		private static QuickButton DragHudPanelLeft;


		private static QuickButton DragHudPanelRight;


		private static QuickButton DragClock;


		private static QuickButton ToggleHudBgr;


		private static QuickButton TogglePartyOrientation;


		private static QuickButton ToggleBuffsSide;


		private static QuickButton DragFormationBar;


		private static QuickButton ToggleButtonsBgr;


		private static QuickButton TogglePortraitHighlights;


		private static QuickButton ToggleCustomTextures;


		private static QuickDropdown<string> FrameDropdown;
		private static QuickCheckbox TooltipOffsetCheckbox;

		#endregion

	}
}