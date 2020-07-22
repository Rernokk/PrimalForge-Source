using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting_Select_Interface : MonoBehaviour
{
	int ind = 0;
	public int Ind
	{
		get
		{
			return ind;
		}

		set
		{
			ind = value;
		}
	}
	public void SelectItem()
	{
		GetComponentInParent<Profession_Interface>().SelectItem(Ind);
	}
}
