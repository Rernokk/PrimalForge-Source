using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_Base : ScriptableObject
{
	[SerializeField]
	protected List<string> objectiveText = new List<string>();
	protected string objectiveName;
	protected QUESTNAME questEnum;

	protected string objectiveIcon;
	protected int objectiveStage = 0;

	#region Properties
	public string ObjectiveName
	{
		get
		{
			return objectiveName;
		}

		set
		{
			objectiveName = value;
		}
	}

	public List<string> ObjectiveText
	{
		get
		{
			return objectiveText;
		}

		set
		{
			objectiveText = value;
		}
	}

	public string ObjectiveIcon
	{
		get
		{
			return objectiveIcon;
		}

		set
		{
			objectiveIcon = value;
		}
	}

	public QUESTNAME QuestEnumeration
	{
		get
		{
			return questEnum;
		}

		set
		{
			questEnum = value;
		}
	}

	public int QuestStage
	{
		get
		{
			return objectiveStage;
		}

		set
		{
			objectiveStage = value;
		}
	}
	#endregion

	public Objective_Base()
	{
		objectiveText = new List<string>();
		objectiveName = "Default Name!";
		objectiveIcon = null;
	}

	public virtual void PushStage()
	{
		if (Objective_Interface.instance.QuestCompletionStates[questEnum] == 0)
			return;

		objectiveStage++;
		Objective_Interface.instance.QuestProgressStates[questEnum] = objectiveStage;
		if (objectiveStage >= objectiveText.Count)
		{
			CompleteObjective();
		}
		else
		{
			Objective_Interface.instance.SaveQuestLog();
			Objective_Interface.instance.UpdateObjectiveInterface();
			Objective_Interface.instance.PlayProgressAnimation(this);
		}
	}

	public string FetchObjectiveText()
	{
		return objectiveText[objectiveStage];
	}

	public virtual void CompleteObjective()
	{
		Objective_Interface.instance.RemoveObjective(this);
		Objective_Interface.instance.CompleteObjective(QuestEnumeration);
		Debug.Log("Objective Completed: " + ObjectiveName);
	}
}
