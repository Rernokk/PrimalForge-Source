using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot_Tracker : MonoBehaviour
{
	private CanvasGroup descriptionPane;

	public int Slot { get; set; }
	public bool Resource { get; set; }

	public Player_Inventory_Script PlayerInventory { get; set; }

	public Player_Equipment_Script PlayerEquipment { get; set; }

	private void Start()
	{

	}

	public void EquipItemFromSlot()
	{
		PlayerEquipment.EquipItem(Slot);
		if (PlayerInventory.FetchItemInSlot(Slot) != null)
		{
			ToggleInfoDisplayOn();
		}
		else
		{
			ToggleInfoDisplayOff();
		}
	}

	public void ToggleInfoDisplayOn()
	{
		if (!Resource)
		{
			if (PlayerInventory.FetchItemInSlot(Slot) != null)
			{
				if (PlayerInventory.FetchItemInSlot(Slot).ItemName == null || PlayerInventory.FetchItemInSlot(Slot).ItemName == "")
				{
					return;
				}
				Inventory_Interface.instance.HoveredItemPanel.GetComponent<CanvasGroup>().alpha = 1;
				ItemType itemType = PlayerInventory.FetchItemInSlot(Slot).MyItemType;
				Inventory_Interface.instance.PopulateHoverData(PlayerInventory.FetchItemInSlot(Slot));

				if (itemType == ItemType.WEAPON)
				{
					if (PlayerEquipment.EquippedWeapon != null)
					{
						Inventory_Interface.instance.ComparedItemPanel.GetComponent<CanvasGroup>().alpha = 1;
						Inventory_Interface.instance.PopulateCompareData(PlayerEquipment.EquippedWeapon);
					}
				}
				else if (itemType == ItemType.HELMET)
				{
					if (PlayerEquipment.Helmet != null)
					{
						Inventory_Interface.instance.ComparedItemPanel.GetComponent<CanvasGroup>().alpha = 1;
						Inventory_Interface.instance.PopulateCompareData(PlayerEquipment.Helmet);
					}
				}
				else if (itemType == ItemType.CHESTPLATE)
				{
					if (PlayerEquipment.Chestplate != null)
					{
						Inventory_Interface.instance.ComparedItemPanel.GetComponent<CanvasGroup>().alpha = 1;
						Inventory_Interface.instance.PopulateCompareData(PlayerEquipment.Chestplate);
					}
				}
				else if (itemType == ItemType.PLATELEGS)
				{
					if (PlayerEquipment.LegArmor != null)
					{
						Inventory_Interface.instance.ComparedItemPanel.GetComponent<CanvasGroup>().alpha = 1;
						Inventory_Interface.instance.PopulateCompareData(PlayerEquipment.LegArmor);
					}
				}
				else if (itemType == ItemType.TOOL)
				{
					if (PlayerEquipment.EquippedTool != null)
					{
						Inventory_Interface.instance.ComparedItemPanel.GetComponent<CanvasGroup>().alpha = 1;
						Inventory_Interface.instance.PopulateCompareData(PlayerEquipment.EquippedTool);
					}
				}
			}
		}
		else
		{
			Inventory_Interface.instance.PopulateHoverData(transform.name);
		}
	}

	public void ToggleInfoDisplayOff()
	{
		Inventory_Interface.instance.HoveredItemPanel.GetComponent<CanvasGroup>().alpha = 0;
		Inventory_Interface.instance.HoveredReagentItemPanel.GetComponent<CanvasGroup>().alpha = 0;
		Inventory_Interface.instance.ComparedItemPanel.GetComponent<CanvasGroup>().alpha = 0;
	}

	public void DebugPrintItemName()
	{
		print(PlayerInventory.FetchItemInSlot(Slot).ItemName);
	}
}
