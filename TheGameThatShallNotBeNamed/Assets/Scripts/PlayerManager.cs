using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {

    public Transform player;
	public GameObject selectedObject; //Currently selected player or enemy or tile
	public List<PlayerController> allPlayers;
	public List<Enemy> allEnemies;
	public List<PathTile> allTiles;
    public TileMap tileMap;
    public GameObject endTile;
	public GameObject arrow;
	GameObject arrowObj;
	GameObject playerUI;
	GameObject enemyUI;
	
	CameraController camera;

	public Predicate<PathTile> walkableTiles;

	enum Turn {PlayerTurn, EnemyTurn};
	Turn currentTurn;

	enum Action {Attack, Move, None};
	Action currentAction;

	public int currentEnemy = 0;
	public bool enemyMoving = false;
	public bool playerMoving = false;

	// Use this for initialization
	void Start () {
		playerUI = GameObject.Find ("PlayerCombatUI");
		playerUI.SetActive(false);

		enemyUI = GameObject.Find ("EnemyCombatUI");
		enemyUI.SetActive(false);
		
		camera = GameObject.Find("Main Camera").GetComponent<CameraController>();

		Vector3 arrowPos = new Vector3(0.0f,-5.0f,0.0f);
		Quaternion rotation = Quaternion.Euler(45.0f, 180.0f, 0.0f);
		arrowObj = (GameObject)Instantiate (arrow, arrowPos, rotation);

		currentTurn = Turn.PlayerTurn;
		currentAction = Action.None;

        tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
        tileMap.UpdateConnections();

        GameObject[] tempPlayers = GameObject.FindGameObjectsWithTag("Player");
		for(int i = 0; i < tempPlayers.Length; i++){
			allPlayers.Add(tempPlayers[i].GetComponent<PlayerController>());
		}

		GameObject[] tempEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		for(int i = 0; i < tempEnemies.Length; i++){
			allEnemies.Add (tempEnemies[i].GetComponent<Enemy>());
		}

		GameObject[] tempTiles = GameObject.FindGameObjectsWithTag("Tile");
		for(int i = 0; i < tempTiles.Length; i++){
			allTiles.Add(tempTiles[i].GetComponent<PathTile>());
		}
        //Debug.Log (allPlayers.Length);
        //Debug.Log (allEnemies.Length);

        endTile = GameObject.FindGameObjectWithTag("EndTile");

		walkableTiles = new Predicate<PathTile>(isWalkable);
    }
	
	// Update is called once per frame
	void Update () {
		//Player Turn
		#region playerTurn
		if(currentTurn == Turn.PlayerTurn)
		{
			camera.followEnemy = false;
			
			#region hotkeys
			if (selectedObject) {
				arrowObj.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y + 2.0f, selectedObject.transform.position.z+1.0f);
				//Key 1, Move
				if (Input.GetKeyDown ("1") || Input.GetKeyDown (KeyCode.Keypad1))
				{
					currentAction = Action.Move;
				}
				//Key 2, Attack
				else if (Input.GetKeyDown ("2") || Input.GetKeyDown (KeyCode.Keypad2))
				{
					currentAction = Action.Attack;
				}
				//Key 3, Inactive Player
				else if (Input.GetKeyDown ("3") || Input.GetKeyDown (KeyCode.Keypad3))
				{
					if(selectedObject.GetComponent<PlayerController>().active)
					{
						selectedObject.GetComponent<Character>().active = false;
						selectedObject = null;
					}
					else
					{
						selectedObject.GetComponent<PlayerController>().active = true;
					}
				}
				//Right Click, Cancel Action
				else if (Input.GetMouseButtonDown (1))
				{
					currentAction = Action.None;
				}
				if(currentAction != Action.Move)
				{
					HighlightMoveTiles(false);
				}
				else if (currentAction != Action.Attack)
				{
					HighlightAttackTiles(false);
				}
			}
			else
			{
				arrowObj.transform.position = new Vector3(0.0f, -5.0f, 0.0f);
				currentAction = Action.None;
			}
			#endregion

			#region highlighting
			if(selectedObject && selectedObject.tag == "Player")
	        {
				if(currentAction == Action.Move)
				{
					HighlightMoveTiles(true);
				}
				else if(currentAction == Action.Attack)
				{
					HighlightAttackTiles(true);
				}
	            if (selectedObject.GetComponent<Character>().doneMoving)
	            {
	                HighlightMoveTiles(true);
	                selectedObject.GetComponent<Character>().doneMoving = false;
	            }
	        }
			#endregion
			
			#region enemyDrops
			for(int i = 0; i < allEnemies.Count; i++)
			{
				if(allEnemies[i].health <= 0)
				{

	                // Update the inventory with the enemy's drop
	                XmlDocument xmlDoc = new XmlDocument();

	                //Make sure the file exists before loading
	                if (File.Exists(Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml"))
	                {
	                    xmlDoc.Load(Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml");

	                    XmlNodeList items = xmlDoc.GetElementsByTagName("item");

	                    bool newdrop = true;

	                    //Check to see if we already have that item
	                    foreach (XmlNode member in items)
	                    {
	                        if (member.Attributes["name"].Value == allEnemies[i].drop)
	                        {
	                            //Update the count of the item
	                            Debug.Log("Found a: " + member.Attributes["name"].Value);
	                            int currentCount = int.Parse(member.Attributes["count"].Value);
	                            currentCount++;
	                            member.Attributes["count"].Value = currentCount.ToString();

	                            //Don't need to make a new entry
	                            newdrop = false;
	                        }
	                    }

	                    //If we don't have that item yet
	                    if (newdrop)
	                    {
	                        Debug.Log("Adding a new " + allEnemies[i].drop);

	                        //Create the new item
	                        XmlNodeList root = xmlDoc.GetElementsByTagName("inventory");
	                        XmlElement newItem = xmlDoc.CreateElement("item");
	                        newItem.SetAttribute("name", allEnemies[i].drop);
	                        newItem.SetAttribute("count", "1");
	                        root[0].AppendChild(newItem);
	                    }

	                }
	                xmlDoc.Save(Application.dataPath + @"/ItemsAndEquipment/ItemInventory.xml");
	                Destroy(allEnemies[i].gameObject);
					allEnemies.RemoveAt(i);
				}
			}
			#endregion
			
			#region playerDeath
			//Check for dead Players
			for(int i = 0; i < allPlayers.Count; i++)
			{
				if(allPlayers[i].health <= 0)
				{
					//allPlayers[i].gameObject.SetActive(false);
					allPlayers[i].gameObject.name += " (Dead)";
					allPlayers.RemoveAt(i);
				}
			}
			#endregion

			//Check if the selected player is moving
			if(selectedObject && selectedObject.GetComponent<Character>().end == null)
			{
				playerMoving = false;
			}

			#region mouse input
			//Mouse Input Starts
			if (Input.GetMouseButtonDown(0) && !playerMoving) {
				
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				
				if (Physics.Raycast(ray, out hit, 100)) {
					
					//Clicked on Tile
					if(hit.collider.gameObject.tag == "Tile")
					{
						if(currentAction == Action.Move && selectedObject && selectedObject.GetComponent<PlayerController>() && !selectedObject.GetComponent<Character>().end)
						{
							Character tempChar = selectedObject.GetComponent<Character>();
							List<PathTile> tempList = new List<PathTile>();
							tileMap.FindPath(tempChar.start, hit.collider.gameObject.GetComponent<PathTile>(), tempList, isWalkable);
							
							if ((tempList.Count - 1) <= tempChar.currentActionPoints)
							{
								tempChar.isWalkable = this.isWalkable;
								tempChar.end = hit.collider.gameObject.GetComponent<PathTile>();
								playerMoving = true;
							}
							else
							{
								HighlightMoveTiles(false);
								HighlightAttackTiles(false);
								selectedObject = null;
								playerUI.SetActive(false);
							}
						}
						else //if (selectedObject && selectedObject.GetComponent<Enemy>())
						{
							currentAction = Action.None;
							HighlightMoveTiles(false);
							HighlightAttackTiles(false);
							selectedObject = null;
							playerUI.SetActive(false);
							enemyUI.SetActive(false);
						}
					}
					//Clicked on Player
					else if(hit.collider.gameObject.tag == "Player")
					{
						currentAction = Action.Move;
						selectedObject = hit.collider.gameObject;

						playerUI.SetActive(true);
						SetPlayerUI(selectedObject.GetComponent<PlayerController>());
						
						arrowObj.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y + 2.0f, selectedObject.transform.position.z+1.0f); 
					}
					//Clicked on Enemy
					else if(hit.collider.gameObject.tag == "Enemy")
					{
						if (selectedObject) {
							if(selectedObject.GetComponent<PlayerController>() && currentAction == Action.Attack)
							{
								currentAction = Action.Move;
								selectedObject.GetComponent<Character>().basicAttack(hit.collider.gameObject.GetComponent<Enemy>());
								SetEnemyUI(hit.collider.gameObject.GetComponent<Enemy>());
							}
							else {
								selectedObject = hit.collider.gameObject;
								SetEnemyUI(selectedObject.GetComponent<Character>());
								enemyUI.SetActive(true);
							}
						}
						else
						{
							selectedObject = hit.collider.gameObject;
							SetEnemyUI(selectedObject.GetComponent<Character>());
							enemyUI.SetActive(true);
						}
						HighlightMoveTiles(false);
						HighlightAttackTiles(false);
					}
				}
			}
			//Mouse Input Ends
			#endregion
			
			//If all players are inactive
			if(InactivePlayers(allPlayers))
			{
				playerUI.SetActive(false);
				enemyUI.SetActive(false);

				currentTurn = Turn.EnemyTurn;
				foreach(PlayerController pc in allPlayers)
				{
					pc.GetComponent<PlayerController>().resetStatus();
				}
			}

			foreach (PlayerController pc in allPlayers)
			{
				if (Vector3.Distance(pc.transform.position, endTile.transform.position) <= 0.6f)
				{
					pc.transform.position = new Vector3(5.0f, 0, 5.0f);
					//Tell the dungeon manager to load the next level
					DungeonManager tempMan = GameObject.FindGameObjectWithTag("DungeonManager").GetComponent<DungeonManager>();
					tempMan.incrementFloor();
					break;
				}
			}
			//End Player Turn
		}
		#endregion
		#region enemyTurn
		else //Enemy Turn
		{
			camera.followEnemy = true;
			//find closest player, find path to player and stop 1 tile before. Attack player.
//			for(int i = 0; i < allEnemies.Count; i++){
//				allEnemies[i].target = allEnemies[i].FindClosestPlayer(allPlayers.ToArray()).GetComponent<PlayerController>().start;
//			}
			if(currentEnemy < allEnemies.Count)
			{
				allEnemies[currentEnemy].EnemyUpdate();
				if(!enemyMoving)
				{
					enemyMoving = true;
					selectedObject = allEnemies[currentEnemy].enemyObject;
					
					camera.selectedObject = this.selectedObject;
					
					allEnemies[currentEnemy].isWalkable = this.isWalkable;
					allEnemies[currentEnemy].target = allEnemies[currentEnemy].FindClosestPlayer(allPlayers.ToArray()).GetComponent<PlayerController>().start;
				}
				else
				{
					if(allEnemies[currentEnemy].end == null && !allEnemies[currentEnemy].active)
					{
						enemyMoving = false;
						currentEnemy++;
					}
				}

			}
			//If all enemies are inactive, end turn
			else if(InactiveEnemies(allEnemies))
			{
				selectedObject = null;
				currentTurn = Turn.PlayerTurn;
				currentEnemy = 0;
				enemyMoving = false;
				foreach(Enemy e in allEnemies)
					e.resetStatus();
			}
		}
		#endregion
	}

	void LateUpdate(){
		if(selectedObject && selectedObject.GetComponent<PlayerController>())
			SetPlayerUI(selectedObject.GetComponent<PlayerController>());

		Debug.Log(currentTurn);
		if(currentTurn == Turn.PlayerTurn){
			foreach( PlayerController p in allPlayers){
				p.PlayerUpdate();
			}
		}
		else if(currentTurn == Turn.EnemyTurn){
			//foreach( Enemy e in allEnemies){
			//	e.EnemyUpdate();
			//}
		}

	}

    void HighlightMoveTiles(bool playerSelected) {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
		Character selectedChar;
		if(selectedObject)
		{
        	selectedChar = selectedObject.GetComponent<Character>();
		}
		else
		{
			return;
		}
        List<PathTile> moveableTiles = new List<PathTile>();
        moveableTiles.Add(selectedChar.start);

		for (int i = 0; i < tiles.Length; i++)
		{
			tiles[i].GetComponent<MeshRenderer>().material.color = Color.white;
		}

        if (playerSelected) {
            for (int i = 0; i < selectedChar.currentActionPoints + 1; i++)
            {
                List<PathTile> tempList = new List<PathTile>();

                for (int j = 0; j < moveableTiles.Count; j++)
                {
                    PathTile tempTile = moveableTiles[j];
                    if (tempTile.GetComponent<MeshRenderer>().material.color != Color.yellow)
                    {
                        tempTile.GetComponent<MeshRenderer>().material.color = Color.yellow;

                        for (int k = 0; k < tempTile.connections.Count; k++)
                        {
                            if (tempTile.connections[k].GetComponent<MeshRenderer>().material.color != Color.yellow && isWalkable(tempTile.connections[k]))
                            {
                                tempList.Add(tempTile.connections[k]);
                            }
                        }
                    }
                }

                for (int l = 0; l < tempList.Count; l++)
                {
                    moveableTiles.Add(tempList[l]);
                }
            }
        }

        //for (int i = 0; i < tiles.Length; i++)
        //{
        //    tileMap.FindPath(selectedObject.GetComponent<Character>().start, tiles[i].GetComponent<PathTile>(), tempList);
        //
        //    if (tempList.Count < (selectedObject.GetComponent<Character>().currentActionPoints + 2))
        //    {
        //        tiles[i].GetComponent<MeshRenderer>().material.color = Color.yellow;
        //    }
        //
        //    tempList.Clear();
        //}
    }

	void HighlightAttackTiles(bool playerSelected) {
		Character selectedChar;
		if(selectedObject)
		{
			selectedChar = selectedObject.GetComponent<Character>();
		}
		else
		{
			return;
		}
		if (playerSelected) {
			List<PathTile> tilesInRange = new List<PathTile>();
			tilesInRange.Add(selectedChar.start);
			for (int i = 0; i < selectedChar.range; i++)
			{
				List<PathTile> tempTilesInRange = new List<PathTile>();
				foreach(PathTile pt in tilesInRange)
				{
					tempTilesInRange.AddRange(pt.connections);
				}
				tilesInRange.AddRange(tempTilesInRange);
			}

			foreach(PathTile pt in tilesInRange)
			{
				pt.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
			}
		}
		else
		{
			GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
			for (int i = 0; i < tiles.Length; i++)
			{
				tiles[i].GetComponent<MeshRenderer>().material.color = Color.white;
			}
		}
	}

    bool isWalkable(PathTile tile)
    {
		if(selectedObject)
		{
			if (selectedObject.GetComponent<Character>())
			{
				if (selectedObject.GetComponent<Character>().start == tile)
				{
					//tile.GetComponent<MeshRenderer>().material.color = Color.green;
					return true;
				}
			}
		
			if (selectedObject.GetComponent<Enemy>()) 
			{
				if (selectedObject.GetComponent<Enemy>().target == tile) 
				{
					//tile.GetComponent<MeshRenderer>().material.color = Color.green;
					return true;
				}
			}

			foreach (PlayerController player in allPlayers) 
			{
				if (player.start == tile)
				{
					//tile.GetComponent<MeshRenderer>().material.color = Color.gray;
					return false;
				}
			}

			foreach (Enemy enemy in allEnemies) 
			{

				if (enemy.start == tile) 
				{
					//tile.GetComponent<MeshRenderer>().material.color = Color.gray;
					return false;
				}
			}
		}
		//tile.GetComponent<MeshRenderer>().material.color = Color.green;
		return true;
    }

    //checks if all of a team is inactive, to progress turns
    bool InactivePlayers(List<PlayerController> players)
	{
		foreach(PlayerController p in players)
		{
			if(p.active)
			{
				return false;
			}
			else
			{
				continue;
			}
		}
		return true;
	}

	bool InactiveEnemies(List<Enemy> enemies)
	{
		foreach(Enemy e in enemies)
		{
			if(e.active)
			{
				return false;
			}
			else
			{
				continue;
			}
		}
		return true;
	}

	public void SetPlayerUI(Character c){
		playerUI.transform.Find("APBG/Text").gameObject.GetComponent<Text>().text = "AP: " + c.currentActionPoints;
		playerUI.transform.Find("NameBG/Text").gameObject.GetComponent<Text>().text = c.charName;
		playerUI.transform.Find("StatBackground/Strength").gameObject.GetComponent<Text>().text = "Str: " + c.strength;
		playerUI.transform.Find("StatBackground/Agility").gameObject.GetComponent<Text>().text = "Agi: " + c.agility;
		playerUI.transform.Find("StatBackground/Magic").gameObject.GetComponent<Text>().text = "Mag: " + c.magicSkill;
		playerUI.transform.Find("StatBackground/Luck").gameObject.GetComponent<Text>().text = "Lck: " + c.luck;
		playerUI.transform.Find("StatBackground/Endurance").gameObject.GetComponent<Text>().text = "End: " + c.endurance;
		playerUI.transform.Find("StatBackground/Range").gameObject.GetComponent<Text>().text = "Rng: " + c.range;
		float value;
		value = ((float)(c.maxHealth - (float)c.health)/(float)c.maxHealth)*-250.0f;
		RectTransform rect = playerUI.transform.Find("StatBackground/HealthBar/Mask").gameObject.GetComponent<RectTransform>();
		rect.offsetMax = new Vector2(value, rect.offsetMax.y);
	}

	public void SetEnemyUI(Character c){
		enemyUI.transform.Find("APBG/Text").gameObject.GetComponent<Text>().text = "AP: " + c.currentActionPoints;
		enemyUI.transform.Find("NameBG/Text").gameObject.GetComponent<Text>().text = c.charName;
		enemyUI.transform.Find("StatBackground/Strength").gameObject.GetComponent<Text>().text = "Str: " + c.strength;
		enemyUI.transform.Find("StatBackground/Agility").gameObject.GetComponent<Text>().text = "Agi: " + c.agility;
		enemyUI.transform.Find("StatBackground/Magic").gameObject.GetComponent<Text>().text = "Mag: " + c.magicSkill;
		enemyUI.transform.Find("StatBackground/Luck").gameObject.GetComponent<Text>().text = "Lck: " + c.luck;
		enemyUI.transform.Find("StatBackground/Endurance").gameObject.GetComponent<Text>().text = "End: " + c.endurance;
		enemyUI.transform.Find("StatBackground/Range").gameObject.GetComponent<Text>().text = "Rng: " + c.range;
		float value;
		value = ((float)(c.maxHealth - (float)c.health)/(float)c.maxHealth)*-250.0f;
		RectTransform rect = enemyUI.transform.Find("StatBackground/HealthBar/Mask").gameObject.GetComponent<RectTransform>();
		//		Debug.Log (c.maxHealth);
		//		Debug.Log (c.health);
		rect.offsetMax = new Vector2(value, rect.offsetMax.y);
		//		Debug.Log (rect.offsetMax);
	}
}