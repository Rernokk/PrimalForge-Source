using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillVFXController : MonoBehaviour {

	Transform myVFX;
	void Start()
	{
		myVFX = transform.Find("Group");
		myVFX.gameObject.SetActive(false);
	}

	public void ToggleVFX()
	{
		myVFX.gameObject.SetActive(!myVFX.gameObject.activeSelf);
	}

	public void DisableVFX()
	{
		myVFX.gameObject.SetActive(false);
	}
}
