using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonProjectile : MonoBehaviour {
  [SerializeField]
  int maxEnemiesHit = 4;

	public float damageVal;
	public ElementalResistances damageType;

  private void Start()
  {
    Rigidbody2D rgd = GetComponent<Rigidbody2D>();
    Vector3 vel = rgd.velocity;
    vel.z = 0;
    vel.Normalize();
		rgd.velocity = vel * rgd.velocity.magnitude;
		if (Player_Accessor_Script.EquipmentScript.ActivePerks.Contains(LegendaryPerk.Violent_Kings_Skullcrown))
		{
			rgd.velocity = rgd.velocity * .5f;
		}
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
		if (Player_Accessor_Script.EquipmentScript.ActivePerks.Contains(LegendaryPerk.Violent_Kings_Skullcrown))
		{
			if (collision.transform.root.GetComponent<EnemyObject>() != null)
			{
				Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 3f, LayerMask.GetMask("Enemies"));
				foreach (Collider2D coll in colls)
				{
					coll.transform.root.GetComponent<EnemyObject>().TakeDamage(damageVal, damageType);
				}
				Destroy(gameObject);
			}
		}
		else
		{
			if (collision.transform.root.GetComponent<EnemyObject>() != null)
			{
				collision.transform.root.GetComponent<EnemyObject>().TakeDamage(damageVal,damageType);
				maxEnemiesHit--;
				if (maxEnemiesHit == 0)
				{
					Destroy(gameObject);
				}
			}
		}
  }
}
