using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { EMPTY, SWORD, RECURVEBOW, CENSER, VESSEL, SACRIFICIALKNIFE, STAFF, };

//Parent class for weapon data. Stores information about weapons.
[Serializable]
public abstract class Weapon : Item
{
  protected WeaponType type;
  protected int might;
  protected int intelligence;
	protected int dexterity;
	protected int durability;
  protected int durabilityMax;
  protected int levelReq;
  protected int itemLevel;
  protected int levelLimit;
	protected int bonusHealth;
	protected string[] skillStringName;
	protected string skillRequired = "";

  [NonSerialized]
  public Ability[] SkillSet;

  [NonSerialized]
  public AbilityLibrary lib;

  public delegate bool AbilitiesList();

  [NonSerialized]
  List<Ability> weaponSkillSet;

  [NonSerialized]
  AbilitiesList[] selectedAbilitySet;

  [NonSerialized]
  Ability[] selectedAbilities;

	public List<LegendaryPerk> perks = new List<LegendaryPerk>();

  public Weapon()
  {
    if (selectedAbilitySet == null)
    {
      selectedAbilitySet = new AbilitiesList[4];
    }

    if (selectedAbilities == null)
    {
      selectedAbilities = new Ability[4];
    }

    if (weaponSkillSet == null)
    {
      weaponSkillSet = new List<Ability>();
    }
		skillStringName = new string[4];// { "Slash", "Slash", "Slash", "Slash" };
  }

  public void CopyStats(Weapon original)
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
    weaponSkillSet = original.weaponSkillSet;
    selectedAbilitySet = original.selectedAbilitySet;
		bonusHealth = original.bonusHealth;
    SkillSet = original.SkillSet;
    lib = original.lib;
  }

	public void CopyStats(CraftedEquipment original)
	{
		Debug.Log("Creating: " + original.ItemName);
		name = original.ItemName;
		might = UnityEngine.Random.Range(original.mightMin, original.mightMax);
		intelligence = UnityEngine.Random.Range(original.intelligenceMin, original.intelligenceMax);
		dexterity = UnityEngine.Random.Range(original.dexterityMin, original.dexterityMax);
		levelReq = UnityEngine.Random.Range(original.minLevelReq, original.maxLevelReq);
		bonusHealth = UnityEngine.Random.Range(original.minBonusHealth, original.maxBonusHealth);
		perks = original.perks;
	}

  public void SwapSelectedSkill(int slot, string ability)
  {
    SkillSet[slot] = lib.AbilityDictionary[ability];
    SelectedAbilities[slot] = SkillSet[slot].UseAbility;
  }
  public void PopulateSkills()
  {
    if (SkillSet == null)
    {
      SkillSet = new Ability[4];
    }

    if (SelectedAbilities == null)
    {
      SelectedAbilities = new AbilitiesList[4];
    }

    if (skillStringName == null)
    {
      skillStringName = new string[4];
    }

    if (lib == null)
    {

    }

    for (int i = 0; i < 4; i++)
    {
      if (SkillSet[i] == null && skillStringName[i] != null)
      {
				if (lib.AbilityDictionary.ContainsKey(skillStringName[i]))
				{
					SkillSet[i] = lib.AbilityDictionary[skillStringName[i]];
				}
      }
      else if (skillStringName[i] == null)
      {
        skillStringName[i] = null;
				//SkillSet[i] = lib.AbilityDictionary[skillStringName[i]];
				SkillSet[i] = null;
      }
    }

    //Denoting abilities available from delegates. Uses player-readable name of ability as key in the dictionary.

		for (int i = 0; i < 4; i++){
			if (SkillSet[i] != null)
			{
				SelectedAbilities[i] = lib.AbilityDictionary[SkillSet[i].abilityName].UseAbility;
			} else {
				SelectedAbilities[i] = null;
			}
		}
    
		/*SelectedAbilities[0] = lib.AbilityDictionary[SkillSet[0].abilityName].UseAbility;
    SelectedAbilities[1] = lib.AbilityDictionary[SkillSet[1].abilityName].UseAbility;
    SelectedAbilities[2] = lib.AbilityDictionary[SkillSet[2].abilityName].UseAbility;
    SelectedAbilities[3] = lib.AbilityDictionary[SkillSet[3].abilityName].UseAbility;*/
  }
  public abstract void PrimaryAttack(Vector2 pos);
  public abstract void SecondaryEffect(Vector2 pos);
  public void TriggerGCDs()
  {
    if (SkillSet != null)
    {
      foreach (Ability abil in SkillSet)
      {
				if (abil != null)
					abil.TriggerGCD();
      }
    }
  }

  public void SetSkill(int slot, Ability skill){
    if (AbilityLibrary.WeaponSupportsSkill(skill.abilityName, type.ToString())){
      skillStringName[slot] = skill.abilityName;
      SkillSet[slot] = skill;
      SelectedAbilities[slot] = lib.AbilityDictionary[SkillSet[slot].abilityName].UseAbility;
    }
  }

  #region Properties
  public WeaponType Type
  {
    get
    {
      return type;
    }

    set
    {
      type = value;
    }
  }
  public string Name
  {
    get
    {
      return name;
    }

    set
    {
      name = value;
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
  public int Durability
  {
    get
    {
      return durability;
    }
    set
    {
      durability = value;
      if (durability > durabilityMax)
      {
        durability = durabilityMax;
      }
    }
  }
  public int DurabilityMax
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
  public int ItemLevel
  {
    get
    {
      return itemLevel;
    }
    set
    {
      dexterity = value;
    }
  }
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
  public int LevelLimit
  {
    get
    {
      return levelLimit;
    }
    set
    {
      dexterity = value;
    }
  }
  public List<Ability> WeaponSkillSet
  {
    get
    {
      if (weaponSkillSet == null)
      {
        weaponSkillSet = new List<Ability>();
        //AllocateAbilities();
      }
      return weaponSkillSet;
    }

    set
    {
      weaponSkillSet = value;
    }
  }
  public AbilitiesList[] SelectedAbilities
  {
    get
    {
      if (selectedAbilitySet == null)
      {
        //AllocateAbilities();
      }
      return selectedAbilitySet;
    }

    set
    {
      selectedAbilitySet = value;
    }
  }
  public Ability[] SelectedAbilityRef
  {
    get
    {
      return selectedAbilities;
    }

    set
    {
      selectedAbilities = value;
    }
  }
  public string[] SkillStrings
  {
    get
    {
      return skillStringName;
    }

    set
    {
      skillStringName = value;
    }
  }
	public string SkillRequired {
		get {
			return skillRequired;
		}

		set {
			skillRequired = value;
		}
	}

	public override Sprite Icon
	{
		get
		{
			if (icon == null)
			{
				icon = Resources.Load<Sprite>("ItemIcons/WeaponIcons/" + Type.ToString());
			}
			return icon;
		}

		set
		{
			icon = value;
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
	#endregion
}
