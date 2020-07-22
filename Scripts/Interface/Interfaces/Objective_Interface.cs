using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Objective_Interface : MonoBehaviour
{
	public static Objective_Interface instance;

	[SerializeField]
	private GameObject objectivePrefab;
	private List<Objective_Base> activeObjectives = new List<Objective_Base>();
	private bool notMadeListener = true;
	private bool questsLoaded = false;
	private Dictionary<QUESTNAME, int> questCompletionStates = new Dictionary<QUESTNAME, int>();
	private Dictionary<QUESTNAME, int> questProgressStates = new Dictionary<QUESTNAME, int>();

	public Dictionary<string, UnityEvent> QuestObjectives = new Dictionary<string, UnityEvent>();


	#region Properties
	public List<Objective_Base> ActiveObjectives
	{
		get
		{
			return activeObjectives;
		}
		set
		{
			activeObjectives = value;
		}
	}

	public Dictionary<QUESTNAME, int> QuestCompletionStates
	{
		get
		{
			return questCompletionStates;
		}

		set
		{
			questCompletionStates = value;
		}
	}

	public Dictionary<QUESTNAME, int> QuestProgressStates
	{
		get
		{
			return questProgressStates;
		}

		set
		{
			questProgressStates = value;
		}
	}
	#endregion

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

		QuestObjectives.Add("InteractBridge", new UnityEvent());
		QuestObjectives.Add("SpeakWithForeman", new UnityEvent());
		QuestObjectives.Add("DeliverPlanks", new UnityEvent());
		QuestObjectives.Add("EquipHatchet", new UnityEvent());
		QuestObjectives.Add("GatherOak", new UnityEvent());
		QuestObjectives.Add("EnterEastForest", new UnityEvent());
		QuestObjectives.Add("CraftPlank", new UnityEvent());
		QuestObjectives.Add("OpenCrafting", new UnityEvent());
		QuestObjectives.Add("SlayGoblins", new UnityEvent());
		QuestObjectives.Add("BuildBridge", new UnityEvent());
		QuestObjectives.Add("ClearRubble", new UnityEvent());

		if (File.Exists("QuestLog.dat"))
		{
			LoadQuestLog();
		}
		else
		{
			SaveQuestLog();
		}
	}

	private void Start()
	{
		//Player hasn't started the first quest! Usually expected to be the start of the game.
		if (!questCompletionStates.ContainsKey(QUESTNAME.TakeUpArms) || questCompletionStates[QUESTNAME.TakeUpArms] == 0)
		{
			questCompletionStates.Add(QUESTNAME.TakeUpArms, 1);
			questProgressStates.Add(QUESTNAME.TakeUpArms, 0);
		}
	}

	private void Update()
	{
		if (!questsLoaded)
		{
			foreach (UnityEngine.Object obj in Resources.LoadAll<UnityEngine.Object>("ObjectiveQuests"))
			{
				Objective_Base temp = (ScriptableObject.CreateInstance(obj.name) as Objective_Base);

				//Quest In Progress
				if (questCompletionStates.ContainsKey(temp.QuestEnumeration) && questCompletionStates[temp.QuestEnumeration] == 1)
				{
					temp.QuestStage = questProgressStates[temp.QuestEnumeration];
					AddNewObjective(temp);
				}

				//Quest Not Started
				else if (!questCompletionStates.ContainsKey(temp.QuestEnumeration))
				{
					questProgressStates.Add(temp.QuestEnumeration, 0);
					questCompletionStates.Add(temp.QuestEnumeration, 0);
				}

				//Quest Completed
				else if (questCompletionStates.ContainsKey(temp.QuestEnumeration) && questCompletionStates[temp.QuestEnumeration] == 2)
				{
					Destroy(temp);
					temp = null;
				}
			}
			questsLoaded = true;
		}
	}

	public void AddNewObjective(Objective_Base referenceObjective)
	{
		activeObjectives.Add(referenceObjective);
		questCompletionStates[referenceObjective.QuestEnumeration] = 1;
		UpdateObjectiveInterface();
		SaveQuestLog();
	}

	public void RemoveObjective(Objective_Base referenceObjective)
	{
		transform.Find(referenceObjective.ObjectiveName).GetComponent<Animator>().Play("CompletedAnim");
		activeObjectives.Remove(referenceObjective);
		Destroy(referenceObjective);
	}

	public void CompleteObjective(QUESTNAME questEnum)
	{
		questCompletionStates[questEnum] = 2;
		SaveQuestLog();
	}

	public void PlayProgressAnimation(Objective_Base referenceObjective)
	{
		transform.Find(referenceObjective.ObjectiveName).GetComponent<Animator>().Play("ProgressAnim");
	}

	public void UpdateObjectiveInterface()
	{
		for (int i = 0; i < activeObjectives.Count; i++)
		{
			if (transform.Find(activeObjectives[i].ObjectiveName) == null)
			{
				GameObject objRef = Instantiate(objectivePrefab, transform, false);
				objRef.transform.name = activeObjectives[i].ObjectiveName;
				objRef.transform.Find("Text").GetComponent<Text>().text = activeObjectives[i].FetchObjectiveText();
				objRef.transform.Find("TitleBG/Title").GetComponent<Text>().text = activeObjectives[i].ObjectiveName;
				objRef.transform.Find("IconBG/Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(activeObjectives[i].ObjectiveIcon);
			}
			else
			{
				GameObject objRef = transform.Find(ActiveObjectives[i].ObjectiveName).gameObject;
				objRef.transform.Find("Text").GetComponent<Text>().text = activeObjectives[i].FetchObjectiveText();
			}
		}
	}

	public bool IsObjectiveActive(string objectiveName)
	{
		foreach (Objective_Base objective in activeObjectives)
		{
			if (objective.ObjectiveName.ToLower() == objectiveName.ToLower())
			{
				print("Found objective: " + objectiveName);
				return true;
			}
		}
		return false;
	}

	public void PushObjectivePhase(string objectiveName)
	{
		foreach (Objective_Base objective in activeObjectives)
		{
			if (objective.ObjectiveName.ToLower() == objectiveName.ToLower())
			{
				objective.PushStage();
				return;
			}
		}
	}

	public void SaveQuestLog()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream ofs = new FileStream("QuestLog.dat", FileMode.OpenOrCreate);
		QuestLog myLog = new QuestLog();
		myLog.questCompletionStates = questCompletionStates;
		myLog.questProgressState = questProgressStates;
		bf.Serialize(ofs, myLog);
		ofs.Close();
	}

	public void LoadQuestLog()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream ifs = new FileStream("QuestLog.dat", FileMode.Open);
		QuestLog myLog = (QuestLog)bf.Deserialize(ifs);
		questCompletionStates = myLog.questCompletionStates;
		questProgressStates = myLog.questProgressState;
		ifs.Close();

		if (!questsLoaded)
		{
			foreach (UnityEngine.Object obj in Resources.LoadAll<UnityEngine.Object>("ObjectiveQuests"))
			{
				Objective_Base temp = (ScriptableObject.CreateInstance(obj.name) as Objective_Base);

				//Quest In Progress
				if (questCompletionStates.ContainsKey(temp.QuestEnumeration) && questCompletionStates[temp.QuestEnumeration] == 1)
				{
					temp.QuestStage = questProgressStates[temp.QuestEnumeration];
					AddNewObjective(temp);
				}

				//Quest Not Started
				else if (!questCompletionStates.ContainsKey(temp.QuestEnumeration))
				{
					questProgressStates.Add(temp.QuestEnumeration, 0);
					questCompletionStates.Add(temp.QuestEnumeration, 0);
				}

				//Quest Completed
				else if (questCompletionStates.ContainsKey(temp.QuestEnumeration) && questCompletionStates[temp.QuestEnumeration] == 2)
				{
					Destroy(temp);
					temp = null;
				}
			}
			questsLoaded = true;
		}
	}
}

[Serializable]
public class QuestLog
{
	public Dictionary<QUESTNAME, int> questCompletionStates;
	public Dictionary<QUESTNAME, int> questProgressState;

	public QuestLog()
	{
		questCompletionStates = new Dictionary<QUESTNAME, int>();
		questProgressState = new Dictionary<QUESTNAME, int>();
	}
}