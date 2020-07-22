using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArmorerActions : MonoBehaviour
{
	public void GiftPlayerItem()
	{
		if (PlayerPrefs.GetInt("ArmorerTrainingSwordObtained") == 0)
		{
			Sword trainingSword = new Sword(Player_Accessor_Script.EquipmentScript.aLib);
			trainingSword.Name = "Training Sword";
			trainingSword.Dexterity = 1;
			trainingSword.Intelligence = 1;
			trainingSword.Might = 6;
			Player_Accessor_Script.InventoryScript.AddItemToInventory(trainingSword);
			PlayerPrefs.SetInt("ArmorerTrainingSwordObtained", 1);
		}

		if (Facepunch.Steamworks.Client.Instance != null)
		{
			if (Facepunch.Steamworks.Client.Instance.Achievements.Find("TUTORIAL_SWORD").State == false)
			{
				Facepunch.Steamworks.Client.Instance.Achievements.Find("TUTORIAL_SWORD").Trigger();
			}
		}
		else
		{
			print("Client Null!");
		}

		if (Objective_Interface.instance.IsObjectiveActive("Take Up Arms!"))
		{
			Objective_Interface.instance.PushObjectivePhase("Take Up Arms!");
		}
	}
}
