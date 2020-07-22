using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Accessor_Script : MonoBehaviour {
  public static Player_Equipment_Script EquipmentScript;
  public static Player_Skills_Script SkillsScript;
  public static Player_Inventory_Script InventoryScript;
  public static Player_Controller_Script ControllerScript;
  public static Player_Details_Script DetailsScript;
	public static Player_Reputation_Script ReputationScript;
	public static Player_Animator_Script AnimatorScript;

	public static Dialog_Interface DialogInterface;
	public static Abilities_Interface AbilitiesInterface;
	public static Objective_Interface ObjectiveInterface;
  
  // Use this for initialization
  void Awake () {
		if (GameObject.FindGameObjectsWithTag(transform.tag).Length > 1){
			Destroy(gameObject);
			return;
		}

    EquipmentScript = GetComponent<Player_Equipment_Script>();
    SkillsScript = GetComponent<Player_Skills_Script>();
    InventoryScript = GetComponent<Player_Inventory_Script>();
    ControllerScript = GetComponent<Player_Controller_Script>();
    DetailsScript = GetComponent<Player_Details_Script>();
		ReputationScript = GetComponent<Player_Reputation_Script>();
		AbilitiesInterface = GameObject.Find("Interface/Abilities").GetComponent<Abilities_Interface>();
		DialogInterface = GameObject.Find("Interface/Dialog").GetComponent<Dialog_Interface>();
	}

	void Start()
	{
		AnimatorScript = transform.Find("Player_Anim_Root").GetComponent<Player_Animator_Script>();
	}

	public static void SaveGame(){
    SkillsScript.SaveSkills();
    InventoryScript.SaveInventory();
    EquipmentScript.SaveCharacterData();
		Objective_Interface.instance.SaveQuestLog();
  }

  public static void LoadGame(){
    SkillsScript.LoadSkills();
    InventoryScript.LoadInventory();
    EquipmentScript.LoadCharacterData();
		Objective_Interface.instance.LoadQuestLog();
	}
}
