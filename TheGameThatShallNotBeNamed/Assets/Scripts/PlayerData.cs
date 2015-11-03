using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class PlayerData : MonoBehaviour {
	/*
	 * list of all people in player's current party
	 * current quests
	 * owned items
	*/
	
	/*
	 * everything for equipment
    	 */
	Equipment equipment;
	public List<Equipment.weapon> obtainedWeapons = new List<Equipment.weapon>();
	public List<Equipment.armor> obtainedArmor = new List<Equipment.armor>();
	
	public static PlayerData Instance;

	// Use this for initialization
	void Awake () {
		if(Instance)
		{
			DestroyImmediate(gameObject);
		}
		else
		{
			if (GameObject.FindGameObjectWithTag("Persistent") == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
		}
        
		equipment = this.gameObject.GetComponent<Equipment>();
		equipment.LoadEquipment(@"Assets\Scripts\EquipmentList.txt");
	}


	// Update is called once per frame
	void Update () {
		
	}


}

