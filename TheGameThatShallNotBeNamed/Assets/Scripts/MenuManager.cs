using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO;
public class MenuManager : MonoBehaviour {
	
	public GameObject mainMenu;
	public GameObject workshopMenu;
	public GameObject destMenu;
	public GameObject tavernMenu;
	public GameObject equipmentPrefab;

	Equipment equip;
	
	//set correct menu options to active
	void Start(){
		equip = GameObject.Find ("PersistentData").GetComponent<Equipment>();
		destMenu.SetActive(false);
		workshopMenu.SetActive(false);
		tavernMenu.SetActive(false);
		SetupWorkshop();
	}
	
	//setup workshop
	public void SetupWorkshop(){
		int currentY = 200;
		for(int i=0;i<equip.allWeapons.Count;i++){
			GameObject currentEquip = equipmentPrefab;
			GameObject imgObj = currentEquip.transform.FindChild("Image").gameObject;
			//set image = equipment image path
			GameObject nameObj = currentEquip.transform.FindChild("NameDesc").gameObject;
			nameObj.GetComponent<Text>().text = equip.allWeapons[i].name;
			GameObject statsObj = currentEquip.transform.FindChild("Stats").gameObject;
			string text = equip.allWeapons[i].str + "    " + equip.allWeapons[i].end + "     " + equip.allWeapons[i].agi + "     " + equip.allWeapons[i].mag + "     " + equip.allWeapons[i].luck + "    " + equip.allWeapons[i].rangeMin + " - " + equip.allWeapons[i].rangeMax;
			statsObj.GetComponent<Text>().text = text;
			GameObject recipeObj = currentEquip.transform.FindChild("Recipe").gameObject;
			
			GameObject createObj = currentEquip.transform.FindChild("Create").gameObject;
			GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(workshopMenu.transform.position.x-150.0f,workshopMenu.transform.position.y+currentY,0.0f), Quaternion.identity);
			finished.transform.parent = workshopMenu.transform;
			currentY -= 75;
		}
		
		for(int i=0;i<equip.allArmor.Count;i++){
			GameObject currentEquip = equipmentPrefab;
			GameObject imgObj = currentEquip.transform.FindChild("Image").gameObject;
			//set image = equipment image path
			GameObject nameObj = currentEquip.transform.FindChild("NameDesc").gameObject;
			nameObj.GetComponent<Text>().text = equip.allArmor[i].name;
			GameObject statsObj = currentEquip.transform.FindChild("Stats").gameObject;
			string text = equip.allArmor[i].str + "    " + equip.allArmor[i].end + "     " + equip.allArmor[i].agi + "     " + equip.allArmor[i].mag + "     " + equip.allArmor[i].luck + "     " ;
			statsObj.GetComponent<Text>().text = text;
			GameObject recipeObj = currentEquip.transform.FindChild("Recipe").gameObject;
			
			GameObject createObj = currentEquip.transform.FindChild("Create").gameObject;
			GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(workshopMenu.transform.position.x-150.0f,workshopMenu.transform.position.y+currentY,0.0f), Quaternion.identity);
			finished.transform.parent = workshopMenu.transform;
			currentY -= 75;
		}
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
		root.Add(new XAttribute("name", "New Member"));
		root.Add(new XAttribute("desc", "A new recruit"));
		root.Add(new XAttribute("health", "50"));
		root.Add(new XAttribute("str", "7"));
		root.Add(new XAttribute("end", "7"));
		root.Add(new XAttribute("agi", "7"));
		root.Add(new XAttribute("mag", "7"));
		root.Add(new XAttribute("luck", "7"));
		root.Add(new XAttribute("range", "2"));
		root.Add(new XAttribute("active", "True"));
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
}
