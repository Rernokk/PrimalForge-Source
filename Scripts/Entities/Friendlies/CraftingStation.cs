using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CraftingStationType { FORGE, ALCHEMY, OVEN };

public class CraftingStation : Townsfolk {
	public CraftingStationType stationType = CraftingStationType.FORGE;
	protected override void Start()
	{
		villagerType = VillagerType.STATION;
		base.Start();

		UpdateDisplayInfo();
	}

	public override void Interact()
	{
		Player_Accessor_Script.DialogInterface.StartDialog(this);
	}

	protected override void UpdateDisplayInfo()
	{
		Text disp = display.transform.Find("Title").GetComponent<Text>();
		disp.text = villagerName;
	}
}
