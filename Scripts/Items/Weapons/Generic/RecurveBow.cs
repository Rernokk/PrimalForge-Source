using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RecurveBow : Weapon {
  [Space]
  [Header("Recurve Bow Specific")]
  [SerializeField]
  private float atkSpd;

  public override void PrimaryAttack(Vector2 pos)
  {
    throw new NotImplementedException();
  }

  public override void SecondaryEffect(Vector2 pos)
  {
    throw new NotImplementedException();
  }

  public RecurveBow (AbilityLibrary scriptRef) : base()
  {
    Type = WeaponType.RECURVEBOW;
    lib = scriptRef;
    SkillSet = new Ability[4];
    ItemName = "Recurve Bow";
		skillRequired = "Marksmanship";
	}
}
