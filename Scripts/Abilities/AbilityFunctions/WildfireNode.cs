using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildfireNode : MonoBehaviour
{
	[SerializeField]
	private float explosionDelay = 1f;
	private float explosionRadius = 3f;
	private float timer = 0;

	public float damageVal;
	public ElementalResistances damageType;

	// Use this for initialization
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > explosionDelay)
		{
			TriggerExplosion();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		TriggerExplosion();
	}

	private void TriggerExplosion()
	{
		Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemies"));
		foreach (Collider2D coll in collisions)
		{
			if (coll.transform.root.GetComponent<EnemyObject>() != null)
			{
				WildfireDot refDot = coll.gameObject.AddComponent<WildfireDot>();
				refDot.duration = 3f;
				refDot.damageValue = damageVal;
				refDot.burnCount = 0;
			}
		}
		if (collisions.Length > 0)
		{
			Ability.DebugDrawRadius(transform.position, explosionRadius, Color.red, 1f);
			Destroy(gameObject);
		}
	}
}
