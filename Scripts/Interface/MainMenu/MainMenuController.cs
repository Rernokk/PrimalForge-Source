using Facepunch.Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
	public void StartNewGame()
	{
		if (File.Exists("CharData.dat"))
		{
			File.Delete("CharData.dat");
		}
		if (File.Exists("InterfaceData.dat"))
		{
			File.Delete("InterfaceData.dat");
		}
		if (File.Exists("Inventory.dat"))
		{
			File.Delete("Inventory.dat");
		}
		if (File.Exists("SkillSet.dat"))
		{
			File.Delete("SkillSet.dat");
		}
		if (File.Exists("QuestLog.dat"))
		{
			File.Delete("QuestLog.dat");
		}
		PlayerPrefs.DeleteAll();
		SceneManager.LoadScene("Tutoria");
	}

	public void LoadPreviousGame()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream ifs = new FileStream("CharData.dat", FileMode.Open);
		CharacterData dat = (CharacterData) bf.Deserialize(ifs);

		SceneManager.LoadScene(dat.currentScene);
	}

	public void MoveToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void MoveToInstructions()
	{
		SceneManager.LoadScene("InstructionsScene");
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void MoveToSithrokFight()
	{
		SceneManager.LoadScene("Sithrok'sLair");
	}

	public void Start()
	{
		if (!File.Exists("CharData.dat"))
		{
			transform.Find("ButtonGroup/Resume").GetComponent<Button>().interactable = false;
		}
	}

	public void Update()
	{
		//if (Input.GetKeyDown(KeyCode.Alpha7))
		//{
		//	print("Achievements Reset!");
		//	foreach (Facepunch.Steamworks.Achievement achievo in Client.Instance.Achievements.All)
		//	{
		//		achievo.Reset();
		//	}
		//}
	}
}
