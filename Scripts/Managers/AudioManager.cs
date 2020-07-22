using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;
	public AudioMixer MasterMixer;

	[Button("Sort Audio Files")]
	public void SortSoundList()
	{
		sounds.Sort((q1, q2) => q1.name.CompareTo(q2.name));
	}

	public List<Sound> sounds;


	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(this);
			return;
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
			s.source.bypassReverbZones = s.ignoreReverb;
			s.source.outputAudioMixerGroup = s.mixerGroup;
		}
	}

	private void Start()
	{
		Play(SceneManager.GetActiveScene().name + "Music");
	}

	public void Play(string name)
	{
		Sound s = sounds.Find(sound => sound.name == name);
		if (s == null)
			return;
		s.source.Play();
	}

	public void Stop(string name)
	{
		Sound s = sounds.Find(sound => sound.name == name);
		if (s == null)
			return;
		s.source.Stop();
	}

	public void UpdateAudioClip(string name, AudioClip newAudioClip)
	{
		Sound s = sounds.Find(sound => sound.name == name);
		if (s == null)
			return;
		s.source.clip = newAudioClip;
	}

	public Sound FetchSoundClip(string name)
	{
		return sounds.Find(sound => sound.name == name);
	}
}
