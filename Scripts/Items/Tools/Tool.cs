using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType { HATCHET, PICKAXE, FISHING_ROD, SICKLE }

[Serializable]
public class Tool : Item
{

	[SerializeField]
	protected ToolType toolType;
	protected Skills requiredSkill;
	protected int durability;
	protected int durabilityMax;
	protected int levelReq;
	protected int itemLevel;

	protected int bonusMight;
	protected int bonusIntelligence;
	protected int bonusDexterity;
	protected int bonusArmor;

	#region Properties
	public int BonusArmor
	{
		get
		{
			return bonusArmor;
		}

		set
		{
			bonusArmor = value;
		}
	}
	public int BonusMight
	{
		get
		{
			return bonusMight;
		}

		set
		{
			bonusMight = value;
		}
	}
	public int BonusIntelligence
	{
		get
		{
			return bonusIntelligence;
		}

		set
		{
			bonusIntelligence = value;
		}
	}
	public int BonusDexterity
	{
		get
		{
			return bonusDexterity;
		}

		set
		{
			bonusDexterity = value;
		}
	}
	public ToolType ToolType
	{
		get
		{
			return toolType;
		}

		set
		{
			toolType = value;
		}
	}

	protected int Durability
	{
		get
		{
			return durability;
		}

		set
		{
			durability = value;
		}
	}

	protected int DurabilityMax
	{
		get
		{
			return durabilityMax;
		}

		set
		{
			durabilityMax = value;
		}
	}

	public int LevelReq
	{
		get
		{
			return levelReq;
		}

		set
		{
			levelReq = value;
		}
	}

	public int ItemLevel
	{
		get
		{
			return itemLevel;
		}

		set
		{
			itemLevel = value;
		}
	}

	public override Sprite Icon
	{
		get
		{
			if (icon == null)
			{
				icon = Resources.Load<Sprite>("ItemIcons/ToolIcons/" + toolType);
			}
			return icon;
		}

		set
		{
			icon = value;
		}
	}

	public Skills RequiredSkill
	{
		get
		{
			return requiredSkill;
		}

		set
		{
			requiredSkill = value;
		}
	}
	#endregion

	public Tool() : base()
	{
		ItemName = "Basic Tool";
		myItemType = ItemType.TOOL;
		BonusMight = 0;
		bonusArmor = 0;
		bonusDexterity = 0;
		BonusIntelligence = 0;
		levelReq = 1;
	}

	public void CopyStats(Tool refTool)
	{
		toolType = refTool.ToolType;
		name = refTool.ItemName;
		durability = refTool.Durability;
		durabilityMax = refTool.DurabilityMax;
		levelReq = refTool.LevelReq;
		itemLevel = refTool.ItemLevel;
	}
}
