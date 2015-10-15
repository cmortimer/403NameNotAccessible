using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class MenuManager : MonoBehaviour {
	
	public GameObject mainMenu;
	public GameObject workshopMenu;
	public GameObject destMenu;
	
	Equipment equip;
	
	//set correct menu options to active
	void Start(){
		equip = GameObject.Find ("PersistentData").GetComponent<Equipment>();
		destMenu.SetActive(false);
		workshopMenu.SetActive(false);
		SetupWorkshop();
	}
	
	//setup workshop
	public void SetupWorkshop(){
		for(int i=0;i<equip.allWeapons.Count;i++){
			//equip.allWeapons[i]
		}
	}
	
	//function handlers for each button yes this is ugly lay off...
	public void MainToDest(){
		mainMenu.SetActive(false);
		destMenu.SetActive(true);
		
	}
	
	public void DestToMain(){
		destMenu.SetActive(false);
		mainMenu.SetActive(true);
	}
	
	public void MainToWorkshop(){
		mainMenu.SetActive(false);
		workshopMenu.SetActive(true);
	}
	
	public void VisitDest(int level){
		Application.LoadLevel (2);
	}
}
