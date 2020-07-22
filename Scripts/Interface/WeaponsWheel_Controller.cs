using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsWheel_Controller : MonoBehaviour
{
	bool fetchedSlot = false;
	private void Start()
	{
	}

	void Update()
	{
		if (!fetchedSlot)
		{
			int childCounter = 0;
			for (int i = 0; i < 4; i++)
			{
				if (Player_Accessor_Script.EquipmentScript.CurrentWeaponSlot != i)
				{
					transform.GetChild(childCounter).GetComponent<WheelMouseover>().currWeaponSlot = i;
					transform.GetChild(childCounter).transform.name = i.ToString();
					childCounter++;
				}
			}
			fetchedSlot = true;
		}

		if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.I_WEAPON_WHEEL]))
		{
			CanvasGroup group = GetComponent<CanvasGroup>();
			group.alpha = 1;
			group.interactable = group.blocksRaycasts = true;
			Time.timeScale = .2f;
			Time.fixedDeltaTime = .02f * Time.timeScale;
			Player_Accessor_Script.EquipmentScript.PlayerHasControl = false;
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).GetComponent<WheelMouseover>().UpdateIcon();
			}
		}

		if (Input.GetKeyUp(KeybindManager.Keybinds[KeybindFunction.I_WEAPON_WHEEL]))
		{
			CanvasGroup group = GetComponent<CanvasGroup>();
			group.alpha = 0;
			group.interactable = group.blocksRaycasts = false;
			Time.timeScale = 1f;
			Time.fixedDeltaTime = .02f;
			Player_Accessor_Script.EquipmentScript.PlayerHasControl = true;
		}
	}
}
