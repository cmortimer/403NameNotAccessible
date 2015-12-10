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
	public Image[] armorImages;

	Equipment equip;
	PlayerData inventory;
	
	//set correct menu options to active
	void Start(){
        equip = GameObject.FindGameObjectWithTag("Persistent").GetComponent<Equipment>();
		inventory = GameObject.FindGameObjectWithTag("Persistent").GetComponent<PlayerData>();
		weaponImages = GameObject.Find("AllWeaponImages").GetComponentsInChildren<Image>();
		armorImages = GameObject.Find("AllArmorImages").GetComponentsInChildren<Image>();
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

			GameObject nameObj = finished.transform.FindChild("Name").gameObject;
			nameObj.GetComponent<Text>().text = equip.allArmor[i].name;
			GameObject descObj = finished.transform.FindChild("Description").gameObject;
			//string text = equip.allArmor[i].str + "    " + equip.allArmor[i].end + "     " + equip.allArmor[i].agi + "     " + equip.allArmor[i].mag + "     " + equip.allArmor[i].luck + "     " ;
			descObj.GetComponent<Text>().text = equip.allArmor[i].desc;

			GameObject imgObj = finished.transform.FindChild("Image").gameObject;
			foreach(Image armorImg in armorImages){
				if(equip.allArmor[i].name.Contains(armorImg.name)){
					imgObj.GetComponent<Image>().sprite = armorImg.sprite;
					imgObj.GetComponent<Image>().SetNativeSize();
				}
			}
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
				else{
					itemObj.transform.FindChild("RecipeFull/RecipeItem"+j).gameObject.GetComponent<Text>().color = new Color(.1f,.1f,.1f);
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
		if(inventory.obtainedWeapons[i] != null){
			int count = (int)inventory.obtainedWeapons[i];
			inventory.obtainedWeapons[i] = ++count;
		}
		else{
			inventory.obtainedWeapons[i] = 1;
		}
		
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
		if(inventory.obtainedArmor[i] != null){
			int count = (int)inventory.obtainedArmor[i];
			inventory.obtainedArmor[i] = ++count;
		}
		else{
			inventory.obtainedArmor[i] = 1;
		}

		foreach(string s in equip.allArmor[i].recipe){
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

		float currentY = -50.0f;
		Vector3 basePos = tavernMenu.transform.FindChild ("Window/AllObjects").position;
		
		
		//change height based on total objs
		int totalObjs = inventory.allPlayers.Count;
		Vector2 oldSize = tavernMenu.transform.FindChild ("Window/AllObjects").gameObject.GetComponent<RectTransform>().sizeDelta;
		tavernMenu.transform.FindChild ("Window/AllObjects").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(oldSize.x, totalObjs*75.0f);

		for(int i=0;i<inventory.allPlayers.Count;i++){
			GameObject currentChar = charStatPrefab;
			GameObject finished = (GameObject)Instantiate(currentChar, new Vector3(basePos.x+-25.0f, basePos.y+currentY, 0.0f), Quaternion.identity);
			finished.transform.parent = tavernMenu.transform.FindChild("Window/AllObjects");


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

			GameObject removeObj = finished.transform.FindChild("Remove").gameObject;
			removeObj.GetComponent<Button>().onClick.AddListener(() => RemoveGuildMember(captured));

			//Set edit button to edit the correct character
			GameObject editButton = finished.transform.FindChild("Edit").gameObject;
			editButton.GetComponent<Button>().onClick.AddListener(() => setUpCharacterEquipment(captured));
			
			currentY -= 75;
		}

    }

    public void setUpCharacterEquipment(int charPos)
    {
		//Debug.Log (charPos);

        //Clear old window
        Transform oldMenu = tavernMenu.transform.FindChild("Window/AllObjects");
        foreach(Transform child in oldMenu)
        {
            Destroy(child.gameObject);
        }

		float currentY = -50.0f;
		Vector3 basePos = tavernMenu.transform.FindChild ("Window/AllObjects").position;
		int totalObjs = 0;

        //Add weapons
		for(int i=0;i<equip.allWeapons.Count;i++){
			int captured = i;

			//check if you have more of that weapon to give
			if(captured == inventory.allPlayers[charPos].weaponID || inventory.obtainedWeapons[i] != null && (int)inventory.obtainedWeapons[i] > 0){
				totalObjs++;
				GameObject currentEquip = editEquipmentPrefab;
				GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(basePos.x+10.0f, 
				                                        		basePos.y + currentY, 0.0f), Quaternion.identity);
				finished.transform.parent = tavernMenu.transform.FindChild("Window/AllObjects");

				GameObject imgObj = finished.transform.FindChild("Image").gameObject;

				//set image = equipment image path
				foreach(Image weaponImg in weaponImages){
					if(equip.allWeapons[i].name.Equals(weaponImg.name)){
						imgObj.GetComponent<Image>().sprite = weaponImg.sprite;
						imgObj.GetComponent<Image>().SetNativeSize();
					}
				}

				GameObject nameObj = finished.transform.FindChild("NameDesc").gameObject;
				nameObj.GetComponent<Text>().text = equip.allWeapons[i].name;
				GameObject statsObj = finished.transform.FindChild("Stats").gameObject;
				string text = equip.allWeapons[i].str + "     " + equip.allWeapons[i].end + "      " + equip.allWeapons[i].agi + "      " + equip.allWeapons[i].mag
											+ "      " + equip.allWeapons[i].luck + "     " + equip.allWeapons[i].rangeMin + " - " + equip.allWeapons[i].rangeMax;
				statsObj.GetComponent<Text>().text = text;

				GameObject recipeObj = finished.transform.FindChild("Recipe").gameObject;
				recipeObj.GetComponent<Text>().text = "";
				
				GameObject createObj = finished.transform.FindChild("Create").gameObject;
				createObj.transform.FindChild("Text").GetComponent<Text>().text = "Equip";			
				if (captured == inventory.allPlayers[charPos].weaponID)
				{
					createObj.GetComponent<Button>().interactable = false;
					createObj.transform.FindChild("Text").GetComponent<Text>().text = "Equipped";
				}

				createObj.GetComponent<Button>().onClick.AddListener(() => EquipWeapon(charPos, captured));

				currentY -= 75;
			}
		}

		for(int i=0;i<equip.allArmor.Count;i++){
			int captured = i;

			//check if you have more of that armor to give
			if(captured == inventory.allPlayers[charPos].armorID || inventory.obtainedArmor[i] != null && (int)inventory.obtainedArmor[i] > 0){
				totalObjs++;
				GameObject currentEquip = editEquipmentPrefab;
				GameObject finished = (GameObject)Instantiate(currentEquip, new Vector3(basePos.x+10.0f, 
				                                                                        basePos.y + currentY, 0.0f), Quaternion.identity);
				finished.transform.parent = tavernMenu.transform.FindChild("Window/AllObjects");
				
				GameObject imgObj = finished.transform.FindChild("Image").gameObject;
				
				//set image = equipment image path
/*				foreach(Image armorImg in armorImages){
					if(equip.allArmor[i].name.Equals(armorImg.name)){
						imgObj.GetComponent<Image>().sprite = armorImg.sprite;
						imgObj.GetComponent<Image>().SetNativeSize();
					}
				}
*/				
				GameObject nameObj = finished.transform.FindChild("NameDesc").gameObject;
				nameObj.GetComponent<Text>().text = equip.allArmor[i].name;
				GameObject statsObj = finished.transform.FindChild("Stats").gameObject;
				string text = equip.allArmor[i].str + "     " + equip.allArmor[i].end + "      " + equip.allArmor[i].agi + "      " + equip.allArmor[i].mag
					+ "      " + equip.allArmor[i].luck;
				statsObj.GetComponent<Text>().text = text;
				
				GameObject recipeObj = finished.transform.FindChild("Recipe").gameObject;
				recipeObj.GetComponent<Text>().text = "";
				
				GameObject createObj = finished.transform.FindChild("Create").gameObject;
				createObj.transform.FindChild("Text").GetComponent<Text>().text = "Equip";

				if (captured == inventory.allPlayers[charPos].armorID)
				{
					createObj.GetComponent<Button>().interactable = false;
					createObj.transform.FindChild("Text").GetComponent<Text>().text = "Equipped";
				}
				createObj.GetComponent<Button>().onClick.AddListener(() => EquipArmor(charPos, captured));
				
				currentY -= 75;
			}
		}

		//change height based on total objs
		Vector2 oldSize = tavernMenu.transform.FindChild ("Window/AllObjects").gameObject.GetComponent<RectTransform>().sizeDelta;
		tavernMenu.transform.FindChild ("Window/AllObjects").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(oldSize.x, totalObjs*75.0f);

    
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
	}

    //Equips a weapon to a character
    void EquipWeapon(int charPos, int inventoryPos)
    {
		//place previous weapon in inventory
		int temp = (int)inventory.obtainedWeapons[inventory.allPlayers[charPos].weaponID];
		inventory.obtainedWeapons[inventory.allPlayers[charPos].weaponID] = ++temp;

		//equip the new weapon and remove it from inventory
		inventory.allPlayers[charPos].weaponID = inventoryPos;
		temp = (int)inventory.obtainedWeapons[inventoryPos];
		inventory.obtainedWeapons[inventoryPos] = --temp;

		//reload equipping menu
		setUpCharacterEquipment(charPos);
    }

    //Equips armor to a character
    void EquipArmor(int charPos, int inventoryPos)
    {
		//place previous weapon in inventory
		int temp = (int)inventory.obtainedArmor[inventory.allPlayers[charPos].armorID];
		inventory.obtainedArmor[inventory.allPlayers[charPos].armorID] = ++temp;
		
		//equip the new weapon and remove it from inventory
		inventory.allPlayers[charPos].armorID = inventoryPos;
		temp = (int)inventory.obtainedArmor[inventoryPos];
		inventory.obtainedArmor[inventoryPos] = --temp;
		
		//reload equipping menu
		setUpCharacterEquipment(charPos);
    }



	public void RecruitGuildMember(){
		PlayerShell temp = new PlayerShell();
		temp.name = genName ();
		temp.desc = "A new Recruit";
		//temp.health = 50;

		temp.health = (int)UnityEngine.Random.Range(10,100);

		double str = UnityEngine.Random.Range(30, 70);
		double end = UnityEngine.Random.Range(30, 70);
		double agi = UnityEngine.Random.Range(30, 70);
		double mag = UnityEngine.Random.Range(30, 70);
		double luck = UnityEngine.Random.Range(30, 70);

		double total = str+end+agi+mag+luck;

		temp.str = (int)(str/total*42);
		temp.end = (int)(end/total*42);
		temp.agi = (int)(agi/total*42);
		temp.mag = (int)(mag/total*42);
		temp.luck = (int)(luck/total*42);
		temp.weaponID = (int)UnityEngine.Random.Range (0,4);
		temp.armorID = (int)UnityEngine.Random.Range (0,4);
		temp.active = false;

		inventory.allPlayers.Add(temp);
		setUpGuild();
	}
	
	public void RemoveGuildMember(int position){
		inventory.allPlayers.RemoveAt(position);
		setUpGuild();
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
	public void SaveAll(){
		saveInventoryXML();
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

    public void saveInventoryXML()
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
				newEntry = true;
				//Check to see if we already have that item
				foreach (XmlNode member in weapons)
				{
					if (member.Attributes["id"].Value == i.ToString())
					{
						//Update the count of the item
						Debug.Log("Found a: " + member.Attributes["name"].Value);
						member.Attributes["count"].Value = inventory.obtainedWeapons[i].ToString();

						//Don't need to make a new entry
						newEntry = false;
					}
				}
				//If we don't have that item yet
				if (newEntry)
				{
					Debug.Log("Adding a new " + equip.allWeapons[i].name);
					
					//Create the new item
					if(inventory.obtainedWeapons[i] != null){
						XmlNodeList root = xmlDoc.GetElementsByTagName("inventory");
						XmlElement newItem = xmlDoc.CreateElement("weapon");
						newItem.SetAttribute("name", equip.allWeapons[i].name);
						newItem.SetAttribute("id", i.ToString());

						newItem.SetAttribute("count", inventory.obtainedWeapons[i].ToString());
						root[0].AppendChild(newItem);
					}
				}
			}

		}
        xmlDoc.Save(Application.dataPath + @"/ItemsAndEquipment/WeaponInventory.xml");

        //saving items
        path = Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml";
        if (File.Exists(path))
        {
        	xmlDoc.Load(path);

            XmlNodeList items = xmlDoc.GetElementsByTagName("item");
				//Check to see if we already have that item
			foreach (XmlNode member in items)
			{
				//Update the count of the item
				Debug.Log("Found a: " + member.Attributes["name"].Value);
				member.Attributes["count"].Value = inventory.obtainedItems[member.Attributes["name"].Value].ToString();
			}
		}
        xmlDoc.Save(Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml");


		//saving armor
		path = Application.dataPath + @"/ItemsAndEquipment/ArmorInventory.xml";
		if (File.Exists(path))
		{
			xmlDoc.Load(path);
			
			XmlNodeList armor = xmlDoc.GetElementsByTagName("armor");
			bool newEntry = true;
			for(int i=0;i<equip.allArmor.Count;i++){
				newEntry = true;
				//Check to see if we already have that item
				foreach (XmlNode member in armor)
				{
					if (member.Attributes["id"].Value == i.ToString())
					{
						//Update the count of the item
						Debug.Log("Found a: " + member.Attributes["name"].Value);
						if(inventory.obtainedArmor[i]!= null){
							member.Attributes["count"].Value = inventory.obtainedArmor[i].ToString();
						}
						
						//Don't need to make a new entry
						newEntry = false;
					}
				}
				//If we don't have that item yet
				if (newEntry)
				{
					Debug.Log("Adding a new " + equip.allArmor[i].name);
					
					//Create the new item
					if(inventory.obtainedArmor[i]!=null){
						XmlNodeList root = xmlDoc.GetElementsByTagName("inventory");
						XmlElement newItem = xmlDoc.CreateElement("armor");
						newItem.SetAttribute("name", equip.allArmor[i].name);
						newItem.SetAttribute("id", i.ToString());

						newItem.SetAttribute("count", inventory.obtainedArmor[i].ToString());
						root[0].AppendChild(newItem);
					}
				}
			}
			
		}
		xmlDoc.Save(Application.dataPath + @"/ItemsAndEquipment/ArmorInventory.xml");
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
