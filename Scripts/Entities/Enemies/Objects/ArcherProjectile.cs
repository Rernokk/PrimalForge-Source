using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArcherProjectile : MonoBehaviour {

	[SerializeField]
	bool EnemyProjectile = true;

	[SerializeField]
	ElementalResistances damageType = ElementalResistances.PHYSICAL;

	public float damageValue = 1;
	SpriteRenderer mySprite;

	void Start()
	{
		GetComponent<Rigidbody2D>().gravityScale = 0;
		transform.right = GetComponent<Rigidbody2D>().velocity.normalized;
		Destroy(gameObject, 4f);
		mySprite = GetComponentInChildren<SpriteRenderer>();
	}

	void Update()
	{
		mySprite.sortingOrder = -(int)(transform.position.y * 10);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (EnemyProjectile && (other.transform.tag == "Player" || other.transform.tag == "Minion" || other.transform.tag == "Wall"))
		{
			if (other.transform.tag != "Wall")
			{
				other.GetComponent<AttackableEntity>().TakeDamage(damageValue, damageType);
			}
			Destroy(gameObject);
		} else if (!EnemyProjectile)
		{
			AttackableEntity otherEntity = other.GetComponent<AttackableEntity>();
			if (otherEntity != null)
			{
				otherEntity.TakeDamage(damageValue, damageType);
				Destroy(gameObject);
			}
		}
	}
}
