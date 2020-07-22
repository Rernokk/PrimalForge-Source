using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AftershockNode : MonoBehaviour
{
  [SerializeField]
  GameObject AftershockPrefab;

  public int count = 0;
  public int maxCount = 7;

	public float damageVal;
	public ElementalResistances damageType = ElementalResistances.PHYSICAL;
	bool checkForDamage = true;

  // Use this for initialization
  void Start()
  {
    if (count < maxCount)
    {
      StartCoroutine(DelayedWave());
    }
    else
    {
      StartCoroutine(DelayedDestroy());
    }
		LightCamShake();

		foreach (Collider2D coll in Physics2D.OverlapCircleAll(transform.position, .5f))
		{
			EnemyObject obj = coll.transform.root.GetComponent<EnemyObject>();
			if (obj != null)
			{
				obj.TakeDamage(damageVal, damageType);
			}
		}
  }

	private void LightCamShake()
	{
		//Camera.main.GetComponent<CameraShake>().ShakeCamera(1f, .1f);
	}

  IEnumerator DelayedWave()
  {
    for (int i = 0; i < 2; i++)
    {
      yield return null;
    }
    if (count == 0)
    {
      transform.Rotate(0, 0, -37.5f);
      for (int i = 0; i < 6; i++) {
        GameObject temp = Instantiate(AftershockPrefab, transform.position + transform.up * .5f, transform.rotation);
        temp.GetComponent<AftershockNode>().count = count + 1;
        transform.Rotate(0, 0, 15);
      }
			Camera.main.GetComponent<Camera_Controller_Script>().TriggerShake(.2f, .1f);
    }
    else
    {
      GameObject temp = Instantiate(AftershockPrefab, transform.position + transform.up * .5f, transform.rotation);
      temp.GetComponent<AftershockNode>().count = count + 1;
			//if (count + 1 > 5)
			//{
			//	temp.transform.Find("Clouds").gameObject.SetActive(true);
			//}
		}
    Destroy(gameObject, .125f);
  }

  //private void OnTriggerEnter2D(Collider2D collision)
  //{
		//if (checkForDamage)
		//{
		//	EnemyObject obj = collision.transform.root.GetComponent<EnemyObject>();
		//	if (obj != null)
		//	{
		//		obj.TakeDamage(damageVal, damageType);
		//	}
		//}
		////checkForDamage = false;
  //}

  IEnumerator DelayedDestroy()
  {
    for (int i = 0; i < 6; i++)
    {
      yield return null;
    }
    Destroy(gameObject, .125f);
  }
}
