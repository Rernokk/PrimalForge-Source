using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftableReagent : CraftableItem
{
	public CraftableReagent() : base()
	{
		itemType = CraftedItemType.REAGENT;
		MyCraftedItemType = CraftedItemType.REAGENT;
	}

	public CraftableReagent(CraftableReagent template) : base()
	{
		itemType = CraftedItemType.REAGENT;
		MyCraftedItemType = CraftedItemType.REAGENT;
	}

	public override Sprite Icon
	{
		get
		{
			if (icon == null)
			{
				icon = Resources.Load<Sprite>("ItemIcons/ResourceIcons/" + spritePath);
			}
			if (icon == null)
			{
				icon = Resources.Load<Sprite>("ItemIcons/EmptySlot");
			}
			return icon;
		}

		set
		{
			icon = value;
		}
	}

	public override string ToString()
	{
		return ItemName;
	}
}
