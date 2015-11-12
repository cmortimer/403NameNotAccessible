using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System;

public class MenuManager : MonoBehaviour {
	
	public GameObject mainMenu;
	public GameObject workshopMenu;
	public GameObject destMenu;
	public GameObject tavernMenu;
	public GameObject equipmentPrefab;
    public GameObject charStatPrefab;

	List<GameObject> weaponButtons;
	List<GameObject> armorButtons;

	Equipment equip;
	
	//set correct menu options to active
	void Start(){
		equip = GameObject.Find ("PersistentData").GetComponent<Equipment>();
		destMenu.SetActive(false);
		workshopMenu.SetActive(false);
		tavernMenu.SetActive(false);
		SetupWorkshop();
        setUpGuild();
        
	}
	
	//setup workshop
	public void SetupWorkshop(){
		int currentY = 200;
		for(int i=0;i<equip.allWeapons.Count;i++){
			GameObject currentEquip = equipmentPrefab;
			GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(workshopMenu.transform.position.x-200.0f,workshopMenu.transform.position.y+currentY,0.0f), Quaternion.identity);
			finished.transform.parent = workshopMenu.transform.FindChild("Window/AllObjects");

			GameObject imgObj = finished.transform.FindChild("Image").gameObject;
			//set image = equipment image path
			GameObject nameObj = finished.transform.FindChild("NameDesc").gameObject;
			nameObj.GetComponent<Text>().text = equip.allWeapons[i].name;
			GameObject statsObj = finished.transform.FindChild("Stats").gameObject;
			string text = equip.allWeapons[i].str + "    " + equip.allWeapons[i].end + "     " + equip.allWeapons[i].agi + "     " + equip.allWeapons[i].mag + "     " + equip.allWeapons[i].luck + "    " + equip.allWeapons[i].rangeMin + " - " + equip.allWeapons[i].rangeMax;
			statsObj.GetComponent<Text>().text = text;
			GameObject recipeObj = finished.transform.FindChild("Recipe").gameObject;

			GameObject createObj = finished.transform.FindChild("Create").gameObject;
			int captured = i;
			createObj.GetComponent<Button>().onClick.AddListener(() => GiveWeapon(captured));

			currentY -= 75;
		}
		
		for(int i=0;i<equip.allArmor.Count;i++){
			GameObject currentEquip = equipmentPrefab;
			GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(workshopMenu.transform.position.x-200.0f,workshopMenu.transform.position.y+currentY,0.0f), Quaternion.identity);
			finished.transform.parent = workshopMenu.transform.FindChild("Window/AllObjects");

			GameObject imgObj = finished.transform.FindChild("Image").gameObject;
			//set image = equipment image path
			GameObject nameObj = finished.transform.FindChild("NameDesc").gameObject;
			nameObj.GetComponent<Text>().text = equip.allArmor[i].name;
			GameObject statsObj = finished.transform.FindChild("Stats").gameObject;
			string text = equip.allArmor[i].str + "    " + equip.allArmor[i].end + "     " + equip.allArmor[i].agi + "     " + equip.allArmor[i].mag + "     " + equip.allArmor[i].luck + "     " ;
			statsObj.GetComponent<Text>().text = text;
			GameObject recipeObj = finished.transform.FindChild("Recipe").gameObject;
			
			GameObject createObj = finished.transform.FindChild("Create").gameObject;
			int captured = i;
			createObj.GetComponent<Button>().onClick.AddListener(() => GiveArmor(captured));
			//armorButtons.Add(createObj);

			currentY -= 75;
		}

	}



    //Set up the guild list
    public void setUpGuild()
    {
		//Clear old window if it exists
		Transform oldMenu = tavernMenu.transform.FindChild("Window/AllObjects");
		foreach (Transform child in oldMenu)
		{
			Destroy(child.gameObject);
		}

//        Debug.Log("Setting up tavern stats");
        int currentY = 180;
		int posInArray = 0;

        XmlDocument xmlDoc = new XmlDocument();

        string path = Application.dataPath + @"/Characters/GuildList.xml";

        if (File.Exists(path))
        {
            xmlDoc.Load(path);
            XmlNodeList members = xmlDoc.GetElementsByTagName("char");



            foreach (XmlNode mem in members)
            {
                //Stats
                string name = "";
                int hp = 0;
                int st = 0;
                int en = 0;
                int ag = 0;
                int mg = 0;
                int lu = 0;
                int rng = 0;
				bool active = false;
                foreach (XmlAttribute val in mem.Attributes)
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

					if(val.Name == "active"){
						if(val.InnerText.Equals("True"))
						   	active = true;
						else
							active = false;
					}
                }

                GameObject currentChar = charStatPrefab;
                GameObject finished = (GameObject)Instantiate(currentChar, new Vector3(tavernMenu.transform.position.x - 200.0f, tavernMenu.transform.position.y + currentY, 0.0f), Quaternion.identity);
                finished.transform.parent = tavernMenu.transform.FindChild("Window/AllObjects");

                GameObject imgObj = finished.transform.FindChild("Image").gameObject;
                //set image = equipment image path
                GameObject nameObj = finished.transform.FindChild("NameDesc").gameObject;
                nameObj.GetComponent<Text>().text = name;
                GameObject statsObj = finished.transform.FindChild("Stats").gameObject;
                string text = hp + "     " + st + "     " + en + "      " +ag + "      " + mg + "      " +lu + "     " + rng;
                
                statsObj.GetComponent<Text>().text = text;

				GameObject activeButton = finished.transform.FindChild("Active").gameObject;

				if(active){
					Color col = activeButton.GetComponent<Image>().color;
					activeButton.GetComponent<Image>().color = new Color(col.g,col.r,col.b);
				}

				int captured = posInArray;
				activeButton.GetComponent<Button>().onClick.AddListener(() => ChangeColor(activeButton.GetComponent<Image>()));
				activeButton.GetComponent<Button>().onClick.AddListener(() => ToggleActive(captured));

                //Set edit button to edit the correct character
                GameObject editButton = finished.transform.FindChild("Edit").gameObject;
                editButton.GetComponent<Button>().onClick.AddListener(() => setUpCharacterEquipment(name));

				posInArray++;

                currentY -= 75;
            }
        }
    }

    public void setUpCharacterEquipment(string n)
    {
        string charName = n;
        int currentY = 200;

        //Clear old window
        Transform oldMenu = tavernMenu.transform.FindChild("Window/AllObjects");
        foreach(Transform child in oldMenu)
        {
            Destroy(child.gameObject);
        }

        //Add weapons
        XmlDocument wepDoc = new XmlDocument();
        string path = Application.dataPath + @"/ItemsAndEquipment/WeaponInventory.xml";

        if (File.Exists(path))
        {
            wepDoc.Load(path);

            XmlNodeList members = wepDoc.GetElementsByTagName("weapon");
            List<int> wepIDs = new List<int>();

            //Get held weapons
            foreach (XmlNode mem in members)
            {
                Debug.Log(mem.Attributes);
                wepIDs.Add(int.Parse(mem.Attributes.GetNamedItem("id").Value));
            }

            foreach(int i in wepIDs)
            {
                int id = i;
                GameObject currentEquip = equipmentPrefab;
                GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(tavernMenu.transform.position.x - 250.0f, tavernMenu.transform.position.y + currentY, 0.0f), Quaternion.identity);
                finished.transform.parent = tavernMenu.transform.FindChild("Window/AllObjects");

                GameObject imgObj = finished.transform.FindChild("Image").gameObject;
                //set image = equipment image path
                GameObject nameObj = finished.transform.FindChild("NameDesc").gameObject;
                nameObj.GetComponent<Text>().text = equip.allWeapons[i].name;
                GameObject statsObj = finished.transform.FindChild("Stats").gameObject;
                string text = equip.allWeapons[i].str + "    " + equip.allWeapons[i].end + "     " + equip.allWeapons[i].agi + "     " + equip.allWeapons[i].mag + "     " + equip.allWeapons[i].luck + "    " + equip.allWeapons[i].rangeMin + " - " + equip.allWeapons[i].rangeMax;
                statsObj.GetComponent<Text>().text = text;
                GameObject recipeObj = finished.transform.FindChild("Recipe").gameObject;

                GameObject createObj = finished.transform.FindChild("Create").gameObject;
                int captured = i;
                createObj.transform.FindChild("Text").GetComponent<Text>().text = "Equip";
                createObj.GetComponent<Button>().onClick.AddListener(() => EquipWeapon(charName, id));

                currentY -= 75;
            }
        }


        //Add armor
        XmlDocument armDoc = new XmlDocument();
        path = Application.dataPath + @"/ItemsAndEquipment/ArmorInventory.xml";

        if (File.Exists(path))
        {
            armDoc.Load(path);

            XmlNodeList members = armDoc.GetElementsByTagName("armor");
            List<int> armIDs = new List<int>();

            //Get held weapons
            foreach (XmlNode mem in members)
            {
                Debug.Log(mem.Attributes);
                armIDs.Add(int.Parse(mem.Attributes.GetNamedItem("id").Value));
            }

            foreach (int i in armIDs)
            {
                int id = i;
                GameObject currentEquip = equipmentPrefab;
                GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(tavernMenu.transform.position.x - 250.0f, tavernMenu.transform.position.y + currentY, 0.0f), Quaternion.identity);
                finished.transform.parent = tavernMenu.transform.FindChild("Window/AllObjects");

                GameObject imgObj = finished.transform.FindChild("Image").gameObject;
                //set image = equipment image path
                GameObject nameObj = finished.transform.FindChild("NameDesc").gameObject;
                nameObj.GetComponent<Text>().text = equip.allArmor[i].name;
                GameObject statsObj = finished.transform.FindChild("Stats").gameObject;
                string text = equip.allArmor[i].str + "    " + equip.allArmor[i].end + "     " + equip.allArmor[i].agi + "     " + equip.allArmor[i].mag + "     " + equip.allArmor[i].luck;
                statsObj.GetComponent<Text>().text = text;
                GameObject recipeObj = finished.transform.FindChild("Recipe").gameObject;

                GameObject createObj = finished.transform.FindChild("Create").gameObject;
                int captured = i;
                createObj.transform.FindChild("Text").GetComponent<Text>().text = "Equip";
                createObj.GetComponent<Button>().onClick.AddListener(() => EquipArmor(charName, id));

                currentY -= 75;
            }
        }
    }

	void ChangeColor(Image img){
		float g = img.color.r;
		float r = img.color.g;
		float b = img.color.b;
		img.color = new Color(r,g,b);
	}

	void ToggleActive(int position){
		Debug.Log (position);
		XmlDocument xmlDoc = new XmlDocument();
		string path = Application.dataPath + @"/Characters/GuildList.xml";
		string activeVal = "";

		if (File.Exists(path)){
			xmlDoc.Load(path);
			XmlNodeList members = xmlDoc.GetElementsByTagName("char");
			//Debug.Log(members[position-1]);
			foreach(XmlAttribute val in members[position].Attributes){
				if(val.Name == "active"){
					activeVal = val.InnerText;
					if(val.InnerText.Equals("True")){
						val.InnerText = "False";
					}
					else if(val.InnerText.Equals("False")){
						val.InnerText = "True";
					}
					break;
				}
			}
			xmlDoc.Save(path);

		}
	}


    void GiveWeapon(int i){
		GameObject perData = GameObject.FindGameObjectWithTag("Persistent");

		Debug.Log (perData.GetComponent<PlayerData>().obtainedWeapons.Count);
		perData.GetComponent<PlayerData>().obtainedWeapons.Add(equip.allWeapons[i]);

        saveXML(i, true);
	}
	void GiveArmor(int i){
		GameObject perData = GameObject.FindGameObjectWithTag("Persistent");

		Debug.Log (perData.GetComponent<PlayerData>().obtainedArmor.Count);
		perData.GetComponent<PlayerData>().obtainedArmor.Add(equip.allArmor[i]);

        saveXML(i, false);
	}

    //Equips a weapon to a character
    void EquipWeapon(string n, int i)
    {
        string path = Application.dataPath + @"/Characters/GuildList.xml";

        //Load the character document
        XmlDocument xmlDoc = new XmlDocument();
        if(File.Exists(path))
        {
            xmlDoc.Load(path);
            XmlNodeList members = xmlDoc.GetElementsByTagName("char");

            //Loop through the guild, and change the relevant members equipment
            foreach(XmlNode mem in members)
            {
                if(mem.Attributes.GetNamedItem("name").Value == n)
                {
                    mem.Attributes.GetNamedItem("weaponID").Value = i.ToString();
                }
            }
        }

        xmlDoc.Save(path);
        
    }

    //Equips armor to a character
    void EquipArmor(string n, int i)
    {
        string path = Application.dataPath + @"/Characters/GuildList.xml";

        //Load the character document
        XmlDocument xmlDoc = new XmlDocument();
        if (File.Exists(path))
        {
            xmlDoc.Load(path);
            XmlNodeList members = xmlDoc.GetElementsByTagName("char");

            //Loop through the guild, and change the relevant members equipment
            foreach (XmlNode mem in members)
            {
                if (mem.Attributes.GetNamedItem("name").Value == n)
                {
                    mem.Attributes.GetNamedItem("armorID").Value = i.ToString();
                }
            }
        }

        xmlDoc.Save(path);

    }

    //function handlers for each button yes this is ugly lay off...
    public void MainToDest(){
		mainMenu.SetActive(false);
		destMenu.SetActive(true);
	}
	public void DestToMain(){
		destMenu.SetActive(false);
		mainMenu.SetActive(true);
	}
	public void MainToWorkshop(){
		mainMenu.SetActive(false);
		workshopMenu.SetActive(true);
	}
	public void MainToTavern(){
		mainMenu.SetActive(false);
		tavernMenu.SetActive(true);
	}
	public void TaverToMain(){
		mainMenu.SetActive(true);
		tavernMenu.SetActive(false);

        setUpGuild();
	}
	public void WorkToMain(){
		workshopMenu.SetActive(false);
		mainMenu.SetActive(true);
	}
	public void Quit(){
		Application.Quit();
	}
	public void VisitDest(int level){
		Application.LoadLevel (2);
	}

	public void RecruitGuildMember(){
		XDocument doc = XDocument.Load("Assets/Characters/GuildList.xml");
		XElement root = new XElement("char");
		root.Add(new XAttribute("name", genName()));
		root.Add(new XAttribute("desc", "A new recruit"));
		root.Add(new XAttribute("health", "50"));
		root.Add(new XAttribute("str", "7"));
		root.Add(new XAttribute("end", "7"));
		root.Add(new XAttribute("agi", "7"));
		root.Add(new XAttribute("mag", "7"));
		root.Add(new XAttribute("luck", "7"));
		root.Add(new XAttribute("range", "2"));
        root.Add(new XAttribute("weaponID", "0"));
        root.Add(new XAttribute("armorID", "0"));
        root.Add(new XAttribute("active", "False"));
		doc.Element("guild").Add(root);
		doc.Save("Assets/Characters/GuildList.xml");
	}

	public void RemoveGuildMember(){
		
	}

    //Loads a dungeon based off of its name
    public void goToDest(string name)
    {
        //Debug.Log("Going to " + name);
        string filePath = Application.dataPath + @"/Dungeons/DungeonList.xml";

        XmlDocument dunXML = new XmlDocument();

        if (File.Exists(filePath))
        {
            dunXML.Load(filePath);

            //List of our possible dungeons
            XmlNodeList dungeons = dunXML.GetElementsByTagName("dungeon");

            //Set the selected dungeon to active
            foreach (XmlNode member in dungeons)
            {
                if (member["name"].InnerText == name)
                {
                    member["active"].InnerText = "true";
                    dunXML.Save(filePath);
                }

            }
        }
        Application.LoadLevel(2);
    }

    public void saveXML(int i, bool wep)
    {
        XmlDocument xmlDoc = new XmlDocument();
        string path;

        //Add a weapon
        if(wep)
        {
            path = Application.dataPath + @"/ItemsAndEquipment/WeaponInventory.xml";

            if (File.Exists(path))
            {
                xmlDoc.Load(path);

                XmlNodeList weapons = xmlDoc.GetElementsByTagName("weapon");

                    bool newEntry = true;

                    //Check to see if we already have that item
                    foreach (XmlNode member in weapons)
                    {
                        if (member.Attributes["id"].Value == i.ToString())
                        {
                            //Update the count of the item
                            Debug.Log("Found a: " + member.Attributes["name"].Value);
                            int currentCount = int.Parse(member.Attributes["count"].Value);
                            currentCount++;
                            member.Attributes["count"].Value = currentCount.ToString();

                            //Don't need to make a new entry
                            newEntry = false;
                        }
                    }

                    //If we don't have that item yet
                    if (newEntry)
                    {
                        Debug.Log("Adding a new " + equip.allWeapons[i].name);

                        //Create the new item
                        XmlNodeList root = xmlDoc.GetElementsByTagName("inventory");
                        XmlElement newItem = xmlDoc.CreateElement("weapon");
                        newItem.SetAttribute("name", equip.allWeapons[i].name);
                        newItem.SetAttribute("id", i.ToString());
                        newItem.SetAttribute("count", "1");
                        root[0].AppendChild(newItem);
                    }

                }
                xmlDoc.Save(Application.dataPath + @"/ItemsAndEquipment/WeaponInventory.xml");
            }

        //Add armor
        else
        {
            path = Application.dataPath + @"/ItemsAndEquipment/ArmorInventory.xml";

            if (File.Exists(path))
            {
                xmlDoc.Load(path);

                XmlNodeList armor = xmlDoc.GetElementsByTagName("armor");

                bool newEntry = true;

                //Check to see if we already have that item
                foreach (XmlNode member in armor)
                {
                    if (member.Attributes["id"].Value == i.ToString())
                    {
                        //Update the count of the item
                        Debug.Log("Found a: " + member.Attributes["name"].Value);
                        int currentCount = int.Parse(member.Attributes["count"].Value);
                        currentCount++;
                        member.Attributes["count"].Value = currentCount.ToString();

                        //Don't need to make a new entry
                        newEntry = false;
                    }
                }

                //If we don't have that item yet
                if (newEntry)
                {
                    Debug.Log("Adding a new " + equip.allWeapons[i].name);

                    //Create the new item
                    XmlNodeList root = xmlDoc.GetElementsByTagName("inventory");
                    XmlElement newItem = xmlDoc.CreateElement("armor");
                    newItem.SetAttribute("name", equip.allArmor[i].name);
                    newItem.SetAttribute("id", i.ToString());
                    newItem.SetAttribute("count", "1");
                    root[0].AppendChild(newItem);
                }

            }
            xmlDoc.Save(Application.dataPath + @"/ItemsAndEquipment/ArmorInventory.xml");

        }
    }

    //Returns a random name because neww member is boring
    private string genName()
    {
        string[] nameList = { "Concetta", "Bethany", "Krystina", "Chi", "Eugenia", "Carmon", "Kemberly", "Temple", "Layne", "Stormy", "Lakiesha", "Bertie", "Sherill", "Christopher", "Tristan", "Troy", "Darleen", "Josette", "Silvia", "Bret", "Ernesto", "Nancy", "Kyung", "Ozie", "Evalyn", "Bernard", "Shelly", "Cyndy", "Veronica", "Porter", "Priscilla", "Aleisha", "Lyla", "Pete", "Stacee", "Basilia", "Afton", "Douglass", "Wilda", "Andera", "Misti", "Anton", "Chase", "Marlo", "Darcel", "Yvette", "Joy", "Reagan", "Penni", "Terrence" };


        int i = UnityEngine.Random.Range(0, 50);

        return nameList[i];
    }
}
