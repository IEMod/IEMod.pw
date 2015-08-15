using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEMod.Helpers;
using IEMod.Mods.UICustomization;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace IEMod.Mods.DropButtonMod {

	[NewType]
	public static class DropButton {

		private static GameObject _invManager;

		private static void Initialize() {
			_invManager = UIInventoryManager.Instance.gameObject.ChildPath("InventoryWindow/UnlitContent/StuffBar");
		}

		internal static void InjectDropInvButton() {
			Initialize();
			if (_invManager.transform.childCount >= 5) { return; }
			var craftButton = _invManager.Child("Craft").Component<UIMultiSpriteImageButton>();
			craftButton.transform.localPosition += new Vector3(0, 25, 0);
			craftButton.transform.localScale = new Vector3(0.82f, 0.82f, 0.82f);
			var cf = new IEControlFactory() {
				ExampleButton = craftButton.gameObject,
				CurrentParent = craftButton.transform.parent
			};
			GameObject myButton = cf.Button("Drop Items", "DropItems",
				localPos: craftButton.transform.localPosition.Plus(y: -43f));

			myButton.transform.localScale = craftButton.transform.localScale;
			var uiMultiSpriteImageButton = myButton.Component<UIMultiSpriteImageButton>();
			uiMultiSpriteImageButton.Label.multiLine = false;
			uiMultiSpriteImageButton.Label.shrinkToFit = true;
			uiMultiSpriteImageButton.onClick = DropInventory;
		}

		private static void DropInventory(GameObject go)
		{
			PartyMemberAI selChar = UIInventoryManager.Instance.SelectedCharacter.gameObject.Component<PartyMemberAI>();

			Container container = GameResources.LoadPrefab<Container>("DefaultDropItem", false);

			//assign another mesh, less ugly, less giant, less looking like a fucking 3 year old baby? ugh..................

			var dropObject = GameResources.Instantiate<GameObject>(container.gameObject, selChar.gameObject.transform.position, Quaternion.identity);

			dropObject.tag = "DropItem";
			dropObject.layer = LayerUtility.FindLayerValue("Dynamics");
			dropObject.transform.localRotation = Quaternion.AngleAxis(Random.Range((float)0f, (float)360f), Vector3.up);
			InstanceID eid = dropObject.Component<InstanceID>();
			eid.Guid = Guid.NewGuid();
			eid.UniqueID = eid.Guid.ToString();
			dropObject.Component<Persistence>().TemplateOnly = true;
			
			Container component = dropObject.Component<Container>();
			component.ManualLabelName = "Drop Items";
			component.DeleteMeIfEmpty = true;
			AlphaControl control = dropObject.AddComponent<AlphaControl>();
			if (control != null)
			{
				control.Alpha = 0f;
				control.FadeIn(1f);
			}

			// opening created container... code taken from Container.Open()
			UILootManager.Instance.SetData(selChar, dropObject.Component<Inventory>(), dropObject.gameObject);
			UILootManager.Instance.ShowWindow();
			dropObject.Component<Inventory>().CloseInventoryCB = component.CloseInventoryCB;
		}
	}
}
