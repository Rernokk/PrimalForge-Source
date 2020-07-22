using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowstormNode : MonoBehaviour
{
  public float remDur = 0f;
  public float tickMax = .5f;
  public float radius = 6f;
	public float damagePerTick = 1f;
	public ElementalResistances damageType = ElementalResistances.PHYSICAL;
  float tickCycle = 0f;

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    tickCycle += Time.deltaTime;
    if (tickCycle > tickMax)
    {
      tickCycle -= tickMax;
      Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, radius);
      foreach (Collider2D coll in colls)
      {
        EnemyObject obj = coll.transform.root.GetComponent<EnemyObject>();
        if (obj != null)
        {
          obj.TakeDamage(damagePerTick, damageType);
        }
      }
    }

    remDur -= Time.deltaTime;
    if (remDur < 0)
    {
      Destroy(gameObject);
    }
  }
}
