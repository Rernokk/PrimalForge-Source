using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrifyNode : MonoBehaviour {
  public float Radius;
  public float Duration;
  public float DampenAmount;

	// Use this for initialization
	void Start () {
    Ability.DebugDrawRadius(transform.position, Radius, Color.blue, Duration);
    Collider2D[] tars = Physics2D.OverlapCircleAll(transform.position, Radius);
    foreach (Collider2D tar in tars){
      EnemyObject obj = tar.transform.root.GetComponent<EnemyObject>();
      if (obj !=  null){
        obj.DampenAttributes(Duration, DampenAmount, AttributeType.ATTACKMULT);
      }
    }
    Destroy(gameObject, Duration);
	}
}
