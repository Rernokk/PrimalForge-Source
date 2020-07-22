using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_WeaponType_Controller : MonoBehaviour {
	Dropdown list;

	[SerializeField]
	bool isEquipmentType = true;
	public bool displayingWeapons;
	public bool displayingArmor;
	public bool displayingReagents;
	// Use this for initialization
	void Start () {
		list = GetComponent<Dropdown>();
		list.ClearOptions();
		if (isEquipmentType)
		{
			PopulateItemTypeOptions();
		}
		else
		{
			list.ClearOptions();
			List<string> professionList = new List<string>();
			professionList.Add("Blacksmithing");
			professionList.Add("Alchemy");
			professionList.Add("Infusion");
			professionList.Add("Divination");
			professionList.Add("Bowcrafting");
			professionList.Add("Gem Mastery");
			professionList.Add("Cooking");
			professionList.Add("Carpentry");
			professionList.Add("Etching");
			professionList.Add("Weaving");
			professionList.Add("Leatherworking");
			professionList.Add("Lumbering");
			professionList.Add("Mining");
			professionList.Add("Gathering");
			professionList.Add("Fishing");
			professionList.Sort();
			professionList.Insert(0, "N/A");
			list.AddOptions(professionList);
		}
	}

	public void PopulateItemTypeOptions()
	{
		List<string> craftingItemType = new List<string>();
		if (displayingWeapons)
		{
			for (int i = 0; i < Enum.GetNames(typeof(WeaponType)).Length; i++)
			{
				string temp = ((WeaponType)i).ToString();
				temp = temp[0] + temp.ToLower().Substring(1);
				craftingItemType.Add(temp);
			}
		}
		else if (displayingArmor)
		{
			for (int i = 0; i < Enum.GetNames(typeof(ArmorType)).Length; i++)
			{
				if (((ArmorType)i) != ArmorType.EMPTY)
				{
					string temp = ((ArmorType)i).ToString();
					temp = temp[0] + temp.ToLower().Substring(1);
					craftingItemType.Add(temp);
				}
			}
		} else if (displayingReagents)
		{

		}
		craftingItemType.Remove("Empty");
		craftingItemType.Sort();
		craftingItemType.Insert(0, "Empty");
		list.ClearOptions();
		list.AddOptions(craftingItemType);
	}
}
