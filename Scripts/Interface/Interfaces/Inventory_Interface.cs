using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory_Interface : MonoBehaviour
{
	public static Inventory_Interface instance;
	private Dictionary<string, Sprite> statIcons = new Dictionary<string, Sprite>();
	private Transform itemPane, characterPane;
	private Player_Inventory_Script playerInventory;
	private Player_Equipment_Script playerEquipment;

	[SerializeField]
	private bool displayEquippedWeaponTab = true, displayResistsTab = false;

	[SerializeField]
	private GameObject perkCardEntry, reagentItemCard;

	[SerializeField]
	private GhostIcon_Element ghostIcon;

	private Transform hoveredItemPanel, hoveredReagentItemPanel, comparedItemPanel, currentGearPanel;
	private Sprite emptySlotIcon;

	[SerializeField]
	private Transform resourcePanel, equipmentPanel, statsPanel;

	#region Properties
	public Transform HoveredItemPanel
	{
		get
		{
			return hoveredItemPanel;
		}

		set
		{
			hoveredItemPanel = value;
		}
	}

	public Transform HoveredReagentItemPanel
	{
		get
		{
			return hoveredReagentItemPanel;
		}

		set
		{
			hoveredReagentItemPanel = value;
		}
	}

	public Transform ComparedItemPanel
	{
		get
		{
			return comparedItemPanel;
		}

		set
		{
			comparedItemPanel = value;
		}
	}

	public Dictionary<string, Sprite> StatIcons
	{
		get
		{
			return statIcons;
		}
		set
		{
			statIcons = value;
		}
	}

	#endregion

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

		Interface_Controller.instance.InterfaceEvents.Add("EquipWeapon", new UnityEngine.Events.UnityEvent());
		Interface_Controller.instance.InterfaceEvents.Add("OpenInventory", new UnityEngine.Events.UnityEvent());

		itemPane = transform.Find("Background/Inventory/Item_Pane/ItemSection");
		characterPane = transform.Find("Background/Inventory/Character_Pane");

		playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Inventory_Script>();
		playerEquipment = playerInventory.GetComponent<Player_Equipment_Script>();
		emptySlotIcon = Resources.Load<Sprite>("ItemIcons/EmptySlot");

		for (int i = 0; i < itemPane.childCount; i++)
		{
			for (int j = 0; j < itemPane.GetChild(i).childCount; j++)
			{
				Slot_Tracker scr = itemPane.GetChild(i).GetChild(j).gameObject.GetComponent<Slot_Tracker>();
				scr.Slot = (i * itemPane.GetChild(i).childCount + j);
				scr.transform.name = scr.Slot.ToString();
				//scr.GetComponent<Button>().onClick.AddListener(scr.EquipItemFromSlot);
				scr.PlayerInventory = playerInventory;
				scr.PlayerEquipment = playerEquipment;
			}
		}

		//Stat Icon References
		Sprite[] statIconSprites = Resources.LoadAll<Sprite>("StatIcons");
		foreach (Sprite statSprite in statIconSprites)
		{
			statIcons.Add(statSprite.name, statSprite);
		}
		//Inventory Hovering & Comparison
		hoveredItemPanel = transform.Find("Background/HoverItems/Hovered");
		hoveredReagentItemPanel = transform.Find("Background/HoverItems/HoveredReagent");
		comparedItemPanel = transform.Find("Background/HoverItems/Current");
		hoveredItemPanel.GetComponent<CanvasGroup>().alpha = 0;
		hoveredReagentItemPanel.GetComponent<CanvasGroup>().alpha = 0;
		comparedItemPanel.GetComponent<CanvasGroup>().alpha = 0;

		//Drag events for inventory items
		for (int i = 0; i < itemPane.childCount; i++)
		{
			Transform rowRef = itemPane.GetChild(i);
			for (int j = 0; j < rowRef.childCount; j++)
			{
				EventTrigger trigger = rowRef.GetChild(j).GetComponent<EventTrigger>();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.Drop;
				int temp = new int();
				temp = i * rowRef.childCount + j;
				entry.callback.AddListener((data) => { DropActiveItem((PointerEventData)data, temp); });
				trigger.triggers.Add(entry);

				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.Drag;
				temp = new int();
				temp = i * rowRef.childCount + j;
				entry.callback.AddListener((data) => { DragActiveItem((PointerEventData)data, temp); });
				trigger.triggers.Add(entry);
			}
		}

		currentGearPanel = characterPane.Find("Equipment/Gear_Column");
		for (int i = 1; i < 5; i++)
		{
			Transform section = currentGearPanel.GetChild(i);
			EventTrigger trigger = section.GetComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.Drop;
			entry.callback.AddListener((data) => { EquipDraggedItem((PointerEventData)data, section.name); });
			trigger.triggers.Add(entry);
		}

		currentGearPanel.GetChild(0).Find("Weapons").GetComponent<Button>().interactable = !displayEquippedWeaponTab;
		currentGearPanel.GetChild(0).Find("Armor").GetComponent<Button>().interactable = displayEquippedWeaponTab;

		//Reagent Cards
		for (int i = 0; i < EquipmentCrafting_Controller.instance.CraftableReagents.Count; i++)
		{
			GameObject reagentRef = Instantiate(reagentItemCard, resourcePanel.transform, false);
			if (playerInventory.GetResourceQuantity(EquipmentCrafting_Controller.instance.CraftableReagents[i].ItemName) > -1)
			{
				reagentRef.GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/ResourceIcons/" + EquipmentCrafting_Controller.instance.CraftableReagents[i].SpritePath);
			}
			else
			{
				reagentRef.GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/LockedSlot");
			}
			reagentRef.name = EquipmentCrafting_Controller.instance.CraftableReagents[i].ItemName;
			reagentRef.GetComponent<Slot_Tracker>().Resource = true;
		}

		GetComponent<CanvasGroup>().alpha = 0;
	}

	private void Update()
	{
		if (RecipeCrafter.IsUsingInput)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			CanvasGroup group = GetComponent<CanvasGroup>();
			group.alpha = 0;
			group.blocksRaycasts = false;
			group.interactable = false;
		}

		if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.I_INVENTORY]))
		{
			CanvasGroup group = GetComponent<CanvasGroup>();
			group.alpha = Mathf.Abs(group.alpha - 1);
			if (group.alpha == 1)
			{
				Interface_Controller.instance.InterfaceEvents["OpenInventory"].Invoke();
			}
			group.interactable = group.blocksRaycasts = !group.blocksRaycasts;
			UpdateResourceListQuantities();
		}


		for (int i = 0; i < itemPane.childCount; i++)
		{
			for (int j = 0; j < itemPane.GetChild(i).childCount; j++)
			{
				Item tempItem = playerInventory.FetchItemInSlot(i * itemPane.GetChild(i).childCount + j);
				if (tempItem != null)
				{
					itemPane.GetChild(i).GetChild(j).GetComponent<Image>().sprite = tempItem.Icon;
				}
				else
				{
					itemPane.GetChild(i).GetChild(j).GetComponent<Image>().sprite = emptySlotIcon;
				}
			}
		}

		//Connecting Equipped Items
		Transform equippedRoot = characterPane.Find("Equipment/Gear_Column");
		if (displayEquippedWeaponTab)
		{
			for (int i = 1; i < 5; i++)
			{
				Item myItem = Player_Accessor_Script.EquipmentScript.FetchItemSlot(i - 1);
				if (myItem != null)
				{
					equippedRoot.GetChild(i).GetChild(0).GetComponent<Image>().sprite = myItem.Icon;
					equippedRoot.GetChild(i).GetChild(1).GetComponent<Text>().text = myItem.ItemName;
				}
				else
				{
					equippedRoot.GetChild(i).GetChild(0).GetComponent<Image>().sprite = emptySlotIcon;
					equippedRoot.GetChild(i).GetChild(1).GetComponent<Text>().text = "Empty";
				}
			}
		}
		else
		{
			if (Player_Accessor_Script.EquipmentScript.Helmet != null)
			{
				equippedRoot.GetChild(1).GetChild(0).GetComponent<Image>().sprite = Player_Accessor_Script.EquipmentScript.Helmet.Icon;
				equippedRoot.GetChild(1).GetChild(1).GetComponent<Text>().text = Player_Accessor_Script.EquipmentScript.Helmet.ItemName;
			}
			else
			{
				equippedRoot.GetChild(1).GetChild(0).GetComponent<Image>().sprite = emptySlotIcon;
				equippedRoot.GetChild(1).GetChild(1).GetComponent<Text>().text = "Empty";
			}

			if (Player_Accessor_Script.EquipmentScript.Chestplate != null)
			{
				equippedRoot.GetChild(2).GetChild(0).GetComponent<Image>().sprite = Player_Accessor_Script.EquipmentScript.Chestplate.Icon;
				equippedRoot.GetChild(2).GetChild(1).GetComponent<Text>().text = Player_Accessor_Script.EquipmentScript.Chestplate.ItemName;
			}
			else
			{
				equippedRoot.GetChild(2).GetChild(0).GetComponent<Image>().sprite = emptySlotIcon;
				equippedRoot.GetChild(2).GetChild(1).GetComponent<Text>().text = "Empty";
			}

			if (Player_Accessor_Script.EquipmentScript.LegArmor != null)
			{
				equippedRoot.GetChild(3).GetChild(0).GetComponent<Image>().sprite = Player_Accessor_Script.EquipmentScript.LegArmor.Icon;
				equippedRoot.GetChild(3).GetChild(1).GetComponent<Text>().text = Player_Accessor_Script.EquipmentScript.LegArmor.ItemName;
			}
			else
			{
				equippedRoot.GetChild(3).GetChild(0).GetComponent<Image>().sprite = emptySlotIcon;
				equippedRoot.GetChild(3).GetChild(1).GetComponent<Text>().text = "Empty";
			}

			if (Player_Accessor_Script.EquipmentScript.EquippedTool != null)
			{
				equippedRoot.GetChild(4).GetChild(0).GetComponent<Image>().sprite = Player_Accessor_Script.EquipmentScript.EquippedTool.Icon;
				equippedRoot.GetChild(4).GetChild(1).GetComponent<Text>().text = Player_Accessor_Script.EquipmentScript.EquippedTool.ItemName;
			}
			else
			{
				equippedRoot.GetChild(4).GetChild(0).GetComponent<Image>().sprite = emptySlotIcon;
				equippedRoot.GetChild(4).GetChild(1).GetComponent<Text>().text = "Empty";
			}
		}

		//Calculating Stats for Display
		if (displayResistsTab)
		{
			Transform resistPanel = statsPanel.Find("Resists");
			//resistPanel.Find("MightText").GetComponent<Text>().text = "Might: " + Player_Accessor_Script.EquipmentScript.Might;
			//resistPanel.Find("DexterityText").GetComponent<Text>().text = "Dexterity: " + Player_Accessor_Script.EquipmentScript.Dexterity;
			//resistPanel.Find("IntelligenceText").GetComponent<Text>().text = "Intelligence: " + Player_Accessor_Script.EquipmentScript.Intelligence;
			//resistPanel.Find("ArmorText").GetComponent<Text>().text = "Armor: " + Player_Accessor_Script.DetailsScript.ElementalResists[ElementalResistances.PHYSICAL];
			for (int i = 0; i < 4; i++)
			{
				resistPanel.GetChild(i).Find(((ElementalResistances)(i * 2)).ToString() + "Text").GetComponent<Text>().text = Mathf.RoundToInt(Player_Accessor_Script.DetailsScript.ElementalResists[((ElementalResistances)(i * 2))]).ToString();
				resistPanel.GetChild(i).Find(((ElementalResistances)(i * 2)).ToString() + "Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("StatIcons/" + ((ElementalResistances)(i * 2)).ToString());
				resistPanel.GetChild(i).Find(((ElementalResistances)(1 + i * 2)).ToString() + "Text").GetComponent<Text>().text = Mathf.RoundToInt(Player_Accessor_Script.DetailsScript.ElementalResists[((ElementalResistances)(1 + i * 2))]).ToString();
				resistPanel.GetChild(i).Find(((ElementalResistances)(1 + i * 2)).ToString() + "Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("StatIcons/" + ((ElementalResistances)(i * 2 + 1)).ToString());
			}
		}
		else
		{
			statsPanel.Find("Stats/MightText").GetComponent<Text>().text = "Might: " + Player_Accessor_Script.EquipmentScript.Might;
			statsPanel.Find("Stats/DexterityText").GetComponent<Text>().text = "Dexterity: " + Player_Accessor_Script.EquipmentScript.Dexterity;
			statsPanel.Find("Stats/IntelligenceText").GetComponent<Text>().text = "Intelligence: " + Player_Accessor_Script.EquipmentScript.Intelligence;
			statsPanel.Find("Stats/ArmorText").GetComponent<Text>().text = "Armor: " + Player_Accessor_Script.DetailsScript.ElementalResists[ElementalResistances.PHYSICAL];
		}

		//Populating active perks
		Transform perkRoot = characterPane.Find("Attributes/Perks/Stats");
		for (int i = perkRoot.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(perkRoot.GetChild(i).gameObject);
		}

		for (int i = 0; i < playerEquipment.ActivePerks.Count; i++)
		{
			GameObject entry = Instantiate(perkCardEntry, perkRoot, false);
			entry.GetComponent<Text>().text = playerEquipment.ActivePerks[i].ToString().Replace('_', ' ');
		}
	}

	public void PopulateHoverData(Item refItem)
	{
		if (refItem == null)
		{
			return;
		}

		if (refItem.MyItemType == ItemType.WEAPON)
		{
			Weapon wepRef = (refItem as Weapon);
			Transform header = HoveredItemPanel.Find("Header");
			if (wepRef == null)
			{
				return;
			}

			header.Find("Item_Sprite").GetComponent<Image>().sprite = wepRef.Icon;
			header.Find("Item_Name").GetComponent<Text>().text = wepRef.ItemName;
			header.Find("Item_Level").GetComponent<Text>().text = "";
			header.Find("Required_Level").GetComponent<Text>().text = wepRef.LevelRequirement.ToString();
			header.Find("Skill_Sprite").GetComponent<Image>().sprite = Resources.Load<Sprite>("SkillIcons/" + wepRef.SkillRequired);

			Transform stats = HoveredItemPanel.Find("Stats");
			Transform mightSection = stats.Find("Column").GetChild(0);
			mightSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[mightSection.name];
			mightSection.GetChild(1).GetComponent<Text>().text = wepRef.Might.ToString();


			Transform dexSection = stats.Find("Column").GetChild(1);
			dexSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[dexSection.name];
			dexSection.GetChild(1).GetComponent<Text>().text = wepRef.Dexterity.ToString();

			Transform intSection = stats.Find("Column").GetChild(2);
			intSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			intSection.GetChild(1).GetComponent<Text>().text = wepRef.Intelligence.ToString();

			Transform armorSection = stats.Find("Column").GetChild(3);
			armorSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			armorSection.GetChild(1).GetComponent<Text>().text = 0.ToString();

			//Resistances:
			//Fire, Ice
			//Explosive, Shock
			//Poison, Void
			//Arcane, Spectral

			Transform leftResistColumn = stats.Find("Column1");
			Transform rightResistColumn = stats.Find("Column2");

			for (int i = 0; i < 4; i++)
			{
				leftResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[leftResistColumn.GetChild(i).name];
				rightResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[rightResistColumn.GetChild(i).name];
				leftResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
				rightResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
			}
		}
		else if (Player_Equipment_Script.ArmorItemTypes.Contains(refItem.MyItemType))
		{
			Armor armorRef = (refItem as Armor);
			Transform header = HoveredItemPanel.Find("Header");
			header.Find("Item_Sprite").GetComponent<Image>().sprite = armorRef.Icon;
			header.Find("Item_Name").GetComponent<Text>().text = armorRef.ItemName;
			header.Find("Item_Level").GetComponent<Text>().text = "";
			header.Find("Required_Level").GetComponent<Text>().text = armorRef.LevelRequirement.ToString();
			header.Find("Skill_Sprite").GetComponent<Image>().sprite = Resources.Load<Sprite>("SkillIcons/" + armorRef.SkillRequired);

			Transform stats = HoveredItemPanel.Find("Stats");
			Transform mightSection = stats.Find("Column").GetChild(0);
			mightSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[mightSection.name];
			mightSection.GetChild(1).GetComponent<Text>().text = armorRef.Might.ToString();


			Transform dexSection = stats.Find("Column").GetChild(1);
			dexSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[dexSection.name];
			dexSection.GetChild(1).GetComponent<Text>().text = armorRef.Dexterity.ToString();

			Transform intSection = stats.Find("Column").GetChild(2);
			intSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			intSection.GetChild(1).GetComponent<Text>().text = armorRef.Intelligence.ToString();

			Transform armorSection = stats.Find("Column").GetChild(3);
			armorSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			armorSection.GetChild(1).GetComponent<Text>().text = 0.ToString();

			Transform leftResistColumn = stats.Find("Column1");
			Transform rightResistColumn = stats.Find("Column2");

			for (int i = 0; i < 4; i++)
			{
				leftResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[leftResistColumn.GetChild(i).name];
				rightResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[rightResistColumn.GetChild(i).name];
				leftResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
				rightResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
			}
		}
		else if (refItem.MyItemType == ItemType.TOOL)
		{
			print("Hit Tool population");
			Tool toolRef = (refItem as Tool);
			Transform header = HoveredItemPanel.Find("Header");
			header.Find("Item_Sprite").GetComponent<Image>().sprite = toolRef.Icon;
			header.Find("Item_Name").GetComponent<Text>().text = toolRef.ItemName;
			header.Find("Item_Level").GetComponent<Text>().text = "";
			header.Find("Required_Level").GetComponent<Text>().text = toolRef.LevelReq.ToString();
			header.Find("Skill_Sprite").GetComponent<Image>().sprite = Resources.Load<Sprite>("SkillIcons/" + toolRef.RequiredSkill.ToString());

			Transform stats = HoveredItemPanel.Find("Stats");
			Transform mightSection = stats.Find("Column").GetChild(0);
			mightSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[mightSection.name];
			mightSection.GetChild(1).GetComponent<Text>().text = toolRef.BonusMight.ToString();


			Transform dexSection = stats.Find("Column").GetChild(1);
			dexSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[dexSection.name];
			dexSection.GetChild(1).GetComponent<Text>().text = toolRef.BonusDexterity.ToString();

			Transform intSection = stats.Find("Column").GetChild(2);
			intSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			intSection.GetChild(1).GetComponent<Text>().text = toolRef.BonusIntelligence.ToString();

			Transform armorSection = stats.Find("Column").GetChild(3);
			armorSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			armorSection.GetChild(1).GetComponent<Text>().text = toolRef.BonusArmor.ToString();
			
			//Resists
			Transform leftResistColumn = stats.Find("Column1");
			Transform rightResistColumn = stats.Find("Column2");

			for (int i = 0; i < 4; i++)
			{
				leftResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[leftResistColumn.GetChild(i).name];
				rightResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[rightResistColumn.GetChild(i).name];
				leftResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
				rightResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
			}
		}
		else if (refItem.MyItemType == ItemType.RESOURCE)
		{

		}
		else if (refItem.MyItemType != ItemType.UNASSIGNED)
		{

		}
		else
		{
			print("Item unassigned item type, somehow.");
		}
	}

	//Resource Hover Data
	public void PopulateHoverData(string resourceName)
	{
		Reagent reagentRef = playerInventory.GetResourceReference(resourceName);
		if (reagentRef == null || reagentRef.ReagentQuantity < 0)
		{
			return;
		}
		Inventory_Interface.instance.HoveredItemPanel.GetComponent<CanvasGroup>().alpha = 1;

		hoveredReagentItemPanel.GetComponent<CanvasGroup>().alpha = 1;
		comparedItemPanel.GetComponent<CanvasGroup>().alpha = 0;
		hoveredItemPanel.GetComponent<CanvasGroup>().alpha = 0;

		Transform header = hoveredReagentItemPanel.Find("Header");
		header.Find("Item_Sprite").GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/ResourceIcons/" + reagentRef.ReagentSprite);
		header.Find("Item_Name").GetComponent<Text>().text = reagentRef.ReagentName;
		header.Find("Required_Level").GetComponent<Text>().text = "";
		//header.Find("Skill_Sprite").GetComponent<Image>().sprite = Resources.Load<Sprite>("SkillIcons/" + reagentRef.SkillRequired);
	}

	public void PopulateCompareData(Item refItem)
	{
		if (refItem.MyItemType == ItemType.WEAPON)
		{
			Weapon wepRef = (refItem as Weapon);
			Transform header = ComparedItemPanel.Find("Header");
			header.Find("Item_Sprite").GetComponent<Image>().sprite = wepRef.Icon;
			header.Find("Item_Name").GetComponent<Text>().text = wepRef.ItemName;
			header.Find("Item_Level").GetComponent<Text>().text = "";
			header.Find("Required_Level").GetComponent<Text>().text = wepRef.LevelRequirement.ToString();
			header.Find("Skill_Sprite").GetComponent<Image>().sprite = Resources.Load<Sprite>("SkillIcons/" + wepRef.SkillRequired);

			Transform stats = ComparedItemPanel.Find("Stats");
			Transform mightSection = stats.Find("Column").GetChild(0);
			mightSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[mightSection.name];
			mightSection.GetChild(1).GetComponent<Text>().text = wepRef.Might.ToString();


			Transform dexSection = stats.Find("Column").GetChild(1);
			dexSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[dexSection.name];
			dexSection.GetChild(1).GetComponent<Text>().text = wepRef.Dexterity.ToString();

			Transform intSection = stats.Find("Column").GetChild(2);
			intSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			intSection.GetChild(1).GetComponent<Text>().text = wepRef.Intelligence.ToString();

			Transform armorSection = stats.Find("Column").GetChild(3);
			armorSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			armorSection.GetChild(1).GetComponent<Text>().text = 0.ToString();


			//Resists
			Transform leftResistColumn = stats.Find("Column1");
			Transform rightResistColumn = stats.Find("Column2");

			for (int i = 0; i < 4; i++)
			{
				leftResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[leftResistColumn.GetChild(i).name];
				rightResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[rightResistColumn.GetChild(i).name];
				leftResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
				rightResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
			}
		}
		else if (refItem.MyItemType == ItemType.CHESTPLATE || refItem.MyItemType == ItemType.PLATELEGS || refItem.MyItemType == ItemType.HELMET || refItem.MyItemType == ItemType.ROBE || refItem.MyItemType == ItemType.TROUSERS || refItem.MyItemType == ItemType.HAT || refItem.MyItemType == ItemType.CHAPS || refItem.MyItemType == ItemType.CHESTGUARD || refItem.MyItemType == ItemType.COIF)
		{
			Armor armorRef = (refItem as Armor);
			Transform header = ComparedItemPanel.Find("Header");
			header.Find("Item_Sprite").GetComponent<Image>().sprite = armorRef.Icon;
			header.Find("Item_Name").GetComponent<Text>().text = armorRef.ItemName;
			header.Find("Item_Level").GetComponent<Text>().text = "";
			header.Find("Required_Level").GetComponent<Text>().text = armorRef.LevelRequirement.ToString();
			header.Find("Skill_Sprite").GetComponent<Image>().sprite = Resources.Load<Sprite>("SkillIcons/" + armorRef.SkillRequired);

			Transform stats = ComparedItemPanel.Find("Stats");
			Transform mightSection = stats.Find("Column").GetChild(0);
			mightSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[mightSection.name];
			mightSection.GetChild(1).GetComponent<Text>().text = armorRef.Might.ToString();


			Transform dexSection = stats.Find("Column").GetChild(1);
			dexSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[dexSection.name];
			dexSection.GetChild(1).GetComponent<Text>().text = armorRef.Dexterity.ToString();

			Transform intSection = stats.Find("Column").GetChild(2);
			intSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			intSection.GetChild(1).GetComponent<Text>().text = armorRef.Intelligence.ToString();

			Transform armorSection = stats.Find("Column").GetChild(3);
			armorSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			armorSection.GetChild(1).GetComponent<Text>().text = 0.ToString();

			//Resists
			Transform leftResistColumn = stats.Find("Column1");
			Transform rightResistColumn = stats.Find("Column2");

			for (int i = 0; i < 4; i++)
			{
				leftResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[leftResistColumn.GetChild(i).name];
				rightResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[rightResistColumn.GetChild(i).name];
				leftResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
				rightResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
			}
		}
		else if (refItem.MyItemType == ItemType.TOOL)
		{
			Tool toolRef = (refItem as Tool);
			Transform header = ComparedItemPanel.Find("Header");
			header.Find("Item_Sprite").GetComponent<Image>().sprite = toolRef.Icon;
			header.Find("Item_Name").GetComponent<Text>().text = toolRef.ItemName;
			header.Find("Item_Level").GetComponent<Text>().text = "";
			header.Find("Required_Level").GetComponent<Text>().text = toolRef.LevelReq.ToString();
			header.Find("Skill_Sprite").GetComponent<Image>().sprite = Resources.Load<Sprite>("SkillIcons/" + toolRef.RequiredSkill.ToString());

			Transform stats = ComparedItemPanel.Find("Stats");
			Transform mightSection = stats.Find("Column").GetChild(0);
			mightSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[mightSection.name];
			mightSection.GetChild(1).GetComponent<Text>().text = toolRef.BonusMight.ToString();


			Transform dexSection = stats.Find("Column").GetChild(1);
			dexSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[dexSection.name];
			dexSection.GetChild(1).GetComponent<Text>().text = toolRef.BonusDexterity.ToString();

			Transform intSection = stats.Find("Column").GetChild(2);
			intSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			intSection.GetChild(1).GetComponent<Text>().text = toolRef.BonusIntelligence.ToString();

			Transform armorSection = stats.Find("Column").GetChild(3);
			armorSection.GetChild(0).GetComponent<Image>().sprite = StatIcons[intSection.name];
			armorSection.GetChild(1).GetComponent<Text>().text = toolRef.BonusArmor.ToString();

			//Resists
			Transform leftResistColumn = stats.Find("Column1");
			Transform rightResistColumn = stats.Find("Column2");

			for (int i = 0; i < 4; i++)
			{
				leftResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[leftResistColumn.GetChild(i).name];
				rightResistColumn.GetChild(i).GetChild(0).GetComponent<Image>().sprite = StatIcons[rightResistColumn.GetChild(i).name];
				leftResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
				rightResistColumn.GetChild(i).GetChild(1).GetComponent<Text>().text = 0.ToString();
			}
		}
		else if (refItem.MyItemType == ItemType.RESOURCE)
		{

		}
		else if (refItem.MyItemType != ItemType.UNASSIGNED)
		{

		}
		else
		{
			print("Item unassigned item type, somehow.");
		}
	}

	public void SwapEquipmentDisplay()
	{
		displayEquippedWeaponTab = !displayEquippedWeaponTab;
		currentGearPanel.GetChild(0).Find("Weapons").GetComponent<Button>().interactable = !displayEquippedWeaponTab;
		currentGearPanel.GetChild(0).Find("Armor").GetComponent<Button>().interactable = displayEquippedWeaponTab;
	}

	private void DropActiveItem(PointerEventData dropEvent, int inventorySlot)
	{
		Item dragItem = playerInventory.PullItemFromInventory("Weapon", int.Parse(dropEvent.pointerDrag.transform.name));
		Item dropItem = playerInventory.PullItemFromInventory("Weapon", inventorySlot);
		playerInventory.ItemInventory[inventorySlot] = dragItem;
		playerInventory.ItemInventory[int.Parse(dropEvent.pointerDrag.transform.name)] = dropItem;
		PopulateHoverData(dragItem);
		playerInventory.SaveInventory();
	}

	public void DragActiveItem(PointerEventData dragEvent, int inventorySlot)
	{
		Item fetchedItem = playerInventory.FetchItemInSlot(inventorySlot);
		if (fetchedItem == null)
		{
			return;
		}

		if (fetchedItem != null)
		{
			ghostIcon.ActivateTrack();
			ghostIcon.GetComponent<Image>().sprite = fetchedItem.Icon;
		}
		else
		{
			print("Slot null: " + inventorySlot);
		}
	}

	public void EquipDraggedItem(PointerEventData dropEvent, string transformName)
	{
		switch (transformName)
		{
			case ("Element (1)"):
				Item refItem = playerInventory.FetchItemInSlot(int.Parse(dropEvent.pointerDrag.transform.name));

				if (refItem == null)
				{
					return;
				}
				print(refItem.ItemName);
				if (refItem.ItemName == "Training Sword")
				{
					Interface_Controller.instance.InterfaceEvents["EquipWeapon"].Invoke();
				}

				if (displayEquippedWeaponTab)
				{
					if (refItem.MyItemType == ItemType.WEAPON)
					{
						playerEquipment.EquipItem(int.Parse(dropEvent.pointerDrag.transform.name), 0);
					}
				}
				else
				{
					if (refItem.MyItemType == ItemType.HELMET || refItem.MyItemType == ItemType.HAT || refItem.MyItemType == ItemType.COIF)
					{
						playerEquipment.EquipItem(int.Parse(dropEvent.pointerDrag.transform.name), 0);
					}
				}
				break;
			case ("Element (2)"):
				refItem = playerInventory.FetchItemInSlot(int.Parse(dropEvent.pointerDrag.transform.name));
				if (refItem == null)
				{
					return;
				}

				if (displayEquippedWeaponTab)
				{
					if (refItem.MyItemType == ItemType.WEAPON)
					{
						playerEquipment.EquipItem(int.Parse(dropEvent.pointerDrag.transform.name), 1);
					}
				}
				else
				{
					if (refItem.MyItemType == ItemType.CHESTPLATE || refItem.MyItemType == ItemType.CHESTGUARD || refItem.MyItemType == ItemType.ROBE)
					{
						playerEquipment.EquipItem(int.Parse(dropEvent.pointerDrag.transform.name), 1);
					}
				}
				break;
			case ("Element (3)"):
				refItem = playerInventory.FetchItemInSlot(int.Parse(dropEvent.pointerDrag.transform.name));
				if (refItem == null)
				{
					return;
				}

				if (displayEquippedWeaponTab)
				{
					if (refItem.MyItemType == ItemType.WEAPON)
					{
						playerEquipment.EquipItem(int.Parse(dropEvent.pointerDrag.transform.name), 2);
					}
				}
				else
				{
					if (refItem.MyItemType == ItemType.PLATELEGS || refItem.MyItemType == ItemType.TROUSERS || refItem.MyItemType == ItemType.CHAPS)
					{
						playerEquipment.EquipItem(int.Parse(dropEvent.pointerDrag.transform.name), 2);
					}
				}
				break;
			case ("Element (4)"):
				refItem = playerInventory.FetchItemInSlot(int.Parse(dropEvent.pointerDrag.transform.name));
				if (refItem == null)
				{
					return;
				}

				if (displayEquippedWeaponTab)
				{
					if (refItem.MyItemType == ItemType.WEAPON)
					{
						playerEquipment.EquipItem(int.Parse(dropEvent.pointerDrag.transform.name), 3);
					}
				}
				else
				{
					if (refItem.MyItemType == ItemType.TOOL)
					{
						playerEquipment.EquipItem(int.Parse(dropEvent.pointerDrag.transform.name), 3);
						if ((refItem as Tool).ToolType == ToolType.HATCHET)
						{
							Objective_Interface.instance.QuestObjectives["EquipHatchet"].Invoke();
						}
					}
				}
				break;
			default:
				break;
		}
	}

	public void DisplayEquipmentList()
	{
		CanvasGroup resourceGroup = resourcePanel.GetComponent<CanvasGroup>();
		CanvasGroup equipmentGroup = equipmentPanel.GetComponent<CanvasGroup>();
		resourceGroup.alpha = 0;
		resourceGroup.interactable = false;
		resourceGroup.blocksRaycasts = false;
		equipmentGroup.alpha = 1;
		equipmentGroup.interactable = true;
		equipmentGroup.blocksRaycasts = true;
	}

	public void DisplayResourceList()
	{
		CanvasGroup resourceGroup = resourcePanel.GetComponent<CanvasGroup>();
		CanvasGroup equipmentGroup = equipmentPanel.GetComponent<CanvasGroup>();
		equipmentGroup.alpha = 0;
		equipmentGroup.interactable = false;
		equipmentGroup.blocksRaycasts = false;
		resourceGroup.alpha = 1;
		resourceGroup.interactable = true;
		resourceGroup.blocksRaycasts = true;
		UpdateResourceListQuantities();
	}

	public void ToggleDisplayResists()
	{
		displayResistsTab = !displayResistsTab;
		statsPanel.Find("Stats").gameObject.SetActive(!displayResistsTab);
		statsPanel.Find("Resists").gameObject.SetActive(displayResistsTab);
	}

	public void UpdateResourceListQuantities()
	{
		for (int i = 0; i < resourcePanel.childCount; i++)
		{
			Reagent regRef = playerInventory.GetResourceReference(resourcePanel.GetChild(i).name);
			if (regRef != null && regRef.ReagentQuantity > -1)
			{
				resourcePanel.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemIcons/ResourceIcons/" + regRef.ReagentSprite);
				resourcePanel.GetChild(i).GetComponentInChildren<Text>().text = playerInventory.GetResourceQuantity(resourcePanel.GetChild(i).name).ToString();
			}
			else
			{
				resourcePanel.GetChild(i).GetComponentInChildren<Text>().text = "";
			}
		}
	}
}
