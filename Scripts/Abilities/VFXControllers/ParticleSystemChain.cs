using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleSystemChain : MonoBehaviour
{
	[SerializeField]
	UnityEvent eventToTrigger;

	public void PlayParticleSystem()
	{
		GetComponent<ParticleSystem>().Play();
	}

	public void OnParticleSystemStopped()
	{
		eventToTrigger.Invoke();
	}
}
