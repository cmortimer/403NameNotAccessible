using UnityEngine;
using System.Collections;

public class SpawnDungeonManager : MonoBehaviour {

    public GameObject dungeonObj;
    public GameObject persistentObj;

	// Use this for initialization
	void Awake () {

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


        //Does the dungeon manager exist yet?
        temp = GameObject.FindGameObjectWithTag("Persistent");
        //Debug.Log("Temp: " + temp);
        if (temp == null)
        {
            //Debug.Log("No Manager");
            Vector3 pos = new Vector3(6.318826f, 1.210302f, 6.469446f);
            GameObject.Instantiate(persistentObj, pos, Quaternion.identity);
        }
        else
        {
            //Debug.Log("Persistent Manager Exists");
        }

    }

	// Update is called once per frame
	void Update () {
	
	}
}
