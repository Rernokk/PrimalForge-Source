using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RecipeCrafter : MonoBehaviour
{
	private EquipmentCrafting_Controller craftingController;

	[SerializeField]
	private GameObject currentRecipePrefab, perkPrefab;

	[SerializeField]
	private Transform currentRecipeTransform, perkListTransform;

	[SerializeField]
	private Transform recipeCreatorTransform;
	private CraftableItem currItem;
	private Vector3 ListPositionDefault;
	private Reagent_Controller reagentController;
	private InputField[] fields;
	private List<LegendaryPerk> activePerks = new List<LegendaryPerk>();
	private bool displayAltPane = false;

	public static bool IsUsingInput
	{
		get;
		set;
	}

	public CraftableItem CurrentItem
	{
		get
		{
			return currItem;
		}

		set
		{
			currItem = value;
		}
	}

	public Reagent_Controller ReagentController
	{
		get
		{
			return reagentController;
		}
		set
		{
			reagentController = value;
		}
	}

	// Use this for initialization
	private void Start()
	{
		craftingController = GameObject.FindGameObjectWithTag("PrimaryInterface").transform.Find("ItemCrafting").GetComponent<EquipmentCrafting_Controller>();
		Player_Accessor_Script.DetailsScript.IsStunned = true;
		Player_Accessor_Script.EquipmentScript.PlayerHasControl = false;
		ListPositionDefault = currentRecipeTransform.GetChild(0).position;
		reagentController = transform.GetComponentInChildren<Reagent_Controller>();
		ClearTemplate();
		fields = GetComponentsInChildren<InputField>(true);
	}

	private void Update()
	{
		IsUsingInput = false;
		foreach (InputField f in fields)
		{
			if (f.isFocused)
			{
				IsUsingInput = true;
				break;
			}
		}
	}

	public void SaveDesign()
	{
		int ind = craftingController.CraftableItems.IndexOf(currItem);
		if (displayAltPane && currItem != null)
		{
			currItem.MyCraftedItemType = CraftedItemType.REAGENT;
		}

		if (!displayAltPane || (currItem != null && (currItem.MyCraftedItemType == CraftedItemType.WEAPON || currItem.MyCraftedItemType == CraftedItemType.ARMOR)))
		{
			Transform mainItemPane = recipeCreatorTransform.Find("ContentBG/ItemTab");
			if (mainItemPane.Find("Name/Name").GetComponent<InputField>().text == "")
			{
				return;
			}

			CraftedEquipment itemRecipe = null;
			if (currItem == null)
			{
				itemRecipe = new CraftedEquipment();
			}
			else
			{
				if (currItem.MyCraftedItemType == CraftedItemType.WEAPON)
				{
					itemRecipe = currItem as CraftedWeapon;
				}
				else if (currItem.MyCraftedItemType == CraftedItemType.ARMOR)
				{
					itemRecipe = currItem as CraftedArmor;
				}
			}

			Dropdown dropList = mainItemPane.Find("ItemType").GetComponent<Dropdown>();
			if (dropList.GetComponent<UI_WeaponType_Controller>().displayingWeapons)
			{
				itemRecipe.MyCraftedItemType = CraftedItemType.WEAPON;
			}
			else
			{
				itemRecipe.MyCraftedItemType = CraftedItemType.ARMOR;
			}

			if (itemRecipe.MyCraftedItemType == CraftedItemType.ARMOR)
			{
				itemRecipe = new CraftedArmor();
				(itemRecipe as CraftedArmor).tarType = ArmorTypeParse(dropList.options[dropList.value].text);
			}
			else if (itemRecipe.MyCraftedItemType == CraftedItemType.WEAPON)
			{
				itemRecipe = new CraftedWeapon();
				(itemRecipe as CraftedWeapon).tarType = WeaponTypeParse(dropList.options[dropList.value].text);
			}

			itemRecipe.ItemName = mainItemPane.Find("Name/Name").GetComponent<InputField>().text;
			itemRecipe.baseName = mainItemPane.Find("Name/Name").GetComponent<InputField>().text;
			itemRecipe.dexterityMin = int.Parse(mainItemPane.Find("Dexterity/Min").GetComponent<InputField>().text);
			itemRecipe.dexterityMax = int.Parse(mainItemPane.Find("Dexterity/Max").GetComponent<InputField>().text);
			itemRecipe.mightMin = int.Parse(mainItemPane.Find("Might/Min").GetComponent<InputField>().text);
			itemRecipe.mightMax = int.Parse(mainItemPane.Find("Might/Max").GetComponent<InputField>().text);
			itemRecipe.intelligenceMin = int.Parse(mainItemPane.Find("Intelligence/Min").GetComponent<InputField>().text);
			itemRecipe.intelligenceMax = int.Parse(mainItemPane.Find("Intelligence/Max").GetComponent<InputField>().text);
			itemRecipe.minLevelReq = int.Parse(mainItemPane.Find("LevelRange/Min").GetComponent<InputField>().text);
			itemRecipe.maxLevelReq = int.Parse(mainItemPane.Find("LevelRange/Max").GetComponent<InputField>().text);
			itemRecipe.minBonusHealth = int.Parse(mainItemPane.Find("Health/Min").GetComponent<InputField>().text);
			itemRecipe.maxBonusHealth = int.Parse(mainItemPane.Find("Health/Max").GetComponent<InputField>().text);

			dropList = mainItemPane.Find("ProfGroup/ProfessionType").GetComponent<Dropdown>();
			itemRecipe.RequiredProfession = Player_Skills_Script.ConvertStringToSkill(dropList.options[dropList.value].text);
			if (itemRecipe.RequiredProfession == Skills.None)
			{
				print("Failed to find skill for: " + itemRecipe.ItemName);
			}
			itemRecipe.RequiredProfLevel = int.Parse(mainItemPane.Find("ProfGroup/RequiredLevel").GetComponent<InputField>().text);
			itemRecipe.RewardedExp = int.Parse(mainItemPane.Find("ProfGroup/RewardedExp").GetComponent<InputField>().text);
			reagentController.SaveReagentList();
			itemRecipe.RequiredReagents = new List<Reagent>();
			itemRecipe.RequiredReagents = reagentController.reagentList;
			itemRecipe.perks = activePerks;
			currItem = itemRecipe;
		}
		else if (displayAltPane || currItem.MyCraftedItemType == CraftedItemType.REAGENT)
		{
			Transform mainItemPane = recipeCreatorTransform.Find("ReagentBG/ItemTab");
			CraftableReagent tempReagent = new CraftableReagent();

			tempReagent.ItemName = mainItemPane.Find("Name/Name").GetComponent<InputField>().text;
			tempReagent.RewardedExp = int.Parse(mainItemPane.Find("ProfGroup/RewardedExp").GetComponent<InputField>().text);
			tempReagent.RequiredProfLevel = int.Parse(mainItemPane.Find("ProfGroup/RequiredLevel").GetComponent<InputField>().text);

			Dropdown dropList = mainItemPane.Find("ProfGroup/ProfessionType").GetComponent<Dropdown>();
			tempReagent.RequiredProfession = Player_Skills_Script.ConvertStringToSkill(dropList.options[dropList.value].text);

			tempReagent.SpritePath = mainItemPane.Find("SpriteIcon/InputField").GetComponent<InputField>().text;
			reagentController.availableReagents.Add(tempReagent);
			tempReagent.RequiredReagents = reagentController.reagentList;
			currItem = tempReagent;
			reagentController.SaveReagentList();
		}

		if (ind == -1)
		{
			if (currItem.MyCraftedItemType != CraftedItemType.REAGENT)
			{
				craftingController.CraftableItems.Add(currItem);
			}
			else
			{
				if (!craftingController.CraftableReagents.Contains(currItem as CraftableReagent))
				{
					craftingController.CraftableReagents.Add(currItem as CraftableReagent);
				}
			}
		}
		else
		{
			craftingController.CraftableItems[ind] = currItem;
		}
		craftingController.SaveRecipes();
		LoadDesignList();
		ClearTemplate();
	}

	public WeaponType WeaponTypeParse(string type)
	{
		WeaponType val = WeaponType.EMPTY;
		for (int i = 0; i < System.Enum.GetNames(typeof(WeaponType)).Length; i++)
		{
			if (type.ToUpper() == ((WeaponType)i).ToString())
			{
				val = (WeaponType)i;
				break;
			}
		}
		return val;
	}

	public ArmorType ArmorTypeParse(string type)
	{
		ArmorType val = ArmorType.EMPTY;
		for (int i = 0; i < System.Enum.GetNames(typeof(ArmorType)).Length; i++)
		{
			if (type.ToUpper() == ((ArmorType)i).ToString())
			{
				val = (ArmorType)i;
				break;
			}
		}
		return val;
	}

	public void LoadDesignList()
	{
		craftingController.LoadRecipes();
		reagentController.LoadAvailableReagentList();
		for (int i = currentRecipeTransform.GetChild(0).childCount - 1; i >= 0; i--)
		{
			Destroy(currentRecipeTransform.GetChild(0).GetChild(i).gameObject);
		}
		currentRecipeTransform.GetChild(0).GetComponent<Scrollbar_Controller>().ResetBar();
		//string strRef = currentRecipeTransform.parent.Find("Profession/InputField").GetComponent<InputField>().text;
		Dropdown dd = currentRecipeTransform.parent.Find("Profession/ProfessionType").GetComponent<Dropdown>();
		string strRef = dd.options[dd.value].text;
		for (int i = 0; i < craftingController.CraftableItems.Count; i++)
		{
			if (craftingController.CraftableItems[i] != null && craftingController.CraftableItems[i].RequiredProfession == Player_Skills_Script.ConvertStringToSkill(strRef))
			{
				GameObject obj = Instantiate(currentRecipePrefab, currentRecipeTransform.GetChild(0), false);
				obj.transform.GetChild(0).GetComponent<Text>().text = craftingController.CraftableItems[i].ItemName;
				obj.GetComponent<RecipeReference>().itemRef = craftingController.CraftableItems[i];
				obj.GetComponent<RecipeReference>().recipeCrafter = this;
			}
		}

		for (int i = 0; i < craftingController.CraftableReagents.Count; i++)
		{
			if (craftingController.CraftableReagents[i] != null && craftingController.CraftableReagents[i].RequiredProfession == Player_Skills_Script.ConvertStringToSkill(strRef))
			{
				GameObject obj = Instantiate(currentRecipePrefab, currentRecipeTransform.GetChild(0), false);
				obj.transform.GetChild(0).GetComponent<Text>().text = craftingController.CraftableReagents[i].ItemName;
				obj.GetComponent<RecipeReference>().itemRef = craftingController.CraftableReagents[i];
				obj.GetComponent<RecipeReference>().recipeCrafter = this;
			}
		}
	}

	public void ExportCraftingRecipesToCSV()
	{
		List<string> equipmentRecipes = new List<string>();
		equipmentRecipes.Add("ItemName,ItemType,ItemClass,MightMin,MightMax,DexterityMin,DexterityMax,IntelligenceMin,IntelligenceMax,LevelReqMin,LevelReqMax,Profession,ProfessionLevel,ExpReward,Reagents,Perks");
		foreach (CraftableItem i in craftingController.CraftableItems)
		{
			if (i.MyCraftedItemType == CraftedItemType.ARMOR || i.MyCraftedItemType == CraftedItemType.WEAPON)
			{
				string entry = i.ItemName + "," + i.MyCraftedItemType + ",";
				switch (i.MyCraftedItemType)
				{
					case (CraftedItemType.WEAPON):
						entry += (i as CraftedWeapon).tarType.ToString() + ",";
						break;
					case (CraftedItemType.ARMOR):
						entry += (i as CraftedArmor).tarType.ToString() + ",";
						break;
					default:
						entry += ",";
						break;
				}
				entry += (i as CraftedEquipment).mightMin + ",";
				entry += (i as CraftedEquipment).mightMax + ",";
				entry += (i as CraftedEquipment).dexterityMin + ",";
				entry += (i as CraftedEquipment).dexterityMax + ",";
				entry += (i as CraftedEquipment).intelligenceMin + ",";
				entry += (i as CraftedEquipment).intelligenceMax + ",";
				//entry += (i as CraftedEquipment).
				entry += (i as CraftedEquipment).minLevelReq + ",";
				entry += (i as CraftedEquipment).maxLevelReq + ",";
				entry += i.RequiredProfession.ToString() + ",";
				entry += i.RequiredProfLevel.ToString() + ",";
				entry += i.RewardedExp.ToString() + ",";
				foreach (Reagent req in i.RequiredReagents)
				{
					entry += req.ReagentName + "," + req.ReagentQuantity + ",";
				}
				entry += "-,";
				foreach (LegendaryPerk perk in (i as CraftedEquipment).perks)
				{
					entry += perk.ToString() + ",";
				}
				equipmentRecipes.Add(entry);
			}
		}
		File.WriteAllLines("CraftingEquipmentExport.csv", equipmentRecipes.ToArray());

		List<string> reagentRecipes = new List<string>();
		reagentRecipes.Add("ItemName,ItemType,Profession,ProfessionLevel,ExpReward,Reagents");
		foreach (CraftableReagent i in craftingController.CraftableReagents)
		{
			string entry = i.ItemName + "," + i.MyCraftedItemType + ",";
			entry += i.RequiredProfession.ToString() + ",";
			entry += i.RequiredProfLevel.ToString() + ",";
			entry += i.RewardedExp.ToString() + ",";
			foreach (Reagent req in i.RequiredReagents)
			{
				entry += req.ReagentName + "," + req.ReagentQuantity + ",";
			}
			reagentRecipes.Add(entry);
		}
		File.WriteAllLines("CraftingReagentsExport.csv", reagentRecipes.ToArray());
	}

	public void LoadSelectedItem(CraftableItem itemRef)
	{
		if (itemRef != null)
		{
			CraftedEquipment equipRef;
			Transform mainItemPane = recipeCreatorTransform.Find("ContentBG/ItemTab");
			if (itemRef.MyCraftedItemType == CraftedItemType.ARMOR)
			{
				PopulateEquipmentList();
			}
			else if (itemRef.MyCraftedItemType == CraftedItemType.WEAPON)
			{
				PopulateWeaponList();
			}
			else if (itemRef.MyCraftedItemType == CraftedItemType.REAGENT)
			{
				PopulateReagentList();
			}

			if (itemRef.MyCraftedItemType == CraftedItemType.ARMOR || itemRef.MyCraftedItemType == CraftedItemType.WEAPON)
			{
				equipRef = itemRef as CraftedEquipment;
				currItem = itemRef;
				mainItemPane.Find("Name/Name").GetComponent<InputField>().text = equipRef.baseName;
				if (itemRef.MyCraftedItemType == CraftedItemType.ARMOR || itemRef.MyCraftedItemType == CraftedItemType.WEAPON)
				{
					mainItemPane.Find("Dexterity/Min").GetComponent<InputField>().text = equipRef.dexterityMin.ToString();
					mainItemPane.Find("Dexterity/Max").GetComponent<InputField>().text = equipRef.dexterityMax.ToString();
					mainItemPane.Find("Might/Min").GetComponent<InputField>().text = equipRef.mightMin.ToString();
					mainItemPane.Find("Might/Max").GetComponent<InputField>().text = equipRef.mightMax.ToString();
					mainItemPane.Find("Intelligence/Min").GetComponent<InputField>().text = equipRef.intelligenceMin.ToString();
					mainItemPane.Find("Intelligence/Max").GetComponent<InputField>().text = equipRef.intelligenceMax.ToString();
					mainItemPane.Find("LevelRange/Min").GetComponent<InputField>().text = equipRef.minLevelReq.ToString();
					mainItemPane.Find("LevelRange/Max").GetComponent<InputField>().text = equipRef.maxLevelReq.ToString();
					mainItemPane.Find("Health/Min").GetComponent<InputField>().text = equipRef.minBonusHealth.ToString();
					mainItemPane.Find("Health/Max").GetComponent<InputField>().text = equipRef.maxBonusHealth.ToString();

					mainItemPane.Find("Dexterity/Min").GetComponent<InputField>().interactable = true;
					mainItemPane.Find("Dexterity/Max").GetComponent<InputField>().interactable = true;
					mainItemPane.Find("Might/Min").GetComponent<InputField>().interactable = true;
					mainItemPane.Find("Might/Max").GetComponent<InputField>().interactable = true;
					mainItemPane.Find("Intelligence/Min").GetComponent<InputField>().interactable = true;
					mainItemPane.Find("Intelligence/Max").GetComponent<InputField>().interactable = true;
					mainItemPane.Find("LevelRange/Min").GetComponent<InputField>().interactable = true;
					mainItemPane.Find("LevelRange/Max").GetComponent<InputField>().interactable = true;
					mainItemPane.Find("Health/Min").GetComponent<InputField>().interactable = true;
					mainItemPane.Find("Health/Max").GetComponent<InputField>().interactable = true;

					FetchPerkList();

					int val = 0;
					Dropdown tempdd = mainItemPane.Find("ItemType").GetComponent<Dropdown>();
					if (itemRef.MyCraftedItemType == CraftedItemType.WEAPON)
					{
						for (int i = 0; i < System.Enum.GetNames(typeof(WeaponType)).Length; i++)
						{
							if (tempdd.options[i].text.ToLower() == (itemRef as CraftedWeapon).tarType.ToString().ToLower())
							{
								val = i;
								break;
							}
						}
					}
					else if (itemRef.MyCraftedItemType == CraftedItemType.ARMOR)
					{
						for (int i = 0; i < System.Enum.GetNames(typeof(ArmorType)).Length; i++)
						{
							if (tempdd.options[i].text.ToLower() == (itemRef as CraftedArmor).tarType.ToString().ToLower())
							{
								val = i;
								break;
							}
						}
					}
					tempdd.value = val;

					for (int i = 0; i < perkListTransform.childCount; i++)
					{
						Toggle t = perkListTransform.GetChild(i).GetComponentInChildren<Toggle>();
						if (equipRef.perks.Contains(Perks.ConvertTextToPerk(perkListTransform.GetChild(i).GetChild(0).GetComponent<Text>().text)))
						{
							t.isOn = true;
						}
						else
						{
							t.isOn = false;
						}
					}
					print(itemRef.ItemName);
				}
			}
			else if (itemRef.MyCraftedItemType == CraftedItemType.REAGENT)
			{
				mainItemPane = recipeCreatorTransform.Find("ReagentBG/ItemTab");
				mainItemPane.Find("Name/Name").GetComponent<InputField>().text = itemRef.ItemName;
				mainItemPane.Find("SpriteIcon/BG/Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/ResourceIcons/" + itemRef.SpritePath);
				mainItemPane.Find("SpriteIcon/InputField").GetComponent<InputField>().text = itemRef.SpritePath;
				currItem = itemRef;
			}

			Dropdown tempDD = mainItemPane.Find("ProfGroup/ProfessionType").GetComponent<Dropdown>();
			for (int i = 0; i < tempDD.options.Count; i++)
			{
				if (Player_Skills_Script.ConvertStringToSkill(tempDD.options[i].text) == itemRef.RequiredProfession)
				{
					tempDD.value = i;
					break;
				}
			}
			mainItemPane.Find("ProfGroup/RequiredLevel").GetComponent<InputField>().text = itemRef.RequiredProfLevel.ToString();
			mainItemPane.Find("ProfGroup/RewardedExp").GetComponent<InputField>().text = itemRef.RewardedExp.ToString();

			reagentController.LoadReagentList();
		}
	}

	public void DeleteSelectedItem()
	{
		craftingController.CraftableItems.Remove(CurrentItem);
		if (CurrentItem.MyCraftedItemType == CraftedItemType.REAGENT)
		{
			craftingController.CraftableReagents.Remove(CurrentItem as CraftableReagent);
		}
		craftingController.SaveRecipes();
		LoadDesignList();
		ClearTemplate();
	}
	

	public void ClearTemplate()
	{
		Transform equipmentItemPane = recipeCreatorTransform.Find("ContentBG/ItemTab");
		equipmentItemPane.Find("Name/Name").GetComponent<InputField>().text = "";
		equipmentItemPane.Find("Dexterity/Min").GetComponent<InputField>().text = 0.ToString();
		equipmentItemPane.Find("Dexterity/Max").GetComponent<InputField>().text = 0.ToString();
		equipmentItemPane.Find("Might/Min").GetComponent<InputField>().text = 0.ToString();
		equipmentItemPane.Find("Might/Max").GetComponent<InputField>().text = 0.ToString();
		equipmentItemPane.Find("Intelligence/Min").GetComponent<InputField>().text = 0.ToString();
		equipmentItemPane.Find("Intelligence/Max").GetComponent<InputField>().text = 0.ToString();
		equipmentItemPane.Find("LevelRange/Min").GetComponent<InputField>().text = 0.ToString();
		equipmentItemPane.Find("LevelRange/Max").GetComponent<InputField>().text = 0.ToString();
		equipmentItemPane.Find("ItemType").GetComponent<Dropdown>().value = 0;
		equipmentItemPane.Find("ProfGroup/ProfessionType").GetComponent<Dropdown>().value = 0;
		equipmentItemPane.Find("ProfGroup/RequiredLevel").GetComponent<InputField>().text = 0.ToString();
		equipmentItemPane.Find("ProfGroup/RewardedExp").GetComponent<InputField>().text = 0.ToString();

		Transform reagentItemPane = recipeCreatorTransform.Find("ReagentBG/ItemTab");
		reagentItemPane.Find("Name/Name").GetComponent<InputField>().text = "";
		reagentItemPane.Find("ProfGroup/ProfessionType").GetComponent<Dropdown>().value = 0;
		reagentItemPane.Find("ProfGroup/RequiredLevel").GetComponent<InputField>().text = 0.ToString();
		reagentItemPane.Find("ProfGroup/RewardedExp").GetComponent<InputField>().text = 0.ToString();
		reagentItemPane.Find("SpriteIcon/BG/Image").GetComponent<Image>().sprite = null;
		reagentItemPane.Find("SpriteIcon/InputField").GetComponent<InputField>().text = null;

		currItem = null;
		reagentController.ClearReagentList();
		ClearPerkList();
	}

	public void ClearPerkList()
	{
		for (int i = perkListTransform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(perkListTransform.GetChild(i).gameObject);
		}
	}

	public void FetchPerkList()
	{
		Transform mainItemPane = recipeCreatorTransform.Find("ContentBG/ItemTab");
		Dropdown dropList = mainItemPane.Find("ItemType").GetComponent<Dropdown>();
		WeaponType wepType = WeaponTypeParse(dropList.options[dropList.value].text);
		PerkSlot registeredSlot = Perks.ConvertTypeToSlot(wepType);
		if (registeredSlot != PerkSlot.Unassigned)
		{
			ClearPerkList();

			Dictionary<LegendaryPerk, string> dict = Perks.perkDescriptions[registeredSlot];
			foreach (LegendaryPerk perk in dict.Keys)
			{
				GameObject obj = Instantiate(perkPrefab, perkListTransform, false);
				obj.transform.Find("Name").GetComponent<Text>().text = perk.ToString();
				obj.transform.Find("Description").GetComponent<Text>().text = dict[perk].ToString();
			}
		}
	}

	public void TogglePerk(Transform entry)
	{
		if (entry.GetComponentInChildren<Toggle>().isOn)
		{
			LegendaryPerk perk = Perks.ConvertTextToPerk(entry.Find("Name").GetComponent<Text>().text);
			if (!activePerks.Contains(perk))
			{
				activePerks.Add(perk);
			}
			print("Perk Added: " + entry.Find("Name").GetComponent<Text>().text);
		}
		else
		{
			LegendaryPerk perk = Perks.ConvertTextToPerk(entry.Find("Name").GetComponent<Text>().text);
			if (activePerks.Contains(perk))
			{
				activePerks.Remove(perk);
			}
			print("Perk Removed: " + entry.Find("Name").GetComponent<Text>().text);
		}
	}

	public void PopulateEquipmentList()
	{
		UI_WeaponType_Controller dropList = recipeCreatorTransform.Find("ContentBG/ItemTab").Find("ItemType").GetComponent<UI_WeaponType_Controller>();
		dropList.displayingWeapons = false;
		dropList.displayingArmor = true;
		dropList.displayingReagents = false;
		dropList.PopulateItemTypeOptions();
		transform.Find("CurrentRecipe/Panel/HeaderBG/Weapon").GetComponent<Button>().interactable = true;
		transform.Find("CurrentRecipe/Panel/HeaderBG/Equipment").GetComponent<Button>().interactable = false;
		transform.Find("CurrentRecipe/Panel/HeaderBG/Reagent").GetComponent<Button>().interactable = true;
		displayAltPane = true;
		transform.Find("CurrentRecipe/Panel/ReagentBG").gameObject.SetActive(false);
		transform.Find("CurrentRecipe/Panel/ContentBG").gameObject.SetActive(true);
	}

	public void PopulateWeaponList()
	{
		UI_WeaponType_Controller dropList = recipeCreatorTransform.Find("ContentBG/ItemTab").Find("ItemType").GetComponent<UI_WeaponType_Controller>();
		dropList.displayingWeapons = true;
		dropList.displayingArmor = false;
		dropList.displayingReagents = false;
		dropList.PopulateItemTypeOptions();
		transform.Find("CurrentRecipe/Panel/HeaderBG/Weapon").GetComponent<Button>().interactable = false;
		transform.Find("CurrentRecipe/Panel/HeaderBG/Equipment").GetComponent<Button>().interactable = true;
		transform.Find("CurrentRecipe/Panel/HeaderBG/Reagent").GetComponent<Button>().interactable = true;
		displayAltPane = false;
		transform.Find("CurrentRecipe/Panel/ReagentBG").gameObject.SetActive(false);
		transform.Find("CurrentRecipe/Panel/ContentBG").gameObject.SetActive(true);
	}

	public void PopulateReagentList()
	{
		UI_WeaponType_Controller dropList = recipeCreatorTransform.Find("ContentBG/ItemTab").Find("ItemType").GetComponent<UI_WeaponType_Controller>();
		dropList.displayingWeapons = false;
		dropList.displayingArmor = false;
		dropList.displayingReagents = true;
		dropList.PopulateItemTypeOptions();
		transform.Find("CurrentRecipe/Panel/HeaderBG/Weapon").GetComponent<Button>().interactable = true;
		transform.Find("CurrentRecipe/Panel/HeaderBG/Equipment").GetComponent<Button>().interactable = true;
		transform.Find("CurrentRecipe/Panel/HeaderBG/Reagent").GetComponent<Button>().interactable = false;
		displayAltPane = true;
		transform.Find("CurrentRecipe/Panel/ReagentBG").gameObject.SetActive(true);
		transform.Find("CurrentRecipe/Panel/ContentBG").gameObject.SetActive(false);
	}

	public void PopulateConsumableList()
	{

	}

	public void SetSelectedSprite()
	{
		if (displayAltPane)
		{
			transform.Find("CurrentRecipe/Panel/ReagentBG/ItemTab/SpriteIcon/BG/Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/ResourceIcons/" + transform.Find("CurrentRecipe/Panel/ReagentBG/ItemTab/SpriteIcon/InputField").GetComponent<InputField>().text);
		}
	}
}
