using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBomber : Goblin {
	private Vector2 spawnLocation;
	private float attackRange = 2f;

	private void Awake()
	{
		base.Awake();
		spawnLocation = transform.position;
	}

	private void Update()
	{
		base.Update();
		CheckForBleed();
		//if (TargetEntity != null)
		//{
		//	Debug.DrawLine(transform.position, TargetEntity.transform.position, Color.red, .1f);
		//}

		if (!CheckIfNotStunned())
			return;
		if (Vector2.Distance(transform.position, player.transform.position) > ActiveRange)
			return;

		if (TargetEntity == null && isAggressive)
		{
			//TargetEntity = player.GetComponent<AttackableEntity>();
			if (isInsane)
			{
				Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, AggroRange, 2816);
				TargetEntity = enemies[Random.Range(0, enemies.Length)].GetComponent<AttackableEntity>();
			}
			else
			{
				Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, AggroRange, 2560);
				if (enemies.Length > 0)
				{
					List<Collider2D> sorted = new List<Collider2D>();
					for (int i = 0; i < enemies.Length; i++)
					{
						sorted.Add(enemies[i]);
					}
					sorted.Sort((q1, q2) => -q1.gameObject.layer.CompareTo(q2.gameObject.layer));
					TargetEntity = sorted[0].GetComponent<AttackableEntity>();
				}
			}
		}

		//Target in range, Attack
		if (TargetEntity != null && Vector2.Distance(transform.position, TargetEntity.transform.position) < attackRange)
		{
			if (attackTimer == 0)
			{
				//Change attack behaviour
				print("Bomber Attacking!");
			}

			if (rgd2d.velocity != Vector2.zero)
			{
				rgd2d.velocity = Vector2.zero;
			}

			return;
		}
		else if (TargetEntity != null && Vector2.Distance(transform.position, TargetEntity.transform.position) > attackRange && (Vector2.Distance(transform.position, TargetEntity.transform.position) < AggroRange) || isChasing)
		{
			if (!isChasing)
			{
				isChasing = true;
			}
			else if (TargetEntity != null && Vector2.Distance(transform.position, TargetEntity.transform.position) > FalloffRange)
			{
				isChasing = false;

				//Wander back towards spawn area
				currentPath.Clear();
				currentPath = graphManager.Dijkstra(transform.position, spawnLocation);
				return;
			}
			if (TargetEntity != null)
			{
				rgd2d.velocity = (TargetEntity.transform.position - transform.position).normalized * ChaseSpeed;
			}
		}
		else
		{
			if (TargetEntity != null)
			{
				TargetEntity = null;
			}

			UpdateWandering();
		}
	}
}
