using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TwinDaggers : Weapon {
  [Space]
  [Header("Twin Dagger Specific")]
  [SerializeField]
  private float strikeRange = 1f;

  public override void PrimaryAttack(Vector2 pos)
  {
    //print("Sticks with daggers.");
    //RaycastHit2D[] info = Physics2D.RaycastAll(transform.position, (pos - (Vector2)transform.position).normalized, strikeRange);
    //Debug.DrawLine(transform.position, pos, Color.red, 1f);
    //foreach (RaycastHit2D inf in info){
    //  if (inf.transform.tag == "Enemy"){
    //    inf.transform.GetComponent<EnemyObject>().TakeDamage(1, ElementalResistances.PHYSICAL);
    //  }
    //}
  }

  public override void SecondaryEffect(Vector2 pos)
  {
    throw new System.NotImplementedException();
  }

  public TwinDaggers(AbilityLibrary scriptRef) : base()
  {
    //Type = WeaponType.TWINDAGGERS;
    lib = scriptRef;
    SkillSet = new Ability[4];
    ItemName = "Twin Daggers";
    //for (int i = 0; i < 4; i++)
    //{
    //  SkillSet[i] = lib.AbilityDictionary["Slash"];
    //}

    //SkillSet[0] = lib.AbilityDictionary["Ghost Strike"];
    //SkillSet[1] = lib.AbilityDictionary["Heart Strike"];
    //SkillSet[2] = lib.AbilityDictionary["Shadewalk"];
    //SkillSet[3] = lib.AbilityDictionary["Lunging Swipe"];

    ////Denoting abilities available from delegates. Uses player-readable name of ability as key in the dictionary.
    //SelectedAbilities[0] = lib.AbilityDictionary[SkillSet[0].abilityName].UseAbility;
    //SelectedAbilities[1] = lib.AbilityDictionary[SkillSet[1].abilityName].UseAbility;
    //SelectedAbilities[2] = lib.AbilityDictionary[SkillSet[2].abilityName].UseAbility;
    //SelectedAbilities[3] = lib.AbilityDictionary[SkillSet[3].abilityName].UseAbility;
  }
}
