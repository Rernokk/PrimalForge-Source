using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Profession_Select_Interface : MonoBehaviour {
	public void SelectProfession(){
		GetComponentInParent<Profession_Interface>().SelectProfession(transform.name);
	}

	private void Start()
	{
		if (EquipmentCrafting_Controller.instance.FetchItemsForProfession(Player_Skills_Script.ConvertStringToSkill(transform.name)) == null)
		{
			GetComponent<Image>().color = new Color(.2f, .2f, .2f);
		}
	}
}
