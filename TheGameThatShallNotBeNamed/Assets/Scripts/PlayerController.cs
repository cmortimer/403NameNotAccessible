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
    public PathTile end;
    public List<PathTile> tileList;
    public TileMap tileMap;
    public int listIndex;

    void Move() {

        if (listIndex != tileList.Count - 1) {

            tileMap.FindPath(start, end, tileList);

            Debug.Log(tileList.Count);

            //movement = (tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) - transform.position;
            //movement = movement.normalized * speed;
            //
            //transform.Translate(movement * Time.deltaTime);
            //
            //if (Vector3.Distance(transform.position, tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) < 0.05f) {
            //    listIndex++;
            //}
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

        return closest.GetComponent<PathTile>();
    }

	// Use this for initialization
	void Start () {

        health = 1;
        attack = 1;
        defense = 1;
        speed = 5.0f;

        listIndex = 0;

        tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();

        start = findClosestTile();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100)) {
                end = hit.collider.gameObject.GetComponent<PathTile>();
            }
        }

        if (start && end) {
            Move();
        }
    }
}
