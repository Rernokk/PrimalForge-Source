using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartseekerProjectile : MonoBehaviour {
  [SerializeField]
  int HitsLeft = 6;

	public ElementalResistances damageType = ElementalResistances.SHOCK;
	public float damageVal;
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.transform.root.GetComponent<EnemyObject>() != null)
    {
      collision.transform.root.GetComponent<EnemyObject>().TakeDamage(damageVal, damageType);
      HitsLeft--;
    }
    if (HitsLeft == 0 || collision.tag == "Wall"){
      Destroy(gameObject);
    }
  }
}
