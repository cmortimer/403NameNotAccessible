using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class SpawnEnemies : MonoBehaviour {

	public List<Enemy> enemyList;
    public GameObject enemy;
    private TileMap tileMap;
    private int[,] rooms;
    private string pool;

	// Use this for initialization
	void Start () {
        pool = GameObject.FindGameObjectWithTag("DungeonManager").GetComponent<DungeonManager>().getPool();

		enemyList = new List<Enemy>();
		XmlReader xmlReader = XmlReader.Create("Assets/Scripts/EnemyList.xml");
		while(xmlReader.Read ())
		{
			if((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "Enemy"))
			{
				if(xmlReader.HasAttributes)
				{
                    if (xmlReader.GetAttribute("pool") == pool)
                    {
                        Enemy tempEnemy = new Enemy();
                        tempEnemy.charName = xmlReader.GetAttribute("name");
                        //tempEnemy. = xmlReader.GetAttribute("desc");
                        tempEnemy.health = int.Parse(xmlReader.GetAttribute("health"));
                        tempEnemy.strength = int.Parse(xmlReader.GetAttribute("str"));
                        tempEnemy.endurance = int.Parse(xmlReader.GetAttribute("end"));
                        tempEnemy.agility = int.Parse(xmlReader.GetAttribute("agi"));
                        tempEnemy.magicSkill = int.Parse(xmlReader.GetAttribute("mag"));
                        tempEnemy.luck = int.Parse(xmlReader.GetAttribute("luck"));
                        tempEnemy.range = int.Parse(xmlReader.GetAttribute("range"));
                        tempEnemy.drop = xmlReader.GetAttribute("drop");
                        enemyList.Add(tempEnemy);
                    }
				}
			}
        }
        //Debug.Log("Possible enemies: " + enemyList.Count);
        populateFloor();
	}
	
	// Update is called once per frame
	void Update () {

	}

    //Populates the floor with enemies
    public void populateFloor()
    {
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<TileMap>();
        rooms = tileMap.getRooms();

        //Try to spawn enemies in each room
        for (int i = 0; i < 7; i++)
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
						
                    	GameObject tempEnemyRef = (GameObject)GameObject.Instantiate(enemy, new Vector3(spawnX, 1, spawnZ), Quaternion.identity);
						Enemy tempEnemy = enemyList[Mathf.FloorToInt(Random.value * enemyList.Count)];
						tempEnemyRef.GetComponent<Enemy>().setStats(tempEnemy.charName, tempEnemy.health, tempEnemy.strength,
						                                            tempEnemy.endurance, tempEnemy.agility,tempEnemy.magicSkill,
						                                            tempEnemy.luck, tempEnemy.range, tempEnemy.drop);

						//tempEnemyRef.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>(tempEnemy.charName + ".png");
					}
                }
            }
        }
    }
}
