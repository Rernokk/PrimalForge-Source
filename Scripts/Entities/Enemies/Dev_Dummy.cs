using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dev_Dummy : EnemyObject
{
	protected override void ActionOnDeath()
	{
		currentHealth = MaxHealth;
		isFlaggedDead = false;
	}
}
