using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {
	protected int health;
	protected int strength;
	protected int endurance;
	protected int agility;
	protected int magicSkill;
	protected int luck;

	public int currentActionPoints;
	public int maxActionPoints;

	public TileMap tileMap;
	public int listIndex;
	public PathTile start;
	public PathTile end;
	public List<PathTile> tileList;
	public bool active;

	protected float speed;
	public Vector3 movement;

	protected void Move() {

		
		if (listIndex != tileList.Count && currentActionPoints > 0) {
			
			movement = (tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) - transform.position;
			movement = movement.normalized * speed;
			
			transform.Translate(movement * Time.deltaTime);
			
			if (Vector3.Distance(transform.position, tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) < 0.05f) {
				if (listIndex == tileList.Count - 1) {
					start = end;
					end = null;
					tileList.Clear();
					listIndex = 0;
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

    public void basicAttack(Character target) {
        //target.health -= this.strength;
		if(currentActionPoints > 1)
		{
        	Debug.Log(this.gameObject.name + " attacks " + target.gameObject.name);
			currentActionPoints -= 2;
			if(currentActionPoints <= 0)
				active = false;
		}
		else
		{
			Debug.Log ("Not enough action points to attack.");
			active = false;
		}
    }

	public void resetStatus()
	{
		//IMPORTANT NOTE: current action points breaks with movement, drops to -1 to insure full movement
		currentActionPoints += agility;
		active = true;
		end = null;
		start = findClosestTile();
	}


	protected PathTile findClosestTile() {
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
