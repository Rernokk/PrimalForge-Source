using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelMouseover : MonoBehaviour
{
	[SerializeField]
	public int currWeaponSlot;
	private bool hasFethcedIcon = false;
	public void HighlightColor()
	{
		transform.Find("Sprite").GetComponent<Image>().color = Color.red;
	}

	public void ResetColor()
	{
		transform.Find("Sprite").GetComponent<Image>().color = Color.white;
	}

	public void EquipSlotItem()
	{
		if (!Player_Accessor_Script.EquipmentScript.WeaponSlotUnlocked[currWeaponSlot])
		{
			return;
		}

		int temp = Player_Accessor_Script.EquipmentScript.CurrentWeaponSlot;
		Player_Accessor_Script.EquipmentScript.CurrentWeaponSlot = currWeaponSlot;
		currWeaponSlot = temp;
		transform.name = currWeaponSlot.ToString();
		UpdateIcon();
	}

	public void LateUpdate()
	{
		if (!hasFethcedIcon)
		{
			UpdateIcon();
			hasFethcedIcon = true;
		}
	}

	public void UpdateIcon()
	{
		if (Player_Accessor_Script.EquipmentScript.EquippedWeapons[currWeaponSlot] != null)
		{
			transform.Find("Image").GetComponent<Image>().sprite = Player_Accessor_Script.EquipmentScript.EquippedWeapons[currWeaponSlot].Icon;
		}
		else
		{
			if (Player_Accessor_Script.EquipmentScript.WeaponSlotUnlocked[currWeaponSlot])
			{
				transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/EmptySlot");
			}
			else
			{
				transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/LockedSlot");
			}
		}
	}
}
