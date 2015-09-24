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

	protected int currentActionPoints;
	protected int maxActionPoints;

	public TileMap tileMap;
	public int listIndex;
	protected PathTile start;
	public PathTile end;
	public List<PathTile> tileList;
	public bool active;

	protected float speed;
	public Vector3 movement;

	protected void Move() {

		
		if (listIndex != tileList.Count && currentActionPoints > -1) {
			
			movement = (tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) - transform.position;
			movement = movement.normalized * speed;
			
			transform.Translate(movement * Time.deltaTime);
			
			if (Vector3.Distance(transform.position, tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) < 0.05f) {
				if (listIndex == tileList.Count - 1) {
					start = end;
					end = null;
					tileList.Clear();
					listIndex = 0;
				} else {
					currentActionPoints--;
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
		}
		else
		{
			Debug.Log ("Not enough action points to attack.");
		}
    }

	public void resetStatus()
	{
		//IMPORTANT NOTE: current action points breaks with movement, drops to -1 to insure full movement
		currentActionPoints += agility;
		active = true;
	}



}
