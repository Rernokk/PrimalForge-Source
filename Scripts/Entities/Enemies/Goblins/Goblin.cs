using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : EnemyObject
{
  [Header("Goblin Specific Details")]
  [SerializeField]
  protected float AggroRange = 5f;

  [SerializeField]
  protected float ChaseSpeed = 7f;

	protected SpriteRenderer sprRend;

	protected override void Start()
	{
		base.Start();
		sprRend = transform.GetComponentInChildren<SpriteRenderer>();
	}

	protected override void Update()
	{
		base.Update();
		if (rgd2d != null)
		{
			sprRend.flipX = (rgd2d.velocity.x >= 0 ? false : true);
		}
		if (targetEntity != null && targetEntity.transform.tag == "Player" && Objective_Interface.instance.QuestCompletionStates[QUESTNAME.SlayGoblins] == 0)
		{
			Objective_Interface.instance.AddNewObjective(new SlayGoblinsObjective());
		}
	}

	protected override void ActionOnDeath()
	{
		base.ActionOnDeath();
		Objective_Interface.instance.QuestObjectives["SlayGoblins"].Invoke();
	}
}
