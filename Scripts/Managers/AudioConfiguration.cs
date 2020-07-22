using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioConfiguration : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Slider>().value = PlayerPrefs.GetFloat("MasterVolume");
	}
	public void SetMasterAudio()
	{
		PlayerPrefs.SetFloat("MasterVolume", GetComponent<Slider>().value);
		ConfigurationManager.instance.UpdateMasterAudio();
	}
}
