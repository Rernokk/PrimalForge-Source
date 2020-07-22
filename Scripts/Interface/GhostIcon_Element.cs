using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostIcon_Element : MonoBehaviour
{
	public bool FollowMouse = false;
	private CanvasGroup canvas;

	private void Awake()
	{
		canvas = GetComponent<CanvasGroup>();
	}

	public void ActivateTrack()
	{
		FollowMouse = true;
		canvas.alpha = 1;
	}

	public void DisableTrack()
	{
		FollowMouse = false;
		canvas.alpha = 0;
	}

	private void LateUpdate()
	{
		if (FollowMouse)
		{
			Vector2 tar = Input.mousePosition;
			tar.x *= 2f;
			tar.y *= 2f;
			GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
		}

		if (FollowMouse && !Input.GetKey(KeyCode.Mouse0))
		{
			DisableTrack();
		}
	}
}
