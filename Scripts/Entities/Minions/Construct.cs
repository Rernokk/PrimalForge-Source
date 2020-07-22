using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construct : Minion
{
  [SerializeField]
  float increasedResistanceMultiplier;

  protected override void AttackTarget()
  {
    base.AttackTarget();
  }

  protected override void Empower()
  {
    base.Empower();
    currentAttackRange = empoweredAttackRange;
    currentAttackSpeed = empoweredAttackSpeed;
    regenRate = empoweredRegenRate;
    resistanceMultiplier += increasedResistanceMultiplier;
  }
  protected override void DeEmpower()
  {
    base.DeEmpower();
    currentAttackRange = baseAttackRange;
    currentAttackSpeed = baseAttackSpeed;
    regenRate = baseRegenRate;
    resistanceMultiplier -= increasedResistanceMultiplier;
  }
}
