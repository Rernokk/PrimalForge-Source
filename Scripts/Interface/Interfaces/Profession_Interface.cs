using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Profession_Interface : MonoBehaviour
{
	[SerializeField]
	private GameObject ItemListingPrefab, ProfessionListingPrefab;

	[SerializeField]
	private GameObject reagentCard;

	[SerializeField]
	private Transform ProfessionGroup, ItemListGroup, SelectedItem;
	private EquipmentCrafting_Controller craftingController;
	private string selectedProfession = "";
	private CraftableItem[] selectedItems;
	private CraftableItem currentSelectedItem;
	private CanvasGroup craftingGroup;

	private void Start()
	{
		craftingController = transform.parent.GetComponent<EquipmentCrafting_Controller>();
		for (int i = 0; i < 10; i++)
		{
			GameObject obj = Instantiate(ProfessionListingPrefab, ProfessionGroup.Find("Categories"), false);
			obj.name = Player_Accessor_Script.SkillsScript.GetSkill(i).skillName;
			obj.transform.Find("Title").GetComponent<Text>().text = obj.name;
			Sprite skillSpr = Resources.Load<Sprite>("SkillIcons/" + obj.name);
			if (skillSpr != null)
			{
				obj.transform.Find("Icon").GetComponent<Image>().sprite = skillSpr;
			}
			else
			{
				obj.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("SkillIcons/Unassigned");
			}
		}
		List<Transform> professions = new List<Transform>();
		for (int i = 0; i < ProfessionGroup.Find("Categories").childCount; i++)
		{
			professions.Add(ProfessionGroup.Find("Categories").GetChild(i));
		}
		ProfessionGroup.Find("Categories").DetachChildren();
		professions.Sort((q1, q2) => q1.name.CompareTo(q2.name));
		foreach (Transform t in professions)
		{
			t.parent = ProfessionGroup.Find("Categories");
		}

		craftingGroup = GetComponent<CanvasGroup>();
		PopulateItemList();
	}

	private void Update()
	{
		if (RecipeCrafter.IsUsingInput)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			craftingGroup.alpha = 0;
			craftingGroup.interactable = false;
			craftingGroup.blocksRaycasts = false;
		}

		if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.I_CRAFTING]))
		{
			craftingGroup.alpha = Mathf.Abs(craftingGroup.alpha - 1);
			if (craftingGroup.alpha == 1)
			{
				Objective_Interface.instance.QuestObjectives["OpenCrafting"].Invoke();
			}
			craftingGroup.interactable = !craftingGroup.interactable;
			craftingGroup.blocksRaycasts = !craftingGroup.blocksRaycasts;
			if (craftingGroup.interactable)
			{
				UpdateProfessionInterface();
			}
		}
	}

	public void PopulateItemList()
	{
		ClearItemList();
		CraftableItem[] itemArray = craftingController.FetchItemsForProfession(Player_Skills_Script.ConvertStringToSkill(selectedProfession));
		selectedItems = itemArray;
		if (itemArray != null)
		{
			for (int i = 0; i < itemArray.Length; i++)
			{
				GameObject obj = Instantiate(ItemListingPrefab, ItemListGroup.Find("Categories"), false);
				obj.GetComponent<Crafting_Select_Interface>().Ind = i;
				obj.transform.Find("Name").GetComponent<Text>().text = itemArray[i].ItemName;
				obj.transform.Find("Level").GetComponent<Text>().text = itemArray[i].RequiredProfLevel.ToString();
				obj.transform.Find("Icon").GetComponent<Image>().sprite = itemArray[i].Icon;
			}
		}
	}

	public void ClearItemList()
	{
		if (ItemListGroup.Find("Categories").childCount > 0)
		{
			for (int i = ItemListGroup.Find("Categories").childCount - 1; i >= 0; i--)
			{
				Destroy(ItemListGroup.Find("Categories").GetChild(i).gameObject);
			}
		}
		ResetSelectedItem();
	}

	public void ResetSelectedItem()
	{
		SelectedItem.Find("Background/NameBG/ItemName").GetComponent<Text>().text = "";
		SelectedItem.Find("Background/AttributeBG/Top/ItemValues").GetComponent<Text>().text = "";
	}

	public void SelectItem(int ind)
	{
		CraftableItem item = selectedItems[ind];
		currentSelectedItem = item;
		SelectedItem.Find("Background/NameBG/ItemName").GetComponent<Text>().text = item.ItemName;
		UpdateProfessionInterface();
	}

	public void UpdateProfessionInterface()
	{
		if (currentSelectedItem != null && (currentSelectedItem.MyCraftedItemType == CraftedItemType.WEAPON || currentSelectedItem.MyCraftedItemType == CraftedItemType.ARMOR))
		{
			CraftedEquipment item2 = (currentSelectedItem as CraftedEquipment);
			if (item2 == null)
			{
				return;
			}

			SelectedItem.Find("Background/AttributeBG/Top/ItemValues").GetComponent<Text>().text = item2.dexterityMin + " - " + item2.dexterityMax + "\n" + item2.mightMin + " - " + item2.mightMax + "\n" + item2.intelligenceMin + " - " + item2.intelligenceMax + "\n";
			if (currentSelectedItem.MyCraftedItemType == CraftedItemType.ARMOR)
			{
				SelectedItem.Find("Background/Icon").GetComponent<Image>().sprite = item2.Icon;
			}
			else if (currentSelectedItem.MyCraftedItemType == CraftedItemType.WEAPON)
			{
				SelectedItem.Find("Background/Icon").GetComponent<Image>().sprite = (currentSelectedItem as CraftedWeapon).Icon;
			}

			ClearCraftingReagents();
			Transform reagentTransform = SelectedItem.Find("Background/AttributeBG/Bottom");
			SelectedItem.Find("Background/Actions/CraftButton").GetComponent<Button>().interactable = true;
			foreach (Reagent r in item2.RequiredReagents)
			{
				GameObject cardRef = Instantiate(reagentCard, reagentTransform, false);
				int ownedReagent = Player_Accessor_Script.InventoryScript.GetResourceQuantity(r.ReagentName);
				if (ownedReagent > -1)
				{
					Text nameComp = cardRef.transform.Find("Name").GetComponent<Text>();
					Text quantityComp = cardRef.transform.Find("Quantity").GetComponent<Text>();
					nameComp.text = r.ReagentName;
					quantityComp.text = ownedReagent + "/" + r.ReagentQuantity;
					SelectedItem.Find("Background/Actions/CraftButton").GetComponent<Button>().interactable = true;

					if (ownedReagent < r.ReagentQuantity)
					{
						nameComp.color = Color.red;
						quantityComp.color = Color.red;
						SelectedItem.Find("Background/Actions/CraftButton").GetComponent<Button>().interactable = false;
					}
				}
				else
				{
					Text nameComp = cardRef.transform.Find("Name").GetComponent<Text>();
					Text quantityComp = cardRef.transform.Find("Quantity").GetComponent<Text>();
					nameComp.color = Color.red;
					nameComp.text = "???";
					quantityComp.color = Color.red;
					quantityComp.text = 0 + "/" + r.ReagentQuantity;
					SelectedItem.Find("Background/Actions/CraftButton").GetComponent<Button>().interactable = false;
				}
			}
		}
		else if (currentSelectedItem != null && currentSelectedItem.MyCraftedItemType == CraftedItemType.REAGENT)
		{

			CraftableReagent item2 = (currentSelectedItem as CraftableReagent);
			if (item2 == null)
			{
				return;
			}

			SelectedItem.Find("Background/AttributeBG/Top/ItemValues").GetComponent<Text>().text = "";
			SelectedItem.Find("Background/Icon").GetComponent<Image>().sprite = item2.Icon;
			ClearCraftingReagents();
			Transform reagentTransform = SelectedItem.Find("Background/AttributeBG/Bottom");
			SelectedItem.Find("Background/Actions/CraftButton").GetComponent<Button>().interactable = true;

			foreach (Reagent r in item2.RequiredReagents)
			{
				GameObject cardRef = Instantiate(reagentCard, reagentTransform, false);
				int ownedReagent = Player_Accessor_Script.InventoryScript.GetResourceQuantity(r.ReagentName);
				if (ownedReagent > -1)
				{
					Text nameComp = cardRef.transform.Find("Name").GetComponent<Text>();
					Text quantityComp = cardRef.transform.Find("Quantity").GetComponent<Text>();
					nameComp.text = r.ReagentName;
					quantityComp.text = ownedReagent + "/" + r.ReagentQuantity;
					SelectedItem.Find("Background/Actions/CraftButton").GetComponent<Button>().interactable = true;

					if (ownedReagent < r.ReagentQuantity)
					{
						nameComp.color = Color.red;
						quantityComp.color = Color.red;
						SelectedItem.Find("Background/Actions/CraftButton").GetComponent<Button>().interactable = false;
					}
				}
				else
				{
					print(r.ReagentName);
					Text nameComp = cardRef.transform.Find("Name").GetComponent<Text>();
					Text quantityComp = cardRef.transform.Find("Quantity").GetComponent<Text>();
					nameComp.color = Color.red;
					nameComp.text = "???";
					quantityComp.color = Color.red;
					quantityComp.text = 0 + "/" + r.ReagentQuantity;
					SelectedItem.Find("Background/Actions/CraftButton").GetComponent<Button>().interactable = false;
				}
			}
		}
	}

	public void CraftItem()
	{
		print(currentSelectedItem.ItemName);
		Player_Accessor_Script.InventoryScript.AddItemToInventory(craftingController.GenerateItem(currentSelectedItem));
		UpdateProfessionInterface();
	}

	public void SelectProfession(string prof)
	{
		selectedProfession = prof;
		ClearCraftingReagents();
		PopulateItemList();
	}

	public void DisplayInterface(string prof)
	{
		SelectProfession(prof);
		craftingGroup.alpha = 1;
		craftingGroup.interactable = true;
		craftingGroup.blocksRaycasts = true;
	}

	private void ClearCraftingReagents()
	{
		Transform reagentTransform = SelectedItem.Find("Background/AttributeBG/Bottom");
		for (int i = reagentTransform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(reagentTransform.GetChild(i).gameObject);
		}
	}
}
