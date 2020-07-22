using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller_Script : MonoBehaviour
{
	[SerializeField] [Header("Player Movement Attributes")] private float playerSpeed;
	[SerializeField] private Transform directionalArrowReference;

	private float currentPlayerSpeed;
	private float speedMultiplier = 1f;
	private Rigidbody2D rgd;
	private Player_Equipment_Script equipmentScript;
	private SpriteRenderer sprite;
	private bool isCasting;
	private Animator animController;
	private Transform vfxTransform;
	private float speedPercentage = 0f;

	#region Properties
	public float PlayerSpeedMultiplier
	{
		get
		{
			return speedMultiplier;
		}
		set
		{
			speedMultiplier = value;
		}
	}
	public float CurrentPlayerSpeed
	{
		get
		{
			return currentPlayerSpeed * speedMultiplier;
		}
		set
		{
			currentPlayerSpeed = value;
		}
	}
	public float PlayerSpeed
	{
		get
		{
			return playerSpeed;
		}
	}
	public bool IsCasting
	{
		get
		{
			return isCasting;
		}

		set
		{
			isCasting = value;
		}
	}
	public Rigidbody2D Rgd2D
	{
		get
		{
			return rgd;
		}
	}
	#endregion
	private void Start()
	{
		rgd = GetComponent<Rigidbody2D>();
		equipmentScript = GetComponent<Player_Equipment_Script>();
		sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		//animController = transform.Find("Player_Anim_Root").GetComponent<Animator>();
		animController = transform.Find("Model/Root").GetComponent<Animator>();
		vfxTransform = transform.Find("Model/FX_Root/Anchor");
	}

	// Update is called once per frame
	private void Update()
	{
		if (equipmentScript.PlayerHasControl)
		{
			Vector2 movementVector = Vector2.zero;
			if (Input.GetKey(KeybindManager.Keybinds[KeybindFunction.A_MOVEUP]))
			{
				movementVector += (Vector2.up);
			}
			if (Input.GetKey(KeybindManager.Keybinds[KeybindFunction.A_MOVERIGHT]))
			{
				movementVector += (Vector2.right);
			}
			if (Input.GetKey(KeybindManager.Keybinds[KeybindFunction.A_MOVEDOWN]))
			{
				movementVector += (Vector2.down);
			}
			if (Input.GetKey(KeybindManager.Keybinds[KeybindFunction.A_MOVELEFT]))
			{
				movementVector += (Vector2.left);
			}

			if (IsCasting && movementVector != Vector2.zero)
			{
				IsCasting = false;
			}

			rgd.velocity = movementVector.normalized * playerSpeed * speedMultiplier;
			//sprite.sortingOrder = -(int)(transform.position.y * 10);

			//if (!Player_Accessor_Script.AnimatorScript.IsCastingAnimation)
			//{
			//	if (rgd.velocity.magnitude == 0)
			//	{
			//		if (equipmentScript.EquippedWeapon != null)
			//		{
			//			animController.Play(equipmentScript.EquippedWeapon.Type + "_IdleAnim");
			//		} else
			//		{
			//			animController.Play("_IdleAnim");
			//		}
			//	}
			//	else
			//	{
			//		if (equipmentScript.EquippedWeapon != null)
			//		{
			//			animController.Play(equipmentScript.EquippedWeapon.Type + "_WalkAnim");
			//		} else
			//		{
			//			animController.Play("_WalkAnim");
			//		}
			//		if (rgd.velocity.x < 0)
			//		{
			//			transform.Find("Player_Anim_Root").localScale = new Vector3(-3, 3, 3);
			//		}
			//		else
			//		{
			//			transform.Find("Player_Anim_Root").localScale = new Vector3(3, 3, 3);
			//		}
			//	}
			//}

			if (rgd.velocity.magnitude > 0)
			{
				Vector2 motionDir = rgd.velocity;
				animController.transform.localRotation = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, motionDir), 0);
				vfxTransform.localRotation = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, motionDir), 0);
				speedPercentage += Time.deltaTime * 4f;
				rgd.velocity = rgd.velocity.normalized * playerSpeed * speedPercentage;
			}
			else
			{
				speedPercentage -= Time.deltaTime * 4f;
			}
			speedPercentage = Mathf.Clamp01(speedPercentage);
			animController.SetFloat("WalkRate", speedPercentage);

			//Redirecting Indication by Mouse
			Vector3 val = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
			val.z = 0;
			directionalArrowReference.localRotation = Quaternion.Euler(0, -Vector3.SignedAngle(Vector3.up, val, Vector3.forward), 0);
		}
	}
}
