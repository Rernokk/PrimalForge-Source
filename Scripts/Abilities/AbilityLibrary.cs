using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEngine.Events;

//Provides Access to abilities and cooldowns.
public class AbilityLibrary : MonoBehaviour
{
  public static Dictionary<string, Ability> abilityDictionary;
	public static Dictionary<string, UnityEvent> AbilityEvents = new Dictionary<string, UnityEvent>();

  static Dictionary<string, WeaponType> pairIndex;
  public Dictionary<string, Ability> AbilityDictionary
  {
    get
    {
      if (abilityDictionary == null)
      {
        abilityDictionary = new Dictionary<string, Ability>();
      }
      return abilityDictionary;
    }
  }
  public static bool WeaponSupportsSkill(string skill, string weapon)
  {
    WeaponType retWeaponVal;
    if (pairIndex.TryGetValue(skill, out retWeaponVal))
    {
      return retWeaponVal.ToString() == weapon;
    }
    return false;
  }
  public static List<Ability> FetchSkillList(string weaponType){
    List<Ability> retList = new List<Ability>();
    foreach (string s in pairIndex.Keys){
      WeaponType res;
      if (pairIndex.TryGetValue(s, out res) && res.ToString() == weaponType){
        retList.Add(abilityDictionary[s]);
      }
    }
    return retList;
  }
  private void Awake()
  {
    if (pairIndex == null)
    {
      pairIndex = new Dictionary<string, WeaponType>
      {
        //Sword (Swordsmanship)
        { "Enrage", WeaponType.SWORD },
        { "Heart Strike", WeaponType.SWORD },
        { "Eviscerate", WeaponType.SWORD },
        { "Cleave", WeaponType.SWORD },
        { "Ghost Strike", WeaponType.SWORD },
        { "Lunging Swipe", WeaponType.SWORD },

        //Recurve Bow (Marksmanship)
        { "Evade", WeaponType.RECURVEBOW },
        { "Heartseeker", WeaponType.RECURVEBOW },
        { "Spray", WeaponType.RECURVEBOW },
        { "Snipe", WeaponType.RECURVEBOW },
        { "Arrow Storm", WeaponType.RECURVEBOW },
        { "Shadewalk", WeaponType.RECURVEBOW },

        //Censer (Lifeweaving)
        { "Mend", WeaponType.CENSER },
        { "Resonance", WeaponType.CENSER },
        { "Sustain", WeaponType.CENSER },
        { "Protective Light", WeaponType.CENSER },
        { "Resurgence", WeaponType.CENSER },
        { "Protect", WeaponType.CENSER },

        //Sac. Knife (Soulbinding)
        { "Ghoulish Glee", WeaponType.SACRIFICIALKNIFE },
        { "Command Undead", WeaponType.SACRIFICIALKNIFE },
        { "Flesh Construct", WeaponType.SACRIFICIALKNIFE },
        { "Dark Arcane Magic", WeaponType.SACRIFICIALKNIFE },
        { "Sinful Harvest", WeaponType.SACRIFICIALKNIFE },
        { "Devils Deal", WeaponType.SACRIFICIALKNIFE },

        //Vessel (Witchcraft)
        { "Soul Tether", WeaponType.VESSEL },
        { "Hysteria", WeaponType.VESSEL },
        { "Infest", WeaponType.VESSEL },
        { "Plague Doctor", WeaponType.VESSEL },
        { "Terrify", WeaponType.VESSEL },
        { "Hex Master", WeaponType.VESSEL },


        //Staff (Elementalism)
        { "Shockwave", WeaponType.STAFF },
        { "Wildfire", WeaponType.STAFF },
        { "Jolt", WeaponType.STAFF },
        { "Aftershock", WeaponType.STAFF },
        { "Flamewave", WeaponType.STAFF },
        { "Guiding Current", WeaponType.STAFF },

      };
    }
  }

  private void Start()
  {
    if (abilityDictionary == null)
    {
      abilityDictionary = new Dictionary<string, Ability>();
    }

		if (AbilityEvents == null)
		{
			AbilityEvents = new Dictionary<string, UnityEvent>();
		}

    foreach (Component t in GetComponents<Ability>())
    {
			if (!abilityDictionary.ContainsKey((t as Ability).abilityName))
			{
				AbilityDictionary.Add((t as Ability).abilityName, t as Ability);
				AbilityEvents.Add((t as Ability).abilityName, new UnityEvent());
			}
    }
    GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Equipment_Script>().PopulateWeaponSkills();
  }
}
