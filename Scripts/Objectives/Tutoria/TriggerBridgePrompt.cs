using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBridgePrompt : MonoBehaviour
{
	bool listenerAdded = false;

	[SerializeField]
	Sprite completedBridge;
	private void Start()
	{
		if (Objective_Interface.instance.QuestCompletionStates.ContainsKey(QUESTNAME.CrossBridge) && Objective_Interface.instance.QuestCompletionStates[QUESTNAME.CrossBridge] == 2)
		{
			ClearBlock();
			Destroy(gameObject);
		}
		else
		{
			if (Objective_Interface.instance.QuestObjectives.ContainsKey("BuildBridge"))
				Objective_Interface.instance.QuestObjectives["BuildBridge"].AddListener(ClearBlock);
		}
	}

	private void Update()
	{
		if (!listenerAdded && Objective_Interface.instance.QuestObjectives.ContainsKey("BuildBridge"))
		{
			Objective_Interface.instance.QuestObjectives["BuildBridge"].AddListener(ClearBlock);
			listenerAdded = true;
		}
	}

	private void ClearBlock()
	{
		//transform.root.Find("BridgeBlock").GetComponent<BoxCollider2D>().enabled = false;
		Destroy(transform.root.Find("BridgeBlock").gameObject);
		transform.root.GetComponent<SpriteRenderer>().sprite = completedBridge;
	}

	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.transform.tag == "Player" && Objective_Interface.instance.QuestCompletionStates[QUESTNAME.CrossBridge] == 1)
		{
			Objective_Interface.instance.QuestObjectives["InteractBridge"].Invoke();
		}
	}
}
