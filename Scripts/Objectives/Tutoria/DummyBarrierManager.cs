using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBarrierManager : MonoBehaviour
{
	private void Start()
	{
		if (Objective_Interface.instance.QuestCompletionStates.ContainsKey(QUESTNAME.CleaveYourFoes) && Objective_Interface.instance.QuestCompletionStates[QUESTNAME.CleaveYourFoes] == 2)
		{
			GetComponent<BoxCollider2D>().enabled = false;
			GetComponentInChildren<SpriteRenderer>().enabled = false;
		}
	}
}
