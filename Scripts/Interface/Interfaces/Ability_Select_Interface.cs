using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ability_Select_Interface : MonoBehaviour
{
	public static Ability_Select_Interface instance;
	private Player_Equipment_Script eqRef;

	[SerializeField]
	private GameObject AbilitySlotPrefab;

	public GhostIcon_Element ghostIcon;

	[SerializeField]
	private int ind = 0, prevInd = 0;
	private int skillSlot = 0;

	[SerializeField]
	private Sprite defaultIcon;
	private Weapon selectedWeapon;
	private Vector3 offset = Vector3.zero;
	private CanvasGroup canvas;
	private Transform weaponParent, abilityParent, abilitySlotParent;
	private bool isMoving = false;
	private bool setDefault;

	private void Awake()
	{
	}
	// Use this for initialization
	private void Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
			return;
		}

		selectedWeapon = null;
		weaponParent = transform.Find("Window/Header/Weapons");
		abilityParent = transform.Find("Window/ActiveAbilities");
		abilitySlotParent = transform.Find("Window/CenterSection/AbilitySlots");
		setDefault = true;
		canvas = GetComponent<CanvasGroup>();
		eqRef = Player_Accessor_Script.EquipmentScript;
		for (int i = 0; i < 4; i++)
		{
			EventTrigger trigger = abilityParent.GetChild(i).GetComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.Drop;
			int temp = new int();
			temp = i;
			entry.callback.AddListener((data) => { DropAbilityActivation((PointerEventData)data, temp); });
			trigger.triggers.Add(entry);
		}
	}

	// Update is called once per frame
	private void Update()
	{
		if (RecipeCrafter.IsUsingInput)
		{
			return;
		}

		if (selectedWeapon == null && setDefault)
		{
			selectedWeapon = Player_Accessor_Script.EquipmentScript.FetchItemSlot(0);
			setDefault = false;
		}
		for (int i = 0; i < 4; i++)
		{
			Weapon temp = Player_Accessor_Script.EquipmentScript.FetchItemSlot(i);
			if (temp == null)
			{
				continue;
			}


			//Fetches the sprite for the weapon with a look-up table.
			weaponParent.transform.GetChild(i).GetComponent<Image>().sprite = temp.Icon;
		}

		if (selectedWeapon != null)
		{
			for (int j = 0; j < 4; j++)
			{
				if (selectedWeapon.SkillSet[j] != null)
				{
					abilityParent.GetChild(j).GetComponent<Image>().sprite = selectedWeapon.SkillSet[j].skillIcon;
				}
				else
				{
					abilityParent.GetChild(j).GetComponent<Image>().sprite = defaultIcon;
				}
			}

			List<Ability> tempList = AbilityLibrary.FetchSkillList(selectedWeapon.Type.ToString());
			if (abilitySlotParent.childCount == 0 && selectedWeapon != null)
			{
				for (int i = 0; i < tempList.Count; i++)
				{
					GameObject temp = Instantiate(AbilitySlotPrefab, abilitySlotParent);
				}
			}

			//Sorting Abilities by level requirement
			tempList.Sort((q1, q2) => q1.SkillLevelRequirement.CompareTo(q2.SkillLevelRequirement));

			//Describing Header
			Transform skillHeader = transform.Find("Window/Header/SkillSection");
			skillHeader.Find("WeaponIcon").GetComponent<Image>().sprite = selectedWeapon.Icon;
			skillHeader.Find("Weapon").GetComponent<Text>().text = selectedWeapon.ItemName;
			skillHeader.Find("SkillName").GetComponent<Text>().text = selectedWeapon.SkillRequired;
			Skills skillref = Player_Skills_Script.ConvertStringToSkill(selectedWeapon.SkillRequired);
			skillHeader.Find("Level").GetComponent<Text>().text = "Lv.\n" + Player_Accessor_Script.SkillsScript.SkillDetails.skillList[skillref].maxLevel;

			for (int i = 0; i < tempList.Count; i++)
			{
				Transform cardEntry = abilitySlotParent.GetChild(i);
				cardEntry.name = tempList[i].abilityName;
				cardEntry.GetComponent<CanvasGroup>().interactable = Player_Accessor_Script.SkillsScript.GetSkill(tempList[i].SkillRequired).currLevel >= tempList[i].SkillLevelRequirement;
				cardEntry.Find("BG/Icon").GetComponent<Image>().sprite = tempList[i].skillIcon;
				cardEntry.Find("BG/TextBG/Text").GetComponent<Text>().text = tempList[i].abilityName;

				//Description Stuff
				cardEntry.Find("Description/TextBG/Text").GetComponent<Text>().text = tempList[i].SkillDescription;
				if (tempList[i].ModifiedByStats)
				{
					cardEntry.Find("Description/MightScale/Might").GetComponent<Text>().text = (tempList[i].MightRatio * 100).ToString("n1") + "%\n" + (tempList[i].MightRatio * eqRef.Might).ToString("n1");
					cardEntry.Find("Description/DexterityScale/Dexterity").GetComponent<Text>().text = (tempList[i].DexterityRatio * 100).ToString("n1") + "%\n" + (tempList[i].DexterityRatio * eqRef.Dexterity).ToString("n1");
					cardEntry.Find("Description/IntelligenceScale/Intelligence").GetComponent<Text>().text = (tempList[i].IntelligenceRatio * 100).ToString("n1") + "%\n" + (tempList[i].IntelligenceRatio * eqRef.Intelligence).ToString("n1");
				}
				else
				{
					cardEntry.Find("Description/MightScale/Might").GetComponent<Text>().text = "N/A";
					cardEntry.Find("Description/DexterityScale/Dexterity").GetComponent<Text>().text = "N/A";
					cardEntry.Find("Description/IntelligenceScale/Intelligence").GetComponent<Text>().text = "N/A";
				}
				cardEntry.Find("Description/LevelReq/Text").GetComponent<Text>().text = "Lv.\n" + tempList[i].SkillLevelRequirement;
			}
		}
		else
		{
			for (int j = 0; j < 4; j++)
			{
				abilityParent.GetChild(j).GetComponent<Image>().sprite = defaultIcon;
			}
		}

		if (ind != prevInd)
		{
			selectedWeapon = Player_Accessor_Script.EquipmentScript.FetchItemSlot(ind);
			//for (int i = 0; i < abilitySlotParent.childCount; i++)
			while (abilitySlotParent.childCount > 0)
			{
				DestroyImmediate(abilitySlotParent.GetChild(0).gameObject);
			}

			if (selectedWeapon != null)
			{
				List<Ability> tempList = AbilityLibrary.FetchSkillList(selectedWeapon.Type.ToString());
				for (int i = 0; i < tempList.Count; i++)
				{
					GameObject temp = Instantiate(AbilitySlotPrefab, abilitySlotParent);
				}
			}
			prevInd = ind;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			canvas.alpha = 0;
			canvas.blocksRaycasts = false;
			canvas.interactable = false;
		}

		if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.I_WEAPON_ABILITIES]))
		{
			canvas.alpha = Mathf.Abs(canvas.alpha - 1);
			if (canvas.alpha == 1)
			{
				Interface_Controller.instance.InterfaceEvents["AbilityInterfaceOpen"].Invoke();
			}
			canvas.interactable = !canvas.interactable;
			canvas.blocksRaycasts = !canvas.blocksRaycasts;
		}
	}

	public void SetRefWeapon(int val)
	{
		ind = val;
		ResetRefIndex();
	}

	public void SetRefSkill(int val)
	{
		skillSlot = val;
	}

	public void ResetRefIndex()
	{
		prevInd = -1;
	}

	public void ReplaceSkill(string skill)
	{
		if (skill == "Cleave")
		{
			Interface_Controller.instance.InterfaceEvents["CleaveSelectedPhase"].Invoke();
		}

		if (AbilityLibrary.abilityDictionary[skill].SkillLevelRequirement <= Player_Accessor_Script.SkillsScript.SkillDetails.skillList[AbilityLibrary.abilityDictionary[skill].SkillRequired].maxLevel)
		{
			if (!AbilityLibrary.abilityDictionary[skill].isSelected)
			{
				for (int i = 0; i < 4; i++)
				{
					if (selectedWeapon.SkillSet[i] == null)
					{
						selectedWeapon.SetSkill(i, AbilityLibrary.abilityDictionary[skill]);
						selectedWeapon.SkillSet[i].isSelected = true;
						break;
					}
				}
			}
		}
	}
	public void ReplaceSkill(string skill, int index)
	{
		if (skill == "Cleave")
		{
			Interface_Controller.instance.InterfaceEvents["CleaveSelectedPhase"].Invoke();
		}

		if (AbilityLibrary.abilityDictionary[skill].SkillLevelRequirement <= Player_Accessor_Script.SkillsScript.SkillDetails.skillList[AbilityLibrary.abilityDictionary[skill].SkillRequired].maxLevel)
		{
			if (!AbilityLibrary.abilityDictionary[skill].isSelected || true)
			{
				if (selectedWeapon.SkillSet[index] != null)
				{
					selectedWeapon.SkillSet[index].isSelected = false;
					selectedWeapon.SkillSet[index] = null;
				}
				selectedWeapon.SetSkill(index, AbilityLibrary.abilityDictionary[skill]);
				selectedWeapon.SkillSet[index].isSelected = true;
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					if (selectedWeapon.SkillSet[i] == AbilityLibrary.abilityDictionary[skill])
					{
						selectedWeapon.SkillSet[i] = null;
						break;
					}
				}
				if (selectedWeapon.SkillSet[index] != null)
				{
					selectedWeapon.SkillSet[index].isSelected = false;
				}

				selectedWeapon.SkillSet[index] = AbilityLibrary.abilityDictionary[skill];
				selectedWeapon.SkillSet[index].isSelected = true;
			}
		}
	}

	public void DropAbilityActivation(PointerEventData dropEvent, int abilityIndex)
	{
		ghostIcon.DisableTrack();
		skillSlot = abilityIndex;
		if (dropEvent.pointerPress.transform.parent.name != "ActiveAbilities")
		{
			print(dropEvent.pointerPress.transform.parent.name);
			ReplaceSkill(dropEvent.pointerPress.transform.parent.name, skillSlot);
		}
	}

}
