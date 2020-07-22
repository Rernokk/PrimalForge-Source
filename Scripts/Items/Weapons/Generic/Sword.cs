using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sword : Weapon {
  public override void PrimaryAttack(Vector2 pos)
  {
		Collider2D[] hits = Physics2D.OverlapCircleAll(Player_Accessor_Script.DetailsScript.transform.position, 1.5f);
		for (int i = 0; i < hits.Length; i++){
			if (Vector3.Dot((hits[i].transform.position - Player_Accessor_Script.DetailsScript.transform.position).normalized, Camera_Controller_Script.CalculateVectorFromPlayerToMouse()) > .7) {
				EnemyObject obj = hits[i].GetComponent<EnemyObject>();
				if (obj != null){
					obj.TakeDamage(3f, ElementalResistances.PHYSICAL);
				}
			}
		}
  }

  public override void SecondaryEffect(Vector2 pos)
  {
		throw new NotImplementedException();
  }

  public Sword(AbilityLibrary scriptRef): base()
  {
    Type = WeaponType.SWORD;
    lib = scriptRef;
    SkillSet = new Ability[4];
    ItemName = "Sword";
		skillRequired = "Swordsmanship";
	}
}
