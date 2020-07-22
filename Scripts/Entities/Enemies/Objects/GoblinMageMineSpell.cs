using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinMageMineSpell : MonoBehaviour
{

	[SerializeField]
	private Color startCol, endCol;
	private SpriteRenderer sprRend;
	private float countdown = 3.5f;
	private float maxCount = 1f;

	public float DamageValue = 1f;
	public ElementalResistances DamageType = ElementalResistances.FIRE;
	public float SpellRadius = 1f;

	public float Countdown
	{
		get
		{
			return countdown;
		}

		set
		{
			countdown = value;
			maxCount = value;
		}
	}

	private void Start()
	{
		//sprRend = transform.GetComponentInChildren<SpriteRenderer>();
		//sprRend.sortingOrder = -(int)(transform.position.y * 10);
	}

	// Update is called once per frame
	private void Update()
	{
		//sprRend.color = Color.Lerp(endCol, startCol, countdown / maxCount);
		//sprRend.transform.localScale = new Vector3(10f,10f,0f) * SpellRadius;
		countdown -= Time.deltaTime;

		if (countdown <= 0)
		{
			//Detonate();
		}
	}

	public void Detonate()
	{
		Collider2D[] inCircle = Physics2D.OverlapCircleAll(transform.position, SpellRadius);
		foreach (Collider2D coll in inCircle)
		{
			if(coll.transform.tag == "Player" || coll.transform.tag == "Minion")
			{
				coll.GetComponent<AttackableEntity>().TakeDamage(DamageValue, DamageType);
			}
		}
		GetComponentInChildren<SpriteRenderer>().enabled = false;
		Destroy(gameObject, 1f);
	}
}
