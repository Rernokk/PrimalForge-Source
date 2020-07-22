using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementalResistances { FIRE, ICE, EXPLOSIVE, SHOCK, POISON, VOID, ARCANE, SPECTRAL, PHYSICAL, TRUE }

public class Player_Details_Script : AttackableEntity
{
	private SpawnShrine lastShrine;
	private float currentResource = 0, maxResource = 100;

	[SerializeField]
	private float manaRegenRate;
	private Animator animController;

	protected override void Start()
	{
		base.Start();
		currentResource = maxResource;
		animController = transform.Find("Model/Root").GetComponent<Animator>();
	}

	public float ResourcePercent
	{
		get
		{
			return currentResource / maxResource;
		}
	}

	public float MaxHealth
	{
		get
		{
			return maxHealth;
		}

		set
		{
			maxHealth = 100 + value;
			if (currentHealth > maxHealth)
			{
				currentHealth = maxHealth;
			}
		}
	}

	public float MaxResource
	{
		get
		{
			return maxResource;
		}

		set
		{
			maxResource = value;
		}
	}
	public float CurrentResource
	{
		get
		{
			return currentResource;
		}

		set
		{
			currentResource = value;
		}
	}

	private float ExperienceCalc(int level)
	{
		float val = 0;
		if (level > 0)
		{
			val += ExperienceCalc(level - 1);
			val = Mathf.Round((300 + (val * .05f) + Mathf.Pow(level, 3) / 6));
			return val;
		}
		else
		{
			return val;
		}
	}
	public override void TakeDamage(float damageAmount, ElementalResistances type)
	{
		base.TakeDamage(damageAmount, type);
		if (currentHealth <= 0)
		{


			if (Player_Accessor_Script.EquipmentScript.PlayerHasControl)
			{
				Player_Accessor_Script.EquipmentScript.PlayerHasControl = false;
				animController.Play("Dying");
			}
		}
	}

	public void Respawn()
	{
		//Respawning at Shrine
		SpawnShrine.ActiveShrines.Sort((q1, q2) => Vector2.Distance(q1.transform.position, transform.position).CompareTo(Vector2.Distance(q2.transform.position, transform.position)));
		transform.position = SpawnShrine.ActiveShrines[0].transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * 2.0f;
		currentHealth = .7f * MaxHealth;
		currentResource = .7f * maxResource;
		foreach (string s in AbilityLibrary.abilityDictionary.Keys)
		{
			AbilityLibrary.abilityDictionary[s].ResetCoolDown();
		}
		Camera.main.GetComponent<Camera_Controller_Script>().TriggerUnfade(1f);
	}

	protected override void Update()
	{
		base.Update();
		RegenerateMana(manaRegenRate * Time.deltaTime);
	}

	public void RegenerateMana(float manaTick)
	{
		currentResource += manaTick;
		if (currentResource > maxResource)
		{
			currentResource = maxResource;
		}
	}

	public bool CanSpendMana(float amount)
	{
		return currentResource >= amount;
	}

	public void SpendMana(float amnt)
	{
		currentResource -= amnt;
	}

	public override void HealSelf(float healingAmount)
	{
		if (Player_Accessor_Script.EquipmentScript.ActivePerks.Contains(LegendaryPerk.Etherias_Gift))
		{
			base.HealSelf(healingAmount * .75f);
			foreach (Minion myMinion in CommandUndead.activeUndead)
			{
				myMinion.HealSelf(healingAmount * .25f);
			}
			if (FleshConstruct.ReservedConstruct != null)
			{
				FleshConstruct.ReservedConstruct.HealSelf(healingAmount * .25f);
			}
		}
		else
		{
			base.HealSelf(healingAmount);
		}
	}

	public override void Knockback(Vector3 direction, float magnitude)
	{
		base.Knockback(direction, magnitude);
		StartCoroutine(Player_Accessor_Script.EquipmentScript.StunPlayerControl(1f));
		if ((AbilityLibrary.abilityDictionary["Lunging Swipe"] as Lungingswipe).dashRoutine != null)
		{
			StopCoroutine((AbilityLibrary.abilityDictionary["Lunging Swipe"] as Lungingswipe).dashRoutine);
			(AbilityLibrary.abilityDictionary["Lunging Swipe"] as Lungingswipe).dashRoutine = null;
		}
	}
}
