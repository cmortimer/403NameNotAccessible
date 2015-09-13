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

            movement = (tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) - transform.position;
            movement = movement.normalized * speed;

            transform.Translate(movement * Time.deltaTime);

            if (Vector3.Distance(transform.position, tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) < 0.05f) {
                listIndex++;
            }
        }
    }

	// Use this for initialization
	void Start () {

        health = 1;
        attack = 1;
        defense = 1;
        speed = 5.0f;

        tileMap = GameObject.Find("TileMap").GetComponent<TileMap>();

        //start = tileMap.GetPathTile(transform.position);      // cannot find correct path tile; always returns null
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100)) {
                //end = tileMap.GetPathTile(hit.collider.gameObject.transform.position);        // cannot find correct path tile; always returns null
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
            }
        }

        if (start && end) {
            Move();
        }
    }
}
