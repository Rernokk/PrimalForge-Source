using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum VillagerType { CITIZEN, SHOPKEEP, STATION }

public class Townsfolk : MonoBehaviour
{
	[System.Serializable]
	public struct ChatActionPair
	{
		public int chatIndex;
		public UnityEvent refAction;
	}
	public VillagerType villagerType = VillagerType.CITIZEN;
	public string villagerName;
	public List<string> chatDialog = new List<string>();

	protected Transform display;
	private Transform interaction;
	private SpriteRenderer sprite;
	private bool isInInteractionRange = false;
	public List<ChatActionPair> chatActions = new List<ChatActionPair>();

	protected virtual void Start()
	{
		if (villagerName == "" && villagerType != VillagerType.STATION)
		{
			villagerName = "Stranger";
		}

		display = transform.Find("Display");
		sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		interaction = display.Find("Interact");
		UpdateSortingLayer();
		UpdateDisplayInfo();
	}

	protected void Update()
	{
		if (!isInInteractionRange && Vector2.Distance(transform.position, Player_Accessor_Script.EquipmentScript.transform.position) <= 2f)
		{
			isInInteractionRange = true;
			ToggleDisplayInteractionMessage();
		}
		else if (isInInteractionRange && Vector2.Distance(transform.position, Player_Accessor_Script.EquipmentScript.transform.position) > 2f)
		{
			isInInteractionRange = false;
			ToggleDisplayInteractionMessage();
		}
	}

	protected void UpdateSortingLayer()
	{
		sprite.sortingOrder = -(int)(transform.position.y * 10);
	}

	protected virtual void UpdateDisplayInfo()
	{
		Text disp = display.transform.Find("Title").GetComponent<Text>();
		disp.text = villagerName + "\n" + villagerType.ToString()[0] + villagerType.ToString().ToLower().Substring(1);
	}

	protected void ToggleDisplayInteractionMessage()
	{
		CanvasGroup canvGroup = interaction.GetComponent<CanvasGroup>();
		canvGroup.alpha = Mathf.Abs(canvGroup.alpha - 1);

	}

	public virtual void Interact()
	{
		Player_Accessor_Script.DialogInterface.StartDialog(this);
	}
}
