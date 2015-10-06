using UnityEngine;
using System.Collections;

public class SpawnEnemies : MonoBehaviour {

    public GameObject enemy;
    private TileMap tileMap;
    private int[,] rooms;

	// Use this for initialization
	void Start () {
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();
        rooms = tileMap.getRooms();

        populateFloor();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //Populates the floor with enemies
    void populateFloor()
    {
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                if(rooms[i,j] == 1)
                {
					int spawnCheck = Random.Range(0, 2);
					if(spawnCheck == 0)
					{
                    	float spawnX = transform.position.x + i * 5 + 2;
                    	float spawnZ = transform.position.z + j * 5 + 2;
						
                    	GameObject.Instantiate(enemy, new Vector3(spawnX, 1, spawnZ), Quaternion.identity);
					}
                }
            }
        }
    }
}
