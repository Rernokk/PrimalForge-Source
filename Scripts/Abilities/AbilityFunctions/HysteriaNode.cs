using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HysteriaNode : MonoBehaviour {

	// Use this for initialization
	void Start () {
    Collider2D[] AreaTargets = Physics2D.OverlapCircleAll(transform.position, 5f);
    Ability.DebugDrawRadius(transform.position, 5f, Color.green, 1f);
    foreach (Collider2D coll in AreaTargets){
      EnemyObject obj = coll.transform.root.GetComponent<EnemyObject>();
      if (obj != null){
        obj.SetInsane();
      }
    }
    Destroy(gameObject);
	}
}
