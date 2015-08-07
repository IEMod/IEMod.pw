using System.IO;
using System.Xml;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.Memorials {
	[ModifiesType]
	public class mod_MemorialContainer : MemorialContainer
	{
		[ModifiesMember("PopulateMemorials")]
		private void PopulateMemorialsNew()
		{
			XmlDocument document = new XmlDocument();
			string xml; 

			string xmlPath = Path.Combine (Path.Combine (Application.dataPath, "Managed/iemod"), "MemorialEntries.xml"); // added line
			if (File.Exists (xmlPath))
			{
				xml = File.ReadAllText (xmlPath);
			} else
			{
				xml = Resources.Load ("Data/UI/BackerMemorials").ToString (); // original line
			}

			document.LoadXml(xml);

			//Debug.Log ("IEMod Debug: Entries: " + document.DocumentElement.ChildNodes.Count);
			for (int i = this.StartingIndex; i < this.StartingIndex + this.NumMemorials; i++)
			{
				if (document.DocumentElement.ChildNodes [i].FirstChild.InnerText != "" && document.DocumentElement.ChildNodes [i].LastChild.InnerText != "")
				{
					this.m_Memorials.Add (new Memorial (document.DocumentElement.ChildNodes [i].FirstChild.InnerText, document.DocumentElement.ChildNodes [i].LastChild.InnerText));
				}
			}

			this.m_Loaded = true;
		}
	}
}
