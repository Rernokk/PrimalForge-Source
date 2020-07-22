using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayProjectile : MonoBehaviour {
	public float damageValue;
	public ElementalResistances damageType = ElementalResistances.PHYSICAL;

  private void Start()
  {
    GetComponent<Rigidbody2D>().AddForce(transform.right * 40f, ForceMode2D.Impulse);
    //Debug.DrawRay(transform.position, transform.right * 10f, Color.red, 1f);
  }
  private void OnTriggerEnter2D(Collider2D coll)
  {
    EnemyObject obj = coll.transform.root.GetComponent<EnemyObject>();
    if (obj != null){
      obj.TakeDamage(damageValue, damageType);
    }
    Destroy(gameObject);
  }
}
