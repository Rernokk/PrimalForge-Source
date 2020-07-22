using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SacrificialKnife : Weapon {

  public override void PrimaryAttack(Vector2 pos)
  {
    throw new System.NotImplementedException();
  }

  public override void SecondaryEffect(Vector2 pos)
  {
    throw new System.NotImplementedException();
  }

  public SacrificialKnife(AbilityLibrary scriptRef) : base()
  {
    Type = WeaponType.SACRIFICIALKNIFE;
    lib = scriptRef;
    SkillSet = new Ability[4];
    ItemName = "Sacrificial Knife";
		skillRequired = "Necromancy";
	}
}
