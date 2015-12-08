using UnityEngine;
using System.Collections;

public class SpawnPlayers : MonoBehaviour {

    public GameObject player;   //Temporary player prefab, in the future will load from a file

	// Use this for initialization
	void Start () {
		Debug.Log ("here");
        //Check that players aren't already spawned
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj == null)
        {
			Debug.Log ("Here");
            GameObject.Instantiate(player, new Vector3(5, 0.5f, 5), Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
