using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dagger : Weapon
{
  public override void PrimaryAttack(Vector2 pos)
  {
    //print("Stabs with dagger.");
  }

  public override void SecondaryEffect(Vector2 pos)
  {
    //throw new System.NotImplementedException();
  }

  public Dagger (AbilityLibrary scriptRef) : base()
  {
    //Type = WeaponType.DAGGER;
    lib = scriptRef;
    ItemName = "Dagger";
    SkillSet = new Ability[4];
    //for (int i = 0; i < 4; i++)
    //{
    //  SkillSet[i] = lib.AbilityDictionary["Slash"];
    //}

    ////Denoting abilities available from delegates. Uses player-readable name of ability as key in the dictionary.
    //SelectedAbilities[0] = lib.AbilityDictionary[SkillSet[0].abilityName].UseAbility;
    //SelectedAbilities[1] = lib.AbilityDictionary[SkillSet[1].abilityName].UseAbility;
    //SelectedAbilities[2] = lib.AbilityDictionary[SkillSet[2].abilityName].UseAbility;
    //SelectedAbilities[3] = lib.AbilityDictionary[SkillSet[3].abilityName].UseAbility;
  }
}
