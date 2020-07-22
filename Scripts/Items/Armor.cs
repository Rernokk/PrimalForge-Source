using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmorType { CHESTPLATE, ROBE, CHESTGUARD, HELMET, HAT, COIF, PLATELEGS, TROUSERS, CHAPS, EMPTY }

[Serializable]
public class Armor : Item
{

	protected ArmorType type;
	protected int might;
	protected int intelligence;
	protected int dexterity;
	protected int durability;
	protected int durabilityMax;
	protected int levelReq;
	protected int itemLevel;
	protected int levelLimit;
	protected int bonusHealth;
	protected string skillRequired = "";
	public List<LegendaryPerk> perks = new List<LegendaryPerk>();

	#region Properties
	public int LevelRequirement
	{
		get
		{
			return levelReq;
		}
		set
		{
			dexterity = value;
		}
	}
	public string SkillRequired
	{
		get
		{
			return skillRequired;
		}

		set
		{
			skillRequired = value;
		}
	}
	public int Might
	{
		get
		{
			return might;
		}

		set
		{
			might = value;
		}
	}

	public int Dexterity
	{
		get
		{
			return dexterity;
		}

		set
		{
			dexterity = value;
		}
	}

	public int Intelligence
	{
		get
		{
			return intelligence;
		}

		set
		{
			intelligence = value;
		}
	}

	public int BonusHealth
	{
		get
		{
			return bonusHealth;
		}

		set
		{
			bonusHealth = value;
		}
	}

	public override Sprite Icon
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
					icon = Resources.Load<Sprite>("ItemIcons/ArmorIcons/" + myItemType.ToString());
				}
			}
			return icon;
		}

		set
		{
			icon = value;
		}
	}
	#endregion

	public void Equip()
	{

	}

	public void Unequip()
	{

	}

	public Vector3 FetchStats()
	{
		return new Vector3(dexterity, intelligence, might);
	}

	public Armor()
	{
		might = 1;
		dexterity = 1;
		intelligence = 1;
	}
	public void CopyStats(Armor original)
	{
		type = original.type;
		name = original.name;
		might = original.might;
		intelligence = original.intelligence;
		dexterity = original.dexterity;
		durability = original.durability;
		durabilityMax = original.durabilityMax;
		levelReq = original.levelReq;
		itemLevel = original.itemLevel;
		levelLimit = original.levelLimit;
	}

	public void CopyStats(CraftedArmor original)
	{
		name = original.ItemName;
		might = UnityEngine.Random.Range(original.mightMin, original.mightMax);
		intelligence = UnityEngine.Random.Range(original.intelligenceMin, original.intelligenceMax);
		dexterity = UnityEngine.Random.Range(original.dexterityMin, original.dexterityMax);
		levelReq = UnityEngine.Random.Range(original.minLevelReq, original.maxLevelReq);
		bonusHealth = UnityEngine.Random.Range(original.minBonusHealth, original.maxBonusHealth);
		perks = original.perks;
	}
}
