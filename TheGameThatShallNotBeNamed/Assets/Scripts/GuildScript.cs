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
        filePath = Application.dataPath + @"/Characters/GuildList.xml";
        
    }
	//loads data from struct
	void loadFromData(Character c, GameObject player){
		Vector3 spawnPos = new Vector3(5.0f, 0.5f, 24.0f);

		for(int i=0;i<inventory.allPlayers.Count;i++){
			int range = equip.allWeapons[inventory.allPlayers[i].weaponID].rangeMax;
			c.setStats(inventory.allPlayers[i].name, inventory.allPlayers[i].health, inventory.allPlayers[i].str, inventory.allPlayers[i].end, 
			           inventory.allPlayers[i].agi, inventory.allPlayers[i].mag, inventory.allPlayers[i].luck, range);

			GameObject currentPlayer = (GameObject)GameObject.Instantiate(player, spawnPos, Quaternion.identity);
			spawnPos.x += 0.0f;
			spawnPos.z -= 1.0f;
		}
	}


  
}
