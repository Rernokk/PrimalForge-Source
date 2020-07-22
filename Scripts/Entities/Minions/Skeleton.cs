using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Minion {

  [SerializeField]
  GameObject EmpoweredProjectilePrefab;

  protected override void AttackTarget()
  {
    if (!empowered)
    {
      base.AttackTarget();
    } else
    {
      //print("Attacking!");
      //targetEntity.TakeDamage(AttackPower, ElementalResistances.PHYSICAL);
      GameObject obj = Instantiate(EmpoweredProjectilePrefab, transform.position + (targetEntity.transform.position - transform.position).normalized, Quaternion.identity);
      obj.GetComponent<Rigidbody2D>().AddForce((targetEntity.transform.position - transform.position).normalized * 8f, ForceMode2D.Impulse);
			obj.GetComponent<SkeletonProjectile>().damageVal = .25f * AttackPower;
      Destroy(obj, 2f);
      attackTimer = 1.0f / AttackSpeed;
      if (targetEntity.CurrentHealth <= 0)
      {
        targetEntity = null;
        inCombat = false;
      }

    }
  }

  protected override void Empower()
  {
    base.Empower();
    currentAttackRange = empoweredAttackRange;
    currentAttackSpeed = empoweredAttackSpeed;
		if (Player_Accessor_Script.EquipmentScript.ActivePerks.Contains(LegendaryPerk.Oranthuls_Grasp))
		{
			regenRate = -maxHealth * 2f;
		}
		else
		{
			regenRate = empoweredRegenRate;
		}
		transform.Find("EyeGlow_L").gameObject.SetActive(true);
		transform.Find("EyeGlow_R").gameObject.SetActive(true);
	}
  protected override void DeEmpower()
  {
    base.DeEmpower();
    currentAttackRange = baseAttackRange;
    currentAttackSpeed = baseAttackSpeed;
    regenRate = baseRegenRate;
		transform.Find("EyeGlow_L").gameObject.SetActive(false);
		transform.Find("EyeGlow_R").gameObject.SetActive(false);
  }

	protected override void Die()
	{
		base.Die();
		CommandUndead.activeUndead.Remove(this);
	}
}
