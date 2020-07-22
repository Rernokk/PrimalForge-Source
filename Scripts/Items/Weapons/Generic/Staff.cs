using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Staff : Weapon {
  public override void PrimaryAttack(Vector2 pos)
  {
    throw new System.NotImplementedException();
  }

  public override void SecondaryEffect(Vector2 pos)
  {
    throw new System.NotImplementedException();
  }

  public Staff(AbilityLibrary scriptRef) : base()
  {
    Type = WeaponType.STAFF;
    lib = scriptRef;
    SkillSet = new Ability[4];
    ItemName = "Staff";
		skillRequired = "Elementalism";
	}
}
