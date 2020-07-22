using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeReference : MonoBehaviour {
	[HideInInspector]
	public CraftableItem itemRef;

	[HideInInspector]
	public RecipeCrafter recipeCrafter;

	public void SelectItem(){
		if (itemRef.MyCraftedItemType == CraftedItemType.WEAPON)
		{
			recipeCrafter.LoadSelectedItem(itemRef);
		} else {
			recipeCrafter.LoadSelectedItem(itemRef);
		}
	}

	public void DeleteItem()
	{
		recipeCrafter.DeleteSelectedItem();
	}
}
