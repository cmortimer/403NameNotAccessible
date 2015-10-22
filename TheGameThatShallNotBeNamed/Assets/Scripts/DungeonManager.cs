using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class DungeonManager : MonoBehaviour
{

    private int currentFloor;
    private string dungeonName;
    private int floorLimit;
    private string pool;
    private bool loaded;
    private string filePath;

    // Use this for initialization
    void Start()
    {
        cleanXML();
        //Don't destroy on scene load
        GameObject.DontDestroyOnLoad(gameObject);
        currentFloor = 1;
        loaded = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug floor skip
        if (Input.GetKeyDown(KeyCode.Space))
        {
            incrementFloor();
            //Application.LoadLevel(0);
        }
    }

    void Awake()
    {
        filePath = Application.dataPath + @"/Dungeons/DungeonList.xml";

    }

    void loadXMLDungeon()
    {

        XmlDocument dunXML = new XmlDocument();
        //Print filepath and make sure it's valid
        //Debug.Log("FILEPATH: " + filePath);
        if (File.Exists(filePath))
        {
            dunXML.Load(filePath);

            //List of our possible dungeons
            XmlNodeList dungeons = dunXML.GetElementsByTagName("dungeon");

            //Go through each dungeon until we find the active one
            foreach (XmlNode member in dungeons)
            {
                if (member.LastChild.InnerText == "true")
                {
                    dungeonName = member["name"].InnerText;
                    floorLimit = int.Parse(member["floors"].InnerText);
                    pool = member["enemyPool"].InnerText;
                }

            }

            Debug.Log("Dungeon: " + dungeonName + " Floors: " + floorLimit + " Pool: " + pool);
        }
    }

    void OnLevelWasLoaded(int level)
    {
        //Debug.Log("Loading :" + level);
        //Are we loading the dungeon level?
        if (level == 2)
        {
            //First time loading a dungeon
            if (!loaded)
            {
                loadXMLDungeon();
                loaded = true;
            }
        }
    }

    void cleanXML()
    {
        XmlDocument dunXML = new XmlDocument();
        //Print filepath and make sure it's valid
       // Debug.Log("FILEPATH: " + filePath);
        if (File.Exists(filePath))
        {
            Debug.Log("File found for cleaning");
            dunXML.Load(filePath);

            //List of our possible dungeons
            XmlNodeList dungeons = dunXML.GetElementsByTagName("dungeon");

            //Go through each dungeon until we find the active one, then reset it
            foreach (XmlNode member in dungeons)
            {
                if (member.LastChild.InnerText == "true")
                {
                    member.LastChild.InnerText = "false";
                    Debug.Log("Left dungeon: " + member.LastChild.InnerText);
                }

            }
        }
        dunXML.Save(filePath);
    }

    public void incrementFloor()
    {
        currentFloor++;
        //Reset values when dungeon is complete
        if (currentFloor > floorLimit)
        {
            loaded = false;
            currentFloor = 1;
            cleanXML();
            Application.LoadLevel("Menu");
        }
        //If not complete, proceed to the next floor
        else
        {
            Application.LoadLevel(2);
        }

    }

    //Returns the spawn pool
    public string getPool()
    {
        return pool;
    }
}
