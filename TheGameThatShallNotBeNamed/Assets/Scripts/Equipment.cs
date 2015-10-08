using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class Equipment : MonoBehaviour{
	//a single weapon
	public struct weapon{
		public string name;
		public string desc;
		public int str;
		public int end ;
		public int agi ;
		public int mag ;
		public int luck ;
		public int rangeMin ;
		public int rangeMax;
		public List<string> recipe;
		//something for an image to show
	}

	//a single piece of armor
	public struct armor{
		public string name;
		public string desc;
		public int str;
		public int end ;
		public int agi ;
		public int mag ;
		public int luck ;
		public List<string> recipe;
		//image?
	}

	//a list of every weapon that exists in the game...
	public List<weapon> allWeapons = new List<weapon>();

	//same thing for armor...
	public List<armor> allArmor = new List<armor>();

	public bool LoadEquipment(string fileName){

		try
		{
			string line;

			StreamReader reader = new StreamReader(fileName, Encoding.Default);

			using (reader)
			{
				do
				{
					line = reader.ReadLine();

					if (line != null)
					{
						//skip comments
						if(line[0] == 'c'){	continue; }
						//split content lines
						string[] entries = line.Split(',');
						if(entries[0][0] == 'w'){
							weapon temp = new weapon();
							temp.name = entries[1];
							temp.desc = entries[2];
							temp.str = Int32.Parse(entries[3]);
							temp.end = Int32.Parse(entries[4]);
							temp.agi = Int32.Parse(entries[5]);
							temp.mag = Int32.Parse(entries[6]);
							temp.luck = Int32.Parse(entries[7]);
							temp.rangeMin = Int32.Parse(entries[8]);
							temp.rangeMax = Int32.Parse(entries[9]);
							temp.recipe = new List<string>();
							for(int i=10;i<entries.Length;i++){

								temp.recipe.Add(entries[i]);
							}
							allWeapons.Add(temp);
						}
						if(entries[0][0] == 'a'){
							armor temp = new armor();
							temp.name = entries[1];
							temp.desc = entries[2];
							temp.str = Int32.Parse(entries[3]);
							temp.end = Int32.Parse(entries[4]);
							temp.agi = Int32.Parse(entries[5]);
							temp.mag = Int32.Parse(entries[6]);
							temp.luck = Int32.Parse(entries[7]);
							temp.recipe = new List<string>();
							for(int i=8;i<entries.Length;i++){
								temp.recipe.Add(entries[i]);
							}
							allArmor.Add(temp);
						}

					}
				}
				while (line != null);

				reader.Close();
				//Debug.Log(allArmor.Count);
				return true;
			}
		}
		catch(IOException e){
			Debug.Log (e.Message);
			return false;
		}
	}
}
