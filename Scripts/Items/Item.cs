using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
	WEAPON,
	HELMET, CHESTPLATE, PLATELEGS,
	HAT, ROBE, TROUSERS,
	COIF, CHESTGUARD, CHAPS,
	RESOURCE, TOOL, UNASSIGNED
}

[Serializable]
public class Item
{
	[SerializeField]
	protected string name;

	[NonSerialized]
	protected Sprite icon;

	protected ItemType myItemType;

	public string ItemName
	{
		get
		{
			if (name == null)
			{
				name = "";
			}
			return name;
		}

		set
		{
			name = value;
		}
	}
	public virtual Sprite Icon
	{
		get
		{
			if (icon == null)
			{
				if (name == "" || name == null)
				{
					icon = Resources.Load<Sprite>("ItemIcons/EmptySlot");
				}
				else
				{
					icon = Resources.Load<Sprite>("ItemIcons/ResourceIcons/" + name);
				}
			}
			return icon;
		}

		set
		{
			icon = value;
		}
	}

	public ItemType MyItemType
	{
		get
		{
			return myItemType;
		}

		set
		{
			myItemType = value;
		}
	}
}
