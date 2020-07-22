using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfestDot : MonoBehaviour
{

  public float DamagePerSecond;
  public float Duration;
  public float ExpireRadius;
  protected float originalDuration;
  float internalCounter = 0f;
  EnemyObject attached;

  private void Start()
  {
    attached = transform.root.GetComponent<EnemyObject>();
    attached.TakeDamage(DamagePerSecond * .125f, ElementalResistances.POISON);
    originalDuration = Duration;
  }

  // Update is called once per frame
  void Update()
  {
    Duration -= Time.deltaTime;
    internalCounter += Time.deltaTime;
    if (internalCounter > .25f)
    {
      attached.TakeDamage(DamagePerSecond * .125f, ElementalResistances.POISON);
      internalCounter = 0;
    }
    if (Duration <= 0)
    {
      Destroy(this);
    }
  }

  private void OnDestroy()
  {
    if (Duration > 0)
    {
      Collider2D[] area = Physics2D.OverlapCircleAll(transform.position, ExpireRadius);
      Ability.DebugDrawRadius(transform.position, ExpireRadius, Color.green, 1f);
      if (!PlagueDoctor.isActive)
      {
        foreach (Collider2D coll in area)
        {
          EnemyObject obj = coll.transform.root.GetComponent<EnemyObject>();
          if (obj != null && obj.transform.root.GetComponent<InfestDot>() == null)
          {
            InfestDot refDot = obj.transform.root.gameObject.AddComponent<InfestDot>();
            refDot.Duration = Duration;
            refDot.DamagePerSecond = DamagePerSecond;
            refDot.ExpireRadius = ExpireRadius;
          }
        }
      }
      else
      {
        foreach (Collider2D coll in area)
        {
          EnemyObject obj = coll.transform.root.GetComponent<EnemyObject>();
          if (obj != null && obj.transform.root.GetComponent<InfestDot>() == null)
          {
            InfestDot refDot = obj.transform.root.gameObject.AddComponent<InfestDot>();
            refDot.Duration = originalDuration;
            refDot.DamagePerSecond = DamagePerSecond * 1.1f;
            refDot.ExpireRadius = ExpireRadius;
          }
        }
      }
    }
  }
}
