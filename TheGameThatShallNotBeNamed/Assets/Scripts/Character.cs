using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Character : MonoBehaviour {
	public string charName;
	public int health;
	public int strength;
	public int endurance;
	public int agility;
	public int magicSkill;
	public int luck;
	public int range;
	public int maxHealth = 25;

	public int currentActionPoints;
	public int maxActionPoints;

	public TileMap tileMap;
	public int listIndex;
	public PathTile start;
	public PathTile end;
	public List<PathTile> tileList;
	public bool active;
    public bool doneMoving;

	protected float speed;
	public Vector3 movement;

	public Predicate<PathTile> isWalkable;

	protected void Move() {

		if(start == end)
		{
			end = null;
			tileList.Clear();
			listIndex = 0;
			clearHighLights();
			doneMoving = true;
		}
		else if (listIndex != tileList.Count && currentActionPoints > 0) {
			movement = (tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) - transform.position;
			movement = movement.normalized * speed;
			
			transform.Translate(movement * Time.deltaTime);
			
			if (Vector3.Distance(transform.position, tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) < 0.05f) {
				if (listIndex == tileList.Count - 1) {
					start = end;
					end = null;
					tileList.Clear();
					listIndex = 0;
					currentActionPoints--;
                    clearHighLights();
                    doneMoving = true;
                }
				else {
					if(listIndex >= 1)
					{
						currentActionPoints--;
					}
					listIndex++;	
				}
			}
		}
	}

    protected void highLightPath()
    {
        for (int i = 0; i < tileList.Count; i++)
        {
            tileList[i].GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
    }

    protected void clearHighLights()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    public bool basicAttack(Character target) {
		if(currentActionPoints > 1)
		{
			bool inRange = false;
			List<PathTile> tilesInRange = new List<PathTile>();
			tilesInRange.Add(target.start);
			for(int i = 0; i < range; i++)
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
				if(start == pt)
				{
					inRange = true;
					break;
				}
				//This is a test that can be used to see what tiles are in range of an attack
				//The break above must be commented out to run this debug, as well as highlight tiles
				//pt.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
			}

			if(inRange)
			{
				Debug.Log(this.gameObject.name + " attacks " + target.gameObject.name);
				target.health -= (int)(Mathf.Round(this.strength - (target.endurance * .5f)));
				currentActionPoints -= 2;
				if(currentActionPoints <= 0)
					active = false;
			}
			else
			{
				//Move until in range and call attack again
				//Move();
				//basicAttack(target);
			}
			return true;
		}
		else
		{
			Debug.Log ("Not enough action points to attack.");
			return false;

		}
    }

	public void resetStatus()
	{
		currentActionPoints += (int)Mathf.Round(agility/2.0f);
		if(currentActionPoints > maxActionPoints) currentActionPoints = maxActionPoints;
		active = true;
		end = null;
		start = findClosestTile();
		listIndex = 0;
	}


	protected PathTile findClosestTile() {
		GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        //Debug.Log(tiles.Length + " tiles");
		
		if (tiles.Length == 0)
			return null;
		
		GameObject closest = tiles[0];
		float closestDistance = Vector3.Distance(transform.position, closest.transform.position);
		float thisDistance;
		
		for (int i = 1; i < tiles.Length; i++)
		{
			thisDistance = Vector3.Distance(transform.position, tiles[i].transform.position);
			//Debug.Log(thisDistance);
			//Debug.Log(closestDistance);
			if (thisDistance < closestDistance)
			{
				closest = tiles[i];
				closestDistance = thisDistance;
			}
		}
		
		//Debug.Log(closest);
		return closest.GetComponent<PathTile>();
	}

	public bool Defeated()
	{
		if(health <= 0)
		{
			Destroy(this.gameObject);
			return true;
		}
		return false;
	}

    public void setStats(string n, int hp, int st, int en, int ag, int mag, int lu, int rng)
    {
		charName = n;
        gameObject.name = n;
        health = hp;
		maxHealth = hp;
        strength = st;
        endurance = en;
        agility = ag;
        magicSkill = mag;
        luck = lu;
        range = rng;
		maxActionPoints = agility;
		currentActionPoints = (int)Mathf.Round(maxActionPoints/2.0f);
    }
}
