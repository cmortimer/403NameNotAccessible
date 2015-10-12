using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {

    public Transform player;
	public GameObject selectedObject; //Currently selected player or enemy or tile
	public List<PlayerController> allPlayers;
	public List<Enemy> allEnemies;
    public TileMap tileMap;
    public GameObject endTile;

	enum Turn {PlayerTurn, EnemyTurn};
	Turn currentTurn;

	// Use this for initialization
	void Start () {
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
        //Debug.Log (allPlayers.Length);
        //Debug.Log (allEnemies.Length);

        endTile = GameObject.FindGameObjectWithTag("EndTile");
    }
	
	// Update is called once per frame
	void Update () {
		if(selectedObject && selectedObject.tag == "Player")
        {
            //HighlightTiles();
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
                        tileMap.FindPath(tempChar.start, hit.collider.gameObject.GetComponent<PathTile>(), tempList);
                        
                        if ((tempList.Count - 1) <= tempChar.currentActionPoints)
                        {
                            tempChar.end = hit.collider.gameObject.GetComponent<PathTile>();
                        }

                        else
                        {
                            if (selectedObject)
                            {
                                selectedObject.GetComponent<MeshRenderer>().material.color = Color.white;
                            }

                            HighlightTiles(false);
                            selectedObject = null;
                        }
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
						selectedObject.GetComponent<MeshRenderer>().material.color = Color.white;
					}

                    selectedObject = hit.collider.gameObject;
                    selectedObject.GetComponent<MeshRenderer>().material.color = Color.green;
                    HighlightTiles(true);
					//Debug.Log("Hit player");
				}
				//Clicked on Enemy
				else if(hit.collider.gameObject.tag == "Enemy")
				{
                    if (selectedObject) {
						selectedObject.GetComponent<MeshRenderer>().material.color = Color.white;
						if(selectedObject.GetComponent<PlayerController>())
						{
                        	selectedObject.GetComponent<Character>().basicAttack(hit.collider.gameObject.GetComponent<Enemy>());
						}
                    }
                    
                    selectedObject = hit.collider.gameObject;
                    selectedObject.GetComponent<MeshRenderer>().material.color = Color.green;
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
                    Application.LoadLevel("DungeonGenTesting");
                }
            }
        }
		else //Enemy Turn
		{
			//find closest player, find path to player and stop 1 tile before. Attack player.
			for(int i = 0; i < allEnemies.Count; i++){
				allEnemies[i].target = allEnemies[i].FindClosestPlayer(allPlayers.ToArray()).GetComponent<PlayerController>().start;
			}
			//If all enemies are inactive, end turn
			if(InactiveEnemies(allEnemies))
			{
				currentTurn = Turn.PlayerTurn;
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
        List<PathTile> moveableTiles = new List<PathTile>(selectedChar.start.connections);

		for (int i = 0; i < tiles.Length; i++)
		{
			tiles[i].GetComponent<MeshRenderer>().material.color = Color.white;
		}

        if (playerSelected) {
            for (int i = 0; i < selectedChar.currentActionPoints; i++)
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
                            if (tempTile.connections[k].GetComponent<MeshRenderer>().material.color != Color.yellow)
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