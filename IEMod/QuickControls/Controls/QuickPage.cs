using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.QuickControls.Controls {
	[NewType]
	public class QuickPage : QuickControl {

		public QuickPage(Transform parent = null, string name = "QuickPage", GameObject altPrototype = null) {
			GameObject = new GameObject();
			var proto = altPrototype ?? Prefabs.QuickPage;
			Name = name;
			Parent = parent;
			LocalScale = proto.transform.localScale;
			LocalPosition = proto.transform.localPosition;
		}
	}
}
