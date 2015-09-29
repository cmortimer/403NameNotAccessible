using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Character {

	public PathTile target = null;
	protected PlayerController pTarget = null;

	// Use this for initialization
	void Start () {
		active = true;
		maxActionPoints = 10;
		currentActionPoints = maxActionPoints;

		health = 25;
		strength = 5;
		endurance = 5;
		agility = 5;
		magicSkill = 5;
		luck = 5;
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
	public void EnemyUpdate() {
		if(currentActionPoints < 1)
		{
			currentActionPoints = maxActionPoints;
			active = false;
		}

		if (active && start && end) {
	
			tileMap.FindPath(start, end, tileList);

			List<PathTile> newPath = new List<PathTile>();
			for(int i = 0; i < tileList.Count -1; i++){
				newPath.Add(tileList[i]);
			}
			tileList = newPath;

			if(tileList.Count > 0)
				target = tileList[tileList.Count-1];

			Move();
		}

		if(active && target != null && Vector3.Distance(target.transform.position, transform.position) < .515f)
			Attack();
	}

	public void Attack(){
			basicAttack(pTarget);
	}


	//helpers
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

	public PlayerController FindClosestPlayer(PlayerController[] players){
		if(players.Length == 0) return null;

		//Debug.Log (players[0].transform);
		PlayerController closest = players[0];
		float closeDist = Vector3.Distance(transform.position, closest.transform.position);
		float curDist;

		for (int i = 1; i < players.Length; i++)
		{
			curDist = Vector3.Distance(transform.position, players[i].transform.position);
			if (curDist < closeDist)
			{
				closest = players[i];
				closeDist = curDist;
			}
		}
		pTarget = closest;
		return closest;
	}
}


