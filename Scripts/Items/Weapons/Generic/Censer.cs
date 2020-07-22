using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Censer : Weapon
{
  public override void PrimaryAttack(Vector2 pos)
  {
    throw new System.NotImplementedException();
  }

  public override void SecondaryEffect(Vector2 pos)
  {
    throw new System.NotImplementedException();
  }

  public Censer(AbilityLibrary scriptRef) : base()
  {
    Type = WeaponType.CENSER;
    lib = scriptRef;
    SkillSet = new Ability[4];
    ItemName = "Censer";
		skillRequired = "Lifeweaving";
	}
}
