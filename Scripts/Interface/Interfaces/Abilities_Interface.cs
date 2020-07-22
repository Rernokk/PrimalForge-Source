using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Abilities_Interface : MonoBehaviour
{
	private Transform firstWeapon, secondWeapon, thirdWeapon, fourthWeapon;
	private CanvasGroup firstWeaponCanvas, secondWeaponCanvas, thirdWeaponCanvas, fourthWeaponCanvas;
	private CanvasGroup[] canvasGroups;
	private float fadeAmount = .35f;
	private int activeSlot = 0;
	private Player_Equipment_Script playerScript;
	private Slider castbar;
	private Ability castingAbility;
	private Transform[] weaponArray;


	public Slider Castbar
	{
		get
		{
			return castbar;
		}
	}

	private void Start()
	{
		firstWeapon = transform.Find("First/Panel").transform;
		secondWeapon = transform.Find("Second/Panel").transform;
		thirdWeapon = transform.Find("Third/Panel").transform;
		fourthWeapon = transform.Find("Fourth/Panel").transform;
		weaponArray = new Transform[] { firstWeapon, secondWeapon, thirdWeapon, fourthWeapon };
		castbar = transform.Find("CastBar/Slider").GetComponent<Slider>();

		firstWeaponCanvas = firstWeapon.GetComponent<CanvasGroup>();
		secondWeaponCanvas = secondWeapon.GetComponent<CanvasGroup>();
		thirdWeaponCanvas = thirdWeapon.GetComponent<CanvasGroup>();
		fourthWeaponCanvas = fourthWeapon.GetComponent<CanvasGroup>();

		secondWeaponCanvas.alpha = fadeAmount;
		thirdWeaponCanvas.alpha = fadeAmount;
		fourthWeaponCanvas.alpha = fadeAmount;

		playerScript = Player_Accessor_Script.EquipmentScript;
		playerScript.abilities_Interface = this;
		canvasGroups = new CanvasGroup[] { firstWeaponCanvas, secondWeaponCanvas, thirdWeaponCanvas, fourthWeaponCanvas };
		activeSlot = playerScript.CurrentWeaponSlot;
		UpdateInterface();

		for (int i = 1; i <= 4; i++)
		{
			string temp = KeybindManager.Keybinds[KeybindFunction.A_FIRSTSKILL + i - 1].ToString();
			if (temp.IndexOf("Alpha") > -1)
			{
				temp = temp.Substring(temp.IndexOf("Alpha") + 5);
			}
			firstWeapon.Find("Slot-" + i).GetComponentInChildren<Text>().text = temp;
			secondWeapon.Find("Slot-" + i).GetComponentInChildren<Text>().text = temp;
			thirdWeapon.Find("Slot-" + i).GetComponentInChildren<Text>().text = temp;
			fourthWeapon.Find("Slot-" + i).GetComponentInChildren<Text>().text = temp;
		}
	}

	// Update is called once per frame
	private void Update()
	{
		if (RecipeCrafter.IsUsingInput)
		{
			return;
		}

		if (activeSlot != playerScript.CurrentWeaponSlot)
		{
			activeSlot = playerScript.CurrentWeaponSlot;
			UpdateInterface();
		}

		UpdateSkillCooldowns();
		UpdateSkillIcons();
		if (castingAbility != null)
		{
			UpdateCast();
		}
	}

	private void UpdateInterface()
	{
		foreach (CanvasGroup group in canvasGroups)
		{
			group.alpha = fadeAmount;
		}
		canvasGroups[activeSlot].alpha = 1;
	}

	public void UpdateSkillCooldowns()
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 1; j <= 4; j++)
			{
				if (playerScript.FetchItemSlot(i) != null)
				{
					if (playerScript.FetchItemSlot(i).SkillSet[j - 1] != null)
					{
						weaponArray[i].Find("Slot-" + j + "/FG").GetComponent<Image>().fillAmount = 1 - playerScript.FetchItemSlot(i).SkillSet[j - 1].CooldownRemaining;
					}
					else
					{
						weaponArray[i].Find("Slot-" + j + "/FG").GetComponent<Image>().fillAmount = 1;
					}

					if (playerScript.FetchItemSlot(i).Icon != null)
					{
						weaponArray[i].Find("Weapon").GetComponent<Image>().sprite = playerScript.FetchItemSlot(i).Icon;
					}
					else
					{
						weaponArray[i].Find("Weapon").GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/EmptySlot");
					}
				}
				else
				{
					if (playerScript.WeaponSlotUnlocked[i])
					{
						weaponArray[i].Find("Weapon").GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/EmptySlot");
					}
					else
					{
						weaponArray[i].Find("Weapon").GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/LockedSlot");
					}
				}
			}
		}
	}

	public void UpdateSkillIcons()
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 1; j <= 4; j++)
			{
				if (playerScript.FetchItemSlot(i) != null)
				{
					if (playerScript.FetchItemSlot(i).SkillSet[j - 1] != null)
					{
						weaponArray[i].Find("Slot-" + j).GetComponent<Image>().sprite = playerScript.FetchItemSlot(i).SkillSet[j - 1].skillIcon;
						weaponArray[i].Find("Slot-" + j + "/FG").GetComponent<Image>().sprite = playerScript.FetchItemSlot(i).SkillSet[j - 1].skillIcon;
					}
					else
					{
						weaponArray[i].Find("Slot-" + j).GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/EmptySlot");
						weaponArray[i].Find("Slot-" + j + "/FG").GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/EmptySlot");
					}
				}
				else if (playerScript.FetchItemSlot(i) == null)
				{
					weaponArray[i].Find("Slot-" + j).GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/EmptySlot");
					weaponArray[i].Find("Slot-" + j + "/FG").GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/EmptySlot");
				}
			}
		}
	}

	public void StartCast(Ability info)
	{
		Castbar.GetComponent<CanvasGroup>().alpha = 1;
		Castbar.transform.Find("Fill Area/Spell").GetComponent<Text>().text = info.abilityName;
		castingAbility = info;
	}

	private void UpdateCast()
	{
		Castbar.value = (castingAbility.CastTime - castingAbility.CastTick) / castingAbility.CastTime;
	}

	public void EndCast()
	{
		Castbar.value = 0;
		Castbar.GetComponent<CanvasGroup>().alpha = 0;
		Castbar.transform.Find("Fill Area/Spell").GetComponent<Text>().text = "";
		castingAbility = null;
	}
}
