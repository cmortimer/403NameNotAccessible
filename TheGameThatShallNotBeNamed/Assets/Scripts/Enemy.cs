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
		transform.position = new Vector3(start.transform.position.x, 0.5f, start.transform.position.z);
	}
	
	// Update is called once per frame
	public void EnemyUpdate() {
		if(currentActionPoints < 1)
		{
			active = false;
		}

		if (active && start && target) {
	
			tileMap.FindPath(start, target, tileList);

			if(tileList.Count > 2){
				List<PathTile> newPath = new List<PathTile>();
				for(int i = 0; i < tileList.Count -1; i++){
					newPath.Add(tileList[i]);
				}
				tileList = newPath;
					
				if(tileList.Count > 0)
					end = tileList[tileList.Count-1];

				Move();
			}
		}
		else
		{
			listIndex = 0;
		}

		if(tileList.Count == 2)	Attack();
	}

	//helpers
	public void Attack(){
			basicAttack(pTarget);
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


