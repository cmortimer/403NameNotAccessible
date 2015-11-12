using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : Character {
	
	//private int currentActionPoints;
	//private int maxActionPoints;
	//public bool active;

	private GameObject equippedWeapon;
	private GameObject equippedArmor;

    //public float health;
    //public float attack;
    //public float defense;
    //public float speed;

    //public Vector3 movement;

    //public PathTile start;
    //public PathTile end;
    //public List<PathTile> tileList;
    //public TileMap tileMap;
    //public int listIndex;

    //    void Move() {
    //
    //        tileMap.FindPath(start, end, tileList);
    //
    //        if (listIndex != tileList.Count && currentActionPoints > -1) {
    //
    //            movement = (tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) - transform.position;
    //            movement = movement.normalized * speed;
    //
    //            transform.Translate(movement * Time.deltaTime);
    //
    //            if (Vector3.Distance(transform.position, tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) < 0.05f) {
    //                if (listIndex == tileList.Count - 1) {
    //                    start = end;
    //                    end = null;
    //                    tileList.Clear();
    //                    listIndex = 0;
    //                } else {
    //					currentActionPoints--;
    //                    listIndex++;
    //                }
    //            }
    //        }
    //    }


	// Use this for initialization
	void Start () {
		active = true;
		//maxActionPoints = 10;
		//currentActionPoints = maxActionPoints;

		//health = 9999;
		//strength = 5;
		//endurance = 5;
		//agility = 5;
		//magicSkill = 5;
		//luck = 5;
		//range = 2;
        //attack = 1;
        //defense = 1;
        speed = 5.0f;

        listIndex = 0;

        tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
        tileMap.UpdateConnections();

        start = findClosestTile();
		transform.position = new Vector3(start.transform.position.x, 0.5f, start.transform.position.z);
        //Debug.Log(start);
        DontDestroyOnLoad(this.gameObject);
    }

	//Only called on player's turn
	public void PlayerUpdate () {
		if(currentActionPoints < 1)
		{
			active = false;
		}

        if (start && end)
        {
            clearHighLights();
            highLightPath();
			//tileMap.FindPath(start, end, tileList);
            tileMap.FindPath(start, end, tileList, isWalkable);
            Move();
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("Loading :" + level);
        if (level == 2)
        {
            StartCoroutine(nextLevel());
        }
    }

    IEnumerator nextLevel()
    {
        tileList.Clear();
        yield return new WaitForSeconds(0.01f);

        active = true;
        maxActionPoints = 10;
        currentActionPoints = maxActionPoints;

        listIndex = 0;

        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();
        Debug.Log(tileMap);
        tileMap.UpdateConnections();

        start = findClosestTile();
        Debug.Log("Start at " + start.transform.position);
        transform.position = new Vector3(start.transform.position.x, 0.5f, start.transform.position.z);
    }
}
