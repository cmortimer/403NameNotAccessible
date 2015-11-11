using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	[SerializeField]
	Camera mainCamera;
	[SerializeField]
	int maxSize;
	[SerializeField]
	int minSize;
	
	public bool followEnemy;
	public GameObject selectedObject;
	
	// Use this for initialization
	void Start () {
		followEnemy = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey("w"))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + .1f);
		}
		else if(Input.GetKey("s"))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - .1f);
		}
		if(Input.GetKey("d"))
		{
			transform.position = new Vector3(transform.position.x + .1f, transform.position.y, transform.position.z);
		}
		else if(Input.GetKey("a"))
		{
			transform.position = new Vector3(transform.position.x - .1f, transform.position.y, transform.position.z);
		}
		if (followEnemy)
		{
			transform.position = new Vector3(selectedObject.transform.position.x, transform.position.y, selectedObject.transform.position.z);
		}
		if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			mainCamera.orthographicSize++;
			if(mainCamera.orthographicSize > maxSize)
			{
				mainCamera.orthographicSize = maxSize;
			}
		}
		if(Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			mainCamera.orthographicSize--;
			if(mainCamera.orthographicSize < minSize)
			{
				mainCamera.orthographicSize = minSize;
			}
		}
	}
}
