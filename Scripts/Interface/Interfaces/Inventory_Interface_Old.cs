using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Interface_Old : MonoBehaviour
{
  Transform itemPane;
  Player_Inventory_Script playerInventory;
  Player_Equipment_Script playerEquipment;

  void Start()
  {
    itemPane = transform.Find("Panel");
    playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Inventory_Script>();
    playerEquipment = playerInventory.GetComponent<Player_Equipment_Script>();
    for (int i = 0; i < itemPane.childCount; i++)
    {
      for (int j = 0; j < itemPane.GetChild(i).childCount; j++)
      {
        Slot_Tracker scr = itemPane.GetChild(i).GetChild(j).gameObject.GetComponent<Slot_Tracker>();
        scr.Slot = (i * itemPane.childCount) + j;
        scr.GetComponent<Button>().onClick.AddListener(scr.EquipItemFromSlot);
        scr.PlayerInventory = playerInventory;
        scr.PlayerEquipment = playerEquipment;
      }
    }
  }

  void Update()
  {
    if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.I_INVENTORY])){
      CanvasGroup group = GetComponent<CanvasGroup>();
      group.alpha = Mathf.Abs(group.alpha - 1);
      group.interactable = group.blocksRaycasts = !group.blocksRaycasts;
    }

    for (int i = 0; i < 6; i++)
    {
      for (int j = 0; j < 6; j++)
      {
        Item tempItem = playerInventory.FetchItemInSlot((i * 6) + j);
        if (tempItem != null)
        {
					itemPane.GetChild(i).GetChild(j).GetChild(0).GetComponent<Image>().sprite = tempItem.Icon;
        } else
        {
					itemPane.GetChild(i).GetChild(j).GetChild(0).GetComponent<Image>().sprite = null;
        }
      }
    }
  }
  
  public void PrintItemName(string message){
    print(message);
  }
}
