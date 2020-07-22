using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulLinkNode : MonoBehaviour
{
	[SerializeField]
	GameObject VFXPrefab;
  // Use this for initialization
  void Start()
  {
    Ability.DebugDrawRadius(transform.position, 3f, new Color(1, 0, 1, 1), 3f);
    Collider2D[] inRange = Physics2D.OverlapCircleAll(transform.position, 3f);
    if (!HexMaster.isActive)
    {
      for (int i = 0; i < inRange.Length; i++)
      {
        for (int j = 1; j < inRange.Length; j++)
        {
          if (i != j && inRange[i].transform.root.GetComponent<EnemyObject>() != null && inRange[j].transform.root.GetComponent<EnemyObject>() != null)
          {
            if (inRange[i].transform.root.GetComponent<EnemyObject>().LinkedTargets.Count == 0 && inRange[j].transform.root.GetComponent<EnemyObject>().LinkedTargets.Count == 0)
            {
              inRange[i].transform.root.GetComponent<EnemyObject>().LinkedTargets.Add(inRange[j].transform.root.GetComponent<EnemyObject>());
              inRange[j].transform.root.GetComponent<EnemyObject>().LinkedTargets.Add(inRange[i].transform.root.GetComponent<EnemyObject>());
							GameObject obj = Instantiate(VFXPrefab, Vector3.zero, Quaternion.identity);
							LineRenderer LR = obj.GetComponent<LineRenderer>();
							LR.positionCount = 3;
							LR.SetPositions(new Vector3[] { inRange[i].transform.position, (inRange[i].transform.position + inRange[j].transform.position) * .5f, inRange[j].transform.position });
							obj.GetComponent<TetherVFXController>().objRef1 = inRange[i].transform.root.GetComponent<EnemyObject>();
							obj.GetComponent<TetherVFXController>().objRef2 = inRange[j].transform.root.GetComponent<EnemyObject>();
              Destroy(gameObject);
              return;
            }
          }
        }
      }
    } else
    {
      for (int i = 0; i < inRange.Length; i++)
      {
        for (int j = 1; j < inRange.Length; j++)
        {
          if (i != j && inRange[i].transform.root.GetComponent<EnemyObject>() != null && inRange[j].transform.root.GetComponent<EnemyObject>() != null)
          {
            if (inRange[i].transform.root.GetComponent<EnemyObject>().LinkedTargets.Count < 2 && inRange[j].GetComponent<EnemyObject>().LinkedTargets.Count < 2)
            {
              inRange[i].transform.root.GetComponent<EnemyObject>().LinkedTargets.Add(inRange[j].transform.root.GetComponent<EnemyObject>());
              inRange[j].transform.root.GetComponent<EnemyObject>().LinkedTargets.Add(inRange[i].transform.root.GetComponent<EnemyObject>());
            }
          }
        }
      }
      Destroy(gameObject);
    }
  }
}
