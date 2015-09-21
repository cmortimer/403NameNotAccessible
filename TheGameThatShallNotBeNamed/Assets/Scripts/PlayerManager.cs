using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public Transform player;
	public GameObject selectedObject; //Currently selected player or enemy or tile
	public GameObject[] allPlayers;
	public GameObject[] allEnemies;
	enum Turn {PlayerTurn, EnemyTurn};
	Turn currentTurn;

	// Use this for initialization
	void Start () {
		allPlayers = GameObject.FindGameObjectsWithTag("Player");
		allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit, 100)) {
				
				//Debug.Log(hit.collider.gameObject.GetComponent<PathTile>());
				if(hit.collider.gameObject.tag == "Tile")
				{

				}
				else if(hit.collider.gameObject.tag == "Player") //FUTURE REFERENCE, SELECTED PLAYER TAG
				{

				}
				else if(hit.collider.gameObject.tag == "Enemy")
				{

				}
			}
		}
		if(currentTurn == Turn.PlayerTurn)
		{
			if(Inactive(allPlayers, 0))
			{
				currentTurn = Turn.EnemyTurn;
			}
		}
		else //Enemy Turn
		{
			if(Inactive(allEnemies, 1))
			{
				currentTurn = Turn.PlayerTurn;
			}
		}
	}

	bool Inactive(GameObject[] characters, int type)
	{
		foreach(GameObject c in characters)
		{
			if(type == 0)
			{
				if(c.GetComponent<PlayerController>().active)
				{
					return false;
				}
				else
				{
					continue;
				}
			}
			else
			{
//				if(c.GetComponent<EnemyController>().active)
//				{
//					return false;
//				}
//				else
//				{
//					continue;
//				}
				return false;
			}
		}
		return true;
	}
}