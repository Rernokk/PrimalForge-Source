using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnShrine : MonoBehaviour {
	public static List<SpawnShrine> ActiveShrines = new List<SpawnShrine>();

	void Start(){
		ActiveShrines.Add(this);
	}
}
