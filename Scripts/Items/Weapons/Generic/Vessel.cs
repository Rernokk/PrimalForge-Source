using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vessel : Weapon
{
  public override void PrimaryAttack(Vector2 pos)
  {
    throw new System.NotImplementedException();
  }

  public override void SecondaryEffect(Vector2 pos)
  {
    throw new System.NotImplementedException();
  }

  public Vessel(AbilityLibrary scriptRef) : base()
  {
    Type = WeaponType.VESSEL;
    lib = scriptRef;
    SkillSet = new Ability[4];
    ItemName = "Vessel";
		skillRequired = "Witchcraft";
  }
}
