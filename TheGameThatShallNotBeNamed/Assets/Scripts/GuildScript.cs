using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class GuildScript : MonoBehaviour {

    public GameObject playerPrefab;

    private string filePath;

	// Use this for initialization
	void Start () {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            //Test creating a player
            GameObject testPlayer = playerPrefab;

            loadFromXML(testPlayer.GetComponent<Character>());

            GameObject.Instantiate(testPlayer, new Vector3(0, 0.5f, 20), Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Awake()
    {
        filePath = Application.dataPath + @"/Characters/GuildList.xml";
        
    }

    //Loads a character's data from an xml file
    void loadFromXML(Character c)
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
			bool active = false;

            foreach(XmlNode member in characters)
            {
                foreach(XmlNode val in member)
                {
                    //Store values
                    if (val.Name == "name")
                        name = val.InnerText;
                    else if (val.Name == "health")
                        hp = int.Parse(val.InnerText);
                    else if (val.Name == "strength")
                        st = int.Parse(val.InnerText);
                    else if (val.Name == "endurance")
                        en = int.Parse(val.InnerText);
                    else if (val.Name == "agility")
                        ag = int.Parse(val.InnerText);
                    else if (val.Name == "magic")
                        mg = int.Parse(val.InnerText);
                    else if (val.Name == "luck")
                        lu = int.Parse(val.InnerText);
                    else if (val.Name == "range")
                        rng = int.Parse(val.InnerText);
					else if (val.Name == "active")
						active = bool.Parse(val.InnerText);
                }
                if(name != "" && active)
                {
                    Debug.Log("STATS: " + name + ", " + hp + ", " + st + ", " + en + ", " + ag + ", " + mg + ", " + lu + ", " + rng);
                    c.setStats(name, hp, st, en, ag, mg, lu, rng);
                }
            }
        }
    }
}
