using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentFactory : MonoBehaviour
{
  enum WeaponTypes { AXE, DAGGER, SWORD, TWINDAGGERS, WHIP };
  enum ArmorTypes { HELMET, CHEST, LEGS };
  enum Rarity { COMMON, UNCOMMON, RARE, LEGENDARY, UNIQUE }
  // Use this for initialization
  void Start()
  {

  }

  private void Update()
  {
    
  }

  public Item GenerateItemDrop()
  {
    Item genItem = new Item();
    ItemType resType = (ItemType)Random.Range(0, 3);
    if (resType == ItemType.HELMET || resType == ItemType.CHESTPLATE || resType == ItemType.PLATELEGS)
    {
      genItem = new Armor();
			ItemType armorSlot = (ItemType)Random.Range(0, 3);
      switch (armorSlot)
      {
        case (ItemType.HELMET):
          (genItem as Armor).MyItemType = ItemType.HELMET;
          break;
        case (ItemType.CHESTPLATE):
          (genItem as Armor).MyItemType = ItemType.CHESTPLATE;
          break;
        case (ItemType.PLATELEGS):
          (genItem as Armor).MyItemType = ItemType.PLATELEGS;
          break;
      }
    }
    return genItem;
  }
}
