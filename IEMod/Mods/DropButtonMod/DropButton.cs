using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEMod.Helpers;
//using IEMod.Mods.UICustomization;
using IEMod.QuickControls;
using Patchwork.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace IEMod.Mods.DropButtonMod {

	[NewType]
	public static class DropButton {
		private static QuickButton _dropButton;

		internal static void InjectDropInvButton() {
			var invManager = UIInventoryManager.Instance.gameObject.ChildPath("InventoryWindow/UnlitContent/StuffBar");
			if (_dropButton.IsAlive()) return;
			var craftButton = invManager.Child("Craft").Component<UIMultiSpriteImageButton>();
			craftButton.transform.localPosition += new Vector3(0, 25, 0);
			craftButton.transform.localScale = new Vector3(0.82f, 0.82f, 0.82f);

			_dropButton = new QuickButton(craftButton.transform.parent, "DropButton", craftButton.gameObject, false) {
				LocalScale = craftButton.transform.localScale,
				LocalPosition = craftButton.transform.localPosition.Plus(y: -50f),
				Caption = "Drop Items"
			};
			_dropButton.ButtonComponent.Label.multiLine = false;
			_dropButton.ButtonComponent.Label.shrinkToFit = true;
			_dropButton.Click += x => DropInventory();
		}

		private static void DropInventory()
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
