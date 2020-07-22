using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinMage : Goblin
{
	private Vector2 spawnLocation;
	private float attackRange = 7f;

	[SerializeField]
	private GameObject flameErruptionObject;

	[SerializeField]
	private float radius = 1f, duration = 3f;

	private bool empowered = false;

	public bool Empowered
	{
		get
		{
			return empowered;
		}

		set
		{
			empowered = value;
			if (empowered)
			{
				transform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.green;
			}
			else
			{
				transform.Find("Sprite").GetComponent<SpriteRenderer>().color = Color.red;
			}
		}
	}

	private void Awake()
	{
		base.Awake();
		spawnLocation = transform.position;
		shieldAmount = 0;
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

		if (Vector2.Distance(transform.position, player.transform.position) > ActiveRange)
		{
			return;
		}

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
					if (Physics2D.Raycast(transform.position, (sorted[0].transform.position - transform.position), 12f, LayerMask.GetMask("Walls", "Player", "Minions")).transform.tag != "Wall")
					{
						TargetEntity = sorted[0].GetComponent<AttackableEntity>();
					}
				}
			}
		}

		//Target in range, Attack
		if (TargetEntity != null && Vector2.Distance(transform.position, TargetEntity.transform.position) < attackRange)
		{
			if (attackTimer == 0)
			{
				//Change attack behaviour
				GameObject spellCast = Instantiate(flameErruptionObject, TargetEntity.transform.position, Quaternion.identity);
				GoblinMageMineSpell spellCastRef = spellCast.GetComponent<GoblinMageMineSpell>();
				spellCastRef.DamageValue = AttackPower;
				spellCastRef.DamageType = ElementalResistances.FIRE;
				spellCastRef.Countdown = 3f;
				attackTimer = 1f / AttackSpeed;
				if (empowered)
				{
					for (int i = 0; i < 4; i++)
					{
						spellCast = Instantiate(flameErruptionObject, TargetEntity.transform.position + (Vector3)Random.insideUnitCircle.normalized * Random.Range(-4f, 4f), Quaternion.identity);
						spellCastRef = spellCast.GetComponent<GoblinMageMineSpell>();
						spellCastRef.DamageValue = AttackPower * .5f;
						spellCastRef.DamageType = ElementalResistances.FIRE;
						spellCastRef.Countdown = Random.Range(1f, 3f);
						spellCastRef.SpellRadius = .5f;
					}
				}
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
			if (TargetEntity != null)
			{
				TargetEntity = null;
			}

			UpdateWandering();
		}
	}
}
