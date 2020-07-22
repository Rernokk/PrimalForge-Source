using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableItemType { POTION, INFUSION, GEMSTONE, FOOD, ENCHANTMENT, ETCHING, NONE };
public class CraftedConsumable : CraftableItem
{
	private ConsumableItemType consumableItemType;

	#region Properties
	public ConsumableItemType MyConsumableItemType
	{
		get
		{
			return consumableItemType;
		}

		set
		{
			consumableItemType = value;
		}
	}
	#endregion

	public CraftedConsumable()
	{
		itemType = CraftedItemType.CONSUMABLE;
	}
}
