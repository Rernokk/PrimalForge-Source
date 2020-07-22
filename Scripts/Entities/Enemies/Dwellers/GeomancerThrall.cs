using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeomancerThrall : EnemyObject
{
	[SerializeField]
	private Geomancer masterReference;

	#region Properties
	public Geomancer MasterReference
	{
		get
		{
			return masterReference;
		}

		set
		{
			masterReference = value;
		}
	}
	#endregion

	protected override void Start()
	{
		base.Start();
		if (MasterReference != null)
		{
			TargetEntity = MasterReference.TargetEntity;
		}
	}

	protected override void ActionOnDeath()
	{
		MasterReference.ThrallReference = null;
		base.ActionOnDeath();
	}

	protected override void Update()
	{
		base.Update();
		for (int i = 0; i < currentPath.Count - 1; i++)
		{
			Debug.DrawLine(new Vector3(currentPath[i].x, currentPath[i].y, 0), new Vector3(currentPath[i].x, currentPath[i + 1].y, 0), Color.red);
		}

		if (TargetEntity == null)
		{
			TargetEntity = MasterReference.TargetEntity;
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
		else if (TargetEntity != null && Vector2.Distance(transform.position, TargetEntity.transform.position) > 2.0f || isChasing)
		{
			if (!isChasing)
			{
				isChasing = true;
			}
			else if (TargetEntity != null && Vector2.Distance(transform.position, TargetEntity.transform.position) > FalloffRange)
			{
				isChasing = false;
				currentPath.Clear();
				currentPath = graphManager.Dijkstra(transform.position, MasterReference.transform.position);
				return;
			}
			if (TargetEntity != null)
			{
				if (Vector2.Distance(transform.position, targetEntity.transform.position) < 5f)
				{
					rgd2d.velocity = (targetEntity.transform.position - transform.position).normalized * CharacterSpeed;
				}
				else
				{
					if (currentPath.Count == 0 || Vector2.Distance(new Vector2(currentPath[currentPath.Count - 1].x, currentPath[currentPath.Count - 1].y), targetEntity.transform.position) > 5f)
					{
						currentPath = graphManager.Dijkstra(transform.position, targetEntity.transform.position);
					}

					if (currentPath.Count > 0)
					{
						rgd2d.velocity = (new Vector3(currentPath[0].x, currentPath[0].y, 0) - transform.position).normalized * CharacterSpeed;
					}

					if (Vector2.Distance(new Vector3(currentPath[0].x, currentPath[0].y, 0), transform.position) <= .5f)
					{
						currentPath.RemoveAt(0);
					}
				}
			}
		}
	}
}
