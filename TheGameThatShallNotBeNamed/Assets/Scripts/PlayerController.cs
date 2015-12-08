using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : Character {

	private GameObject equippedWeapon;
	private GameObject equippedArmor;

	public string description;
	// Use this for initialization
	void Start () {
		active = true;
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
