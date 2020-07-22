using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class EquipmentCrafting_Controller : MonoBehaviour
{
	public static EquipmentCrafting_Controller instance;


	public AbilityLibrary ALib { get; set; }

	//public CraftableItem[] CraftableItems { get; set; }
	public List<CraftableItem> CraftableItems { get; set; }
	public List<CraftableReagent> CraftableReagents { get; set; }

	[SerializeField]
	private CraftedSword[] swords;

	[SerializeField]
	private CraftedSword[] bows;

	[SerializeField]
	private RecipeCrafter recipeCrafterRef;
	private Dictionary<Skills, List<CraftableItem>> ItemDictionary = new Dictionary<Skills, List<CraftableItem>>();
	private const string recipePath = "/CraftingRecipes.dat";

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
		if (CraftableItems == null)
		{
			CraftableItems = new List<CraftableItem>();
		}

		if (!File.Exists(Application.streamingAssetsPath + recipePath))
		{
			print("File does not exist. Recreating list (DEBUG)");
			SaveRecipes();
		}
		else
		{
			print("File exists, loading.");
			LoadRecipes();
		}

		if (CraftableItems.Count > 0)
		{
			for (int i = 0; i < CraftableItems.Count; i++)
			{
				if (CraftableItems[i] != null && !ItemDictionary.ContainsKey(CraftableItems[i].RequiredProfession))
				{
					ItemDictionary.Add(CraftableItems[i].RequiredProfession, new List<CraftableItem>());
				}
				ItemDictionary[CraftableItems[i].RequiredProfession].Add(CraftableItems[i]);
			}
		}

		if (CraftableReagents == null)
		{
			CraftableReagents = new List<CraftableReagent>();
		}

		if (CraftableReagents.Count > 0)
		{
			for (int i = 0; i < CraftableReagents.Count; i++)
			{
				if (CraftableReagents[i] != null && !ItemDictionary.ContainsKey(CraftableReagents[i].RequiredProfession))
				{
					ItemDictionary.Add(CraftableReagents[i].RequiredProfession, new List<CraftableItem>());
				}
				ItemDictionary[CraftableReagents[i].RequiredProfession].Add(CraftableReagents[i]);
			}
		}
		CraftableReagents.Sort((q1, q2) => q1.ItemName.CompareTo(q2.ItemName));

		if (recipeCrafterRef != null && recipeCrafterRef.enabled)
		{
			recipeCrafterRef.LoadDesignList();
		}
	}

	private void Start()
	{


	}

	public bool CanCraftItem(CraftableItem item)
	{
		Player_Inventory_Script invRef = Player_Accessor_Script.InventoryScript;
		bool craftable = true;
		foreach (Reagent r in item.RequiredReagents)
		{
			int invCount = invRef.GetResourceQuantity(r);
			if (invCount == -1 || invCount < r.ReagentQuantity)
			{
				print("Insufficient resource: " + r.ReagentName + " - " + r.ReagentQuantity);
				craftable = false;
				break;
			}
		}

		if (craftable)
		{
			foreach (Reagent r in item.RequiredReagents)
			{
				invRef.RemoveResource(r, r.ReagentQuantity);
			}
		}

		return craftable;
		//return true;
	}

	public Item GenerateItem(CraftableItem mould)
	{
		if (CanCraftItem(mould))
		{
			if (mould.MyCraftedItemType == CraftedItemType.WEAPON)
			{
				Weapon ret = null;
				switch ((mould as CraftedWeapon).tarType)
				{
					case (WeaponType.RECURVEBOW):
						ret = new RecurveBow(ALib);
						break;
					case (WeaponType.SWORD):
						ret = new Sword(ALib);
						break;
					case (WeaponType.CENSER):
						ret = new Censer(ALib);
						break;
					case (WeaponType.STAFF):
						ret = new Staff(ALib);
						break;
					case (WeaponType.SACRIFICIALKNIFE):
						ret = new SacrificialKnife(ALib);
						break;
					case (WeaponType.VESSEL):
						ret = new Vessel(ALib);
						break;
				}

				ret.CopyStats(mould as CraftedWeapon);
				Player_Accessor_Script.SkillsScript.AddExperience(mould.RequiredProfession, mould.RewardedExp);
				return ret;
			}
			else if (mould.MyCraftedItemType == CraftedItemType.ARMOR)
			{
				Armor ret = null;
				switch ((mould as CraftedArmor).tarType)
				{
					case (ArmorType.CHESTPLATE):
						ret = new Chestplate();
						ret.MyItemType = ItemType.CHESTPLATE;
						break;
					case (ArmorType.PLATELEGS):
						ret = new Platelegs();
						ret.MyItemType = ItemType.PLATELEGS;
						break;
					case (ArmorType.HELMET):
						ret = new Helmet();
						ret.MyItemType = ItemType.HELMET;
						break;

					case (ArmorType.CHESTGUARD):
						ret = new Chestguard();
						ret.MyItemType = ItemType.CHESTGUARD;
						break;
					case (ArmorType.CHAPS):
						ret = new Chaps();
						ret.MyItemType = ItemType.CHAPS;
						break;
					case (ArmorType.COIF):
						ret = new Coif();
						ret.MyItemType = ItemType.COIF;
						break;

					case (ArmorType.ROBE):
						ret = new Robe();
						ret.MyItemType = ItemType.ROBE;
						break;
					case (ArmorType.TROUSERS):
						ret = new Trousers();
						ret.MyItemType = ItemType.TROUSERS;
						break;
					case (ArmorType.HAT):
						ret = new Hat();
						ret.MyItemType = ItemType.HAT;
						break;
				}

				ret.CopyStats(mould as CraftedArmor);
				Player_Accessor_Script.SkillsScript.AddExperience(mould.RequiredProfession, mould.RewardedExp);
				return ret;
			}
			else if (mould.MyCraftedItemType == CraftedItemType.REAGENT)
			{
				print("Reagent Type: " + mould.ItemName);
				Reagent newReagent = new Reagent();
				newReagent.ReagentName = mould.ItemName;
				newReagent.ReagentQuantity = 1;
				newReagent.ReagentSprite = mould.SpritePath;
				Player_Accessor_Script.InventoryScript.AddResourceType(newReagent, newReagent.ReagentQuantity);
				Player_Accessor_Script.SkillsScript.AddExperience(mould.RequiredProfession, mould.RewardedExp);
			}
			else
			{
				print("Unknown crafting type");
			}
		}
		return null;
	}

	public CraftableItem[] FetchItemsForProfession(Skills prof)
	{
		if (!ItemDictionary.ContainsKey(prof))
		{
			return null;
		}
		return ItemDictionary[prof].ToArray();
	}


	public void SaveRecipes()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream fio = File.Open(Application.streamingAssetsPath + recipePath, FileMode.OpenOrCreate);
		CraftingWrapper wrapper = new CraftingWrapper(CraftableItems, CraftableReagents);
		bf.Serialize(fio, wrapper);
		fio.Close();
	}

	public void LoadRecipes()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream fin = File.Open(Application.streamingAssetsPath + recipePath, FileMode.Open);
		CraftingWrapper wrapper = bf.Deserialize(fin) as CraftingWrapper;
		CraftableItems = wrapper.items;
		CraftableReagents = wrapper.reagents;
		fin.Close();
	}
}

[System.Serializable]
public class CraftingWrapper
{
	public List<CraftableItem> items;
	public List<CraftableReagent> reagents;
	public CraftingWrapper()
	{
		items = new List<CraftableItem>();
		reagents = new List<CraftableReagent>();
	}

	public CraftingWrapper(List<CraftableItem> copy, List<CraftableReagent> reagentCopy)
	{
		items = copy;
		reagents = reagentCopy;
	}
}
