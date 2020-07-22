using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Dialog_Interface : MonoBehaviour
{
	private Townsfolk targetDialog;
	private int dialogCounter = 0;
	private CanvasGroup dialogWindow;
	private Coroutine currentMessage;
	private Profession_Interface professionInterfaceRef;

	public static string[] GenericLines;

	private void Start()
	{
		dialogWindow = transform.GetComponent<CanvasGroup>();
		professionInterfaceRef = GameObject.Find("Interface/ItemCrafting/EquipmentInterface").GetComponent<Profession_Interface>();
		GenericLines = new string[] { "Hi", "Hello", "Hell", "What" };
	}

	public void StartDialog(Townsfolk tar)
	{
		if (tar.chatDialog.Count == 0)
		{
			return;
		}

		if (RecipeCrafter.IsUsingInput)
		{
			return;
		}

		targetDialog = tar;
		dialogWindow.alpha = 1;
		dialogWindow.blocksRaycasts = true;
		dialogWindow.interactable = true;
		Player_Accessor_Script.DetailsScript.IsStunned = true;
		Player_Accessor_Script.ReputationScript.IsInteracting = true;
		dialogWindow.transform.Find("Background/Panel/DialogText").GetComponent<Text>().text = targetDialog.chatDialog[dialogCounter];
		foreach (Townsfolk.ChatActionPair pair in targetDialog.chatActions)
		{
			if (pair.chatIndex == dialogCounter)
			{
				pair.refAction.Invoke();
			}
		}
	}

	private IEnumerator DrawMessage()
	{
		Text comp = dialogWindow.transform.Find("Background/Panel/DialogText").GetComponent<Text>();
		for (int i = 0; i <= targetDialog.chatDialog[dialogCounter].Length; i++)
		{
			int offset = 0;
			string refText = targetDialog.chatDialog[dialogCounter];
			string tempText = targetDialog.chatDialog[dialogCounter].Substring(0, i);
			string result = "";
			if (tempText.IndexOf("<") >= 0)
			{
				string tagSubstr = refText.Substring(refText.IndexOf("<"), refText.IndexOf(">") - refText.IndexOf("<") + 1);
				int tagLength = refText.IndexOf(">") - refText.IndexOf("<") + 1;
				print(tagSubstr);
				print(tagSubstr.Insert(1, "/"));
				result = tempText.Substring(0, tempText.IndexOf("<"));
				while (tempText.IndexOf("<") >= 0)
				{
					tempText = tempText.Substring(tempText.IndexOf(tagSubstr) + tagLength);
					result += tempText;
				}
			} else
			{
				result = tempText;
			}
			comp.text = result;
			yield return null;

		}

		currentMessage = null;
	}

	public void ProgressDialog()
	{
		if (currentMessage == null)
		{
			dialogCounter++;
			foreach (Townsfolk.ChatActionPair pair in targetDialog.chatActions)
			{
				if (pair.chatIndex == dialogCounter)
				{
					pair.refAction.Invoke();
				}
			}
		}

		if (dialogCounter == targetDialog.chatDialog.Count)
		{
			EndDialog();
		}
		else if (currentMessage == null)
		{
			//currentMessage = StartCoroutine(DrawMessage());
			dialogWindow.transform.Find("Background/Panel/DialogText").GetComponent<Text>().text = targetDialog.chatDialog[dialogCounter];
		}
		else if (currentMessage != null)
		{
			dialogWindow.transform.Find("Background/Panel/DialogText").GetComponent<Text>().text = targetDialog.chatDialog[dialogCounter];
			StopCoroutine(currentMessage);
			currentMessage = null;
		}
	}

	public void EndDialog()
	{
		if (targetDialog != null && targetDialog.villagerType == VillagerType.STATION)
		{
			switch ((targetDialog as CraftingStation).stationType)
			{
				case (CraftingStationType.FORGE):
					professionInterfaceRef.DisplayInterface("Alchemy");
					break;
			}
		}
		targetDialog = null;
		dialogCounter = 0;
		dialogWindow.alpha = 0;
		dialogWindow.blocksRaycasts = false;
		dialogWindow.interactable = false;
		Player_Accessor_Script.DetailsScript.IsStunned = false;
		Player_Accessor_Script.ReputationScript.IsInteracting = false;
	}

	protected virtual void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			EndDialog();
		}
	}
}
