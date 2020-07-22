using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinWarrior : Goblin
{
	private Vector2 spawnLocation;
	private Transform modelRoot;

	private void Awake()
	{
		base.Awake();
		spawnLocation = transform.position;
		modelRoot = transform.Find("Model").transform.GetChild(0);
	}

	private void Update()
	{
		base.Update();
		CheckForBleed();

		//if (TargetEntity != null)
		//{
		//	Debug.DrawLine(transform.position, TargetEntity.transform.position, Color.red, .1f);
		//}

		if (!CheckIfNotStunned() || isFlaggedDead)
		{
			return;
		}

		if (TargetEntity == null && isAggressive)
		{
			//TargetEntity = player.GetComponent<AttackableEntity>();
			if (isInsane)
			{
				Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, AggroRange, LayerMask.GetMask("Villagers", "Minions", "Player", "Enemies"));
				TargetEntity = enemies[Random.Range(0, enemies.Length)].GetComponent<AttackableEntity>();
			}
			else
			{
				Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, AggroRange, LayerMask.GetMask("Villagers", "Minions", "Player"));
				if (enemies.Length > 0)
				{
					int ind = Random.Range(0, enemies.Length);
					if (Physics2D.Raycast(transform.position, (enemies[ind].transform.position - transform.position), 12f, LayerMask.GetMask("Walls", "Player", "Minions")).transform.tag != "Wall")
					{
						TargetEntity = enemies[ind].GetComponent<AttackableEntity>();
						currentPath = graphManager.Dijkstra(transform.position, TargetEntity.transform.position);
					}
				}
			}
		}

		if (TargetEntity != null && Vector2.Distance(transform.position, TargetEntity.transform.position) < 2.0f)
		{
			if (attackTimer == 0)
			{
				AttackTarget();
			}

			if (rgd2d.velocity != Vector2.zero)
			{
				rgd2d.velocity = Vector2.zero;
			}

			return;
		}
		else if (TargetEntity != null && Vector2.Distance(transform.position, TargetEntity.transform.position) > 2.0f && (Vector2.Distance(transform.position, TargetEntity.transform.position) < AggroRange) || isChasing)
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
				RaycastHit2D info = Physics2D.Raycast(transform.position, targetEntity.transform.position - transform.position, AggroRange, LayerMask.GetMask("Player", "Minions", "Walls"));
				if (info.transform != null && info.transform.tag != "Wall")
				{
					rgd2d.velocity = (TargetEntity.transform.position - transform.position).normalized * ChaseSpeed;
				}
				else
				{
					if (currentPath.Count == 0)
					{
						currentPath = graphManager.Dijkstra(transform.position, targetEntity.transform.position);
					}
					rgd2d.velocity = (new Vector3(currentPath[0].x, currentPath[0].y, 0) - transform.position).normalized * ChaseSpeed;
					if (Vector2.Distance(new Vector3(currentPath[0].x, currentPath[0].y, 0), transform.position) <= .5f)
					{
						currentPath.RemoveAt(0);
					}
				}
			}
		}
		else
		{
			//if (TargetEntity != null){
			//	TargetEntity = null;
			//}

			//UpdateWandering();
		}
		modelRoot.localRotation = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, rgd2d.velocity), 0);
	}
}
