using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialForemanActions : MonoBehaviour
{
	public void GiftAxe()
	{
		if(Objective_Interface.instance.QuestCompletionStates[QUESTNAME.CrossBridge] == 1 && Objective_Interface.instance.QuestProgressStates[QUESTNAME.CrossBridge] == 1)
		{
			Hatchet giftHatchet = new Hatchet();
			giftHatchet.ItemName = "Dull Hatchet";
			Player_Accessor_Script.InventoryScript.AddItemToInventory(giftHatchet);
			Objective_Interface.instance.QuestObjectives["SpeakWithForeman"].Invoke();
		}
	}

	public void FixBridge()
	{
		if (Objective_Interface.instance.QuestProgressStates[QUESTNAME.CrossBridge] == 2 && Player_Accessor_Script.InventoryScript.GetResourceQuantity("Oak Planks") >= 5)
		{
			Player_Accessor_Script.InventoryScript.RemoveResource("Oak Planks", 5);
			Objective_Interface.instance.QuestObjectives["DeliverPlanks"].Invoke();
		}
	}
}
