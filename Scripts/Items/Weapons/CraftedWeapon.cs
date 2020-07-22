using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class CraftedWeapon : CraftedEquipment
{
	public WeaponType tarType;

	public CraftedWeapon() : base()
	{
		itemType = CraftedItemType.WEAPON;
	}

	public CraftedWeapon(CraftedEquipment template) : base(template)
	{
		itemType = CraftedItemType.WEAPON;
		tarType = (template as CraftedWeapon).tarType;
	}

	public override Sprite Icon
	{
		get
		{
			if (icon == null)
			{
				icon = Resources.Load<Sprite>("ItemIcons/WeaponIcons/" + tarType.ToString());
			}
			return icon;
		}

		set
		{
			icon = value;
		}
	}
}