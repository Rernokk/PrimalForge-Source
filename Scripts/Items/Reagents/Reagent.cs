using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reagent {
	string reagentName;
	string reagentSprite;
	int reagentQuantity;

	public string ReagentName{
		get {
			return reagentName;
		}

		set {
			reagentName = value;
		}
	}

	public string ReagentSprite {
		get{
			return reagentSprite;
		}

		set{
			reagentSprite = value;
		}
	}

	public int ReagentQuantity {
		get {
			return reagentQuantity;
		}

		set {
			reagentQuantity = value;
		}
	}

	public Reagent(){

	}

	public Reagent(string name, string sprite, int quantity){

	}
}
