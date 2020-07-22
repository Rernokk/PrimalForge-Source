using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoltNode : MonoBehaviour
{
  [SerializeField]
  int MaxHitTargets = 2;
  float timer = 0;
  Collider2D coll;

  [SerializeField]
  List<EnemyObject> hitTargets;

  LineRenderer myLineRenderer;

	public float damageVal;
	public ElementalResistances damageType = ElementalResistances.SHOCK;

  // Use this for initialization
  void Start()
  {
    coll = GetComponent<CircleCollider2D>();
    hitTargets = new List<EnemyObject>();
    myLineRenderer = GetComponent<LineRenderer>();
    
    //Finding Targets near first point, adding to the list for teh starting position of the chain.
    Collider2D[] res = Physics2D.OverlapCircleAll(transform.position, 6f);
    if (res.Length == 0)
    {
      myLineRenderer.positionCount = 0;
      return;
    }
    List<Collider2D> colls = sortArrayByDistance(res, transform.position);
    hitTargets.Add(colls[0].transform.root.GetComponent<EnemyObject>());

    while (hitTargets.Count < MaxHitTargets)
    {
      List<Collider2D> nextColls = sortArrayByDistance(Physics2D.OverlapCircleAll(hitTargets[hitTargets.Count - 1].transform.position, 3f), hitTargets[hitTargets.Count - 1].transform.position);
      bool hasFoundTarget = false;
      for (int i = 0; i < nextColls.Count; i++)
      {
        if (!hitTargets.Contains(nextColls[i].transform.root.GetComponent<EnemyObject>()))
        {
          hitTargets.Add(nextColls[i].transform.root.GetComponent<EnemyObject>());
          hasFoundTarget = true;
          break;
        }
      }
      if (!hasFoundTarget)
      {
        break;
      }
    }

    Vector3[] positionValues = new Vector3[hitTargets.Count + 1];
    positionValues[0] = Player_Accessor_Script.ControllerScript.transform.position;
    int posValIndex = 1;
    foreach (EnemyObject obj in hitTargets)
    {
      obj.TakeDamage(damageVal, damageType);
      positionValues[posValIndex] = obj.transform.position;
      posValIndex++;
    }
    myLineRenderer.positionCount = positionValues.Length;
    myLineRenderer.SetPositions(positionValues);
  }

  // Update is called once per frame
  void Update()
  {
      //Finding Targets near first point, adding to the list for teh starting position of the chain.
      //Collider2D[] res = Physics2D.OverlapCircleAll(transform.position, 3f);
      //if (res.Length == 0){
      //  myLineRenderer.positionCount = 0;
      //  return;
      //}
      //List<Collider2D> colls = sortArrayByDistance(res, transform.position);
      //hitTargets.Add(colls[0].GetComponent<EnemyObject>());

      //while (hitTargets.Count < MaxHitTargets)
      //{
      //  List<Collider2D> nextColls = sortArrayByDistance(Physics2D.OverlapCircleAll(hitTargets[hitTargets.Count - 1].transform.position, 3f), hitTargets[hitTargets.Count - 1].transform.position);
      //  bool hasFoundTarget = false;
      //  for (int i = 0; i < nextColls.Count; i++)
      //  {
      //    if (!hitTargets.Contains(nextColls[i].GetComponent<EnemyObject>()))
      //    {
      //      hitTargets.Add(nextColls[i].GetComponent<EnemyObject>());
      //      hasFoundTarget = true;
      //      break;
      //    }
      //  }
      //  if (!hasFoundTarget)
      //  {
      //    break;
      //  }
      //}

      //Vector3[] positionValues = new Vector3[hitTargets.Count + 1];
      //positionValues[0] = Player_Accessor_Script.ControllerScript.transform.position;
      //int posValIndex = 1;
      //foreach (EnemyObject obj in hitTargets)
      //{
      //  obj.TakeDamage(1f, ElementalResistances.SHOCK);
      //  positionValues[posValIndex] = obj.transform.position;
      //  posValIndex++;
      //}
      //myLineRenderer.positionCount = positionValues.Length;
      //myLineRenderer.SetPositions(positionValues);
  }

  List<Collider2D> sortArrayByDistance(Collider2D[] array, Vector2 refpt){
    List<Collider2D> retVal = new List<Collider2D>();
    for (int i = 0; i < array.Length; i++)
    {
      if (array[i].transform.root.GetComponent<EnemyObject>() != null)
        retVal.Add(array[i]);
    }
    retVal.Sort((q1, q2) => Vector3.Distance(refpt, q1.transform.position).CompareTo(Vector3.Distance(refpt, q2.transform.position)));
    return retVal;
  }
}
