using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System;

public class MenuManager : MonoBehaviour {

	#region Variables And Start

	public GameObject mainMenu;
	public GameObject workshopMenu;
	public GameObject destMenu;
	public GameObject tavernMenu;
	public GameObject equipmentPrefab;
    public GameObject charStatPrefab;
    public GameObject editEquipmentPrefab;

	List<GameObject> weaponButtons;
	List<GameObject> armorButtons;
	public Image[] weaponImages;

	Equipment equip;
	PlayerData inventory;
	
	//set correct menu options to active
	void Start(){
        equip = GameObject.FindGameObjectWithTag("Persistent").GetComponent<Equipment>();
		inventory = GameObject.FindGameObjectWithTag("Persistent").GetComponent<PlayerData>();
		weaponImages = GameObject.Find("AllWeaponImages").GetComponentsInChildren<Image>();
		destMenu.SetActive(false);
		workshopMenu.SetActive(false);
		tavernMenu.SetActive(false);
		SetupWorkshop();
        setUpGuild();
        
	}

	#endregion

	#region workshop
	#region full menu
	public void SetupWorkshop(){

		float currentY = -50.0f;
		Vector3 basePos = workshopMenu.transform.FindChild ("AllEquipment/Window/AllObjects").position;

		
		//change height based on total objs
		int totalObjs = equip.allWeapons.Count + equip.allArmor.Count;
		Vector2 oldSize = workshopMenu.transform.FindChild ("AllEquipment/Window/AllObjects").gameObject.GetComponent<RectTransform>().sizeDelta;
		workshopMenu.transform.FindChild ("AllEquipment/Window/AllObjects").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(oldSize.x, totalObjs*100.0f);


		for(int i=0;i<equip.allWeapons.Count;i++){
			GameObject currentEquip = equipmentPrefab;
			GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(basePos.x+180.0f,basePos.y+currentY,0.0f), Quaternion.identity);
			finished.transform.parent = workshopMenu.transform.FindChild("AllEquipment/Window/AllObjects");

			GameObject nameObj = finished.transform.FindChild("Name").gameObject;
			nameObj.GetComponent<Text>().text = equip.allWeapons[i].name;
			GameObject descObj = finished.transform.FindChild("Description").gameObject;
			descObj.GetComponent<Text>().text = equip.allWeapons[i].desc;

			GameObject imgObj = finished.transform.FindChild("Image").gameObject;
			foreach(Image weaponImg in weaponImages){
				if(equip.allWeapons[i].name.Contains(weaponImg.name)){
					imgObj.GetComponent<Image>().sprite = weaponImg.sprite;
					imgObj.GetComponent<Image>().SetNativeSize();
				}
			}

			//GameObject recipeObj = finished.transform.FindChild("Recipe").gameObject;

			GameObject selectObj = finished.transform.FindChild("Create").gameObject;
			int captured = i;
			selectObj.GetComponent<Button>().onClick.AddListener(() => SetUpSelectedWeapon(captured));

			currentY -= 100;
		}
		for(int i=0;i<equip.allArmor.Count;i++){
			GameObject currentEquip = equipmentPrefab;
			GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(basePos.x+180.0f,basePos.y+currentY,0.0f), Quaternion.identity);
			finished.transform.parent = workshopMenu.transform.FindChild("AllEquipment/Window/AllObjects");

			GameObject imgObj = finished.transform.FindChild("Image").gameObject;
			//set image = equipment image path
			GameObject nameObj = finished.transform.FindChild("Name").gameObject;
			nameObj.GetComponent<Text>().text = equip.allArmor[i].name;
			GameObject descObj = finished.transform.FindChild("Description").gameObject;
			//string text = equip.allArmor[i].str + "    " + equip.allArmor[i].end + "     " + equip.allArmor[i].agi + "     " + equip.allArmor[i].mag + "     " + equip.allArmor[i].luck + "     " ;
			descObj.GetComponent<Text>().text = equip.allArmor[i].desc;

			//GameObject recipeObj = finished.transform.FindChild("Recipe").gameObject;
			
			GameObject selectObj = finished.transform.FindChild("Create").gameObject;
			int captured = i;
			selectObj.GetComponent<Button>().onClick.AddListener(() => SetUpSelectedArmor(captured));
			//armorButtons.Add(createObj);

			currentY -= 100;
		}

	}
	#endregion

	#region current selection
	void SetUpSelectedArmor(int i){
		Equipment.armor curArm = equip.allArmor[i];
		GameObject itemObj = GameObject.Find("Item");

		itemObj.transform.FindChild("Name").gameObject.GetComponent<Text>().text = curArm.name;

		GameObject imgObj = itemObj.transform.FindChild("Image").gameObject;
		foreach(Image weaponImg in weaponImages){
			if(curArm.name.Contains(weaponImg.name)){
				imgObj.GetComponent<Image>().sprite = weaponImg.sprite;
				imgObj.GetComponent<Image>().SetNativeSize();
			}
		}
		imgObj.SetActive(true);

		itemObj.transform.FindChild("FullStats/Strength/Value").gameObject.GetComponent<Text>().text = curArm.str.ToString();
		itemObj.transform.FindChild("FullStats/Agility/Value").gameObject.GetComponent<Text>().text = curArm.agi.ToString();
		itemObj.transform.FindChild("FullStats/Luck/Value").gameObject.GetComponent<Text>().text = curArm.luck.ToString();
		itemObj.transform.FindChild("FullStats/Endurance/Value").gameObject.GetComponent<Text>().text = curArm.end.ToString();
		itemObj.transform.FindChild("FullStats/Magic/Value").gameObject.GetComponent<Text>().text = curArm.mag.ToString();
		itemObj.transform.FindChild("FullStats/Range").gameObject.SetActive(false);

		//clear old recipe text
		for(int j=1;j<7;j++){
			itemObj.transform.FindChild("RecipeFull/RecipeItem"+j).gameObject.GetComponent<Text>().text = "";
		}

		/*put in stuff for required items*/
		//Find first instance, find total number, change first instance to show total
		Hashtable jValCount = new Hashtable();
		int uiPos = 0;
		for(int j=0;j<curArm.recipe.Count;j++){
			bool cont = false;
			for(int k=0;k<j;k++){
				//if the item has already appeared
				if(curArm.recipe[k].Equals(curArm.recipe[j])){
					//add one to previous item's count
					int temp = (int)jValCount[curArm.recipe[k]];
					jValCount[curArm.recipe[k]] = temp+1;
					//update the view
					itemObj.transform.FindChild("RecipeFull/RecipeItem"+(k+1)).gameObject.GetComponent<Text>().text = 
						curArm.recipe[k] + ": " + inventory.obtainedItems[curArm.recipe[k]] + "/" + jValCount[curArm.recipe[k]];
					//skip the steps below
					cont = true;
					break;
				}
			}
			if(cont){ continue; }

			string goName = "RecipeItem" + (uiPos+1);
			if(inventory.obtainedItems[curArm.recipe[j]] == null){
				inventory.obtainedItems[curArm.recipe[j]] = 0;
			}
			jValCount.Add(curArm.recipe[j], 1);
			itemObj.transform.FindChild("RecipeFull/"+goName).gameObject.GetComponent<Text>().text = curArm.recipe[j] + ": " 
				+ inventory.obtainedItems[curArm.recipe[j]] + "/" + jValCount[curArm.recipe[j]];
			uiPos++;
		}


		GameObject selectObj = itemObj.transform.FindChild("Create").gameObject;
		selectObj.GetComponent<Button>().onClick.RemoveAllListeners();
		selectObj.GetComponent<Button>().onClick.AddListener(() => GiveArmor(i));
	}


	void SetUpSelectedWeapon(int i){
		Equipment.weapon curWep = equip.allWeapons[i];
		GameObject itemObj = GameObject.Find("Item");
		
		itemObj.transform.FindChild("Name").gameObject.GetComponent<Text>().text = curWep.name;
		
		GameObject imgObj = itemObj.transform.FindChild("Image").gameObject;
		foreach(Image weaponImg in weaponImages){
			if(curWep.name.Contains(weaponImg.name)){
				imgObj.GetComponent<Image>().sprite = weaponImg.sprite;
				imgObj.GetComponent<Image>().SetNativeSize();
			}
		}
		imgObj.SetActive(true);
		
		itemObj.transform.FindChild("FullStats/Strength/Value").gameObject.GetComponent<Text>().text = curWep.str.ToString();
		itemObj.transform.FindChild("FullStats/Agility/Value").gameObject.GetComponent<Text>().text = curWep.agi.ToString();
		itemObj.transform.FindChild("FullStats/Luck/Value").gameObject.GetComponent<Text>().text = curWep.luck.ToString();
		itemObj.transform.FindChild("FullStats/Endurance/Value").gameObject.GetComponent<Text>().text = curWep.end.ToString();
		itemObj.transform.FindChild("FullStats/Magic/Value").gameObject.GetComponent<Text>().text = curWep.mag.ToString();
		itemObj.transform.FindChild("FullStats/Range").gameObject.SetActive(true);
		itemObj.transform.FindChild("FullStats/Range/Value").gameObject.GetComponent<Text>().text = curWep.rangeMin.ToString() + " - " + curWep.rangeMax.ToString();

		//clear old recipes
		for(int j=1;j<7;j++){
			itemObj.transform.FindChild("RecipeFull/RecipeItem"+j).gameObject.GetComponent<Text>().text = "";
		}

		//work for recipes
		Hashtable jValCount = new Hashtable();
		int uiPos = 0;
		for(int j=0;j<curWep.recipe.Count;j++){
			bool cont = false;
			for(int k=0;k<j;k++){
					//if the item has already appeared
				if(curWep.recipe[k].Equals(curWep.recipe[j])){
					//add one to previous item's count
					int temp = (int)jValCount[curWep.recipe[k]];
					jValCount[curWep.recipe[k]] = temp+1;
					//update the view
					itemObj.transform.FindChild("RecipeFull/RecipeItem"+(k+1)).gameObject.GetComponent<Text>().text = 
						curWep.recipe[k] + ": " + inventory.obtainedItems[curWep.recipe[k]] + "/" + jValCount[curWep.recipe[k]];
					//skip the steps below
					cont = true;
					break;
				}
			}

			if(cont){ continue; }

			string goName = "RecipeItem" + (uiPos+1);
			if(inventory.obtainedItems[curWep.recipe[j]] == null){
				inventory.obtainedItems[curWep.recipe[j]] = 0;
			}
			jValCount.Add(curWep.recipe[j], 1);
			itemObj.transform.FindChild("RecipeFull/"+goName).gameObject.GetComponent<Text>().text = curWep.recipe[j] + ": " 
				+ inventory.obtainedItems[curWep.recipe[j]] + "/" + jValCount[curWep.recipe[j]];
			uiPos++;
			
		}

		//check if creatable and make button if it is
		bool canCreate = true;
		for(int j=1;j<7;j++){
			string line = itemObj.transform.FindChild("RecipeFull/RecipeItem"+j).gameObject.GetComponent<Text>().text.Split(':')[0];
			if(line != ""){
				if((int)inventory.obtainedItems[line] < (int)jValCount[line]){
					itemObj.transform.FindChild("RecipeFull/RecipeItem"+j).gameObject.GetComponent<Text>().color = new Color(.8f,.1f,.1f);
					canCreate = false;
				}
			}
		}

		GameObject selectObj = itemObj.transform.FindChild("Create").gameObject;
		selectObj.GetComponent<Button>().onClick.RemoveAllListeners();
		if(canCreate)
			selectObj.SetActive(true);
		else
			selectObj.SetActive(false);
		selectObj.GetComponent<Button>().onClick.AddListener(() => GiveWeapon(i));
	}
	void GiveWeapon(int i)
	{
		//give item
		int count = (int)inventory.obtainedWeapons[i];
		inventory.obtainedWeapons[i] = ++count;
		foreach(string s in equip.allWeapons[i].recipe){
			int temp = (int)inventory.obtainedItems[s];
			inventory.obtainedItems[s] = --temp;
		}
		//update selected item
		SetUpSelectedWeapon(i);

		/*
		GameObject perData = GameObject.FindGameObjectWithTag("Persistent");
		//Debug.Log(perData.GetComponent<PlayerData>().obtainedWeapons.Count);
		
		//Check recipe count
		bool craftable = true;

		XmlDocument xmlDoc = new XmlDocument();
		string path = Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml";
		
		if (File.Exists(path))
		{
			xmlDoc.Load(path);
			
			XmlNodeList members = xmlDoc.GetElementsByTagName("item");
			
			foreach (XmlNode mem in members)
			{
				//Check for the required material
				string mat = mem.Attributes.GetNamedItem("name").Value;
				if (equip.allWeapons[i].recipe.Contains(mat))
				{
					//Check how much of the material is required
					int required = 0;
					foreach(string s in equip.allWeapons[i].recipe)
					{
						if(s == mat)
						{
							required++;
						}
					}
					
					//update count
					int itemCount = int.Parse(mem.Attributes.GetNamedItem("count").Value);
					itemCount -= required;
					//Debug.Log(itemCount + " " + mat);
					//Do we have enough of the material?
					if (itemCount >= 0)
					{
						mem.Attributes.GetNamedItem("count").Value = itemCount.ToString();
					}
					else
					{
						craftable = false;
						Debug.Log("Can't craft " + equip.allWeapons[i].name);
					}
				}
			}
			
			//Only save if the weapon is craftable
			if (craftable)
			{
				//Debug.Log("Crafting " + equip.allWeapons[i].name);
				//perData.GetComponent<PlayerData>().obtainedWeapons.Add(equip.allWeapons[i]);
				
				saveXML(i, true);
				xmlDoc.Save(path);
			}
		}*/
	}
	void GiveArmor(int i)
	{
		int count = (int)inventory.obtainedArmor[i];
		inventory.obtainedArmor[i] = ++count;
		foreach(string s in equip.allWeapons[i].recipe){
			int temp = (int)inventory.obtainedItems[s];
			inventory.obtainedItems[s] = --temp;
		}

		SetUpSelectedArmor(i);

		/*
		GameObject perData = GameObject.FindGameObjectWithTag("Persistent");
		
		//Debug.Log(perData.GetComponent<PlayerData>().obtainedArmor.Count);
		
		
		//Check recipe count
		bool craftable = true;
		XmlDocument xmlDoc = new XmlDocument();
		string path = Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml";
		
		if (File.Exists(path))
		{
			xmlDoc.Load(path);
			
			XmlNodeList members = xmlDoc.GetElementsByTagName("item");
			
			foreach (XmlNode mem in members)
			{
				//Check for the required material
				string mat = mem.Attributes.GetNamedItem("name").Value;
				if (equip.allArmor[i].recipe.Contains(mat))
				{
					//Check how much of the material is required
					int required = 0;
					foreach (string s in equip.allWeapons[i].recipe)
					{
						if (s == mat)
						{
							required++;
						}
					}
					
					//update count
					int itemCount = int.Parse(mem.Attributes.GetNamedItem("count").Value);
					itemCount -= 1;
					//Debug.Log(itemCount + " " + mat);
					//Do we have enough of the material?
					if (itemCount >= 0)
					{
						mem.Attributes.GetNamedItem("count").Value = itemCount.ToString();
					}
					else
					{
						craftable = false;
						//Debug.Log("Can't craft " + equip.allArmor[i].name);
					}
				}
			}
			
			//Only save if the weapon is craftable
			if (craftable)
			{
				//Debug.Log("Crafting " + equip.allArmor[i].name);
				//perData.GetComponent<PlayerData>().obtainedArmor.Add(equip.allArmor[i]);
				
				saveXML(i, false);
				xmlDoc.Save(path);
			}
		}
		
		*/
	}
	#endregion

	#endregion

	#region guild
	//Set up the guild list
    public void setUpGuild()
    {
		//Clear old window if it exists
		Transform oldMenu = tavernMenu.transform.FindChild("Window/AllObjects");
		foreach (Transform child in oldMenu)
		{
			Destroy(child.gameObject);
		}
        //int currentY = -50;
		//int posInArray = 0;

        /*XmlDocument xmlDoc = new XmlDocument();

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
				activeButton.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "inactive";

				if(active){
					activeButton.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Active";
					Color col = activeButton.GetComponent<Image>().color;
					activeButton.GetComponent<Image>().color = new Color(col.g,col.r,col.b);
				}

				int captured = posInArray;
				activeButton.GetComponent<Button>().onClick.AddListener(() => ChangeButton(activeButton));
				activeButton.GetComponent<Button>().onClick.AddListener(() => ToggleActive(captured));

                //Set edit button to edit the correct character
                GameObject editButton = finished.transform.FindChild("Edit").gameObject;
                editButton.GetComponent<Button>().onClick.AddListener(() => setUpCharacterEquipment(name));

				posInArray++;

                currentY -= 75;
            }
        }*/

		float currentY = -50.0f;
		Vector3 basePos = tavernMenu.transform.FindChild ("Window/AllObjects").position;
		
		
		//change height based on total objs
		int totalObjs = inventory.allPlayers.Count;
		Vector2 oldSize = tavernMenu.transform.FindChild ("Window/AllObjects").gameObject.GetComponent<RectTransform>().sizeDelta;
		tavernMenu.transform.FindChild ("Window/AllObjects").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(oldSize.x, totalObjs*75.0f);

		for(int i=0;i<inventory.allPlayers.Count;i++){
			GameObject currentChar = charStatPrefab;
			GameObject finished = (GameObject)Instantiate(currentChar, new Vector3(basePos.x+55.0f, basePos.y+currentY, 0.0f), Quaternion.identity);
			finished.transform.parent = tavernMenu.transform.FindChild("Window/AllObjects");

			GameObject imgObj = finished.transform.FindChild("Image").gameObject;
			//set image = equipment image path
			GameObject nameObj = finished.transform.FindChild("NameDesc").gameObject;
			nameObj.GetComponent<Text>().text = inventory.allPlayers[i].name;
			GameObject statsObj = finished.transform.FindChild("Stats").gameObject;
			string text = inventory.allPlayers[i].health + "     " + inventory.allPlayers[i].str + "     " + inventory.allPlayers[i].end + "      " + 
				inventory.allPlayers[i].agi + "      " + inventory.allPlayers[i].mag + "      " + inventory.allPlayers[i].luck;
			statsObj.GetComponent<Text>().text = text;

			GameObject activeButton = finished.transform.FindChild("Active").gameObject;
			activeButton.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Inactive";
			
			if(inventory.allPlayers[i].active){
				activeButton.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Active";
				Color col = activeButton.GetComponent<Image>().color;
				activeButton.GetComponent<Image>().color = new Color(col.g,col.r,col.b);
			}
			
			int captured = i;
			activeButton.GetComponent<Button>().onClick.AddListener(() => ChangeButton(activeButton));
			activeButton.GetComponent<Button>().onClick.AddListener(() => ToggleActive(captured));
			
			//Set edit button to edit the correct character
			GameObject editButton = finished.transform.FindChild("Edit").gameObject;
			editButton.GetComponent<Button>().onClick.AddListener(() => setUpCharacterEquipment(name));
			
			currentY -= 75;
		}

    }

    public void setUpCharacterEquipment(string n)
    {
        string charName = n;

        //Clear old window
        Transform oldMenu = tavernMenu.transform.FindChild("Window/AllObjects");
        foreach(Transform child in oldMenu)
        {
            Destroy(child.gameObject);
        }

		float currentY = -50.0f;
		Vector3 basePos = tavernMenu.transform.FindChild ("Window/AllObjects").position;
		
		//change height based on total objs
		int totalObjs = inventory.allPlayers.Count;
		Vector2 oldSize = tavernMenu.transform.FindChild ("Window/AllObjects").gameObject.GetComponent<RectTransform>().sizeDelta;
		tavernMenu.transform.FindChild ("Window/AllObjects").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(oldSize.x, totalObjs*75.0f);
		
        //Add weapons
		for(int i=0;i<equip.allWeapons.Count;i++){
			//check if you have more of that weapon to give
			if(inventory.obtainedWeapons[i] != null && (int)inventory.obtainedWeapons[i] > 0){
				GameObject currentEquip = editEquipmentPrefab;
				GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(basePos.x +80.0f, 
				                                        		basePos.y + currentY, 0.0f), Quaternion.identity);
				finished.transform.parent = tavernMenu.transform.FindChild("Window/AllObjects");

				GameObject imgObj = finished.transform.FindChild("Image").gameObject;

				//set image = equipment image path
				foreach(Image weaponImg in weaponImages){
					if(equip.allWeapons[i].name.Contains(weaponImg.name)){
						imgObj.GetComponent<Image>().sprite = weaponImg.sprite;
						imgObj.GetComponent<Image>().SetNativeSize();
					}
				}

				GameObject nameObj = finished.transform.FindChild("NameDesc").gameObject;
				nameObj.GetComponent<Text>().text = equip.allWeapons[i].name;
				GameObject statsObj = finished.transform.FindChild("Stats").gameObject;
				string text = equip.allWeapons[i].str + "    " + equip.allWeapons[i].end + "     " + equip.allWeapons[i].agi + "     " + equip.allWeapons[i].mag
											+ "     " + equip.allWeapons[i].luck + "    " + equip.allWeapons[i].rangeMin + " - " + equip.allWeapons[i].rangeMax;
				statsObj.GetComponent<Text>().text = text;

				GameObject recipeObj = finished.transform.FindChild("Recipe").gameObject;
				
				GameObject createObj = finished.transform.FindChild("Create").gameObject;
				int captured = i;
				createObj.transform.FindChild("Text").GetComponent<Text>().text = "Equip";
//				createObj.GetComponent<Button>().interactable = false;
				
/*				if ((int)inventory.obtainedWeapons[id] > 0)
				{
					createObj.GetComponent<Button>().interactable = true;
				}
				createObj.GetComponent<Button>().onClick.AddListener(() => EquipWeapon(charName, id));
				*/
				currentY -= 75;
			}
		}

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
                GameObject currentEquip = editEquipmentPrefab;
                GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(tavernMenu.transform.position.x - 250.0f, tavernMenu.transform.position.y + currentY, 0.0f), Quaternion.identity);
                finished.transform.parent = tavernMenu.transform.FindChild("Window/AllObjects");

                GameObject imgObj = finished.transform.FindChild("Image").gameObject;


                foreach (Image weaponImg in weaponImages)
                {

                    if (equip.allWeapons[i].name.Contains(weaponImg.name))
                    {
                        imgObj.GetComponent<Image>().sprite = weaponImg.sprite;
                        imgObj.GetComponent<Image>().SetNativeSize();
                    }
                }
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
                createObj.GetComponent<Button>().interactable = false;

                if ((int)inventory.obtainedWeapons[id] > 0)
                {
                    createObj.GetComponent<Button>().interactable = true;
                }
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
                GameObject currentEquip = editEquipmentPrefab;
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
                createObj.GetComponent<Button>().interactable = false;

                if ((int)inventory.obtainedArmor[id] > 0)
                {
                    createObj.GetComponent<Button>().interactable = true;
                }
                createObj.GetComponent<Button>().onClick.AddListener(() => EquipArmor(charName, id));

                currentY -= 75;
            }
        }
    }

	void ChangeButton(GameObject but){
		//change text
		string text = but.transform.FindChild("Text").gameObject.GetComponent<Text>().text;
		if(text.Equals("Active"))
			but.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Inactive";
		else if(text.Equals ("Inactive"))
			but.transform.FindChild("Text").gameObject.GetComponent<Text>().text = "Active";

		//change color
		float g = but.GetComponent<Image>().color.r;
		float r = but.GetComponent<Image>().color.g;
		float b = but.GetComponent<Image>().color.b;
		but.GetComponent<Image>().color = new Color(r,g,b);
	}

	void ToggleActive(int position){
		PlayerShell temp = new PlayerShell();
		temp.name = inventory.allPlayers[position].name;
		temp.desc = inventory.allPlayers[position].desc;
		temp.str = inventory.allPlayers[position].str;
		temp.agi = inventory.allPlayers[position].agi;
		temp.end = inventory.allPlayers[position].end;
		temp.mag = inventory.allPlayers[position].mag;
		temp.luck = inventory.allPlayers[position].luck;
		temp.weaponID = inventory.allPlayers[position].weaponID;
		temp.armorID = inventory.allPlayers[position].armorID;
		temp.active = !inventory.allPlayers[position].active;

		inventory.allPlayers[position] = temp;
		Debug.Log (inventory.allPlayers[position].active);
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

	#endregion

	#region menu navigation
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
	//Loads a dungeon based off of its name
	public void goToDest(string name)
    {
        //Destroy previous players
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Dead"))
        {
            Destroy(g);
        }
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

	#endregion

	#region helpers

    public void saveWeaponsXML()
    {
        XmlDocument xmlDoc = new XmlDocument();
        string path;

        //Add a weapon
        path = Application.dataPath + @"/ItemsAndEquipment/WeaponInventory.xml";
		if (File.Exists(path))
        {
        	xmlDoc.Load(path);

            XmlNodeList weapons = xmlDoc.GetElementsByTagName("weapon");
            bool newEntry = true;
			for(int i=0;i<equip.allWeapons.Count;i++){
				//Check to see if we already have that item
				foreach (XmlNode member in weapons)
				{
					if (member.Attributes["id"].Value == i.ToString())
					{
						//Update the count of the item
						Debug.Log("Found a: " + member.Attributes["name"].Value);
						member.Attributes["count"].Value = (string)inventory.obtainedWeapons[i];

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

		}
        xmlDoc.Save(Application.dataPath + @"/ItemsAndEquipment/WeaponInventory.xml");

        //Add to crafting inventory
        /*path = Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml";

        if (File.Exists(path))
        {
        	xmlDoc.Load(path);

            XmlNodeList items = xmlDoc.GetElementsByTagName("item");

            bool newEntry = true;

            //Check to see if we already have that item
            foreach (XmlNode member in items)
            {
            	if (member.Attributes["name"].Value == equip.allWeapons[i].name)
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
            	Debug.Log("Adding a new " + equip.allWeapons[i].name + " to inventory");

                //Create the new item
                XmlNodeList root = xmlDoc.GetElementsByTagName("inventory");
                XmlElement newItem = xmlDoc.CreateElement("item");
                newItem.SetAttribute("name", equip.allWeapons[i].name);
                newItem.SetAttribute("count", "1");
                root[0].AppendChild(newItem);
            }

        	xmlDoc.Save(Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml");
		}*/

        /*/Add armor
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

            //Add to crafting inventory
                path = Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml";

                if (File.Exists(path))
                {
                    xmlDoc.Load(path);

                    XmlNodeList items = xmlDoc.GetElementsByTagName("item");

                    bool newEntry = true;

                    //Check to see if we already have that item
                    foreach (XmlNode member in items)
                    {
                        if (member.Attributes["name"].Value == equip.allArmor[i].name)
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
                        XmlElement newItem = xmlDoc.CreateElement("item");
                        newItem.SetAttribute("name", equip.allArmor[i].name);
                        newItem.SetAttribute("count", "1");
                        root[0].AppendChild(newItem);
                    }

                    xmlDoc.Save(Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml");
                }

        }*/
    }

    //Returns a random name because neww member is boring
    private string genName()
    {
        string[] nameList = { "Concetta", "Bethany", "Krystina", "Chi", "Eugenia", "Carmon", "Kemberly", "Temple", "Layne", "Stormy", "Lakiesha", "Bertie", "Sherill", "Christopher", "Tristan", "Troy", "Darleen", "Josette", "Silvia", "Bret", "Ernesto", "Nancy", "Kyung", "Ozie", "Evalyn", "Bernard", "Shelly", "Cyndy", "Veronica", "Porter", "Priscilla", "Aleisha", "Lyla", "Pete", "Stacee", "Basilia", "Afton", "Douglass", "Wilda", "Andera", "Misti", "Anton", "Chase", "Marlo", "Darcel", "Yvette", "Joy", "Reagan", "Penni", "Terrence" };


        int i = UnityEngine.Random.Range(0, 50);

        return nameList[i];
    }

	#endregion
}
