using UnityEngine;
using System.Collections;

public class SpawnDungeonManager : MonoBehaviour {

    public GameObject dungeonObj;

	// Use this for initialization
	void Start () {

        //Does the dungeon manager exist yet?
        GameObject temp = GameObject.FindGameObjectWithTag("DungeonManager");
        //Debug.Log("Temp: " + temp);
        if (temp == null)
        {
            //Debug.Log("No Manager");
            GameObject.Instantiate(dungeonObj, Vector3.zero, Quaternion.identity);
        }
        else
        {
            //Debug.Log("Manager Exists");
        }

    }

	// Update is called once per frame
	void Update () {
	
	}
}
