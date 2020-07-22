using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class Ability : MonoBehaviour
{
	public Sprite skillIcon;
	public string abilityName;

	public float cooldownMax = 3f;
	public float manaCost = 10f;
	public bool isCooledDown = true;
	public bool isChargeSkill = false;
	public bool isSelected = false;

	protected Rigidbody2D playerRigid;
	protected Player_Equipment_Script playerEquipment;
	protected AudioSource audioSource;
	protected Animator playerAnimator;

	protected bool castingSpell = false;
	protected bool isGCD = false;
	protected static float GCDScale = 1.0f;
	private Coroutine routine;
	private float cooldownTimer;

	[SerializeField]
	protected float castTime = .5f;

	[SerializeField]
	protected float mightRatio = 1.0f, intelRatio = 1.0f, dexRatio = 1.0f;

	protected float GCDMax = 1.5f;
	protected float castTick;

	protected Skills skillRequirement = Skills.Swordsmanship;
	protected int skillLevelReq = 1;

	protected string skillDescription = "This skill does stuff and things.";
	protected bool modifiedByStats = true;

	#region Properties
	public bool ModifiedByStats
	{
		get
		{
			return modifiedByStats;
		}

		set
		{
			modifiedByStats = value;
		}
	}
	public string SkillDescription
	{
		get
		{
			return skillDescription;
		}

		set
		{
			skillDescription = value;
		}
	}
	public float CooldownRemaining
	{
		get
		{
			if (isGCD)
			{
				return cooldownTimer / GCDMax;
			}

			return cooldownTimer / cooldownMax;
		}
		set
		{
			cooldownTimer = value;
		}
	}
	public bool IsCooledDown
	{
		get
		{
			return isCooledDown;
		}

		set
		{
			isCooledDown = value;
		}
	}
	public float CastTime
	{
		get
		{
			return castTime;
		}
	}
	public float CastTick
	{
		get
		{
			return castTick;
		}
	}
	public bool CastingSpell
	{
		get
		{
			return castingSpell;
		}

		set
		{
			playerAnimator.SetBool("WasCasting", castingSpell);
			castingSpell = value;
			playerAnimator.SetBool("CastingSpell", value);
		}
	}
	public static float GCDScaleValue
	{
		get
		{
			return GCDScale;
		}
		set
		{
			GCDScale = value;
		}
	}
	public int SkillLevelRequirement
	{
		get
		{
			return skillLevelReq;
		}
		set
		{
			skillLevelReq = value;
		}
	}
	public Skills SkillRequired
	{
		get
		{
			return skillRequirement;
		}

		set
		{
			skillRequirement = value;
		}
	}

	public float MightRatio
	{
		get
		{
			return mightRatio;
		}
		set
		{
			mightRatio = value;
		}
	}

	public float DexterityRatio
	{
		get
		{
			return dexRatio;
		}
		set
		{
			dexRatio = value;
		}
	}

	public float IntelligenceRatio
	{
		get
		{
			return intelRatio;
		}
		set
		{
			intelRatio = value;
		}
	}

	#endregion

	protected Vector3 CalculateDirectionToMouse()
	{
		Vector3 val = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
		val.z = transform.parent.position.z;
		return val.normalized;
	}

	public void TriggerGCD()
	{
		if (isCooledDown && Player_Accessor_Script.SkillsScript.GetSkill(skillRequirement).currLevel >= skillLevelReq)
		{
			IsCooledDown = false;
			cooldownTimer = GCDMax * GCDScale;
			routine = StartCoroutine(CoolDown());
			isGCD = true;
		}
	}

	public virtual bool UseAbility()
	{
		if (isCooledDown && Player_Accessor_Script.SkillsScript.GetSkill(skillRequirement).currLevel >= skillLevelReq)
		{
			IsCooledDown = false;
			cooldownTimer = cooldownMax;
			routine = StartCoroutine(CoolDown());
			Player_Accessor_Script.DetailsScript.SpendMana(manaCost);
			AbilityLibrary.AbilityEvents[abilityName].Invoke();
			return true;
		}
		return false;
	}

	public void ResetCoolDown()
	{
		cooldownTimer = 0;
		isCooledDown = true;
		if (routine != null)
		{
			StopCoroutine(routine);
			routine = null;
		}
	}

	protected IEnumerator CoolDown()
	{
		while (cooldownTimer > 0)
		{
			cooldownTimer -= Time.deltaTime;
			yield return null;
		}
		isCooledDown = true;
		cooldownTimer = 0;
		if (isGCD)
		{
			isGCD = false;
		}
	}

	public static void DebugDrawRadius(Vector3 pos, float radius, Color col, float dur)
	{
		int rayCount = 32;
		for (int i = 0; i < rayCount; i++)
		{
			Debug.DrawRay(pos, (new Vector3(Mathf.Cos(i * (360 / rayCount) * Mathf.Deg2Rad), Mathf.Sin(i * (360 / rayCount) * Mathf.Deg2Rad), 0)).normalized * radius, col, dur);
		}
	}

	protected virtual void Start()
	{
		cooldownTimer = 0;
		playerRigid = transform.parent.GetComponent<Rigidbody2D>();
		playerEquipment = transform.parent.GetComponent<Player_Equipment_Script>();
		playerAnimator = playerEquipment.animController;
		//(Resources.Load("AbilityIcons/Cleave") as Texture2D)
		Texture2D spriteVal = (Resources.Load("AbilityIcons/" + abilityName) as Texture2D);
		if (spriteVal != null)
		{
			skillIcon = Sprite.Create(spriteVal, new Rect(0, 0, spriteVal.width, spriteVal.height), new Vector2(.5f, .5f));
		}
		else
		{
			skillIcon = null;
		}
		audioSource = GetComponent<AudioSource>();
	}

	protected virtual void InterruptCasting()
	{
		CastingSpell = false;
		castTick = 0;
		Player_Accessor_Script.AbilitiesInterface.EndCast();
		playerAnimator.SetBool("WasCasting", false);
		playerAnimator.Play("FullCastLayer.Empty");
	}
}
