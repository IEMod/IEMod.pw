using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.ObjectBrowser {
	[ModifiesType("Player")]
	public class Mod_OnGUI_Player : Player {
		[NewMember]
		public string modname = "";

		[NewMember]
		public string description = "";

		[NewMember]
		public string author = "";

		[NewMember]
		public bool appearanceToggle;

		[NewMember]
		public bool equipmentToggle;
		
		[NewMember]
		public bool equipDefaultItems;

		[NewMember]
		public bool showModelViewer;

		[NewMember]
		public Transform inspecting;

		[NewMember]
		public Component insComp;

		[NewMember]
		public Vector2 scrollPosition;

		[NewMember]
		public Vector2 scrollPos2;

		[NewMember]
		public Vector2 scrollPos3;

		[NewMember]
		public bool inspectingParentless;

		[NewMember]
		public bool showGameObjectBrowser;

		[NewMember]
		public List<GameObject> parentlessGOs;

		[NewMember]
		private void Rec_PrintGOTree(Transform curTransform, Stack<int> coords, int depth, StringBuilder output)
		{
			output.Append(' ', depth);
			foreach (int n in coords.Reverse<int>())
				output.Append(n.ToString() + '.');
			output.Append(' ');
			output.Append(curTransform.name);
			output.Append('\n');
			foreach (int n in coords.Reverse<int>())
				output.Append(".GetChild(" + n.ToString() + ')');
			output.Append('\n');
			for (int i = 0; i < curTransform.childCount; ++i)
			{
				coords.Push(i);
				Rec_PrintGOTree(curTransform.GetChild(i), coords, depth + 1, output);
				coords.Pop();
			}
		}

		[NewMember]
		private void PrintGOTree(List<GameObject> parentlessGOs) //prints gameobject transform tree to debug log
		{
			StringBuilder output = new StringBuilder();
			for (int i = 0; i < parentlessGOs.Count(); ++i)
			{
				Stack<int> stack = new Stack<int>();
				stack.Push(i);
				Rec_PrintGOTree(parentlessGOs[i].transform, stack, 1, output);
			}
			Debug.Log(output.ToString());
		}

		[MemberAlias(".ctor", typeof(MonoBehaviour))]
		private void MonoBehavior_ctor() {
			
		}

		[ModifiesMember(".ctor")]
		public void CtorNew() {
			MonoBehavior_ctor();
			// added code
			showGameObjectBrowser = false;
			scrollPosition = new Vector2(0, 0);
			scrollPos2 = new Vector2(0, 0);
			scrollPos3 = new Vector2(0, 0);
			inspectingParentless = false;
			parentlessGOs = new List<GameObject> ();

			this.appearanceToggle = true;
			this.equipmentToggle = true;
			this.equipDefaultItems = true;
			this.showModelViewer = false;

			// end of added

			this.move_distance = 0.1f;
			this.move_interpolation_time = 0.01f;
			this.turn_interpolation_time = 0.01f;
			this.collision_layerMask = -1;
			this.m_dragSelectMin = Vector3.zero;
			this.m_dragSelectMax = Vector3.zero;
			this.m_castAnimVariation = -1;
			this.m_playMovementSound = true;
			this.m_startPoint = string.Empty;

			if (InGameHUD.Instance != null)
			{
				float width = IEModOptions.Layout.SelectionCircleWidth;
				InGameHUD.Instance.SelectionCircleWidth = width;
				InGameHUD.Instance.EngagedCircleWidth = width;
			}
		}

		[NewMember]
		private void OnGUI()
		{
//		GUI.Box(new Rect(Screen.width-110,10,100,90), "Bester Tools");
//
//		if(GUI.Button(new Rect(Screen.width-100,40,80,20), "Nothing")) {
//
//			//Console.AddMessage("button pressed");
//		}

			///////////////////////////////////////////////////
			/// 											///
			///			GameObject Browser Start			///
			/// 											///
			///////////////////////////////////////////////////

			if (showGameObjectBrowser)
			{
				float RectWidth = 400;
				float RectHeight = 500;

				float leftMenuStartWidth = Screen.width/2 - RectWidth*1.3f;
				float leftMenuStartHeight = Screen.height/2 - RectHeight/2;

				float rightMenuStartWidth = (Screen.width/2 -1) - RectWidth*0.3f;
				float rightMenuStartHeight = Screen.height/2 - RectHeight/2;

				float smallButtonWidth = 40;

				float buttonWidth = RectWidth-smallButtonWidth;
				float buttonHeight = 20;

				float leftBtnStartWidth = 10;
				float leftBtnStartHeight = 40;

				Component[] components = inspecting.GetComponents<Component>();
				int componentsCount = components.Length;

				float scrollHeight;
				if (!inspectingParentless)
					scrollHeight = 42 + inspecting.childCount*buttonHeight;
				else
					scrollHeight = 42 + (parentlessGOs.Count+1)*buttonHeight;

				float scroll2Height = 42 + componentsCount*buttonHeight;

				if (insComp == null)
					insComp = components[0];

				int tempor1 = insComp.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly).Length;
				int tempor2 = insComp.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly).Length;
				int tempor3 = insComp.GetType().BaseType.GetProperty("depth", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly) != null ? 1 : 0;

				int componentFields = tempor1 + tempor2 + tempor3;
				float scroll3Height = 42 + componentFields*buttonHeight;


				/// first window

				scrollPosition = GUI.BeginScrollView(new Rect(leftMenuStartWidth, leftMenuStartHeight, RectWidth, RectHeight), scrollPosition, new Rect(0, 0, RectWidth-20, scrollHeight < RectHeight ? RectHeight : scrollHeight)); 

				string gameObjectsName = inspecting.ToString();
				gameObjectsName = gameObjectsName.Replace("UnityEngine.", "");
				string windowTitle;

				if (!inspectingParentless)
					windowTitle = "Children for: " + gameObjectsName;
				else
					windowTitle = "Top of the hierarchy";


				GUI.Box(new Rect(0, 0, RectWidth, scrollHeight < RectHeight ? RectHeight : scrollHeight), windowTitle);

				if(GUI.Button(new Rect(0, 20, 50 , buttonHeight), "...")) {
					if (inspecting.parent != null)
					{
						inspecting = inspecting.parent;
						insComp = inspecting.GetComponents<Component> () [0];
						inspectingParentless = false;
					} else
					{
						int num1 = 0;
						int limitObjectsTo = 150;

						//TODO: currently this finds only active objects... that's not right!

						GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
						//GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject> ();
						if (parentlessGOs != null)
							parentlessGOs.Clear ();
						else
							parentlessGOs = new List<GameObject>();
						for (int i = allGameObjects.Length - 1; i > 0 && num1 < limitObjectsTo; i--)
						{
							if (allGameObjects [i].transform.parent == null && allGameObjects [i].activeInHierarchy)
							{
								if (!allGameObjects [i].name.StartsWith ("_UIDrawCall") && !allGameObjects [i].name.StartsWith ("Spellbind_Minor_") && !allGameObjects [i].name.StartsWith ("Destination_Circle"))
								{
									parentlessGOs.Add (allGameObjects [i]);
									num1++;
								}
							}
						}

						//Calling debug output of transform tree:
						//Enable this line to dump the transform tree to the debug log file.
						//PrintGOTree(parentlessGOs);

						inspectingParentless = true;
					}
				}

				if (inspectingParentless == false)
				{
					for (int i = 0; i < inspecting.childCount; i++)
					{
						string typeToDisplay = inspecting.GetChild (i).ToString ();
						typeToDisplay = typeToDisplay.Replace ("UnityEngine.", "");
						string active = inspecting.GetChild (i).gameObject.activeSelf && inspecting.GetChild (i).gameObject.activeInHierarchy ? "ON" : "OFF";
						string nameToDisplay = i + ". " + typeToDisplay + " : " + inspecting.GetChild (i).childCount + " : " + inspecting.GetComponents<Component> ().Length;

						if (GUI.Button (new Rect (leftBtnStartWidth, leftBtnStartHeight + +(i * buttonHeight + 1), smallButtonWidth, buttonHeight), active))
						{
							inspecting.GetChild (i).gameObject.SetActive (inspecting.GetChild (i).gameObject.activeSelf && inspecting.GetChild (i).gameObject.activeInHierarchy ? false : true);
						}

				
						if (GUI.Button (new Rect (leftBtnStartWidth + smallButtonWidth, leftBtnStartHeight + (i * buttonHeight + 1), buttonWidth - smallButtonWidth, buttonHeight), nameToDisplay))
						{
							inspecting = inspecting.GetChild (i);
							insComp = inspecting.GetComponents<Component> () [0];
						}
					}
				} else
				{
					for (int i = 0; i < parentlessGOs.Count ; i++)
					{
						string typeToDisplay = parentlessGOs[i].ToString();
						typeToDisplay = typeToDisplay.Replace("UnityEngine.", "");
						string active = "";
						if (parentlessGOs [i] != null)
							active = parentlessGOs [i].activeSelf && parentlessGOs [i].activeInHierarchy ? "ON" : "OFF";
						else
						{
							parentlessGOs.RemoveAt (i);
							continue;
						} // these disappearing objects were causing a major bug... probably some drawcall objects or something compiler generated...

						string nameToDisplay = i+". "+typeToDisplay+" : "+parentlessGOs[i].transform.childCount+" : "+parentlessGOs[i].GetComponents<Component>().Length;

						// highlighting important buttons
						if (typeToDisplay.Contains("2_Design_Area_"))
							GUI.contentColor = Color.green;

						if (GUI.Button(new Rect(leftBtnStartWidth, leftBtnStartHeight + + (i * buttonHeight + 1), smallButtonWidth, buttonHeight), active))
						{
							if (parentlessGOs [i] != null)
								parentlessGOs [i].SetActive (parentlessGOs [i].activeSelf && parentlessGOs [i].activeInHierarchy ? false : true);
						}

						if(GUI.Button(new Rect(leftBtnStartWidth+smallButtonWidth, leftBtnStartHeight + (i * buttonHeight + 1), buttonWidth-smallButtonWidth, buttonHeight), nameToDisplay)) 
						{

							inspecting = parentlessGOs[i].transform;
							insComp = inspecting.GetComponents<Component>()[0];
							inspectingParentless = false;
						}

						GUI.contentColor = Color.white;
					}
				}

				GUI.EndScrollView(true);

				/// second window

				if (!inspectingParentless)
				{
					scrollPos2 = GUI.BeginScrollView (new Rect (rightMenuStartWidth, rightMenuStartHeight, RectWidth, RectHeight), scrollPos2, new Rect (0, 0, RectWidth - 20, scroll2Height < RectHeight ? RectHeight : scroll2Height));

					GUI.Box (new Rect (0, 0, RectWidth - 100, scroll2Height < RectHeight ? RectHeight : scroll2Height), "Components for: " + gameObjectsName);

					int num2 = 0;

					for (int i = 0; i < componentsCount && num2 < 1000; i++)
					{
						string typeToDisplay = components [i].ToString ();
						typeToDisplay = typeToDisplay.Replace ("UnityEngine.", "");
						string nameToDisplay = i + ": " + typeToDisplay;

						if (GUI.Button (new Rect (leftBtnStartWidth, leftBtnStartHeight + (i * buttonHeight), buttonWidth - 100, buttonHeight), nameToDisplay))
						{
							insComp = components [i];
						}
						num2++;
					}

					GUI.EndScrollView (true);
				}

				/// third window

				if (!inspectingParentless)
				{
					scrollPos3 = GUI.BeginScrollView (new Rect (rightMenuStartWidth + (RectWidth - 100) - 1, rightMenuStartHeight, RectWidth, RectHeight), scrollPos3, new Rect (0, 0, RectWidth - 20, scroll3Height < RectHeight ? RectHeight : scroll3Height));

					GUI.Box (new Rect (0, 0, RectWidth, scroll3Height < RectHeight ? RectHeight : scroll3Height), "Inspecting component: " + insComp.ToString ());

					string fieldNameToDisplay = "";

					int properties = 0;

					int g = 0;
					foreach (var prop in insComp.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
					{
						if (properties < 1000)
						{
							fieldNameToDisplay = prop.Name + ":";

							GUI.Label (new Rect (leftBtnStartWidth, leftBtnStartHeight + (g * buttonHeight), buttonWidth / 2, buttonHeight), fieldNameToDisplay);

							fieldNameToDisplay = prop.GetValue (insComp, null) != null ? prop.GetValue (insComp, null).ToString () : "None";

							GUI.Label (new Rect (leftBtnStartWidth + buttonWidth / 2, leftBtnStartHeight + (g * buttonHeight), buttonWidth / 2, buttonHeight), fieldNameToDisplay);
							g++;
						}

						properties++;
					}

					properties = 0;

					foreach (var field in insComp.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly))
					{
						if (properties < 1000)
						{
							fieldNameToDisplay = field.Name + ":";

							GUI.Label (new Rect (leftBtnStartWidth, leftBtnStartHeight + (g * buttonHeight), buttonWidth / 2, buttonHeight), fieldNameToDisplay);

							fieldNameToDisplay = field.GetValue (insComp) != null ? field.GetValue (insComp).ToString () : "None";

							GUI.Label (new Rect (leftBtnStartWidth + buttonWidth / 2, leftBtnStartHeight + (g * buttonHeight), buttonWidth / 2, buttonHeight), fieldNameToDisplay);
							g++;
						}
						properties++;
					}



					System.Reflection.PropertyInfo depthToDisplay2 = insComp.GetType ().BaseType.GetProperty ("depth", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
					if (depthToDisplay2 != null)
					{
						fieldNameToDisplay = depthToDisplay2.Name + ":";
						GUI.Label (new Rect (leftBtnStartWidth, leftBtnStartHeight + (g * buttonHeight), buttonWidth / 2, buttonHeight), fieldNameToDisplay);

						fieldNameToDisplay = depthToDisplay2.GetValue (insComp, null).ToString ();

						GUI.Label (new Rect (leftBtnStartWidth + buttonWidth / 2, leftBtnStartHeight + (g * buttonHeight), buttonWidth / 2, buttonHeight), fieldNameToDisplay);

					}

					GUI.EndScrollView (true);
				}

			}

			///////////////////////////////////////////////////
			/// 											///
			///			GameObject Browser End				///
			/// 											///
			///////////////////////////////////////////////////

			
			///////////////////////////////////////////////////
			/// 											///
			/// 			ModelViewer Start				///
			/// 											///
			///////////////////////////////////////////////////

			if (showModelViewer)
			{
				NPCAppearance appearance = global::GameState.s_playerCharacter.GetComponent<NPCAppearance> ();
				Equipment equipment = global::GameState.s_playerCharacter.GetComponent<Equipment> ();

				int num = Screen.width - 300;
				int num2 = 50;
				int num3 = 200;
				int num4 = 0x12;
				GUI.Label (new Rect ((float)num, (float)num2, (float)num3, (float)num4), "Race: " + appearance.GetRaceString ());
				num2 += num4 + 5;
				GUI.Label (new Rect ((float)num, (float)num2, (float)num3, (float)num4), "Body Type: " + appearance.GetBodyString ());
				num2 += num4 + 5;
				GUI.Label (new Rect ((float)num, (float)num2, (float)num3, (float)num4), "Gender: " + appearance.GetGenderFullString ());
				num2 += num4 + 5;
				GUI.Label (new Rect ((float)num, (float)num2, (float)num3, (float)num4), string.Concat (new object[] {
					"Head: ",
					appearance.headAppearance.modelVariation,
					", ",
					appearance.headAppearance.materialVariation
				}));
				num2 += num4 + 5;
				if (appearance.hasHair)
				{
					GUI.Label (new Rect ((float)num, (float)num2, (float)num3, (float)num4), string.Concat (new object[] {
						"Hair: ",
						appearance.hairAppearance.modelVariation,
						", ",
						appearance.hairAppearance.materialVariation
					}));
					num2 += num4 + 5;
				}
				if (appearance.hasFacialHair)
				{
					GUI.Label (new Rect ((float)num, (float)num2, (float)num3, (float)num4), string.Concat (new object[] {
						"Facial Hair: ",
						appearance.facialHairAppearance.modelVariation,
						", ",
						appearance.facialHairAppearance.materialVariation
					}));
					num2 += num4 + 5;
				}
				Equippable itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Armor);
				if (itemInSlot != null)
				{
					GUI.Label (new Rect ((float)num, (float)num2, (float)num3, (float)num4), string.Concat (new object[] {
						"Armor: ",
						itemInSlot.Appearance.GetArmorTypeShortString (),
						", ",
						itemInSlot.Appearance.modelVariation,
						", ",
						itemInSlot.Appearance.materialVariation
					}));
					num2 += num4 + 5;
				}
				Equippable equippable2 = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Head);
				if (equippable2 != null)
				{
					GUI.Label (new Rect ((float)num, (float)num2, (float)num3, (float)num4), string.Concat (new object[] {
						"Helm: ",
						equippable2.Appearance.modelVariation,
						", ",
						equippable2.Appearance.materialVariation
					}));
					num2 += num4 + 5;
				}
				Equippable equippable3 = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Hands);
				if (equippable3 != null)
				{
					GUI.Label (new Rect ((float)num, (float)num2, (float)num3, (float)num4), string.Concat (new object[] {
						"Hands: ",
						equippable3.Appearance.modelVariation,
						", ",
						equippable3.Appearance.materialVariation
					}));
					num2 += num4 + 5;
				}
				Equippable equippable4 = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Feet);
				if (equippable4 != null)
				{
					GUI.Label (new Rect ((float)num, (float)num2, (float)num3, (float)num4), string.Concat (new object[] {
						"Feet: ",
						equippable4.Appearance.modelVariation,
						", ",
						equippable4.Appearance.materialVariation
					}));
					num2 += num4 + 5;
				}
				int num5 = 15;
				int num6 = 100;
				int num7 = 0x12;
				int num8 = 10;
				int num9 = 50;
				int num10 = 50;
				if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
				{
					appearance.race -= 1;
					appearance.race = (NPCAppearance.Race)Mathf.Clamp ((int)appearance.race, 0, 5);
					appearance.racialBodyType = (appearance.race != NPCAppearance.Race.GOD) ? appearance.race : NPCAppearance.Race.HUM;
					appearance.avatar = null;
					appearance.gameObject.GetComponent<Animator> ().avatar = null;
					appearance.Generate ();
				}
				GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Race");
				if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
				{
					appearance.race += 1;
					appearance.race = (NPCAppearance.Race)Mathf.Clamp ((int)appearance.race, 0, 5);
					appearance.racialBodyType = (appearance.race != NPCAppearance.Race.GOD) ? appearance.race : NPCAppearance.Race.HUM;
					appearance.avatar = null;
					appearance.gameObject.GetComponent<Animator> ().avatar = null;
					appearance.Generate ();
				}
				num10 += num7 + 5;
				if (appearance.race == NPCAppearance.Race.GOD)
				{
					if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
					{
						appearance.racialBodyType -= 1;
						appearance.racialBodyType = (NPCAppearance.Race)Mathf.Clamp ((int)appearance.racialBodyType, 0, 4);
						appearance.avatar = null;
						appearance.gameObject.GetComponent<Animator> ().avatar = null;
						appearance.Generate ();
					}
					GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Body Type");
					if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
					{
						appearance.racialBodyType += 1;
						appearance.racialBodyType = (NPCAppearance.Race)Mathf.Clamp ((int)appearance.racialBodyType, 0, 4);
						appearance.avatar = null;
						appearance.gameObject.GetComponent<Animator> ().avatar = null;
						appearance.Generate ();
					}
					num10 += num7 + 5;
					if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
					{
						appearance.subrace -= 1;
						appearance.subrace = (NPCAppearance.Subrace)Mathf.Clamp ((int)appearance.subrace, 8, 11);
						appearance.avatar = null;
						appearance.gameObject.GetComponent<Animator> ().avatar = null;
						appearance.Generate ();
					}
					GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Subrace");
					if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
					{
						appearance.subrace += 1;
						appearance.subrace = (NPCAppearance.Subrace)Mathf.Clamp ((int)appearance.subrace, 8, 11);
						appearance.avatar = null;
						appearance.gameObject.GetComponent<Animator> ().avatar = null;
						appearance.Generate ();
					}
				} else
				{
					num10 += num7 + 5;
				}
				num10 += num7 + 5;
				if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
				{
					appearance.gender = NPCAppearance.Sex.Female;
					appearance.avatar = null;
					appearance.gameObject.GetComponent<Animator> ().avatar = null;
					appearance.Generate ();
				}
				GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Gender");
				if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
				{
					appearance.gender = NPCAppearance.Sex.Male;
					appearance.avatar = null;
					appearance.gameObject.GetComponent<Animator> ().avatar = null;
					appearance.Generate ();
				}
				num10 += num7 + 5;
				this.appearanceToggle = GUI.Toggle (new Rect ((float)num9, (float)num10, (float)num6, (float)num7), this.appearanceToggle, "Appearance");
				num10 += num7 + 5;
				if (appearanceToggle)
				{
					num9 += num8;
					GUI.Label (new Rect ((float)num9, (float)num10, (float)num6, (float)num7), "Head");
					num10 += num7 + 5;
					num9 += num8;
					if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
					{
						appearance.headAppearance.modelVariation--;
						appearance.headAppearance.modelVariation = Mathf.Clamp (appearance.headAppearance.modelVariation, 1, 100);
						appearance.Generate ();
					}
					GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Model");
					if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
					{
						appearance.headAppearance.modelVariation++;
						appearance.headAppearance.modelVariation = Mathf.Clamp (appearance.headAppearance.modelVariation, 1, 100);
						appearance.Generate ();
					}
					num10 += num7 + 5;
					if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
					{
						appearance.headAppearance.materialVariation--;
						appearance.headAppearance.materialVariation = Mathf.Clamp (appearance.headAppearance.materialVariation, 1, 100);
						appearance.Generate ();
					}
					GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Material");
					if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
					{
						appearance.headAppearance.materialVariation++;
						appearance.headAppearance.materialVariation = Mathf.Clamp (appearance.headAppearance.materialVariation, 1, 100);
						appearance.Generate ();
					}
					num10 += num7 + 5;
					num9 -= num8;
					bool hasHair = appearance.hasHair;
					appearance.hasHair = GUI.Toggle (new Rect ((float)num9, (float)num10, (float)num6, (float)num7), appearance.hasHair, "Hair");
					if (hasHair != appearance.hasHair)
					{
						appearance.Generate ();
					}
					num10 += num7 + 5;
					if (appearance.hasHair)
					{
						num9 += num8;
						if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
						{
							appearance.hairAppearance.modelVariation--;
							appearance.hairAppearance.modelVariation = Mathf.Clamp (appearance.hairAppearance.modelVariation, 1, 100);
							appearance.Generate ();
						}
						GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Model");
						if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
						{
							appearance.hairAppearance.modelVariation++;
							appearance.hairAppearance.modelVariation = Mathf.Clamp (appearance.hairAppearance.modelVariation, 1, 100);
							appearance.Generate ();
						}
						num10 += num7 + 5;
						if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
						{
							appearance.hairAppearance.materialVariation--;
							appearance.hairAppearance.materialVariation = Mathf.Clamp (appearance.hairAppearance.materialVariation, 1, 100);
							appearance.Generate ();
						}
						GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Material");
						if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
						{
							appearance.hairAppearance.materialVariation++;
							appearance.hairAppearance.materialVariation = Mathf.Clamp (appearance.hairAppearance.materialVariation, 1, 100);
							appearance.Generate ();
						}
						num10 += num7 + 5;
						num9 -= num8;
					}
					bool hasFacialHair = appearance.hasFacialHair;
					appearance.hasFacialHair = GUI.Toggle (new Rect ((float)num9, (float)num10, (float)num6, (float)num7), appearance.hasFacialHair, "Facial Hair");
					if (hasFacialHair != appearance.hasFacialHair)
					{
						appearance.Generate ();
					}
					num10 += num7 + 5;
					if (appearance.hasFacialHair)
					{
						num9 += num8;
						if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
						{
							appearance.facialHairAppearance.modelVariation--;
							appearance.facialHairAppearance.modelVariation = Mathf.Clamp (appearance.facialHairAppearance.modelVariation, 1, 100);
							appearance.Generate ();
						}
						GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Model");
						if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
						{
							appearance.facialHairAppearance.modelVariation++;
							appearance.facialHairAppearance.modelVariation = Mathf.Clamp (appearance.facialHairAppearance.modelVariation, 1, 100);
							appearance.Generate ();
						}
						num10 += num7 + 5;
						if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
						{
							appearance.facialHairAppearance.materialVariation--;
							appearance.facialHairAppearance.materialVariation = Mathf.Clamp (appearance.facialHairAppearance.materialVariation, 1, 100);
							appearance.Generate ();
						}
						GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Material");
						if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
						{
							appearance.facialHairAppearance.materialVariation++;
							appearance.facialHairAppearance.materialVariation = Mathf.Clamp (appearance.facialHairAppearance.materialVariation, 1, 100);
							appearance.Generate ();
						}
						num10 += num7 + 5;
						num9 -= num8;
					}
					num9 -= num8;
				}
				this.equipmentToggle = GUI.Toggle (new Rect ((float)num9, (float)num10, (float)num6, (float)num7), this.equipmentToggle, "Equipment");
				num10 += num7 + 5;
				if (this.equipmentToggle)
				{
					num9 += num8;
					GUI.Label (new Rect ((float)num9, (float)num10, (float)num6, (float)num7), "Body");
					num10 += num7 + 5;
					num9 += num8;
					if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
					{
						Equippable itemInSlotZ = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Armor);
						if (itemInSlotZ != null)
						{
							itemInSlotZ.Appearance.armorType -= 1;
							itemInSlotZ.Appearance.armorType = (AppearancePiece.ArmorType)Mathf.Clamp ((int)itemInSlotZ.Appearance.armorType, 1, 9);
						}
						appearance.Generate ();
					}
					GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Type");
					if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
					{
						Equippable itemInSlotZ = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Armor);
						if (itemInSlotZ != null)
						{
							itemInSlotZ.Appearance.armorType += 1;
							itemInSlotZ.Appearance.armorType = (AppearancePiece.ArmorType)Mathf.Clamp ((int)itemInSlotZ.Appearance.armorType, 1, 9);
						}
						appearance.Generate ();
					}
					num10 += num7 + 5;
					if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
					{
						Equippable itemInSlotZ = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Armor);
						if (itemInSlotZ != null)
						{
							itemInSlotZ.Appearance.modelVariation -= 1;
							itemInSlotZ.Appearance.modelVariation = (int)(AppearancePiece.ArmorType)Mathf.Clamp ((int)itemInSlotZ.Appearance.modelVariation, 1, 9);
						}
						appearance.Generate ();
					}
					GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Model");
					if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
					{
						Equippable itemInSlotZ = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Armor);
						if (itemInSlotZ != null)
						{
							itemInSlotZ.Appearance.modelVariation += 1;
							itemInSlotZ.Appearance.modelVariation = (int)(AppearancePiece.ArmorType)Mathf.Clamp ((int)itemInSlotZ.Appearance.modelVariation, 1, 9);
						}
						appearance.Generate ();
					}
					num10 += num7 + 5;
					if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
					{
						Equippable itemInSlotZ = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Armor);
						if (itemInSlotZ != null)
						{
							itemInSlotZ.Appearance.materialVariation -= 1;
							itemInSlotZ.Appearance.materialVariation = (int)(AppearancePiece.ArmorType)Mathf.Clamp ((int)itemInSlotZ.Appearance.materialVariation, 1, 9);
						}
						appearance.Generate ();
					}
					GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Material");
					if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
					{
						Equippable itemInSlotZ = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Armor);
						if (itemInSlotZ != null)
						{
							itemInSlotZ.Appearance.materialVariation += 1;
							itemInSlotZ.Appearance.materialVariation = (int)(AppearancePiece.ArmorType)Mathf.Clamp ((int)itemInSlotZ.Appearance.materialVariation, 1, 9);
						}
						appearance.Generate ();
					}
					num10 += num7 + 5;
					num9 -= num8;
					Equippable item = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Head);
					bool flag3 = item != null;
					flag3 = GUI.Toggle (new Rect ((float)num9, (float)num10, (float)num6, (float)num7), flag3, "Helm");
					if ((item != null) != flag3)
					{
						if (item != null)
						{
							UnityEngine.Object.Destroy (equipment.UnEquip (item, Equippable.EquipmentSlot.Head).gameObject);
						} else
						{
							item = new GameObject (Equippable.EquipmentSlot.Head.ToString ()).AddComponent<Equippable> ();
							item.Appearance = new AppearancePiece ();
							item.Appearance.armorType = AppearancePiece.ArmorType.None;
							item.Appearance.bodyPiece = AppearancePiece.BodyPiece.Helm;
							item.Appearance.modelVariation = 1;
							item.Appearance.materialVariation = 1;
							equippable2 = equipment.Equip (item, Equippable.EquipmentSlot.Head, false);
							if (equippable2 != null)
							{
								UnityEngine.Object.Destroy (equippable2);
							}
						}
						appearance.Generate ();
					}
					num10 += num7 + 5;
					if (flag3)
					{
						num9 += num8;
						if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Head);
							itemInSlot.Appearance.modelVariation--;
							itemInSlot.Appearance.modelVariation = Mathf.Clamp (itemInSlot.Appearance.modelVariation, 1, 100);
							appearance.Generate ();
						}
						GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Model");
						if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Head);
							itemInSlot.Appearance.modelVariation++;
							itemInSlot.Appearance.modelVariation = Mathf.Clamp (itemInSlot.Appearance.modelVariation, 1, 100);
							appearance.Generate ();
						}
						num10 += num7 + 5;
						if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Head);
							itemInSlot.Appearance.materialVariation--;
							itemInSlot.Appearance.materialVariation = Mathf.Clamp (itemInSlot.Appearance.materialVariation, 1, 100);
							appearance.Generate ();
						}
						GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Material");
						if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Head);
							itemInSlot.Appearance.materialVariation++;
							itemInSlot.Appearance.materialVariation = Mathf.Clamp (itemInSlot.Appearance.materialVariation, 1, 100);
							appearance.Generate ();
						}
						num10 += num7 + 5;
						num9 -= num8;
					}
					Equippable equippable6 = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Hands);
					bool flag4 = equippable6 != null;
					flag4 = GUI.Toggle (new Rect ((float)num9, (float)num10, (float)num6, (float)num7), flag4, "Hands");
					if ((equippable6 != null) != flag4)
					{
						if (equippable6 != null)
						{
							UnityEngine.Object.Destroy (equipment.UnEquip (equippable6, Equippable.EquipmentSlot.Hands).gameObject);
						} else
						{
							item = new GameObject (Equippable.EquipmentSlot.Hands.ToString ()).AddComponent<Equippable> ();
							item.Appearance = new AppearancePiece ();
							item.Appearance.armorType = AppearancePiece.ArmorType.None;
							item.Appearance.bodyPiece = AppearancePiece.BodyPiece.Gloves;
							item.Appearance.modelVariation = 1;
							item.Appearance.materialVariation = 1;
							equippable2 = equipment.Equip (item, Equippable.EquipmentSlot.Hands, false);
							if (equippable2 != null)
							{
								UnityEngine.Object.Destroy (equippable2);
							}
						}
						appearance.Generate ();
					}
					num10 += num7 + 5;
					if (flag4)
					{
						num9 += num8;
						if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Hands);
							itemInSlot.Appearance.modelVariation--;
							itemInSlot.Appearance.modelVariation = Mathf.Clamp (itemInSlot.Appearance.modelVariation, 1, 100);
							appearance.Generate ();
						}
						GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Model");
						if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Hands);
							itemInSlot.Appearance.modelVariation++;
							itemInSlot.Appearance.modelVariation = Mathf.Clamp (itemInSlot.Appearance.modelVariation, 1, 100);
							appearance.Generate ();
						}
						num10 += num7 + 5;
						if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Hands);
							itemInSlot.Appearance.materialVariation--;
							itemInSlot.Appearance.materialVariation = Mathf.Clamp (itemInSlot.Appearance.materialVariation, 1, 100);
							appearance.Generate ();
						}
						GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Material");
						if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Hands);
							itemInSlot.Appearance.materialVariation++;
							itemInSlot.Appearance.materialVariation = Mathf.Clamp (itemInSlot.Appearance.materialVariation, 1, 100);
							appearance.Generate ();
						}
						num10 += num7 + 5;
						num9 -= num8;
					}
					Equippable equippable7 = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Feet);
					bool flag5 = equippable7 != null;
					flag5 = GUI.Toggle (new Rect ((float)num9, (float)num10, (float)num6, (float)num7), flag5, "Feet");
					if ((equippable7 != null) != flag5)
					{
						if (equippable7 != null)
						{
							UnityEngine.Object.Destroy (equipment.UnEquip (equippable7, Equippable.EquipmentSlot.Feet).gameObject);
						} else
						{
							item = new GameObject (Equippable.EquipmentSlot.Feet.ToString ()).AddComponent<Equippable> ();
							item.Appearance = new AppearancePiece ();
							item.Appearance.armorType = AppearancePiece.ArmorType.None;
							item.Appearance.bodyPiece = AppearancePiece.BodyPiece.Boots;
							item.Appearance.modelVariation = 1;
							item.Appearance.materialVariation = 1;
							equippable2 = equipment.Equip (item, Equippable.EquipmentSlot.Feet, false);
							if (equippable2 != null)
							{
								UnityEngine.Object.Destroy (equippable2);
							}
						}
						appearance.Generate ();
					}
					num10 += num7 + 5;
					if (flag5)
					{
						num9 += num8;
						if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Feet);
							itemInSlot.Appearance.modelVariation--;
							itemInSlot.Appearance.modelVariation = Mathf.Clamp (itemInSlot.Appearance.modelVariation, 1, 100);
							appearance.Generate ();
						}
						GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Model");
						if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Feet);
							itemInSlot.Appearance.modelVariation++;
							itemInSlot.Appearance.modelVariation = Mathf.Clamp (itemInSlot.Appearance.modelVariation, 1, 100);
							appearance.Generate ();
						}
						num10 += num7 + 5;
						if (GUI.Button (new Rect ((float)num9, (float)num10, (float)num5, (float)num7), "<"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Feet);
							itemInSlot.Appearance.materialVariation--;
							itemInSlot.Appearance.materialVariation = Mathf.Clamp (itemInSlot.Appearance.materialVariation, 1, 100);
							appearance.Generate ();
						}
						GUI.Label (new Rect ((float)((num9 + num5) + 5), (float)num10, (float)num6, (float)num7), "Material");
						if (GUI.Button (new Rect ((float)(((num9 + num5) + num6) + 10), (float)num10, (float)num5, (float)num7), ">"))
						{
							itemInSlot = equipment.CurrentItems.GetItemInSlot (Equippable.EquipmentSlot.Feet);
							itemInSlot.Appearance.materialVariation++;
							itemInSlot.Appearance.materialVariation = Mathf.Clamp (itemInSlot.Appearance.materialVariation, 1, 100);
							appearance.Generate ();
						}
						num10 += num7 + 5;
						num9 -= num8;
					}
				}


				///////////////////////////////////////////////////
				///											    ///
				/// 			ModelViewer End				    ///
				/// 										    ///
				///////////////////////////////////////////////////
			}
		}
	}
}
