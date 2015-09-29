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
		maxActionPoints = 10;
		currentActionPoints = maxActionPoints;

		health = 25;
		strength = 5;
		endurance = 5;
		agility = 5;
		magicSkill = 5;
		luck = 5;
        //attack = 1;
        //defense = 1;
        speed = 5.0f;

        listIndex = 0;

        tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();
        tileMap.UpdateConnections();

        start = findClosestTile();
		transform.position = new Vector3(start.transform.position.x, 0.5f, start.transform.position.z);
        //Debug.Log(start);
    }
	
	// Update is called once per frame
	public void PlayerUpdate () {

//		if (Input.GetMouseButtonDown(0)) {
//			
//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			RaycastHit hit;
//			
//			if (Physics.Raycast(ray, out hit, 100)) {
//				
//				//Debug.Log(hit.collider.gameObject.GetComponent<PathTile>());
//				if(hit.collider.gameObject.tag == "Tile")
//				{
//					end = hit.collider.gameObject.GetComponent<PathTile>();
//				}
//			}
//		}
		if(currentActionPoints < 1)
		{
			active = false;
		}


        if (start && end) {
			tileMap.FindPath(start, end, tileList);
            Move();
        }
    }
}
