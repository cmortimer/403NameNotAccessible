using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Enemy : Character {

	public PathTile target = null;
	public Predicate<PathTile> isWalkable;
	public GameObject enemyObject;
	[SerializeField]
	protected PlayerController pTarget = null;

    public string drop;     //The item the enemy drops

	public List<PathTile> debugTileList;

    // Use this for initialization
    void Start () {
		active = true;
		//maxActionPoints = 10;
		//currentActionPoints = maxActionPoints;
		enemyObject = this.gameObject;

		//health = 25;
		//strength = 5;
		//endurance = 5;
		//agility = 5;
		//magicSkill = 5;
		//luck = 5;
		//health = 1;
		//attack = 1;
		//defense = 1;
		speed = 5.0f;
		//range = 1;

		tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
		tileMap.UpdateConnections();

		start = findClosestTile();
		transform.position = new Vector3(start.transform.position.x, 0.5f, start.transform.position.z);
	}

	//Only called on enemies turn
	public void EnemyUpdate() {
		if(currentActionPoints < 1)
		{
			active = false;
			if(end)
			{
				start = findClosestTile();
				end = null;
			}
			return;
		}

		if (start && target) {
			tileMap.FindPath(start, target, tileList, isWalkable);
			//tileMap.FindPath(start, target, tileList);
			if(tileList.Count > range + 1){
				List<PathTile> newPath = new List<PathTile>();
				for(int i = 0; i < tileList.Count -1; i++){
					newPath.Add(tileList[i]);
				}
				tileList = newPath;
					
				if(tileList.Count > 0)
				{
					debugTileList = new List<PathTile>(tileList);
					end = tileList[tileList.Count-1];
				}

				Move();
			}
			else if(currentActionPoints <= 1)
				active = false;
			else
				Attack();
		}

		//if(tileList.Count == range)	Attack();
	}

	//helpers
	public void Attack(){
		if(!(basicAttack(pTarget)))
		{
			active = false;
		}
	}

	public PlayerController FindClosestPlayer(PlayerController[] players){
		if(players.Length == 0) return null;

		PlayerController closest = null;
		List<PathTile> currentPath = new List<PathTile>();
		List<PathTile> shortestPath = new List<PathTile>();

		
		for (int i = 0; i < players.Length; i++) 
		{
			target = players[i].start;
			tileMap.FindPath(start, players[i].start, currentPath, isWalkable);

			if(currentPath.Count == 0) continue;

			if (shortestPath.Count == 0 || currentPath.Count <= shortestPath.Count)
			{
				Debug.Log("Changing targets");
				//shortestPath.Clear();
				tileMap.FindPath(start, players[i].start, shortestPath, isWalkable);
				closest = players[i];
			}
			
			//currentPath.Clear();
		}
		
		if(closest)
			pTarget = closest;
		else
			active = false;
		return closest;
	}

    public void setStats(string n, int hp, int st, int en, int ag, int mag, int lu, int rng, string drp)
    {
        charName = n;
        gameObject.name = n;
        health = hp;
		maxHealth = hp;
        strength = st;
        endurance = en;
        agility = ag;
		maxActionPoints = agility;
		currentActionPoints = (int)Mathf.Round(maxActionPoints/2.0f);
        magicSkill = mag;
        luck = lu;
        range = rng;
        drop = drp;
    }

    public void resetStatus()
	{
		target = null;
		base.resetStatus();
	}
}


