using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CraftedItemType { WEAPON, ARMOR, CONSUMABLE, REAGENT, OTHER };

[System.Serializable]
public class CraftableItem : Item
{
	[SerializeField]
	protected Skills requiredProf = Skills.Alchemy;
	protected int requiredProfLevel = 0;
	protected int rewardedExp = 0;
	protected CraftedItemType itemType = CraftedItemType.OTHER;
	protected List<Reagent> requiredReagents = new List<Reagent>();
	protected string spritePath = "";

	public Skills RequiredProfession
	{
		get
		{
			return requiredProf;
		}

		set
		{
			requiredProf = value;
		}
	}

	public CraftedItemType MyCraftedItemType
	{
		get
		{
			return itemType;
		}

		set
		{
			itemType = value;
		}
	}

	public int RequiredProfLevel
	{
		get
		{
			return requiredProfLevel;
		}

		set
		{
			requiredProfLevel = value;
		}
	}

	public List<Reagent> RequiredReagents
	{
		get
		{
			return requiredReagents;
		}

		set
		{
			requiredReagents = value;
		}
	}

	public int RewardedExp
	{
		get
		{
			return rewardedExp;
		}

		set
		{
			rewardedExp = value;
		}
	}

	public CraftableItem()
	{
		itemType = CraftedItemType.OTHER;
	}

	public string SpritePath
	{
		get
		{
			return spritePath;
		}

		set
		{
			spritePath = value;
		}
	}
}
