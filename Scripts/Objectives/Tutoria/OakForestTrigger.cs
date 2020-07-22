using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakForestTrigger : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.transform.tag == "Player")
		{
			Objective_Interface.instance.QuestObjectives["EnterEastForest"].Invoke();
			Destroy(gameObject);
		}
	}
}
