using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;

public class Interface_Controller : MonoBehaviour {
	public static Interface_Controller instance;

	public Dictionary<string, UnityEvent> InterfaceEvents = new Dictionary<string, UnityEvent>();

	Vector3[] originalPositions;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}else if (instance != this)
		{
			Destroy(transform.root.gameObject);
			return;
		}

		InterfaceEvents.Add("CleaveSelectedPhase", new UnityEvent());
		InterfaceEvents.Add("AbilityInterfaceOpen", new UnityEvent());
	}

	void Start () {
		if (GameObject.FindGameObjectsWithTag(transform.tag).Length > 1){
			Destroy(gameObject);
			return;
		}

		originalPositions = new Vector3[] { transform.Find("Levels").transform.position, transform.Find("Inventory_V2").transform.position, transform.Find("Ability_Select_Interface").transform.position, transform.Find("ItemCrafting/EquipmentInterface").transform.position };
		if (File.Exists("InterfaceData.dat")){
			LoadElementPositions();
		} else {
			SaveElementPositions();
		}
	}

	public void SaveElementPositions(){
		InterfaceElements pack = new InterfaceElements(new Vector3[] { transform.Find("Levels").transform.position, transform.Find("Inventory_V2").transform.position, transform.Find("Ability_Select_Interface").transform.position, transform.Find("ItemCrafting/EquipmentInterface").transform.position});
		BinaryFormatter bf = new BinaryFormatter();
		FileStream ofs = new FileStream("InterfaceData.dat", FileMode.Create);
		bf.Serialize(ofs, pack);
		ofs.Close();
	}

	public void LoadElementPositions(){
		InterfaceElements pack;
		BinaryFormatter bf = new BinaryFormatter();
		FileStream ifs = new FileStream("InterfaceData.dat", FileMode.Open);
		pack = (InterfaceElements) bf.Deserialize(ifs);
		ifs.Close();
		transform.Find("Levels").transform.position = new Vector3(pack.ElementPositionsX[0], pack.ElementPositionsY[0]);
		transform.Find("Inventory_V2").transform.position = new Vector3(pack.ElementPositionsX[1], pack.ElementPositionsY[1]);
		transform.Find("Ability_Select_Interface").transform.position = new Vector3(pack.ElementPositionsX[2], pack.ElementPositionsY[2]);
		transform.Find("ItemCrafting/EquipmentInterface").transform.position = new Vector3(pack.ElementPositionsX[3], pack.ElementPositionsY[3]);
	}

	public void ResetElementPositions()
	{
		transform.Find("Levels").transform.position = originalPositions[0];
		transform.Find("Inventory_V2").transform.position = originalPositions[1];
		transform.Find("Ability_Select_Interface").transform.position = originalPositions[2];
		transform.Find("ItemCrafting/EquipmentInterface").transform.position = originalPositions[3];
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.D_RESET_UI]))
		{
			ResetElementPositions();
			SaveElementPositions();
		}
	}

[Serializable]
class InterfaceElements {
	public List<float> ElementPositionsX = new List<float>();
	public List<float> ElementPositionsY = new List<float>();

		public InterfaceElements(Vector3[] ElementArray)
		{
			for (int i = 0; i < ElementArray.Length; i++)
			{
				ElementPositionsX.Add(ElementArray[i].x);
				ElementPositionsY.Add(ElementArray[i].y);
			}
		}
	}
}
