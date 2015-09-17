using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public Transform player;
	public GameObject selectedPlayer;
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
		if(currentTurn == Turn.PlayerTurn)
		{
			if(Inactive(allPlayers, 0))
			{
				currentTurn = Turn.EnemyTurn;
			}
		}
		else
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