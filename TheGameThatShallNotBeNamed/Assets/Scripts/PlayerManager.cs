using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

	public Predicate<PathTile> walkableTiles;

	enum Turn {PlayerTurn, EnemyTurn};
	Turn currentTurn;

	public int currentEnemy = 0;
	public bool enemyMoving = false;

	// Use this for initialization
	void Start () {

		Vector3 arrowPos = new Vector3(0.0f,-5.0f,0.0f);
		Quaternion rotation = Quaternion.Euler(45.0f, 180.0f, 0.0f);
		arrowObj = (GameObject)Instantiate (arrow, arrowPos, rotation);

		currentTurn = Turn.PlayerTurn;

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
		if(selectedObject)
        {
			arrowObj.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y + 2.0f, selectedObject.transform.position.z+1.0f);
        }
        else
        {
            arrowObj.transform.position = new Vector3(0.0f, -5.0f, 0.0f);
        }

		if(selectedObject && selectedObject.tag == "Player")
        {
            if (selectedObject.GetComponent<Character>().doneMoving)
            {
                HighlightTiles(true);
                selectedObject.GetComponent<Character>().doneMoving = false;
            }
        }

		for(int i = 0; i < allEnemies.Count; i++)
		{
			if(allEnemies[i].health <= 0)
			{
				Destroy(allEnemies[i].gameObject);
				allEnemies.RemoveAt(i);
			}
		}

		for(int i = 0; i < allPlayers.Count; i++)
		{
			if(allPlayers[i].health <= 0)
			{
				allPlayers[i].GetComponent<MeshRenderer>().enabled = false;
				allPlayers[i].gameObject.name += " (Dead)";
				//Destroy(allPlayers[i].gameObject);
				allPlayers.RemoveAt(i);
			}
		}

		//Mouse Input Starts
		if (Input.GetMouseButtonDown(0)) {
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit, 100)) {
				
				//Clicked on Tile
				if(hit.collider.gameObject.tag == "Tile")
				{
					if(selectedObject && selectedObject.GetComponent<PlayerController>() && !selectedObject.GetComponent<Character>().end)
					{
                        Character tempChar = selectedObject.GetComponent<Character>();
                        List<PathTile> tempList = new List<PathTile>();
                        tileMap.FindPath(tempChar.start, hit.collider.gameObject.GetComponent<PathTile>(), tempList, isWalkable);
                        
                        if ((tempList.Count - 1) <= tempChar.currentActionPoints)
                        {
							tempChar.isWalkable = this.isWalkable;
                            tempChar.end = hit.collider.gameObject.GetComponent<PathTile>();
                        }
                        else
                        {
                            HighlightTiles(false);
                            selectedObject = null;
                        }
					}
                    else if (selectedObject && selectedObject.GetComponent<Enemy>())
                    {
                        HighlightTiles(false);
                        selectedObject = null;
                    }

//					else if(selectedObject.GetComponent<Enemy>())
//					{
//						selectedObject.GetComponent<Enemy>().end = hit.collider.gameObject.GetComponent<PathTile>();
//					}
				}
				//Clicked on Player
				else if(hit.collider.gameObject.tag == "Player") //FUTURE REFERENCE, SELECTED PLAYER TAG
				{
					if(selectedObject)
					{
						//selectedObject.GetComponent<MeshRenderer>().material.color = Color.white;
					}



                    selectedObject = hit.collider.gameObject;
                   // selectedObject.GetComponent<MeshRenderer>().material.color = Color.green;
                    HighlightTiles(true);

					arrowObj.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y + 2.0f, selectedObject.transform.position.z+1.0f); 
					//Debug.Log("Hit player");
				}
				//Clicked on Enemy
				else if(hit.collider.gameObject.tag == "Enemy")
				{
                    if (selectedObject) {
						//selectedObject.GetComponent<MeshRenderer>().material.color = Color.white;
						if(selectedObject.GetComponent<PlayerController>())
						{
                        	selectedObject.GetComponent<Character>().basicAttack(hit.collider.gameObject.GetComponent<Enemy>());
						}
                    }

                    selectedObject = hit.collider.gameObject;
                    //selectedObject.GetComponent<MeshRenderer>().material.color = Color.green;
                    HighlightTiles(false);

                    //Debug.Log("Hit enemy");
                }
            }
		}
		//Mouse Input Ends

		//Check if all players are inactive to end player turn
		if(currentTurn == Turn.PlayerTurn)
		{
			if(InactivePlayers(allPlayers))
			{
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
        }
		else //Enemy Turn
		{
			//find closest player, find path to player and stop 1 tile before. Attack player.
//			for(int i = 0; i < allEnemies.Count; i++){
//				allEnemies[i].target = allEnemies[i].FindClosestPlayer(allPlayers.ToArray()).GetComponent<PlayerController>().start;
//			}
			if(currentEnemy < allEnemies.Count)
			{
				if(!enemyMoving)
				{
					enemyMoving = true;
					selectedObject = allEnemies[currentEnemy].enemyObject;
					allEnemies[currentEnemy].isWalkable = this.isWalkable;
					allEnemies[currentEnemy].target = allEnemies[currentEnemy].FindClosestPlayer(allPlayers.ToArray()).GetComponent<PlayerController>().start;
				}
				else
				{
					if(allEnemies[currentEnemy].end == null || !allEnemies[currentEnemy].active)
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
	}

	void LateUpdate(){
		Debug.Log(currentTurn);
		if(currentTurn == Turn.PlayerTurn){
			foreach( PlayerController p in allPlayers){
				p.PlayerUpdate();
			}
		}
		else if(currentTurn == Turn.EnemyTurn){
			foreach( Enemy e in allEnemies){
				e.EnemyUpdate();
			}
		}
	}

    void HighlightTiles(bool playerSelected) {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        Character selectedChar = selectedObject.GetComponent<Character>();
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

    bool isWalkable(PathTile tile)
    {
        if (selectedObject.GetComponent<Character>().start == tile)
        {
            return true;
        }

		if (!selectedObject.GetComponent<Enemy>()) {
			foreach (PlayerController player in allPlayers) 
			{
				if (player.start == tile)
				{
					return false;
				}
			}
		}

		foreach (Enemy enemy in allEnemies) 
		{
			if (enemy.start == tile) 
			{
				return false;
			}
		}

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
}