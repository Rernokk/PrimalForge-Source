using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Anim_Controller_Test : MonoBehaviour
{
	[SerializeField]
	private Transform modelRoot;

	[SerializeField]
	private Animator animCtrller;

	private void Update()
	{
		Vector2 dirInput = Vector2.zero;
		if (Input.GetKey(KeyCode.W))
		{
			dirInput += Vector2.up;
		}
		if (Input.GetKey(KeyCode.S))
		{
			dirInput += Vector2.down;
		}
		if (Input.GetKey(KeyCode.A))
		{
			dirInput += Vector2.left;
		}
		if (Input.GetKey(KeyCode.D))
		{
			dirInput += Vector2.right;
		}

		if (dirInput.magnitude > 0)
		{
			dirInput.Normalize();
			Debug.DrawRay(transform.position, dirInput * 3f, Color.red);
			dirInput.y *= -1;
			modelRoot.localRotation = Quaternion.Euler(0, Vector2.SignedAngle(Vector2.down, dirInput), 0);
			animCtrller.Play("Sprint_0");
		} else
		{
			animCtrller.Play("Idle_0");
		}
	}
}
