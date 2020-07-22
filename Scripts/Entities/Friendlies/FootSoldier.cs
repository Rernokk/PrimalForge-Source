using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSoldier : AttackableEntity {
	EnemyObject target;
	List<Node> currentPath = new List<Node>();
	float attackTimer = 0f;

	void Update()
	{
		base.Update();

		if (target == null)
		{
			Collider2D[] area = Physics2D.OverlapCircleAll(transform.position, 15f, LayerMask.GetMask("Enemies"));
			if (area.Length > 0)
			{
				target = area[0].GetComponent<EnemyObject>();
				currentPath = AIManager.instance.Dijkstra(transform.position, target.transform.position);
			}
		} else
		{
			if (Vector2.Distance(transform.position, target.transform.position) < .25f)
			{
				if (attackTimer <= 0)
				{
					target.TakeDamage(1f, ElementalResistances.PHYSICAL);
					attackTimer = 1f;
				}
				else
				{
					attackTimer -= Time.deltaTime;
				}
			}
			else if (currentPath.Count > 0)
			{
				transform.position += (new Vector3(currentPath[0].x, currentPath[0].y) - transform.position).normalized * Time.deltaTime * 3f;
				if (Vector3.Distance(transform.position, new Vector3(currentPath[0].x, currentPath[0].y)) < .25f)
				{
					currentPath.RemoveAt(0);
				}
			}
		}
	}
}
