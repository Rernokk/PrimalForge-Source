using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal enum AttackState { FIRE, ICE, DEFENSE }
public class DancerController : EnemyObject
{
	[SerializeField]
	private AttackState currentState = AttackState.DEFENSE, previousState = AttackState.DEFENSE;
	private ElementalResistances reflectElementA, reflectElementB;
	private Slider specialBar;

	[Header("Dancer Properties")]
	[SerializeField]
	private DancerObeliskController obeliskController;

	[SerializeField]
	private DancerFireController leftFireController, rightFireController;

	[SerializeField]
	private DancerIceController iceController;

	[SerializeField]
	private bool isEnraged = false;

	[SerializeField]
	private float reflectRatio = .5f, transformDuration = 10f, increaseLimit = 30;

	[SerializeField]
	[Header("Active Properties")]
	private float transformProgress = 0f;

	private int specDirection = 1, increaseHitCount = 0;

	#region Properties
	public bool IsEnraged
	{
		get
		{
			return isEnraged;
		}

		set
		{
			isEnraged = value;
		}
	}

	public float TransformProgress
	{
		get
		{
			return transformProgress;
		}

		set
		{
			specialBar.value = transformProgress;
			transformProgress = value;
		}
	}
	#endregion

	protected override void Start()
	{
		base.Start();
		HealthBar = transform.Find("Health_Bar/Background/Slider").GetComponent<Slider>();
		specialBar = transform.Find("Health_Bar/Background/Special").GetComponent<Slider>();
		obeliskController.DancerRootReference = this;
		leftFireController.DancerRootReference = this;
		rightFireController.DancerRootReference = this;
		iceController.DancerRootReference = this;

		obeliskController.ElementalActive = true;
		iceController.ElementalActive = false;
		leftFireController.ElementalActive = false;
		rightFireController.ElementalActive = false;

		List<ElementalResistances> availableElements = new List<ElementalResistances>();
		for (int i = 0; i < System.Enum.GetValues(typeof(ElementalResistances)).Length; i++)
		{
			availableElements.Add((ElementalResistances)i);
		}

		reflectElementA = availableElements[Random.Range(0, availableElements.Count)];
		availableElements.Remove(reflectElementA);

		reflectElementB = availableElements[Random.Range(0, availableElements.Count)];

		Color reflectA = (reflectElementA == ElementalResistances.FIRE ? Color.red : reflectElementA == ElementalResistances.ICE ? Color.cyan : reflectElementA == ElementalResistances.SHOCK ? Color.blue : reflectElementA == ElementalResistances.POISON ? Color.green : reflectElementA == ElementalResistances.PHYSICAL ? Color.yellow : Color.magenta);
		Color reflectB = (reflectElementB == ElementalResistances.FIRE ? Color.red : reflectElementB == ElementalResistances.ICE ? Color.cyan : reflectElementB == ElementalResistances.SHOCK ? Color.blue : reflectElementB == ElementalResistances.POISON ? Color.green : reflectElementB == ElementalResistances.PHYSICAL ? Color.yellow : Color.magenta);
		obeliskController.transform.Find("PrimarySprite/TR_Corner").GetComponent<SpriteRenderer>().color = obeliskController.transform.Find("PrimarySprite/BL_Corner").GetComponent<SpriteRenderer>().color = reflectA;
		obeliskController.transform.Find("PrimarySprite/BR_Corner").GetComponent<SpriteRenderer>().color = obeliskController.transform.Find("PrimarySprite/TL_Corner").GetComponent<SpriteRenderer>().color = reflectB;
	}

	protected override void Update()
	{
		base.Update();
		if (attackTimer > 0)
		{
			attackTimer -= Time.deltaTime;
		}
		else
		{
			AttackTarget();
		}

		TransformProgress += specDirection * Time.deltaTime / transformDuration;
		if ((TransformProgress >= 1f && specDirection == 1) || (TransformProgress < 0 && specDirection == -1))
		{
			if (TransformProgress < 0)
			{
				specDirection = 1;
			}
			else
			{
				specDirection = -1;
			}
			TransformState();
		}
	}

	public override void TakeDamage(float damageAmount, ElementalResistances type)
	{
		if (currentState == AttackState.DEFENSE)
		{
			base.TakeDamage(damageAmount * .5f, type);
			if (type == reflectElementA || type == reflectElementB)
			{
				player.GetComponent<Player_Details_Script>().TakeDamage(reflectRatio * damageAmount, ElementalResistances.TRUE);
			}
			TransformProgress += .01f;
		}
		else
		{
			if ((currentState == AttackState.ICE && type == ElementalResistances.FIRE) || (currentState == AttackState.FIRE && type == ElementalResistances.ICE))
			{
				base.TakeDamage(damageAmount * 1.2f, type);
			}
			else
			{
				base.TakeDamage(damageAmount, type);
			}
			if (increaseHitCount < increaseLimit)
			{
				transformProgress += .01f;
				increaseHitCount++;
			}
		}
	}

	protected override void AttackTarget()
	{
		if (attackTimer <= 0)
		{
			switch (currentState)
			{
				case (AttackState.DEFENSE):
					obeliskController.ActivateAttack();
					break;
				case (AttackState.FIRE):
					leftFireController.ActivateAttack();
					rightFireController.ActivateAttack();
					break;
				case (AttackState.ICE):
					iceController.ActivateAttack();
					break;
			}
		}
	}

	public void SetAttackTimer(float amnt)
	{
		attackTimer = amnt;
	}

	public void TransformState()
	{
		//TransformProgress = Mathf.RoundToInt(TransformProgress);
		if (currentState == AttackState.DEFENSE)
		{
			obeliskController.AttackDuration = 0f;
			obeliskController.transform.Find("PrimarySprite/TR_Corner").GetComponent<SpriteRenderer>().color = obeliskController.transform.Find("PrimarySprite/BL_Corner").GetComponent<SpriteRenderer>().color = Color.white;
			obeliskController.transform.Find("PrimarySprite/BR_Corner").GetComponent<SpriteRenderer>().color = obeliskController.transform.Find("PrimarySprite/TL_Corner").GetComponent<SpriteRenderer>().color = Color.white;
			if (previousState == AttackState.FIRE)
			{
				previousState = currentState;
				currentState = AttackState.ICE;
				iceController.ElementalActive = true;
				obeliskController.ElementalActive = false;
				leftFireController.ElementalActive = false;
				rightFireController.ElementalActive = false;
			}
			else if (previousState == AttackState.ICE)
			{
				previousState = currentState;
				currentState = AttackState.FIRE;
				iceController.ElementalActive = false;
				obeliskController.ElementalActive = false;
				if (IsEnraged)
				{
					leftFireController.ElementalActive = true;
					rightFireController.ElementalActive = true;
				}
				else
				{
					if (Random.Range(0f, 1f) > .5f)
					{
						leftFireController.ElementalActive = true;
						rightFireController.ElementalActive = false;
					}
					else
					{
						leftFireController.ElementalActive = false;
						rightFireController.ElementalActive = true;
					}
				}
			}
			else
			{
				previousState = currentState;
				currentState = Random.Range(0f, 1f) > .5f ? AttackState.ICE : AttackState.FIRE;

				iceController.ElementalActive = false;
				obeliskController.ElementalActive = false;
				leftFireController.ElementalActive = false;
				rightFireController.ElementalActive = false;

				if (currentState == AttackState.FIRE)
				{
					if (Random.Range(0f, 1f) > .5f)
					{
						leftFireController.ElementalActive = true;
						rightFireController.ElementalActive = false;
					}
					else
					{
						leftFireController.ElementalActive = false;
						rightFireController.ElementalActive = true;
					}
				}
				else
				{
					iceController.ElementalActive = true;
				}
			}
		}
		else
		{
			increaseHitCount = 0;
			previousState = currentState;
			currentState = AttackState.DEFENSE;
			iceController.ElementalActive = false;
			obeliskController.ElementalActive = true;
			leftFireController.ElementalActive = false;
			rightFireController.ElementalActive = false;

			List<ElementalResistances> availableElements = new List<ElementalResistances>();
			for (int i = 0; i < System.Enum.GetValues(typeof(ElementalResistances)).Length; i++)
			{
				availableElements.Add((ElementalResistances)i);
			}

			reflectElementA = availableElements[Random.Range(0, availableElements.Count)];
			availableElements.Remove(reflectElementA);

			reflectElementB = availableElements[Random.Range(0, availableElements.Count)];

			Color reflectA = (reflectElementA == ElementalResistances.FIRE ? Color.red : reflectElementA == ElementalResistances.ICE ? Color.cyan : reflectElementA == ElementalResistances.SHOCK ? Color.blue : reflectElementA == ElementalResistances.POISON ? Color.green : reflectElementA == ElementalResistances.PHYSICAL ? Color.yellow : Color.magenta);
			Color reflectB = (reflectElementB == ElementalResistances.FIRE ? Color.red : reflectElementB == ElementalResistances.ICE ? Color.cyan : reflectElementB == ElementalResistances.SHOCK ? Color.blue : reflectElementB == ElementalResistances.POISON ? Color.green : reflectElementB == ElementalResistances.PHYSICAL ? Color.yellow : Color.magenta);
			obeliskController.transform.Find("PrimarySprite/TR_Corner").GetComponent<SpriteRenderer>().color = obeliskController.transform.Find("PrimarySprite/BL_Corner").GetComponent<SpriteRenderer>().color = reflectA;
			obeliskController.transform.Find("PrimarySprite/BR_Corner").GetComponent<SpriteRenderer>().color = obeliskController.transform.Find("PrimarySprite/TL_Corner").GetComponent<SpriteRenderer>().color = reflectB;
		}
	}

	protected override void ActionOnDeath()
	{
		//base.ActionOnDeath();
		Facepunch.Steamworks.Client.Instance.Stats.Set("CK_FROSTFLARE", 1);
		Destroy(gameObject);
	}
}
