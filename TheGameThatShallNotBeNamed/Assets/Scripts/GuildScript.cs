using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class GuildScript : MonoBehaviour {

    public GameObject playerPrefab;
	public PlayerData inventory;
    public Equipment equip;

    private string filePath;

	// Use this for initialization
	void Start () {
        equip = GameObject.FindGameObjectWithTag("Persistent").GetComponent<Equipment>();
		inventory = GameObject.FindGameObjectWithTag("Persistent").GetComponent<PlayerData>();

        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            //Test creating a player
            GameObject testPlayer = playerPrefab;
			loadFromData(testPlayer.GetComponent<Character>(), testPlayer);

        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Awake()
    {
		filePath = Application.dataPath + @"/Resources/XML/GuildList.xml";
        
    }
	//loads data from struct
	void loadFromData(Character c, GameObject player){
		Vector3 spawnPos = new Vector3(5.0f, 0.5f, 24.0f);

		for(int i=0;i<inventory.allPlayers.Count;i++){
			int range = equip.allWeapons[inventory.allPlayers[i].weaponID].rangeMax;
			int str = inventory.allPlayers[i].str + equip.allWeapons[inventory.allPlayers[i].weaponID].str;
			int end = inventory.allPlayers[i].end + equip.allWeapons[inventory.allPlayers[i].weaponID].end;
			int agi = inventory.allPlayers[i].agi + equip.allWeapons[inventory.allPlayers[i].weaponID].agi;
			int mag = inventory.allPlayers[i].mag + equip.allWeapons[inventory.allPlayers[i].weaponID].mag;
			int luck = inventory.allPlayers[i].luck + equip.allWeapons[inventory.allPlayers[i].weaponID].luck;

			c.setStats(inventory.allPlayers[i].name, inventory.allPlayers[i].health, str, end, agi, mag, luck, range);

			GameObject currentPlayer = (GameObject)GameObject.Instantiate(player, spawnPos, Quaternion.identity);
			spawnPos.x += 0.0f;
			spawnPos.z -= 1.0f;
		}
	}


  
}
