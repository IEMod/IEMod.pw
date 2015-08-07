using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Patchwork.Attributes;
using Polenter.Serialization;
using UnityEngine;


namespace IEMod.Mods.SaveXML {
	[ModifiesType]
	public class Mod_SaveRawXML : PersistenceManager
	{
		[ModifiesMember("SaveMobileDataToFile")]
		private static void SaveMobileDataToFileNew()
		{
			string text = PersistenceManager.s_mobileObjPath + ".backup";

			bool saveAsXML = PlayerPrefs.GetInt("SaveLoadRawXML", 0) != 0;

			if (File.Exists(text) && File.Exists(PersistenceManager.s_mobileObjPath))
			{
				File.Delete(text);
			}
			if (File.Exists(PersistenceManager.s_mobileObjPath))
			{
				File.Move(PersistenceManager.s_mobileObjPath, text);
			}
			bool flag = true;
			using (FileStream fileStream = new FileStream(PersistenceManager.s_mobileObjPath, FileMode.OpenOrCreate))
			{
				SharpSerializer binaryXMLSerializer = saveAsXML ? GameResources.GetTextXMLSerializer() : GameResources.GetBinaryXMLSerializer();
				binaryXMLSerializer.Serialize(PersistenceManager.MobileObjects.Count, fileStream);
				binaryXMLSerializer.PropertyProvider.AttributesToIgnore.Add(typeof(XmlIgnoreAttribute));
				foreach (ObjectPersistencePacket current in PersistenceManager.MobileObjects.Values)
				{
					try
					{
						binaryXMLSerializer.Serialize(current, fileStream);
					}
					catch (Exception ex)
					{
						Debug.LogError("Error saving mobile object " + current.ObjectName + ".\n" + ex.ToString());
						flag = false;
					}
				}
			}
			if (!flag)
			{
				if (File.Exists(PersistenceManager.s_mobileObjPath))
				{
					File.Delete(PersistenceManager.s_mobileObjPath);
				}
				if (File.Exists(text))
				{
					File.Move(text, PersistenceManager.s_mobileObjPath);
				}
				if (UIDebug.Instance)
				{
					UIDebug.Instance.LogOnScreenWarning("PersistenceManager.SaveMobileDataToFile failed! Save game likely corrupted!", UIDebug.Department.Programming, 10f);
				}
			}
			else if (File.Exists(text))
			{
				File.Delete(text);
			}
		}
		[ModifiesMember("LoadMobileObjects")]
		public static void LoadMobileObjectsNew()
		{
			bool loadfromXML = PlayerPrefs.GetInt("SaveLoadRawXML", 0) != 0;
			string path = PersistenceManager.s_mobileObjPath;
			if (!File.Exists(path))
			{
				return;
			}
			PersistenceManager.MobileObjects.Clear();
			List<ObjectPersistencePacket> list = new List<ObjectPersistencePacket>(5000);
			using (FileStream fileStream = new FileStream(path, FileMode.Open))
			{
				SharpSerializer binaryXMLSerializer = loadfromXML ? GameResources.GetTextXMLSerializer() : GameResources.GetBinaryXMLSerializer();
				int num = (int)binaryXMLSerializer.Deserialize(fileStream);
				for (int i = 0; i < num; i++)
				{
					try
					{
						ObjectPersistencePacket objectPersistencePacket = binaryXMLSerializer.Deserialize(fileStream) as ObjectPersistencePacket;
						if (objectPersistencePacket != null)
						{
							list.Add(objectPersistencePacket);
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("Object load error! " + ex.ToString());
					}
				}
				fileStream.Close();
			}
			foreach (ObjectPersistencePacket current in list)
			{
				PersistenceManager.MobileObjects.Add(current.GUID, current);
			}
			if (global::GameState.NumSceneLoads == 0)
			{
				PersistenceManager.CleanupInvalidMobileData();
			}
			List<Persistence> list2 = new List<Persistence>();
			foreach (ObjectPersistencePacket current2 in PersistenceManager.MobileObjects.Values)
			{
				if (current2.Global || current2.LevelName == Application.loadedLevelName)
				{
					GameObject gameObject = InstanceID.GetObjectByID(current2.GUID);
					if (gameObject == null && !current2.Packed)
					{
						gameObject = current2.CreateObject(true);
						Persistence component = gameObject.GetComponent<Persistence>();
						if (component)
						{
							list2.Add(component);
						}
					}
					else if (gameObject != null)
					{
						Persistence persistence = gameObject.GetComponent<Persistence>();
						if (persistence == null)
						{
							Debug.LogError(current2.ObjectName + " doesn't have a persistence component, yet was saved in mobile objects. Something isn't right.");
							persistence = gameObject.AddComponent<Persistence>();
						}
						list2.Add(persistence);
					}
				}
			}
			foreach (Persistence current3 in list2)
			{
				if (current3.GetComponent<Equippable>() != null)
				{
					current3.Load();
				}
			}
			foreach (Persistence current4 in list2)
			{
				if (current4.GetComponent<Equippable>() == null)
				{
					current4.Load();
				}
			}
			foreach (ObjectPersistencePacket current5 in PersistenceManager.PendingMobileObjects)
			{
				if (PersistenceManager.MobileObjects.ContainsKey(current5.GUID))
				{
					PersistenceManager.MobileObjects[current5.GUID] = current5;
				}
				else
				{
					PersistenceManager.MobileObjects.Add(current5.GUID, current5);
				}
			}
			PersistenceManager.PendingMobileObjects.Clear();
		}

	}
}

