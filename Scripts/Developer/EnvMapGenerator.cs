using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class EnvMapGenerator : MonoBehaviour
{
	[SerializeField]
	private int MapSize = 32;

	AIManager managerRef;

	private bool saveConfirm = false, destroyConfirm = false;
	private Transform rootObj;

	private AIManager aiManagerRef {
		get {
			if (managerRef == null){
				managerRef = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<AIManager>();
			}
			return managerRef;
		}
	}

	[Button("Create Root")]
	public void CreateRoot()
	{
		if (rootObj == null)
		{
			rootObj = new GameObject("EnvironmentRoot").transform;
		}
	}

	[Button("Save Environment")]
	public void ConfirmSave()
	{
		saveConfirm = true;
	}

	[ShowIf("saveConfirm")]
	[GUIColor(0, 1, 0)]
	[Button("Confirm Save")]
	public void SavePairs()
	{
		string[] entries = new string[rootObj.childCount];
		for (int i = 0; i < rootObj.childCount; i++)
		{
			string objName;
			if (rootObj.GetChild(i).name.IndexOf("(Clone)") == -1)
			{
				objName = rootObj.GetChild(i).name.Split(' ')[0];
			} else {
				objName = rootObj.GetChild(i).name.Split('(')[0];
			}
			entries[i] = objName + "|" + rootObj.GetChild(i).transform.position;
		}
		
		File.WriteAllLines(aiManagerRef.MapObject.name + "_Env.txt", entries);
		saveConfirm = false;
	}



	[Button("Load Environment")]
	public void LoadObjects()
	{
		CreateRoot();
		string[] dict = File.ReadAllLines(aiManagerRef.MapObject.name + "_Env.txt");

		//Spawning World Objects
		for (int i = 0; i < dict.Length; i++) {
			string[] entry = dict[i].Split('|');
			string[] posSplit = entry[1].Split(',');
			posSplit[0] = posSplit[0].Substring(1);
			posSplit[2] = posSplit[2].Substring(0, posSplit[2].Length - 1);
			Vector3 position = new Vector3(float.Parse(posSplit[0]), float.Parse(posSplit[1]), float.Parse(posSplit[2]));
			Instantiate(Resources.Load<GameObject>("Environment/Buildings/" + dict[i].Substring(0, dict[i].IndexOf('|'))), position, Quaternion.identity).transform.parent = rootObj;
		}
	}



	[Button("Confirm Destroy Root")]
	public void ConfirmDestroy()
	{
		destroyConfirm = true;
	}

	[ShowIf("destroyConfirm")]
	[Button("Destroy Root")]
	[GUIColor(1, 0, 0)]
	public void DestroyRoot()
	{
		if (rootObj != null)
		{
			DestroyImmediate(rootObj.gameObject);
		}
		destroyConfirm = false;
	}
}
