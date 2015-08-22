using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEMod.Helpers;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.QuickControls.Behaviors {
	[NewType]
	public static class BlockClicking {
		public static void Apply<T>(QuickDropdown<T> button) {
			var FrameDropdown = button.GameObject;
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
		}
	}
}
