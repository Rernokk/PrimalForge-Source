using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildfireDot : MonoBehaviour {
  public int burnCount;
  public float duration;
  public float damageValue;
  float timeSinceTick = 0;
  EnemyObject obj;

	// Use this for initialization
	void Start () {
    obj = transform.root.GetComponent<EnemyObject>();
	}
	
	// Update is called once per frame
	void Update () {
    duration -= Time.deltaTime;
    timeSinceTick += Time.deltaTime;
    if (obj != null && timeSinceTick > .5f){
      obj.TakeDamage(damageValue, ElementalResistances.FIRE);
      Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 1.75f);
      Ability.DebugDrawRadius(transform.position, 1.75f, Color.red, .35f);
      foreach (Collider2D coll in nearby){
        if (coll.transform.root.GetComponent<EnemyObject>() != null && coll.transform.root.GetComponent<WildfireDot>() == null){
          WildfireDot spreadDot = coll.transform.root.gameObject.AddComponent<WildfireDot>();
          spreadDot.duration = duration;
          spreadDot.damageValue = damageValue * 1.5f;
          spreadDot.burnCount = burnCount + 1;
        }
      }
      timeSinceTick = 0;
    }
    if (duration < 0){
      Destroy(this);
    }
	}
}
