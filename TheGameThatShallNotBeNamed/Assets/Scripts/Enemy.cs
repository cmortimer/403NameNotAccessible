using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Character {



	// Use this for initialization
	void Start () {
		//health = 1;
		//attack = 1;
		//defense = 1;
		speed = 5.0f;
		tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
		tileMap.UpdateConnections();

		start = findClosestTile();
		transform.position.Set(start.transform.position.x, 0.5f, start.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown(0)) {
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit, 100)) {
				
				//Debug.Log(hit.collider.gameObject.GetComponent<PathTile>());
				if(hit.collider.gameObject.tag == "Tile")
				{
					end = hit.collider.gameObject.GetComponent<PathTile>();
				}
			}
		}

		if (start && end) {

			tileMap.FindPath(start, end, tileList);

			List<PathTile> newPath = new List<PathTile>();
			for(int i = 0; i < tileList.Count -1; i++){
				newPath.Add(tileList[i]);
			}
			tileList = newPath;

			Move();
		}
	}

	PathTile findClosestTile() {
		GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
		
		if (tiles.Length == 0)
			return null;
		
		GameObject closest = tiles[0];
		float closestDistance = Vector3.Distance(transform.position, closest.transform.position);
		float thisDistance;
		
		for (int i = 1; i < tiles.Length; i++)
		{
			thisDistance = Vector3.Distance(transform.position, tiles[i].transform.position);
			if (thisDistance < closestDistance)
			{
				closest = tiles[i];
				closestDistance = thisDistance;
			}
		}
		
		//Debug.Log(closest);
		return closest.GetComponent<PathTile>();
	}
}


