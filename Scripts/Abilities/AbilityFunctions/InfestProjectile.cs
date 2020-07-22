using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfestProjectile : MonoBehaviour {

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.transform.root.GetComponent<EnemyObject>() && collision.transform.root.GetComponent<InfestDot>() == null){
      InfestDot dotApp = collision.transform.root.gameObject.AddComponent<InfestDot>();
      dotApp.DamagePerSecond = 12;
      dotApp.Duration = 8;
      dotApp.ExpireRadius = 2;
    }
    Destroy(gameObject);
  }
}
