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


            //loadFromXML(testPlayer.GetComponent<Character>(), testPlayer);

            //GameObject.Instantiate(testPlayer, new Vector3(0, 0.5f, 20), Quaternion.identity);
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


    //Loads a character's data from an xml file
    void loadFromXML(Character c, GameObject player)
    {
        XmlDocument charXML = new XmlDocument();
        //Print filepath and make sure it's valid
        //Debug.Log("FILEPATH: " + filePath);
        if(File.Exists(filePath))
        {
            //Debug.Log("loaded file");
            charXML.Load(filePath);

            //Get our list of characters
            XmlNodeList characters = charXML.GetElementsByTagName("char");

            //Stats
            string name = "";
            int hp =  0;
            int st  = 0;
            int en  = 0;
            int ag  = 0;
            int mg  = 0;
            int lu  = 0;
            int rng = 0;
            int wepID = 0;
            int armID = 0;
            bool active = false;

			Vector3 spawnPos = new Vector3(5.0f, 0.5f, 24.0f);

            foreach(XmlNode member in characters)
            {
                foreach(XmlAttribute val in member.Attributes)
                {
					//Debug.Log ("test");
                    //Store values
                    if (val.Name == "name")
                        name = val.InnerText;
                    else if (val.Name == "health")
                        hp = int.Parse(val.InnerText);
                    else if (val.Name == "str")
                        st = int.Parse(val.InnerText);
                    else if (val.Name == "end")
                        en = int.Parse(val.InnerText);
                    else if (val.Name == "agi")
                        ag = int.Parse(val.InnerText);
                    else if (val.Name == "mag")
                        mg = int.Parse(val.InnerText);
                    else if (val.Name == "luck")
                        lu = int.Parse(val.InnerText);
                    else if (val.Name == "range")
                        rng = int.Parse(val.InnerText);
                    else if (val.Name == "weaponID")
                        wepID = int.Parse(val.InnerText);
                    else if (val.Name == "armorID")
                        armID = int.Parse(val.InnerText);
                    else if (val.Name == "active")
						active = bool.Parse(val.InnerText);
                }
				//Debug.Log("Name: " + name + ", active: " + active);
                if(name != "" && active)
                {
                    //Add armor and weapon stats
                    st += equip.allArmor[armID].str + equip.allWeapons[wepID].str;
                    en += equip.allArmor[armID].end + equip.allWeapons[wepID].end;
                    ag += equip.allArmor[armID].agi + equip.allWeapons[wepID].agi;
                    mg += equip.allArmor[armID].mag + equip.allWeapons[wepID].mag;
                    lu += equip.allArmor[armID].luck + equip.allWeapons[wepID].luck;
                    rng = equip.allWeapons[wepID].rangeMax;

//                    Debug.Log("STATS: " + name + ", " + hp + ", " + st + ", " + en + ", " + ag + ", " + mg + ", " + lu + ", " + rng);
                    c.setStats(name, hp, st, en, ag, mg, lu, rng);
                    //c.setStats(name, 999, st, en, ag, mg, lu, rng);     //Testing with max hp

                    GameObject currentPlayer = (GameObject)GameObject.Instantiate(player, spawnPos, Quaternion.identity);
					spawnPos.x += 0.0f;
					spawnPos.z -= 1.0f;
                }
            }
        }
    }
}
