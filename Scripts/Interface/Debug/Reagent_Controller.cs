using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

//Used in debug interface for building and designing new recipes.
public class Reagent_Controller : MonoBehaviour
{
	public static Reagent_Controller instance;
	public List<Reagent> reagentList = new List<Reagent>();
	public Transform reagentAddOption, reagentCardParent;
	public List<CraftableReagent> availableReagents = new List<CraftableReagent>();

	[SerializeField]
	private GameObject reagentCard;
	private RecipeCrafter crafterRef;
	private List<Text> currentReagents = new List<Text>();

	private void Awake()
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
	}

	private void Start()
	{
		crafterRef = transform.parent.GetComponent<RecipeCrafter>();
		crafterRef.ReagentController = this;
		Dropdown tempDD = reagentAddOption.Find("BG/Dropdown").GetComponent<Dropdown>();
		tempDD.ClearOptions();
		LoadAvailableReagentList();
		ResetReagentAdd();
	}

	public void AddNewReagentToAvailable(string reagentName)
	{
		//availableReagents.Add(reagentName);
		CraftableReagent temp = EquipmentCrafting_Controller.instance.CraftableReagents.Find((q) => q.ItemName == reagentName);
		reagentList.Add(new Reagent(temp.ItemName, temp.SpritePath, 1));
	}

	public void AddReagentToList()
	{
		Dropdown tempDD = reagentAddOption.Find("BG/Dropdown").GetComponent<Dropdown>();
		string reagent = tempDD.options[tempDD.value].text;
		print("Added reagent:" + reagent);

		//Reagent tarReagent = new Reagent();
		//tarReagent.ReagentName = reagent;
		//for (int i = 0; i < reagentCardParent.childCount; i++)
		//{
		//	if (reagentCardParent.GetChild(i).Find("BG/Reagent").GetComponent<Text>().text == tarReagent.ReagentName)
		//	{
		//		tarReagent.ReagentQuantity = int.Parse(reagentCardParent.GetChild(i).Find("BG/Quantity").GetComponent<InputField>().text);
		//		tarReagent.ReagentSprite = EquipmentCrafting_Controller.instance.CraftableReagents.Find((q) => q.ItemName == reagent).SpritePath;
		//		break;
		//	}
		//}

		if (reagent.IndexOf("--") != -1)
		{
			return;
		}

		for (int i = 0; i < currentReagents.Count; i++)
		{
			if (currentReagents[i] == null)
			{
				currentReagents.RemoveAt(i);
				i--;
				continue;
			}
			if (currentReagents[i].text == reagent)
			{
				return;
			}
		}

		GameObject obj = Instantiate(reagentCard, reagentCardParent, false);
		Text textRef = obj.transform.Find("BG/Reagent").GetComponent<Text>();
		obj.transform.Find("BG/Quantity").GetComponent<InputField>().text = reagentAddOption.Find("BG/Quantity").GetComponent<InputField>().text;
		textRef.text = reagent;
		currentReagents.Add(textRef);
		AddNewReagentToAvailable(reagent);
		ResetReagentAdd();
	}

	public void ResetReagentAdd()
	{
		reagentAddOption.Find("BG/Quantity").GetComponent<InputField>().text = 0.ToString();
		Dropdown tempDD = reagentAddOption.Find("BG/Dropdown").GetComponent<Dropdown>();
		tempDD.value = 0;
	}

	public void SaveReagentList()
	{
		print("Saving Design");
		if (reagentList == null)
		{
			reagentList = new List<Reagent>();
		}
		reagentList.Clear();
		for (int i = 1; i < reagentCardParent.childCount; i++)
		{
			Reagent reg = new Reagent();
			reg.ReagentName = reagentCardParent.GetChild(i).Find("BG/Reagent").GetComponent<Text>().text;
			reg.ReagentSprite = reg.ReagentName;
			reg.ReagentQuantity = int.Parse(reagentCardParent.GetChild(i).Find("BG/Quantity").GetComponent<InputField>().text);
			reagentList.Add(reg);
		}
	}

	public void LoadReagentList()
	{
		ClearReagentList();
		ResetReagentAdd();
		if (crafterRef.CurrentItem == null)
		{
			return;
		}

		if (crafterRef.CurrentItem.RequiredReagents == null)
		{
			crafterRef.CurrentItem.RequiredReagents = new List<Reagent>();
		}

		for (int i = 0; i < crafterRef.CurrentItem.RequiredReagents.Count; i++)
		{
			GameObject obj = Instantiate(reagentCard, reagentCardParent, false);
			string reagent = crafterRef.CurrentItem.RequiredReagents[i].ReagentName;
			Text textRef = obj.transform.Find("BG/Reagent").GetComponent<Text>();
			obj.transform.Find("BG/Quantity").GetComponent<InputField>().text = crafterRef.CurrentItem.RequiredReagents[i].ReagentQuantity.ToString();
			Reagent reg = crafterRef.CurrentItem.RequiredReagents[i];
			textRef.text = reagent;
			currentReagents.Add(textRef);
			reagentList.Add(reg);
		}
	}

	public void ClearReagentList()
	{
		reagentList.Clear();
		currentReagents.Clear();
		for (int i = reagentCardParent.childCount - 1; i > 0; i--)
		{
			Destroy(reagentCardParent.GetChild(i).gameObject);
		}
	}

	public void SaveAvailableReagentList()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream ifs = new FileStream(Application.streamingAssetsPath + "/CraftingReagentsList.dat", FileMode.OpenOrCreate);
		bf.Serialize(ifs, availableReagents);
		ifs.Close();
		EquipmentCrafting_Controller.instance.CraftableReagents = availableReagents;
		EquipmentCrafting_Controller.instance.SaveRecipes();
	}

	public void LoadAvailableReagentList()
	{
		//BinaryFormatter bf = new BinaryFormatter();
		//FileStream ofs = new FileStream(Application.streamingAssetsPath + "/CraftingReagentsList.dat", FileMode.OpenOrCreate);
		//availableReagents = (List<CraftableReagent>)bf.Deserialize(ofs);
		//ofs.Close();
		Dropdown tempDD = reagentAddOption.Find("BG/Dropdown").GetComponent<Dropdown>();
		tempDD.ClearOptions();
		List<string> reagentNames = new List<string>();
		if (EquipmentCrafting_Controller.instance.CraftableReagents == null)
		{
			EquipmentCrafting_Controller.instance.CraftableReagents = new List<CraftableReagent>();
		}
		foreach (CraftableReagent reagent in EquipmentCrafting_Controller.instance.CraftableReagents)
		{
			reagentNames.Add(reagent.ItemName);
		}
		tempDD.AddOptions(reagentNames);
	}
}
