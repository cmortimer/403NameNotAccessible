using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

public class PlayerData : MonoBehaviour {
	#region variables
	// list of all people in player's current party
	public List<PlayerShell> allPlayers = new List<PlayerShell>();

	//Equipment
	Equipment equipment;
	//have null count of weapons never before found!!! beware!!!
	//obtainedweapons[i] = # of equipment.weapon[i] objects in inventory
	public Hashtable obtainedWeapons = new Hashtable();
	//obtainedarmor[i] = # of equipment.armor[i] objects in inventory
	public Hashtable obtainedArmor = new Hashtable();
	//obtainedItems["Item Name"] = # of item in inventory
	public Hashtable obtainedItems = new Hashtable();
	
	public static PlayerData Instance;
	#endregion

	// Use this for initialization
	void Awake () {
		if(Instance)
		{
			DestroyImmediate(gameObject);
		}
		else
		{
			if (GameObject.FindGameObjectWithTag("Persistent") != null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
		}
        
		equipment = this.gameObject.GetComponent<Equipment>();
		equipment.LoadEquipment();

		LoadObtainedEquipment();
		LoadGuild();
	}


	void LoadObtainedEquipment(){

		XmlDocument xmlDoc = new XmlDocument();

		string path = Application.dataPath + @"/ItemsAndEquipment/WeaponInventory.xml";

		if (File.Exists(path))
		{
			xmlDoc.Load(path);

			//load weapons
			XmlNodeList weapons = xmlDoc.GetElementsByTagName("weapon");

			foreach(XmlNode wep in weapons){
				for(int i=0; i<equipment.allWeapons.Count;i++){
					if(wep.Attributes["name"].InnerText.Equals(equipment.allWeapons[i].name)){
						obtainedWeapons.Add(i, int.Parse(wep.Attributes["count"].InnerText));
						break;
					}
				}
			}
		}

		path = Application.dataPath + @"/ItemsAndEquipment/ArmorInventory.xml";

		if (File.Exists(path))
		{
			xmlDoc.Load(path);
			
			//load weapons
			XmlNodeList armor = xmlDoc.GetElementsByTagName("armor");
			
			foreach(XmlNode arm in armor){
				for(int i=0; i<equipment.allArmor.Count;i++){
					if(arm.Attributes["name"].InnerText.Equals(equipment.allArmor[i].name)){
						obtainedArmor.Add(i, int.Parse(arm.Attributes["count"].InnerText));
						break;
					}
				}
			}
		}

		path = Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml";
		
		if (File.Exists(path))
		{
			xmlDoc.Load(path);
			
			//load weapons
			XmlNodeList item = xmlDoc.GetElementsByTagName("item");
			
			foreach(XmlNode it in item){
				string name = it.Attributes["name"].InnerText;
				obtainedItems[name] = int.Parse(it.Attributes["count"].InnerText);
			}
		}

		/*for(int i=0;i<equipment.allWeapons.Count;i++){
			//Debug.Log (equipment.allWeapons[i].name + " # of copies: " + obtainedWeapons[i]);
		}
		for(int i=0;i<equipment.allArmor.Count;i++){
			//Debug.Log (equipment.allArmor[i].name + " # of copies: " + obtainedArmor[i]);
		}*/

	}

	void LoadGuild(){

		XmlDocument xmlDoc = new XmlDocument();

		string path = Application.dataPath + @"/Characters/GuildList.xml";
		
		if (File.Exists(path))
		{
			xmlDoc.Load(path);
			
			//load players
			XmlNodeList players = xmlDoc.GetElementsByTagName("char");
			
			foreach(XmlNode player in players){

				PlayerShell p = new PlayerShell();
				p.name = player.Attributes["name"].InnerText;
				p.desc = player.Attributes["desc"].InnerText;
				p.health = int.Parse( player.Attributes["health"].InnerText);
				p.str = int.Parse(player.Attributes["str"].InnerText);
				p.end = int.Parse(player.Attributes["end"].InnerText);
				p.agi = int.Parse(player.Attributes["agi"].InnerText);
				p.mag = int.Parse(player.Attributes["mag"].InnerText);
				p.luck = int.Parse(player.Attributes["luck"].InnerText);
				p.weaponID = int.Parse(player.Attributes["weaponID"].InnerText);
				p.armorID = int.Parse(player.Attributes["armorID"].InnerText);
				if(player.Attributes["active"].InnerText.Equals ("False"))
					p.active = false;
				else
					p.active = true;

				allPlayers.Add(p);
			}
		}
	}
}

//playershell needs to be a class for modifying values
public class PlayerShell{
	public string name;
	public string desc;
	public int health, str, end, agi, mag, luck, weaponID, armorID;
	public bool active;
}

