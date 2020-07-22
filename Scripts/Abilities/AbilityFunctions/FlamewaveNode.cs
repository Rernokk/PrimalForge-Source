using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamewaveNode : MonoBehaviour
{
	bool firstStage = true, pause = false;

  [SerializeField]
  int refFrame = 10;

	public float damageVal;
	public ElementalResistances damageType = ElementalResistances.FIRE;

	private float timeLimit = .5f;
	private Vector3 pausePosition;

  private void Start()
  {
    //StartCoroutine(WaveComponent());
  }

  IEnumerator WaveComponent()
  {
    for (int i = 0; i < refFrame; i++)
    {
      transform.position += transform.up * Time.deltaTime * 12f;
      yield return null;
    }
    firstStage = false;
    yield return new WaitForSeconds(.25f);
    Transform playerPos = Player_Accessor_Script.ControllerScript.transform;
    Vector3 startPos = transform.position;
    for (int i = 0; i < refFrame; i++)
    {
      transform.position = Vector3.Lerp(startPos, playerPos.position, i / (refFrame-1.0f));
      yield return null;
    }
    Destroy(gameObject, .2f);
  }

	void Update()
	{
		if (firstStage)
		{
			transform.position += Time.deltaTime * transform.up * 12f;
			Debug.DrawRay(transform.position, transform.up, Color.red);
			transform.Rotate(0, 0, 360f * Time.deltaTime);
			timeLimit -= Time.deltaTime;
			if (timeLimit <= 0)
			{
				timeLimit = .5f;
				firstStage = false;
				pause = true;
			}
		} else
		{
			if (pause)
			{
				timeLimit -= Time.deltaTime;
				if (timeLimit <= 0)
				{
					pause = false;
					pausePosition = transform.position;
				}
			}
			else
			{
				transform.position = Vector3.Lerp(pausePosition, Player_Accessor_Script.ControllerScript.transform.position, timeLimit);
				timeLimit += Time.deltaTime;
				if (timeLimit >= 1f)
				{
					Destroy(gameObject);
				}
			}
		}
	}

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (firstStage)
    {
      EnemyObject obj = collision.transform.root.GetComponent<EnemyObject>();
      if (obj != null){
        obj.TakeDamage(damageVal, ElementalResistances.FIRE);
      }
    }
    else
    {
      EnemyObject obj = collision.transform.root.GetComponent<EnemyObject>();
      if (obj != null)
      {
        obj.TakeDamage(damageVal * 1.4f, ElementalResistances.FIRE);
      }
    }
  }
}
