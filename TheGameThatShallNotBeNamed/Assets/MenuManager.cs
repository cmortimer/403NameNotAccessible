using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	public Button[] mainButtons = new Button[5];
	public Button[] destinations = new Button[5];

	//set correct menu options to active
	void Start(){
		foreach(Button b in destinations){
			b.gameObject.SetActive (false);
		}
	}


	//function handlers for each button
	public void MainToDest(){
		foreach(Button b in mainButtons){
			b.gameObject.SetActive(false);
		}
		foreach(Button b2 in destinations){
			b2.gameObject.SetActive (true);
		}

	}

	public void DestToMain(){
		foreach(Button b in destinations){
			b.gameObject.SetActive(false);
		}
		foreach(Button b2 in mainButtons){
			b2.gameObject.SetActive (true);
		}
		
	}

	public void VisitDest(int level){
		Application.LoadLevel (1);
	}
}
