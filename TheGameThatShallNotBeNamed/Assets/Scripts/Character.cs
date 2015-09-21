using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {


	public TileMap tileMap;
	public int listIndex;
	protected PathTile start;
	protected PathTile end;
	public List<PathTile> tileList;


	protected float speed;
	public Vector3 movement;

	protected void Move() {

		
		if (listIndex != tileList.Count /*&& currentActionPoints > -1*/) {
			
			movement = (tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) - transform.position;
			movement = movement.normalized * speed;
			
			transform.Translate(movement * Time.deltaTime);
			
			if (Vector3.Distance(transform.position, tileList[listIndex].transform.position + new Vector3(0f, 0.51f, 0f)) < 0.05f) {
				if (listIndex == tileList.Count - 1) {
					start = end;
					end = null;
					tileList.Clear();
					listIndex = 0;
				} else {
					//currentActionPoints--;
					listIndex++;
				}
			}
		}
	}



}
