using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnipeProjectile : MonoBehaviour {
	public float damageValue;
	public ElementalResistances damageType = ElementalResistances.SHOCK;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.transform.root.GetComponent<EnemyObject>() != null){
      collision.transform.root.GetComponent<EnemyObject>().TakeDamage(damageValue, damageType);
      Destroy(gameObject);
    }
  }
}
