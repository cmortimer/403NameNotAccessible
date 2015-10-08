using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class PlayerData : MonoBehaviour {
	/*will contain, possibly a text file for save data
	 *all enemy types and their stats/drops/where they spawn
	 *all equipment; armor and weapons
	 *list of all people in player's current party
	 *current quests
	*/

	/*
	 * everything for equipment
	 */
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
	public Equipment equipment;
	List<weapon> obtainedWeapons;
	List<armor> obtainedArmor;

	public static PlayerData Instance;
	// Use this for initialization
	void Awake () {
		if(Instance)
		{
			DestroyImmediate(gameObject);
		}
		else
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}

		equipment = this.gameObject.GetComponent<Equipment>();
		equipment.LoadEquipment("Assets\\Scripts\\EquipmentList.txt");
	}


	// Update is called once per frame
	void Update () {
	
	}
}
