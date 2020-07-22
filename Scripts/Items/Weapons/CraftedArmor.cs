using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CraftedArmor : CraftedEquipment
{
	public ArmorType tarType;

	public CraftedArmor() : base()
	{
		itemType = CraftedItemType.ARMOR;
	}

	public CraftedArmor(CraftedEquipment template) : base(template)
	{
		itemType = CraftedItemType.ARMOR;
		tarType = (template as CraftedArmor).tarType;
	}

	public override Sprite Icon
	{
		get
		{
			if (icon == null)
			{
				icon = Resources.Load<Sprite>("ItemIcons/ArmorIcons/" + tarType.ToString());
			}
			return icon;
		}

		set
		{
			icon = value;
		}
	}
}
