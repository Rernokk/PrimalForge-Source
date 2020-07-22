using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Equipment_Script : MonoBehaviour
{
	[FoldoutGroup("Weapons")]
	[FoldoutGroup("Weapons", false)]
	[SerializeField]
	private Weapon firstSlot, secondSlot, thirdSlot, fourthSlot;
	[FoldoutGroup("Weapons", false)] [SerializeField] private Weapon equippedWeapon;
	[FoldoutGroup("Misc Equipment", false)] [SerializeField] private Tool equippedTool;
	[FoldoutGroup("Armor Equipment", false)]
	[FoldoutGroup("Armor Equipment/Helmet", false)]
	[SerializeField]
	private Armor helmet;
	[FoldoutGroup("Armor Equipment/Chest", false)] [SerializeField] private Armor chest;
	[FoldoutGroup("Armor Equipment/Legs", false)] [SerializeField] private Armor legs;
	[FoldoutGroup("Attributes", false)] private int might = 1, dexterity = 1, intelligence = 1;

	[HideInInspector] public AbilityLibrary aLib;
	[HideInInspector] public Abilities_Interface abilities_Interface;
	private int currentWeaponSlot = 0;

	private Player_Inventory_Script inventory;
	private bool playerHasControl = true;
	private float attackSpeed = 1;
	private float attackCD = 0;
	private EquipmentCrafting_Controller equipmentController;
	private List<LegendaryPerk> activeLegendaryPerks;
	private Transform vfxTransform;

	public Animator animController;

	[SerializeField]
	private bool[] weaponSlotsUnlocked = new bool[4];

	public static List<ItemType> ArmorItemTypes;

	[SerializeField]
	List<GameObject> weaponLinkModel = new List<GameObject>();

	#region Properties
	public bool[] WeaponSlotUnlocked
	{
		get
		{
			return weaponSlotsUnlocked;
		}

		set
		{
			weaponSlotsUnlocked = value;
		}
	}

	public int CurrentWeaponSlot
	{
		get
		{
			return currentWeaponSlot;
		}
		set
		{
			currentWeaponSlot = value;
			switch (currentWeaponSlot)
			{
				case (0):
					equippedWeapon = firstSlot;
					break;
				case (1):
					equippedWeapon = secondSlot;
					break;
				case (2):
					equippedWeapon = thirdSlot;
					break;
				case (3):
					equippedWeapon = fourthSlot;
					break;
			}
			foreach (GameObject weapon in weaponLinkModel)
			{
				weapon.SetActive(false);
				if (equippedWeapon != null && weapon.name == equippedWeapon.Type.ToString())
				{
					weapon.SetActive(true);
				}
			}
		}
	}
	public Weapon FirstSlot
	{
		get
		{
			return firstSlot;
		}

		set
		{
			firstSlot = value;
		}
	}
	public Weapon SecondSlot
	{
		get
		{
			return secondSlot;
		}

		set
		{
			secondSlot = value;
		}
	}
	public Weapon ThirdSlot
	{
		get
		{
			return thirdSlot;
		}

		set
		{
			thirdSlot = value;
		}
	}
	public Weapon FourthSlot
	{
		get
		{
			return fourthSlot;
		}

		set
		{
			fourthSlot = value;
		}
	}
	public Tool EquippedTool
	{
		get
		{
			return equippedTool;
		}

		set
		{
			equippedTool = value;
		}
	}
	public bool PlayerHasControl
	{
		get
		{
			return playerHasControl;
		}

		set
		{
			playerHasControl = value;
		}
	}
	public int Might
	{
		get
		{
			might = 0;
			if (helmet != null)
			{
				might += helmet.Might;
			}

			if (chest != null)
			{
				might += chest.Might;
			}

			if (legs != null)
			{
				might += legs.Might;
			}

			for (int i = 0; i < 4; i++)
			{
				if (FetchItemSlot(i) != null)
				{
					might += FetchItemSlot(i).Might;
				}
			}
			return might;
		}
	}
	public int Intelligence
	{
		get
		{
			intelligence = 0;
			if (helmet != null)
			{
				intelligence += helmet.Intelligence;
			}

			if (chest != null)
			{
				intelligence += chest.Intelligence;
			}

			if (legs != null)
			{
				intelligence += legs.Intelligence;
			}

			for (int i = 0; i < 4; i++)
			{
				if (FetchItemSlot(i) != null)
				{
					intelligence += FetchItemSlot(i).Intelligence;
				}
			}
			return intelligence;
		}
	}
	public int Dexterity
	{
		get
		{
			dexterity = 0;
			if (helmet != null)
			{
				dexterity += helmet.Dexterity;
			}

			if (chest != null)
			{
				dexterity += chest.Dexterity;
			}

			if (legs != null)
			{
				dexterity += legs.Dexterity;
			}

			for (int i = 0; i < 4; i++)
			{
				if (FetchItemSlot(i) != null)
				{
					dexterity += FetchItemSlot(i).Dexterity;
				}
			}
			return dexterity;
		}
	}
	public Weapon EquippedWeapon
	{
		get
		{
			return equippedWeapon;
		}
	}

	public Armor Helmet
	{
		get
		{
			return helmet;
		}
		set
		{
			helmet = value;
		}
	}
	public Armor Chestplate
	{
		get
		{
			return chest;
		}
		set
		{
			chest = value;
		}
	}
	public Armor LegArmor
	{
		get
		{
			return legs;
		}
		set
		{
			legs = value;
		}
	}

	public List<LegendaryPerk> ActivePerks
	{
		get
		{
			if (activeLegendaryPerks == null)
			{
				activeLegendaryPerks = new List<LegendaryPerk>();
			}
			activeLegendaryPerks.Clear();
			List<Weapon> myWeps = EquippedWeapons;
			for (int i = 0; i < 4; i++)
			{
				if (myWeps[i] != null)
				{
					foreach (LegendaryPerk perk in myWeps[i].perks)
					{
						activeLegendaryPerks.Add(perk);
					}
				}
			}
			return activeLegendaryPerks;
		}

		set
		{
			activeLegendaryPerks = value;
		}
	}

	public List<Weapon> EquippedWeapons
	{
		get
		{
			List<Weapon> myWeapons = new List<Weapon>();
			myWeapons.Add(firstSlot);
			myWeapons.Add(secondSlot);
			myWeapons.Add(thirdSlot);
			myWeapons.Add(fourthSlot);
			return myWeapons;
		}
	}

	public int BonusHealth
	{
		get
		{
			int temp = 0;
			for (int i = 0; i < 4; i++)
			{
				if (EquippedWeapons[i] != null)
				{
					temp += EquippedWeapons[i].BonusHealth;
				}
			}
			if (helmet != null)
			{
				temp += helmet.BonusHealth;
			}
			if (Chestplate != null)
			{
				temp += Chestplate.BonusHealth;
			}
			if (legs != null)
			{
				temp += legs.BonusHealth;
			}
			return temp;
		}
	}
	#endregion

	private void Awake()
	{
		animController = transform.Find("Model/Root").GetComponent<Animator>();
		vfxTransform = transform.Find("Model/FX_Root/Anchor");
		if (!File.Exists("CharData.dat"))
		{
			//Weapons
			//equippedWeapon = FirstSlot = new RecurveBow(aLib);
			equippedWeapon = FirstSlot = null;
			SecondSlot = null;
			ThirdSlot = null;
			FourthSlot = null;

			//Tools
			EquippedTool = new Pickaxe();

			//Equipment
			helmet = null;
			chest = null;
			legs = null;

			weaponSlotsUnlocked[0] = true;

			//Resetting all data
			if (File.Exists("SkillSet.dat"))
			{
				File.Delete("SkillSet.dat");
			}

			if (File.Exists("Inventory.dat"))
			{
				File.Delete("Inventory.dat");
			}
			PlayerPrefs.DeleteAll();
			SaveCharacterData();
		}
	}

	private void Start()
	{
		aLib = GameObject.FindGameObjectWithTag("AbilityDictionary").GetComponent<AbilityLibrary>();
		equipmentController = GameObject.FindGameObjectWithTag("PrimaryInterface").transform.Find("ItemCrafting").GetComponent<EquipmentCrafting_Controller>();
		inventory = GetComponent<Player_Inventory_Script>();
		activeLegendaryPerks = new List<LegendaryPerk>();
		equipmentController.ALib = aLib;

		#region Armor types
		ArmorItemTypes = new List<ItemType>();
		ArmorItemTypes.Add(ItemType.HELMET);
		ArmorItemTypes.Add(ItemType.HAT);
		ArmorItemTypes.Add(ItemType.COIF);
		ArmorItemTypes.Add(ItemType.CHESTPLATE);
		ArmorItemTypes.Add(ItemType.CHESTGUARD);
		ArmorItemTypes.Add(ItemType.ROBE);
		ArmorItemTypes.Add(ItemType.CHAPS);
		ArmorItemTypes.Add(ItemType.PLATELEGS);
		ArmorItemTypes.Add(ItemType.TROUSERS);
		#endregion

		if (File.Exists("CharData.dat"))
		{
			LoadCharacterData();
			GetComponent<Player_Details_Script>().MaxHealth = BonusHealth;
		}
		CurrentWeaponSlot = 0;
	}

	private void Update()
	{
		if (RecipeCrafter.IsUsingInput)
		{
			return;
		}

		if (playerHasControl)
		{
			#region SpellCasting
			int spellCast = -1;
			if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.A_FIRSTSKILL])
				&& !Input.GetKey(KeybindManager.Keybinds[KeybindFunction.A_SKILLMODIFIER]))
			{
				spellCast = 0;
			}
			else if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.A_SECONDSKILL])
				&& !Input.GetKey(KeybindManager.Keybinds[KeybindFunction.A_SKILLMODIFIER]))
			{
				spellCast = 1;
			}
			else if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.A_THIRDSKILL])
				&& !Input.GetKey(KeybindManager.Keybinds[KeybindFunction.A_SKILLMODIFIER]))
			{
				spellCast = 2;
			}
			else if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.A_FOURTHSKILL])
				&& !Input.GetKey(KeybindManager.Keybinds[KeybindFunction.A_SKILLMODIFIER]))
			{
				spellCast = 3;
			}

			if (spellCast != -1)
			{
				if (equippedWeapon != null && !Player_Accessor_Script.ControllerScript.IsCasting
					&& equippedWeapon.SkillSet[spellCast] != null
					&& Player_Accessor_Script.DetailsScript.CanSpendMana(equippedWeapon.SkillSet[spellCast].manaCost))
				{
					CastSpell(ref equippedWeapon, spellCast);
				}
			}
			#endregion
		}
		FetchItemStats();
	}

	private void CastSpell(ref Weapon weapon, int spellSlot)
	{
		bool didCast = false;
		if (weapon.SelectedAbilities[spellSlot] != null)
		{
			didCast = weapon.SelectedAbilities[spellSlot]();
		}
		if (didCast)
		{

			Vector3 dirToMouse = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
			dirToMouse.z = 0;
			dirToMouse.Normalize();

			animController.Play("CastLayer." + EquippedWeapon.Type + "_CastAnim");
			if (firstSlot != null)
			{
				firstSlot.TriggerGCDs();
			}
			if (secondSlot != null)
			{
				secondSlot.TriggerGCDs();
			}
			if (thirdSlot != null)
			{
				thirdSlot.TriggerGCDs();
			}
			if (fourthSlot != null)
			{
				fourthSlot.TriggerGCDs();
			}
			animController.transform.localRotation = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, dirToMouse), 0);
			vfxTransform.localRotation = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, dirToMouse), 0);
		}
	}

	private void UnequipTool()
	{
		inventory.AddItemToInventory(EquippedTool);
		EquippedTool = null;
		//Destroy(EquippedTool);
	}

	private void EquipTool()
	{
		if (equippedTool != null)
		{
			UnequipTool();
		}
		Tool tool = inventory.PullItemFromInventory("Tool", 0) as Tool;
		equippedTool = tool;
	}

	private void UnequipWeapon(int slot)
	{
		Weapon refWeap;
		switch (slot)
		{
			case (0):
				refWeap = firstSlot;
				firstSlot = null;
				break;
			case (1):
				refWeap = secondSlot;
				secondSlot = null;
				break;
			case (2):
				refWeap = thirdSlot;
				thirdSlot = null;
				break;
			case (3):
				refWeap = fourthSlot;
				fourthSlot = null;
				break;
			default:
				refWeap = firstSlot;
				firstSlot = null;
				break;
		}
		if (refWeap != null)
		{
			inventory.AddItemToInventory(refWeap);
			refWeap = null;
			equippedWeapon = null;
			abilities_Interface.UpdateSkillCooldowns();
			abilities_Interface.UpdateSkillIcons();
		}
		SaveCharacterData();
		inventory.SaveInventory();
	}

	public void EquipItem(int slot)
	{
		if (!weaponSlotsUnlocked[currentWeaponSlot])
		{
			return;
		}

		Item itemRef = inventory.FetchItemInSlot(slot);
		if (itemRef == null)
		{
			return;
		}

		if (itemRef.MyItemType == ItemType.WEAPON)
		{
			Weapon wep = (inventory.PullItemFromInventory("Weapon", slot) as Weapon);
			if (wep == null)
			{
				return;
			}

			if (wep.perks == null)
			{
				wep.perks = new List<LegendaryPerk>();
			}

			for (int i = 0; i < wep.perks.Count; i++)
			{
				ActivePerks.Add(wep.perks[i]);
			}

			if (wep == null)
			{
				return;
			}

			wep.lib = aLib;
			wep.PopulateSkills();

			if (equippedWeapon != null)
			{
				for (int i = 0; i < equippedWeapon.perks.Count; i++)
				{
					ActivePerks.Remove(equippedWeapon.perks[i]);
				}
				for (int i = 0; i < 4; i++)
				{
					if (equippedWeapon.SkillSet[i] != null)
					{
						equippedWeapon.SkillSet[i].isSelected = false;
						equippedWeapon.SkillSet[i] = null;
					}
				}
				inventory.AddItemToInventory(equippedWeapon);
			}

			equippedWeapon = wep;
			foreach (GameObject weapon in weaponLinkModel)
			{
				weapon.SetActive(false);
				if (weapon.name == wep.Type.ToString())
				{
					weapon.SetActive(true);
				}
			}

			switch (currentWeaponSlot)
			{
				case (0):
					firstSlot = wep;
					break;
				case (1):
					secondSlot = wep;
					break;
				case (2):
					thirdSlot = wep;
					break;
				case (3):
					fourthSlot = wep;
					break;
				default:
					firstSlot = wep;
					break;
			}
		}
		else if (itemRef.MyItemType == ItemType.HELMET || itemRef.MyItemType == ItemType.PLATELEGS || itemRef.MyItemType == ItemType.CHESTPLATE)
		{
			Armor armorRef;
			if (itemRef.MyItemType == ItemType.HELMET)
			{
				armorRef = inventory.PullItemFromInventory("", slot) as Helmet;
				if (Helmet != null)
				{
					inventory.AddItemToInventory(Helmet);
				}

				Helmet = armorRef;
			}
			else if (itemRef.MyItemType == ItemType.CHESTPLATE)
			{
				armorRef = inventory.PullItemFromInventory("", slot) as Chestplate;
				if (Chestplate != null)
				{
					inventory.AddItemToInventory(Chestplate);
				}

				Chestplate = armorRef;
			}
			else if (itemRef.MyItemType == ItemType.PLATELEGS)
			{
				armorRef = inventory.PullItemFromInventory("", slot) as Platelegs;
				if (LegArmor != null)
				{
					inventory.AddItemToInventory(LegArmor);
				}

				LegArmor = armorRef;
			}
		}
		else if (itemRef.MyItemType == ItemType.TOOL)
		{
			//UnequipTool();
			//EquipTool();
		}
		abilities_Interface.UpdateSkillCooldowns();
		abilities_Interface.UpdateSkillIcons();
		Ability_Select_Interface.instance.ResetRefIndex();
		SaveCharacterData();
		inventory.SaveInventory();
		GetComponent<Player_Details_Script>().MaxHealth = BonusHealth;
	}

	public void EquipItem(int slot, int weaponSlot)
	{

		Item ItemInSlot = (inventory.FetchItemInSlot(slot));
		if (!weaponSlotsUnlocked[weaponSlot] && ItemInSlot.MyItemType == ItemType.WEAPON)
		{
			return;
		}
		if (ItemInSlot == null)
		{
			return;
		}

		print("Called equip: " + ItemInSlot.ItemName);
		if (ItemInSlot.MyItemType == ItemType.WEAPON)
		{
			Weapon wep = (inventory.PullItemFromInventory("", slot) as Weapon);
			if (wep.perks == null)
			{
				wep.perks = new List<LegendaryPerk>();
			}

			for (int i = 0; i < wep.perks.Count; i++)
			{
				ActivePerks.Add(wep.perks[i]);
			}

			if (wep == null)
			{
				return;
			}

			wep.lib = aLib;
			wep.PopulateSkills();

			if (EquippedWeapons[weaponSlot] != null)
			{
				for (int i = 0; i < EquippedWeapons[weaponSlot].perks.Count; i++)
				{
					ActivePerks.Remove(EquippedWeapons[weaponSlot].perks[i]);
				}
				for (int i = 0; i < 4; i++)
				{
					if (EquippedWeapons[weaponSlot].SkillSet[i] != null)
					{
						EquippedWeapons[weaponSlot].SkillSet[i].isSelected = false;
						EquippedWeapons[weaponSlot].SkillSet[i] = null;
					}
				}
				inventory.AddItemToInventory(EquippedWeapons[weaponSlot]);
			}

			EquippedWeapons[weaponSlot] = wep;
			switch (weaponSlot)
			{
				case (0):
					//firstSlot = obj as Weapon;
					firstSlot = wep;
					break;
				case (1):
					//secondSlot = obj as Weapon;
					secondSlot = wep;
					break;
				case (2):
					//thirdSlot = obj as Weapon;
					thirdSlot = wep;
					break;
				case (3):
					//fourthSlot = obj as Weapon;
					fourthSlot = wep;
					break;
				default:
					//firstSlot = obj as Weapon;
					firstSlot = wep;
					break;
			}

			if (EquippedWeapons[currentWeaponSlot] != null)
			{
				equippedWeapon = EquippedWeapons[currentWeaponSlot];
				foreach (GameObject weapon in weaponLinkModel)
				{
					weapon.SetActive(false);
					if (weapon.name == equippedWeapon.Type.ToString())
					{
						weapon.SetActive(true);
					}
				}
			}
			abilities_Interface.UpdateSkillCooldowns();
			abilities_Interface.UpdateSkillIcons();
			Ability_Select_Interface.instance.ResetRefIndex();
		}
		else if (ArmorItemTypes.Contains(ItemInSlot.MyItemType))
		{
			Armor armorRef;
			switch (ItemInSlot.MyItemType)
			{
				case (ItemType.HAT):
				case (ItemType.COIF):
				case (ItemType.HELMET):
					if (ItemInSlot.MyItemType == ItemType.HAT)
					{
						armorRef = inventory.PullItemFromInventory("", slot) as Hat;
					}
					else if (ItemInSlot.MyItemType == ItemType.COIF)
					{
						armorRef = inventory.PullItemFromInventory("", slot) as Coif;
					}
					else
					{
						armorRef = inventory.PullItemFromInventory("", slot) as Helmet;
					}

					if (Helmet != null)
					{
						inventory.AddItemToInventory(Helmet);
					}
					Helmet = armorRef;
					break;

				case (ItemType.ROBE):
				case (ItemType.CHESTGUARD):
				case (ItemType.CHESTPLATE):
					if (ItemInSlot.MyItemType == ItemType.ROBE)
					{
						armorRef = inventory.PullItemFromInventory("", slot) as Robe;
					}
					else if (ItemInSlot.MyItemType == ItemType.CHESTGUARD)
					{
						armorRef = inventory.PullItemFromInventory("", slot) as Chestguard;
					}
					else
					{
						armorRef = inventory.PullItemFromInventory("", slot) as Chestplate;
					}

					if (Chestplate != null)
					{
						inventory.AddItemToInventory(Chestplate);
					}
					Chestplate = armorRef;
					break;

				case (ItemType.PLATELEGS):
				case (ItemType.TROUSERS):
				case (ItemType.CHAPS):
					if (ItemInSlot.MyItemType == ItemType.PLATELEGS)
					{
						armorRef = inventory.PullItemFromInventory("", slot) as Platelegs;
					}
					else if (ItemInSlot.MyItemType == ItemType.TROUSERS)
					{
						armorRef = inventory.PullItemFromInventory("", slot) as Trousers;
					}
					else
					{
						armorRef = inventory.PullItemFromInventory("", slot) as Chaps;
					}

					if (LegArmor != null)
					{
						inventory.AddItemToInventory(LegArmor);
					}
					LegArmor = armorRef;
					break;
			}
		}
		else if (ItemInSlot.MyItemType == ItemType.TOOL)
		{
			Tool toolRef = (inventory.PullItemFromInventory("", slot) as Tool);
			if (EquippedTool != null)
			{
				inventory.AddItemToInventory(EquippedTool);
			}
			EquippedTool = toolRef;
		}

		SaveCharacterData();
		inventory.SaveInventory();
		GetComponent<Player_Details_Script>().MaxHealth = BonusHealth;
	}

	public void FetchItemStats()
	{
		Vector3 v = Vector3.zero;
		if (helmet != null)
		{
			v += helmet.FetchStats();
		}

		if (chest != null)
		{
			v += chest.FetchStats();
		}

		if (legs != null)
		{
			v += legs.FetchStats();
		}

		dexterity = (int)v.x;
		intelligence = (int)v.y;
		might = (int)v.z;
	}
	public void PopulateWeaponSkills()
	{
		if (FirstSlot != null)
		{
			FirstSlot.PopulateSkills();
		}

		if (SecondSlot != null)
		{
			SecondSlot.PopulateSkills();
		}

		if (ThirdSlot != null)
		{
			ThirdSlot.PopulateSkills();
		}

		if (FourthSlot != null)
		{
			FourthSlot.PopulateSkills();
		}
	}
	public void SaveCharacterData()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream ofs = new FileStream("CharData.dat", FileMode.Create);
		CharacterData tempDat = new CharacterData(new Weapon[] { FirstSlot, SecondSlot, ThirdSlot, FourthSlot }, new Armor[] { helmet, chest, legs }, EquippedTool, EquippedWeapon, currentWeaponSlot, weaponSlotsUnlocked, SceneManager.GetActiveScene().name, transform.position.x, transform.position.y);
		bf.Serialize(ofs, tempDat);
		ofs.Close();
	}
	public void LoadCharacterData()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream ifs = new FileStream("CharData.dat", FileMode.Open);
		CharacterData dat = (CharacterData)bf.Deserialize(ifs);

		FirstSlot = dat.weaponArray[0];
		SecondSlot = dat.weaponArray[1];
		thirdSlot = dat.weaponArray[2];
		FourthSlot = dat.weaponArray[3];

		helmet = dat.armorArray[0];
		chest = dat.armorArray[1];
		legs = dat.armorArray[2];

		EquippedTool = dat.equippedTool;
		equippedWeapon = dat.equippedSlot;

		weaponSlotsUnlocked = dat.weaponSlotsUnlocked;

		if (firstSlot != null)
		{
			FirstSlot.lib = aLib;
		}

		if (SecondSlot != null)
		{
			SecondSlot.lib = aLib;
		}

		if (ThirdSlot != null)
		{
			ThirdSlot.lib = aLib;
		}

		if (FourthSlot != null)
		{
			FourthSlot.lib = aLib;
		}

		if (EquippedWeapon != null)
		{
			EquippedWeapon.lib = aLib;
		}

		currentWeaponSlot = dat.currentSlot;
		PopulateWeaponSkills();
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				if (FetchItemSlot(i) != null && FetchItemSlot(i).SkillSet[j] != null)
				{
					FetchItemSlot(i).SkillSet[j].isSelected = true;
				}
			}
		}
		transform.position = new Vector3(dat.savedX, dat.savedY, 0);
		ifs.Close();
	}
	public Weapon FetchItemSlot(int slot)
	{
		switch (slot)
		{
			case (0):
				return FirstSlot;
			case (1):
				return SecondSlot;
			case (2):
				return ThirdSlot;
			case (3):
				return FourthSlot;
		}
		return null;
	}

	public IEnumerator StunPlayerControl(float duration)
	{
		playerHasControl = false;
		yield return new WaitForSeconds(duration);
		playerHasControl = true;
	}
}

[Serializable]
public class CharacterData
{
	public Weapon[] weaponArray;
	public Armor[] armorArray;
	public Tool equippedTool;
	public Weapon equippedSlot;
	public int currentSlot;
	public bool[] weaponSlotsUnlocked;
	public string currentScene;
	public float savedX = 7, savedY = 4;

	public CharacterData(Weapon[] wArray, Armor[] aArray, Tool toolItem, Weapon weaponSlot, int slot, bool[] weaponSlotsUnlocked, string activeScene, float xPos, float yPos)
	{
		weaponArray = wArray;
		armorArray = aArray;
		equippedTool = toolItem;
		equippedSlot = weaponSlot;
		currentSlot = slot;
		this.weaponSlotsUnlocked = weaponSlotsUnlocked;
		currentScene = activeScene;
		savedX = xPos;
		savedY = yPos;
	}
}