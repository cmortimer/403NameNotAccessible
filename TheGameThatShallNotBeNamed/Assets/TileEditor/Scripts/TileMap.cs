using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

public class TileMap : MonoBehaviour
{
	static Queue<PathTile> queue = new Queue<PathTile>();
	static List<PathTile> closed = new List<PathTile>();
	static Dictionary<PathTile, PathTile> source = new Dictionary<PathTile, PathTile>();

	public const int maxColumns = 10000;

	public float tileSize = 1;
	public Transform tilePrefab;
	public Transform endTilePrefab;
	public TileSet tileSet;
	public bool connectDiagonals;
	public bool cutCorners;

	public List<int> hashes = new List<int>(100000);
	public List<Transform> prefabs = new List<Transform>(100000);
	public List<int> directions = new List<int>(100000);
	public List<Transform> instances = new List<Transform>(100000);

	//Room Generation
	private int roomLimit = 13;
	private int[,] rooms;       //Room Array
	private int numRooms;		//How many rooms have been generated
	private int weight;			//The weight as to whether a room is generated
	private bool stairsSpawned;	//Do we have stairs yet?
    private SpawnEnemies spawnEnemies;
    private DungeonManager dungeonManager;

    public GameObject manager;
    public GameObject dungeonObj;

	void Start()
	{
		rooms = new int[7,7];
        spawnEnemies = manager.GetComponent<SpawnEnemies>();

        dungeonManager = GameObject.FindGameObjectWithTag("DungeonManager").GetComponent<DungeonManager>();

		generateFloor();
		//Debug.Log(numRooms);

		UpdateConnections();

	}

    void Update()
    {
    }

	public int GetHash(int x, int z)
	{
		return (x + TileMap.maxColumns / 2) + (z + TileMap.maxColumns / 2) * TileMap.maxColumns;
	}
	
	public int GetIndex(int x, int z)
	{
		return hashes.IndexOf(GetHash(x, z));
	}
	
	public Vector3 GetPosition(int index)
	{
		index = hashes[index];
		return new Vector3(((index % maxColumns) - (maxColumns / 2)) * tileSize, 0, ((index / maxColumns) - (maxColumns / 2)) * tileSize);
	}
	public void GetPosition(int index, out int x, out int z)
	{
		index = hashes[index];
		x = (index % maxColumns) - (maxColumns / 2);
		z = (index / maxColumns) - (maxColumns / 2);
	}

	public void UpdateConnections()
	{
		//Build connections
		PathTile r, l, f, b;
		for (int i = 0; i < instances.Count; i++)
		{
			var tile = instances[i].GetComponent<PathTile>();
			if (tile != null)
			{
				int x, z;
				GetPosition(i, out x, out z);
				tile.connections.Clear();
				r = Connect(tile, x, z, x + 1, z);
				l = Connect(tile, x, z, x - 1, z);
				f = Connect(tile, x, z, x, z + 1);
				b = Connect(tile, x, z, x, z - 1);
				if (connectDiagonals)
				{
					if (cutCorners)
					{
						Connect(tile, x, z, x + 1, z + 1);
						Connect(tile, x, z, x - 1, z - 1);
						Connect(tile, x, z, x - 1, z + 1);
						Connect(tile, x, z, x + 1, z - 1);
					}
					else
					{
						if (r != null && f != null)
							Connect(tile, x, z, x + 1, z + 1);
						if (l != null && b != null)
							Connect(tile, x, z, x - 1, z - 1);
						if (l != null && f != null)
							Connect(tile, x, z, x - 1, z + 1);
						if (r != null && b != null)
							Connect(tile, x, z, x + 1, z - 1);
					}
				}
			}
		}
	}

	PathTile Connect(PathTile tile, int x, int z, int toX, int toZ)
	{
		var index = GetIndex(toX, toZ);
		if (index >= 0)
		{
			var other = instances[index].GetComponent<PathTile>();
			if (other != null)
			{
				tile.connections.Add(other);
				return other;
			}
		}
		return null;
	}

	PathTile GetPathTile(int x, int z)
	{
		var index = GetIndex(x, z);
		if (index >= 0)
			return instances[index].GetComponent<PathTile>();
		else
			return null;
	}
	public PathTile GetPathTile(Vector3 position)
	{
		var x = Mathf.RoundToInt(position.x / tileSize);
		var z = Mathf.RoundToInt(position.z / tileSize);
		return GetPathTile(x, z);
	}
	
	public bool FindPath(PathTile start, PathTile end, List<PathTile> path, Predicate<PathTile> isWalkable)
	{
		if (!isWalkable(end))
			return false;
		closed.Clear();
		source.Clear();
		queue.Clear();
		closed.Add(start);
		source.Add(start, null);
		if (isWalkable(start))
			queue.Enqueue(start);
		while (queue.Count > 0)
		{
			var tile = queue.Dequeue();
			if (tile == end)
			{
				path.Clear();
				while (tile != null)
				{
					path.Add(tile);
					tile = source[tile];
				}
				path.Reverse();
				return true;
			}
			else
			{
				foreach (var connection in tile.connections)
				{
					if (!closed.Contains(connection) && isWalkable(connection))
					{
						closed.Add(connection);
						source.Add(connection, tile);
						queue.Enqueue(connection);
					}
				}
			}
		}
		return false;
	}
	public bool FindPath(PathTile start, PathTile end, List<PathTile> path)
	{
		return FindPath(start, end, path, tile => true);
	}
	public bool FindPath(Vector3 start, Vector3 end, List<PathTile> path, Predicate<PathTile> isWalkable)
	{
		var startTile = GetPathTile(start);
		var endTile = GetPathTile(end);
		return startTile != null && endTile != null && FindPath(startTile, endTile, path, isWalkable);
	}
	public bool FindPath(Vector3 start, Vector3 end, List<PathTile> path)
	{
		return FindPath(start, end, path, tile => true);
	}

	//Generate a single room
	void generateRoom(int xPos, int zPos)
	{
		int hash;
		int index; 

		//Make new tiles
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				//Adjust position to generate correctly
				float spawnX = xPos + i + 4 * xPos;		
				float spawnZ = zPos + j + 4 * zPos;
				hash = GetHash((int)spawnX, (int)spawnZ);
				index = prefabs.Count;
				//Create new tile
				index = prefabs.Count;
				hashes.Add(hash);
				prefabs.Add(tilePrefab);
				directions.Add(0);
				instances.Add(null);
				
				//Destroy Immediate
				//DestroyImmediate(instances[index].gameObject);
				
				//Place the tile
				Transform instance = (Transform)Instantiate(prefabs[index]);
				//Transform instance = (Transform)PrefabUtility.InstantiatePrefab(prefabs[index]);
				instance.parent = transform;
				//instance.localPosition = GetPosition(index);
				instance.localPosition = new Vector3(spawnX, 0, spawnZ);
				instance.localRotation = Quaternion.Euler(0, directions[index] * 90, 0);
				instances[index] = instance;
			}
		}
	}

	//Generate a floor
	void generateFloor()
	{
		float rand;             //randomly generated number to determine if a room spawns
		float weight = 1f;           //weight of a room spawn
		int cycles = 0;
		
		//Room 0
		rooms[1, 1] = 1;
		numRooms = 1;
		
		//Generate until complete
		while (numRooms < roomLimit && cycles < 10)
		{
			for (int i = 1; i < 5; i++)
			{
				for (int j = 1; j < 5; j++)
				{
					//Generate a random number and increase by weight
					rand = UnityEngine.Random.Range(0, 2);
					rand += weight;
					//Are we still under the room limit?
					if (numRooms < roomLimit && rand >= 1)
					{
						//Is the current cell already occupied?
						if (rooms[i, j] == 0)
						{
							//Is there a room to connect to?
							if (rooms[i - 1, j] != 0 || rooms[i + 1, j] != 0 || rooms[i, j - 1] != 0 || rooms[i, j + 1] != 0)
							{
								rooms[i, j] = 1;
								//roomPos = new Vector3(transform.position.x + i * 6, 0, transform.position.z + j*6);
								//Instantiate(rooms[i, j], roomPos, Quaternion.identity);
								numRooms++;
							}
						}
					}
				}
				//Decrease weight as you go further into the dungeon
				weight -= 0.1f;
				cycles++;
			}
		}


		//Populate the game environment
		for (int i =0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(rooms[i,j] == 1)
				{
					generateRoom(i, j);
				}
			}
		}


		//Make the end tile
		Vector3 endTilePos = instances[instances.Count - 1].transform.position;
        endTilePos.y += 0.01f;
		Transform.Instantiate(endTilePrefab, endTilePos, Quaternion.identity);

	}

    //Regenerates a floor
    public void regenFloor()
    {
        //Have we cleared the dungeon

        //Clear arrays and lists
        Array.Clear(rooms, 0, rooms.Length);
        instances.Clear();
        hashes.Clear();
        prefabs.Clear();
        directions.Clear();

        //Destroy existing tiles
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Tile"))
            Destroy(g);

        Destroy(GameObject.FindGameObjectWithTag("EndTile"));

        //Regenerate floor
        generateFloor();

        UpdateConnections();

        manager.GetComponent<PlayerManager>().endTile = GameObject.FindGameObjectWithTag("EndTile");

        //Delete and respawn enemies
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(g);

        spawnEnemies.populateFloor();

        //Update player
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerController p = g.GetComponent<PlayerController>();
            p.resetStatus();
            
            Debug.Log("Player start is " + p.start.gameObject.transform.position);
        }

    }

    public int[,] getRooms()
    {
        return rooms;
    }

}
