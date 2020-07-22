using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

[System.Serializable]
public class Sound
{
	public string name;
	[FoldoutGroup("Details", false)]
	public AudioClip clip;

	[HideInInspector]
	public AudioSource source;

	[FoldoutGroup("Details")]
	public AudioMixerGroup mixerGroup;

	[FoldoutGroup("Details")]
	public bool loop, ignoreReverb;

	[FoldoutGroup("Details")]
	[Range(0f, 1f)]
	public float volume = 1f;

	[FoldoutGroup("Details")]
	[Range(.1f, 3f)]
	public float pitch = 1f;
}
