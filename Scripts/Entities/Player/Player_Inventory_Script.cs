using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Player_Inventory_Script : MonoBehaviour
{
  [FoldoutGroup("Items")]
  [SerializeField]
  Item[] itemInventory;


  Dictionary<string, Reagent> resourceInventory;
  Player_Equipment_Script equipment;
  Transform debugWindow;
	Animator animCtrller;

	public Item[] ItemInventory
	{
		get
		{
			return itemInventory;
		}

		 set
		{
			itemInventory = value;
		}
	}

  void Start()
  {
    equipment = GetComponent<Player_Equipment_Script>();
		animCtrller = transform.Find("Model/Root").GetComponent<Animator>();
		
    if (!File.Exists("Inventory.dat"))
    {
      itemInventory = new Item[192];
      for (int i = 0; i < itemInventory.Length; i++)
      {
        itemInventory[i] = null;
      }
			resourceInventory = new Dictionary<string, Reagent>();
    }
    else
    {
      LoadInventory();
    }
  }

  private int FindEmptySlot()
  {
    int i = 0;
    while (i < 192 && (itemInventory[i] != null || (itemInventory[i] != null && itemInventory[i].ItemName != "")))
    {
      if (itemInventory[i].ItemName == null || itemInventory[i].ItemName == "")
      {
        break;
				return i;
      }
      i++;
    }
    if (i == 192)
    {
      return -1;
    }
    return i;
  }

  public void AddItemToInventory(Item obj)
  {
    int ind = FindEmptySlot();
    if (ind != -1)
    {
      itemInventory[ind] = obj;
      SaveInventory();
    }
  }

  public Item PullItemFromInventory(string slotType, int ind)
  {
    Item retItem = itemInventory[ind];
    itemInventory[ind] = null;
    SaveInventory();
    return retItem;
  }

	public bool SlotHasItem(int slot){
		return !(itemInventory[slot] == null);
	}

	//public Item FindItemInventory(int slot){
	//	return itemInventory[slot];
	//}

  public void AddResourceType(Reagent res, int quantity)
  {
    if (resourceInventory == null)
    {
      resourceInventory = new Dictionary<string, Reagent>();
    }

		if (res.ReagentName == "Oak Logs")
		{
			Objective_Interface.instance.QuestObjectives["GatherOak"].Invoke();
		}
		if (res.ReagentName == "Oak Planks")
		{
			Objective_Interface.instance.QuestObjectives["CraftPlank"].Invoke();
		}

		if (resourceInventory.ContainsKey(res.ReagentName))
    {
			resourceInventory[res.ReagentName].ReagentQuantity += quantity;
      print(res + ": " + resourceInventory[res.ReagentName].ReagentName);
    }
    else
    {
      resourceInventory.Add(res.ReagentName, res);
      print(res + ": " + resourceInventory[res.ReagentName]);
		}
		Inventory_Interface.instance.UpdateResourceListQuantities();
	}

	public void RemoveResource(Reagent res, int quantity)
	{
		resourceInventory[res.ReagentName].ReagentQuantity -= quantity;
		Inventory_Interface.instance.UpdateResourceListQuantities();
	}

	public void RemoveResource(string res, int quantity)
	{
		resourceInventory[res].ReagentQuantity -= quantity;
		Inventory_Interface.instance.UpdateResourceListQuantities();
	}

	public int GetResourceQuantity(Reagent resource)
	{
		if (resourceInventory == null)
		{
			resourceInventory = new Dictionary<string, Reagent>();
		}
		if (!resourceInventory.ContainsKey(resource.ReagentName))
		{
			return -1;
		}
		return resourceInventory[resource.ReagentName].ReagentQuantity;
	}

	public int GetResourceQuantity(string resource)
	{
		if (resourceInventory == null)
		{
			resourceInventory = new Dictionary<string, Reagent>();
		}
		if (!resourceInventory.ContainsKey(resource))
		{
			return -1;
		}
		return resourceInventory[resource].ReagentQuantity;
	}

	public Reagent GetResourceReference(string resource)
	{
		if (resourceInventory == null)
		{
			resourceInventory = new Dictionary<string, Reagent>();
		}
		if (!resourceInventory.ContainsKey(resource))
		{
			return null;
		}
		return resourceInventory[resource];
	}

	public Item FetchItemInSlot(int slot)
  {
		if (itemInventory.Length <= 0)
		{
			return null;
		}
    return itemInventory[slot];
  }

  public void Update()
  {
		//if (Input.GetKeyDown(KeyCode.H))
		//{
		//	AddResourceType("OAKPLANK", 1);
		//}
    if (Input.GetKeyDown(KeyCode.Mouse0))
    {
      RaycastHit2D inf;
      Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
      inf = Physics2D.Raycast(r.origin, r.direction);
      if (inf.transform != null && inf.transform.tag == "ResourceNode")
      {
        if (inf.transform.GetComponent<ResourceNode>().IsNodeInRange(transform.position) && inf.transform.GetComponent<ResourceNode>().IsCorrectTool(equipment.EquippedTool.ToolType))
        {
          ResourceData s = inf.transform.GetComponent<ResourceNode>().Interact();
					if (s.reagentData != null)
					{
						Player_Accessor_Script.SkillsScript.AddExperience(s.skillRef, s.rewardedExp);
						AddResourceType(s.reagentData, s.reagentData.ReagentQuantity);
						animCtrller.transform.localRotation = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, (inf.transform.position - transform.position).normalized ), 0);
						animCtrller.Play("Harvesting");
					}
        }
      }
    }
		if (Input.GetKeyDown(KeyCode.L))
		{
			foreach (Item i in itemInventory)
			{
				print(i.ItemName);
			}
		}
  }
  public void SaveInventory()
  {
    BinaryFormatter bf = new BinaryFormatter();
    FileStream ofs = new FileStream("Inventory.dat", FileMode.Create);
    InventoryContainer container = new InventoryContainer(itemInventory, resourceInventory);
    bf.Serialize(ofs, container);
    ofs.Close();
  }

  public void LoadInventory()
  {
    BinaryFormatter bf = new BinaryFormatter();
    FileStream ifs = new FileStream("Inventory.dat", FileMode.Open);
    InventoryContainer container = (InventoryContainer)bf.Deserialize(ifs);
		itemInventory = new Item[container.myList.Length];
		itemInventory = container.myList;
		resourceInventory = container.ResourceCollection;
    ifs.Close();
  }
}


[Serializable]
public class InventoryContainer
{
  public Item[] myList;
	public Dictionary<string, Reagent> ResourceCollection;
  public InventoryContainer(Item[] itemList, Dictionary<string, Reagent> resourceColl)
  {
    myList = itemList;
		ResourceCollection = resourceColl;
  }
}