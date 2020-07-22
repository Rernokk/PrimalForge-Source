using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SithRok : EnemyObject
{
	[SerializeField]
	private GameObject stalactitePrefab;

	[SerializeField]
	private int phaseState = 0;

	[SerializeField]
	private Collider2D L_StompArea, R_StompArea;

	[SerializeField]
	Collider2D FightBarrier;

	private Animator anim;
	private delegate void ActionSet();
	private List<ActionSet> availableActions = new List<ActionSet>();

	[SerializeField]
	private bool isInIdleAnimation = false;

	[SerializeField]
	LineRenderer left_SpineRender, right_SpineRender, middle_SpineRender;

	[SerializeField]
	private GameObject P4MinionRef;
	private float MinionSummonTimer = 0f;

	#region Properties
	#endregion

	protected override void Start()
	{
		base.Start();
		HealthBar = transform.Find("Health_Bar/Background/Slider").GetComponent<Slider>();
		anim = GetComponent<Animator>();
		attackTimer = AttackSpeed;
		left_SpineRender.positionCount = 2;
	}

	protected override void Update()
	{
		base.Update();

		if (attackTimer <= 0)
		{
			AttackTarget();
		}

		if (HealthPercent < .75f && phaseState == 1)
		{
			PushPhase();
		}
		else if (HealthPercent < .75f && phaseState == 1)
		{
			PushPhase();
		}
		else if (HealthPercent < .5f && phaseState == 2)
		{
			PushPhase();
		}
		else if (HealthPercent < .25f && phaseState == 3)
		{
			PushPhase();
		}
		left_SpineRender.SetPosition(1, (transform.Find("L_Claw").position - left_SpineRender.transform.position));
		right_SpineRender.SetPosition(1, (transform.Find("R_Claw").position - right_SpineRender.transform.position));
		middle_SpineRender.SetPosition(1, (transform.Find("Head").position - middle_SpineRender.transform.position));
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (Player_Accessor_Script.DetailsScript.CurrentHealth <= 0)
		{
			ResetFight();
		}
	}

	protected override void AttackTarget()
	{
		bool hasAttacked = false;
		if (!isInIdleAnimation)
		{
			switch (phaseState)
			{
				case (0):
					break;
				case (1):
					availableActions[Random.Range(0, availableActions.Count)]();
					hasAttacked = true;
					break;
				case (2):
					availableActions[Random.Range(0, availableActions.Count)]();
					hasAttacked = true;
					break;
				case (3):
					availableActions[Random.Range(0, availableActions.Count)]();
					hasAttacked = true;
					break;
				case (4):
					availableActions[Random.Range(0, availableActions.Count)]();
					hasAttacked = true;
					break;
				default:
					print("Boss Broken.");
					break;
			}

			if (phaseState == 4)
			{
				if (MinionSummonTimer <= 0)
				{
					Instantiate(P4MinionRef, BitmaskManager.instance.OpenTiles[Random.Range(0, BitmaskManager.instance.OpenTiles.Count)].transform.position, Quaternion.identity);
				}
			}
		}
		if (hasAttacked)
		{
			attackTimer = AttackSpeed;
		}
	}

	public void PushPhase()
	{
		if (availableActions == null)
		{
			availableActions = new List<ActionSet>();
		}

		switch (phaseState)
		{
			case (0):
				//availableActions.Add(StompLeftClaw);
				//availableActions.Add(StompRightClaw);
				availableActions.Add(StompLeftAnimation);
				availableActions.Add(StompRightAnimation);
				break;
			case (1):
				availableActions.Add(BreatheFireAnimation);
				transform.Find("Head/L_Eye_Glow").gameObject.SetActive(true);
				transform.Find("Head/R_Eye_Glow").gameObject.SetActive(true);
				//transform.Find("L_Claw/L_Eye_Glow").gameObject.SetActive(true);
				//transform.Find("L_Claw/R_Eye_Glow").gameObject.SetActive(true);
				//transform.Find("R_Claw/L_Eye_Glow").gameObject.SetActive(true);
				//transform.Find("R_Claw/R_Eye_Glow").gameObject.SetActive(true);

				transform.Find("Head/L_Eye_Flash").gameObject.SetActive(true);
				transform.Find("Head/R_Eye_Flash").gameObject.SetActive(true);
				//transform.Find("L_Claw/L_Eye_Flash").gameObject.SetActive(true);
				//transform.Find("L_Claw/R_Eye_Flash").gameObject.SetActive(true);
				//transform.Find("R_Claw/L_Eye_Flash").gameObject.SetActive(true);
				//transform.Find("R_Claw/R_Eye_Flash").gameObject.SetActive(true);
				break;
			case (2):
				availableActions.Add(SwipeLeftClaw);
				availableActions.Add(SwipeRightClaw);
				transform.Find("R_Claw/T3Enrage").gameObject.SetActive(true);
				transform.Find("L_Claw/T3Enrage").gameObject.SetActive(true);
				break;
			case (3):
				break;
		}
		phaseState++;
		print("Phase Pushed: " + phaseState);
	}

	protected override void ActionOnDeath()
	{
		Facepunch.Steamworks.Client.Instance.Stats.Set("CK_SITHROK", 1);
		print(Facepunch.Steamworks.Client.Instance.Stats.GetInt("CK_SITHROK"));
		FightBarrier.enabled = false;
	}

	private void StompLeftAnimation()
	{
		anim.Play("L_StompAnim");
	}

	private void StompRightAnimation()
	{
		anim.Play("R_StompAnim");
	}

	private void StompLeftClaw()
	{
		print("Stomping left claw.");
		TriggerImpactShake(.5f, .5f);
		if (L_StompArea.OverlapPoint(player.transform.position))
		{
			Player_Accessor_Script.DetailsScript.TakeDamage(100f, ElementalResistances.PHYSICAL);
		}
		if (phaseState > 1)
		{
			if (phaseState >= 3)
			{
				CrashStalactites(24);
			}
			else
			{
				CrashStalactites(12);
			}
		}
	}

	private void StompRightClaw()
	{
		print("Stomping right claw.");
		TriggerImpactShake(.5f, .5f);
		if (R_StompArea.OverlapPoint(player.transform.position))
		{
			Player_Accessor_Script.DetailsScript.TakeDamage(100f, ElementalResistances.PHYSICAL);
		}
		if (phaseState > 1)
		{
			if (phaseState >= 3)
			{
				CrashStalactites(24);
			}
			else
			{
				CrashStalactites(12);
			}
		}
	}

	private void BreatheFireAnimation()
	{
		anim.Play("M_BreatheFire");
	}

	private void SwipeLeftClaw()
	{
		print("Swiping Left Claw");
		anim.Play("L_SwipeAnim");
	}

	private void SwipeRightClaw()
	{
		print("Swiping Right Claw");
		anim.Play("R_SwipeAnim");
	}

	private void SpawnSkeleton()
	{
		print("Spawning Skeletons");
	}

	private void CrashStalactites(int amnt)
	{
		print("Crashing Stalactites");
		List<int> targetTiles = new List<int>();
		while (targetTiles.Count < amnt)
		{
			int val = Random.Range(0, BitmaskManager.instance.OpenTiles.Count);
			if (!targetTiles.Contains(val))
			{
				targetTiles.Add(val);
			}
		}

		for (int i = 0; i < amnt; i++)
		{
			Instantiate(stalactitePrefab, BitmaskManager.instance.OpenTiles[targetTiles[i]].transform.position, Quaternion.identity);
		}
	}

	private void TriggerImpactShake(float dur, float mag)
	{
		Camera.main.GetComponent<Camera_Controller_Script>().TriggerShake(dur, mag);
	}

	public void EnterIdleState()
	{
		isInIdleAnimation = true;
	}

	public void ExitIdleState()
	{
		isInIdleAnimation = false;
	}

	public void PlayFightStartAnimation()
	{
		anim.Play("FightStartAnim");
	}

	public void ResetFight()
	{
		currentHealth = MaxHealth;
		phaseState = 0;
		FightBarrier.enabled = false;
		anim.Play("PreFightAnimation");
	}
}
