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
		Debug.Log(currentTurn);
		if (Input.GetMouseButtonDown(0)) {
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit, 100)) {
				
				//Debug.Log(hit.collider.gameObject.GetComponent<PathTile>());
				if(hit.collider.gameObject.tag == "Tile")
				{
					if(selectedObject && selectedObject.GetComponent<PlayerController>())
					{
						selectedObject.GetComponent<PlayerController>().end = hit.collider.gameObject.GetComponent<PathTile>();
					}
//					else if(selectedObject.GetComponent<Enemy>())
//					{
//						selectedObject.GetComponent<Enemy>().end = hit.collider.gameObject.GetComponent<PathTile>();
//					}
				}
				else if(hit.collider.gameObject.tag == "Player") //FUTURE REFERENCE, SELECTED PLAYER TAG
				{
					selectedObject = hit.collider.gameObject;
					Debug.Log("Hit player");
				}
				else if(hit.collider.gameObject.tag == "Enemy")
				{
					selectedObject = hit.collider.gameObject;
					Debug.Log("Hit enemy");
				}
			}
		}
		if(currentTurn == Turn.PlayerTurn)
		{
			if(Inactive(allPlayers, 0))
			{
				currentTurn = Turn.EnemyTurn;
				foreach(GameObject g in allPlayers)
				{
					g.GetComponent<PlayerController>().active = true;

				}
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

	//checks if all of a team is inactive, to progress turns
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
				if(c.GetComponent<Enemy>().active)
				{
					return false;
				}
				else
				{
					continue;
				}
				return false;
			}
		}
		return true;
	}
}