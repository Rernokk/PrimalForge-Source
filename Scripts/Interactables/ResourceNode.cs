using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public struct ResourceData
{
	public Skills skillRef;
	public int rewardedExp;
	public Reagent reagentData;
}
public class ResourceNode : Interactable
{
	public string reagentName;
	public float interactionRange;
	public int experienceGranted = 1;
	public int resourceQuantity = 1;
	public int levelReq = 1;
	public ToolType requiredToolType;
	protected Player_Skills_Script playerSkills;
	protected Skills skillExperience = Skills.Lumbering;
	protected AudioSource audioSource;
	//protected EventTrigger et;

	[SerializeField]
	protected bool limitedResource = false;

	[ShowIf("limitedResource")]
	[SerializeField]
	protected int resourceAmount = 10;

	#region Properties
	public int ResourceAmount
	{
		get
		{
			return resourceAmount;
		}
		set
		{
			resourceAmount = value;
		}
	}
	#endregion

	protected void AssignToHolder(string Name)
	{
		Transform nodeParent = null;
		if (GameObject.Find(Name) == null)
		{
			nodeParent = new GameObject(Name).transform;
			if (GameObject.Find("Resource_Master") == null)
			{
				nodeParent.parent = new GameObject("Resource_Master").transform;
			}
			else
			{
				nodeParent.parent = GameObject.Find("Resource_Master").transform;
			}
		}
		else
		{
			nodeParent = GameObject.Find(Name).transform;
		}
		transform.parent = nodeParent;
		transform.tag = "ResourceNode";
	}

	public virtual ResourceData Interact()
	{
		ResourceData resDat = new ResourceData();
		CraftableReagent tarReagent = EquipmentCrafting_Controller.instance.CraftableReagents.Find((q) => q.ItemName == reagentName);
		if (tarReagent == null)
		{
			print("Reagent not found");
			return resDat;
		}

		if (levelReq > playerSkills.GetSkill(skillExperience).currLevel)
		{
			return resDat;
		}
		resDat.skillRef = skillExperience;
		if (audioSource.clip != null)
		{
			audioSource.Play();
		}

		if (limitedResource)
		{
			if (resourceAmount >= resourceQuantity)
			{
				resourceAmount -= resourceQuantity;
			}
			else
			{
				resourceAmount = 0;
			}
		}
		resDat.reagentData = new Reagent();
		resDat.reagentData.ReagentName = reagentName;
		resDat.reagentData.ReagentQuantity = resourceQuantity;
		resDat.reagentData.ReagentSprite = tarReagent.SpritePath;
		resDat.rewardedExp = experienceGranted;
		return resDat;
	}

	public bool IsNodeInRange(Vector3 pos)
	{
		return Vector2.Distance(pos, transform.position) <= interactionRange;
	}

	public bool IsCorrectTool(ToolType type)
	{
		return requiredToolType == type;
	}

	protected virtual void Awake()
	{
		//et = gameObject.AddComponent<EventTrigger>();
	}

	protected virtual void Start()
	{
		playerSkills = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Skills_Script>();
		//GetComponent<SpriteRenderer>().sortingOrder = -(int)(transform.position.y*10);
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = gameObject.AddComponent<AudioSource>();
		}
		audioSource.volume = .3f;
		audioSource.playOnAwake = false;
		audioSource.outputAudioMixerGroup = AudioManager.instance.MasterMixer.FindMatchingGroups("Master/Ambient")[0];

		//et = GetComponent<EventTrigger>();
		//EventTrigger.Entry etEnter = new EventTrigger.Entry();
		//etEnter.eventID = EventTriggerType.PointerDown;
		//etEnter.callback.AddListener((data) => { MouseEnterAction((PointerEventData) data); print("MouseEnterCalled!"); });
		//et.triggers.Add(etEnter);

		//EventTrigger.Entry etExit = new EventTrigger.Entry();
		//etExit.eventID = EventTriggerType.PointerUp;
		//etExit.callback.AddListener((data) => { MouseExitAction((PointerEventData)data); print("MouseExitCalled!"); });
		//et.triggers.Add(etExit);
	}

	public void OnMouseOver()
	{
		GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
	}

	public void OnMouseExit()
	{
		GetComponentInChildren<SpriteRenderer>().color = Color.white;
	}

	protected void Update()
	{
		if (limitedResource && resourceAmount == 0)
		{
			Destroy(gameObject, Time.deltaTime);
		}
	}
}
