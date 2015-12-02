using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

public class Equipment : MonoBehaviour{
	//a single weapon
	public struct weapon{
		public string name;
		public string desc;
		public int str;
		public int end ;
		public int agi ;
		public int mag ;
		public int luck ;
		public int rangeMin ;
		public int rangeMax;
		public List<string> recipe;
		//something for an image to show
	}

	//a single piece of armor
	public struct armor{
		public string name;
		public string desc;
		public int str;
		public int end ;
		public int agi ;
		public int mag ;
		public int luck ;
		public List<string> recipe;
		//image?
	}

	//a list of every weapon that exists in the game...
	public List<weapon> allWeapons = new List<weapon>();

	//same thing for armor...
	public List<armor> allArmor = new List<armor>();

	public void LoadEquipment(){

		XmlDocument xmlDoc = new XmlDocument();

		string path = Application.dataPath + @"/Scripts/EquipmentList.xml";

		if (File.Exists(path))
		{
			xmlDoc.Load(path);

			//load weapons
			XmlNodeList weapons = xmlDoc.GetElementsByTagName("Weapon");

			foreach(XmlNode wep in weapons){
				weapon temp = new weapon();
                temp.recipe = new List<string>();
				foreach (XmlAttribute val in wep.Attributes)
				{
					//Store values
					if (val.Name == "name")
						temp.name = val.InnerText;
					else if (val.Name == "str")
						temp.str = int.Parse(val.InnerText);
					else if (val.Name == "end")
						temp.end = int.Parse(val.InnerText);
					else if (val.Name == "agi")
						temp.agi = int.Parse(val.InnerText);
					else if (val.Name == "mag")
						temp.mag = int.Parse(val.InnerText);
					else if (val.Name == "luck")
						temp.luck = int.Parse(val.InnerText);
					else if (val.Name == "rangemin")
						temp.rangeMin = int.Parse(val.InnerText);
					else if (val.Name == "rangemax")
						temp.rangeMax = int.Parse(val.InnerText);
                    else if (val.Name.Contains("recipe"))
                        temp.recipe.Add(val.InnerText);
				}
				allWeapons.Add(temp);
			}

			//load armor
			XmlNodeList armors = xmlDoc.GetElementsByTagName("Armor");

			foreach(XmlNode arm in armors){
				armor temp2 = new armor();
                temp2.recipe = new List<string>();
				foreach (XmlAttribute val in arm.Attributes)
				{

					//Store values
					if (val.Name == "name")
						temp2.name = val.InnerText;
					else if (val.Name == "str")
						temp2.str = int.Parse(val.InnerText);
					else if (val.Name == "end")
						temp2.end = int.Parse(val.InnerText);
					else if (val.Name == "agi")
						temp2.agi = int.Parse(val.InnerText);
					else if (val.Name == "mag")
						temp2.mag = int.Parse(val.InnerText);
					else if (val.Name == "luck")
						temp2.luck = int.Parse(val.InnerText);
					else if (val.Name.Contains("recipe")){
						temp2.recipe.Add(val.InnerText);
					}
				}
				allArmor.Add(temp2);
			}
		}
	} 
}
