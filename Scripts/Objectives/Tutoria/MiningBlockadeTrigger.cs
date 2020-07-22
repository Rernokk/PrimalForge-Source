using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningBlockadeTrigger : MonoBehaviour
{
	private bool listenerAdded = false;
	private ResourceNode myNode;
	private void Start()
	{
		if (Objective_Interface.instance.QuestCompletionStates.ContainsKey(QUESTNAME.MineBlockade) && Objective_Interface.instance.QuestCompletionStates[QUESTNAME.MineBlockade] == 2)
		{
			Destroy(gameObject);
		}
		else
		{
			myNode = GetComponent<ResourceNode>();
		}
	}

	private void Update()
	{
		if (myNode == null)
		{
			myNode = GetComponent<ResourceNode>();
		}
		if (myNode.ResourceAmount == 0)
		{
			Objective_Interface.instance.QuestObjectives["ClearRubble"].Invoke();
		}
	}

	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.transform.tag == "Player" && Objective_Interface.instance.QuestCompletionStates[QUESTNAME.MineBlockade] == 0)
		{
			Objective_Interface.instance.AddNewObjective(new MineBlockadeObjective());
		}
	}
}
