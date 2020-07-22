using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CraftedEquipment : CraftableItem
{

	public string baseName;

	[FoldoutGroup("Attributes")]
	[Header("Dexterity")]
	public int dexterityMin;

	[FoldoutGroup("Attributes")]
	public int dexterityMax;

	[FoldoutGroup("Attributes")]
	[Header("Might")]
	public int mightMin;
	[FoldoutGroup("Attributes")]
	public int mightMax;

	[FoldoutGroup("Attributes")]
	[Header("Intelligence")]
	public int intelligenceMin;
	[FoldoutGroup("Attributes")]
	public int intelligenceMax;

	[FoldoutGroup("Attributes")]
	[Header("Level")]
	public int minLevelReq;
	[FoldoutGroup("Attributes")]
	public int maxLevelReq;
	public int minBonusHealth, maxBonusHealth;
	public int bonusHealth;
	private int dexterity = 0;
	private int might = 0;
	private int intelligence = 0;
	private int levelReq = 0;

	public List<LegendaryPerk> perks = new List<LegendaryPerk>();

	public CraftedEquipment() : base()
	{

	}

	public CraftedEquipment(CraftedEquipment template) : base()
	{
		minLevelReq = template.minLevelReq;
		maxLevelReq = template.maxLevelReq;
		dexterityMin = template.dexterityMin;
		dexterityMax = template.dexterityMax;
		intelligenceMin = template.intelligenceMin;
		intelligenceMax = template.intelligenceMax;
		mightMin = template.mightMin;
		mightMax = template.mightMax;
		baseName = template.baseName;
		//tarType = template.tarType;

		ItemName = template.baseName;
		Dexterity = UnityEngine.Random.Range(template.dexterityMin, template.dexterityMax);
		Might = UnityEngine.Random.Range(template.dexterityMin, template.dexterityMax);
		Intelligence = UnityEngine.Random.Range(template.dexterityMin, dexterityMax);
		LevelReq = UnityEngine.Random.Range(template.minLevelReq, template.maxLevelReq);
		bonusHealth = UnityEngine.Random.Range(template.minBonusHealth, template.maxBonusHealth);

		Intelligence = Mathf.Clamp(Intelligence, 0, 999999);
		Might = Mathf.Clamp(Might, 0, 999999);
		Dexterity = Mathf.Clamp(Dexterity, 0, 999999);
		Icon = null;
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
}