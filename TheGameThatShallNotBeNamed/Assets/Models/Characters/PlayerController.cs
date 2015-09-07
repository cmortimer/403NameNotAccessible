using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public float health;
    public float attack;
    public float defense;
    public float speed;

    public Vector3 movement;

    public PathTile start;
    public List<PathTile> tileList;
    public int listIndex;

    void Move(PathTile target) {
        /*
        GameObject.Find("TileMap").GetComponent<TileMap>().FindPath(start, target, tileList);

        movement = (tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) - transform.position;
        movement = movement.normalized * speed;

        transform.Translate(movement * Time.deltaTime);

        if (Vector3.Distance(transform.position, tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) < 0.05f)
        {
            if (listIndex == tileList.Count - 1)
            {
            }

            else
            {
                listIndex++;
            }
        }
        */
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
