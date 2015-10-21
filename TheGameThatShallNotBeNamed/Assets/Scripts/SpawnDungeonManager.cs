using UnityEngine;
using System.Collections;

public class SpawnDungeonManager : MonoBehaviour {

    public GameObject dungeonObj;

	// Use this for initialization
	void Start () {

       //Does the dungeon manager exist yet?
       if (GameObject.FindGameObjectWithTag("DungeonManager") == null)
       {
           GameObject.Instantiate(dungeonObj);
       }

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
