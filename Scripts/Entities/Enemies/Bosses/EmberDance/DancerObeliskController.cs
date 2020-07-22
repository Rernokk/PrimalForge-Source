using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerObeliskController : MonoBehaviour
{
	private DancerController rootReference;
	private bool elementalActive = false;
	private Animator animCtrller;
	private Player_Details_Script playerDetsRef;

	[SerializeField]
	private float attackDuration, maxDuration = 6f, meleeRadius = 3f;

	[Header("Projectile Properties")]
	[SerializeField]
	private float projectileDelay = .1f;
	[SerializeField]
	private float projectileSpeed  = 3f;
	[SerializeField]
	private float projectileLoopCycle = 3f;

	[SerializeField]
	private int projectileOriginPoints = 4;

	[SerializeField]
	private GameObject obeliskProjectileObject;

	[SerializeField]
	private int direction = 1;

	private float projectileTick = 0f, projectileCycle;
	private float deltaTheta = 0f, currentTheta = 0f;

	#region Properties
	public DancerController DancerRootReference
	{
		get
		{
			return rootReference;
		}

		set
		{
			rootReference = value;
		}
	}

	public float AttackDuration
	{
		get
		{
			return attackDuration;
		}
		
		set
		{
			attackDuration = value;
		}
	}

	public bool ElementalActive
	{
		get
		{
			return elementalActive;
		}

		set
		{
			if (!elementalActive && value)
			{
				GetComponent<Animator>().Play("Activate");
			}
			else if (!value && elementalActive)
			{
				GetComponent<Animator>().Play("Deactivate");
			}
			elementalActive = value;
		}
	}
	#endregion

	public void ActivateAttack()
	{
		if (!elementalActive)
		{
			return;
		}

		rootReference.SetAttackTimer(maxDuration);
		attackDuration = maxDuration;
		direction = -direction;
		deltaTheta = (2 * Mathf.PI) * projectileLoopCycle / AttackDuration;
		currentTheta = 0f;
	}

	public void Awake()
	{
		animCtrller = GetComponent<Animator>();
	}

	private void Start()
	{
		projectileTick = 0f;
		playerDetsRef = Player_Accessor_Script.DetailsScript;
	}

	private void Update()
	{
		if (!elementalActive)
		{
			return;
		}

		if (attackDuration > 0f)
		{
			if (projectileTick <= 0f)
			{
				for (int i = 0; i < projectileOriginPoints; i++)
				{
					float amnt = (2 * Mathf.PI * (i / (float)projectileOriginPoints)) + currentTheta * direction;
					GameObject projectileReference = Instantiate(obeliskProjectileObject, transform.position + new Vector3(Mathf.Cos(amnt), Mathf.Sin(amnt), 0) * .35f, Quaternion.identity);
					projectileReference.GetComponent<Rigidbody2D>().velocity = (projectileReference.transform.position - transform.position).normalized * projectileSpeed;
					Destroy(projectileReference, 4f);
				}

				if (rootReference.IsEnraged)
				{
					for (int i = 0; i < projectileOriginPoints; i++)
					{
						float amnt = (2 * Mathf.PI * (i / (float)projectileOriginPoints)) + currentTheta * -direction;
						GameObject projectileReference = Instantiate(obeliskProjectileObject, transform.position + new Vector3(Mathf.Cos(amnt), Mathf.Sin(amnt), 0) * .35f, Quaternion.identity);
						projectileReference.GetComponent<Rigidbody2D>().velocity = (projectileReference.transform.position - transform.position).normalized * projectileSpeed;
						Destroy(projectileReference, 4f);
					}
				}
				projectileTick = projectileDelay;
			}
			else if (projectileTick > 0)
			{
				projectileTick -= Time.deltaTime;
			}
			currentTheta += deltaTheta * Time.deltaTime;
			attackDuration -= Time.deltaTime;
		}

		if (Vector2.Distance(transform.position, playerDetsRef.transform.position) < meleeRadius)
		{
			playerDetsRef.TakeDamage(10f, ElementalResistances.TRUE);
			playerDetsRef.Knockback((playerDetsRef.transform.position - transform.position).normalized, 12f);
		}
		for (int i = 0; i < projectileOriginPoints * 2; i++)
		{
			float amnt = (2 * Mathf.PI * (i / (float)(2 * projectileOriginPoints))) + currentTheta * direction;
			Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(amnt), Mathf.Sin(amnt), 0) * meleeRadius, Color.red);
		}
	}
}
