using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class Camera_Controller_Script : MonoBehaviour
{
	private Transform player;

	[SerializeField]
	private float lerpVal = 1;

	[SerializeField]
	private float shakeDuration = 1f, shakeMagnitude = .7f;

	[SerializeField]
	private Material cameraMat;

	private float fadeDuration = 0f;
	private bool isFadingOut = false;

	[SerializeField]
	private Volume sceneSettings, deathSettings;

	private void Start()
	{
		if (GameObject.FindGameObjectsWithTag(transform.tag).Length > 1)
		{
			Destroy(gameObject);
			return;
		}
		player = GameObject.FindGameObjectWithTag("Player").transform;
		sceneSettings = GameObject.FindGameObjectWithTag("SceneSettings").GetComponent<Volume>();
		CorrectCameraPosition();
	}

	public void CorrectCameraPosition()
	{
		transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
	}

	private void FixedUpdate()
	{
		if (!Application.isPlaying)
		{
			return;
		}

		transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, player.position.y, transform.position.z), lerpVal);
		if (shakeDuration > 0)
		{
			Vector2 shakeVector = Random.insideUnitCircle;
			transform.position += (Vector3)shakeVector * shakeMagnitude;
			shakeDuration -= Time.deltaTime;
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (cameraMat != null)
		{
			Graphics.Blit(src, dest, cameraMat);
		}
		else
		{
			Graphics.Blit(src, dest);
		}
	}

	public void TriggerShake(float dur, float mag)
	{
		shakeDuration = dur;
		shakeMagnitude = mag;
	}

	public static Vector3 CalculateVectorFromPlayerToMouse()
	{
		Vector3 val = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - Player_Accessor_Script.DetailsScript.transform.position);
		val.z = Player_Accessor_Script.DetailsScript.transform.position.z;
		return val.normalized;
	}

	public void TriggerFade(float dur)
	{
		isFadingOut = true;
	}

	public void TriggerUnfade(float dur)
	{
		isFadingOut = false;
	}

	private void Update()
	{
		if (sceneSettings == null)
		{
			sceneSettings = GameObject.FindGameObjectWithTag("SceneSettings").GetComponent<Volume>();
		}

		if (deathSettings == null)
		{
			deathSettings = GameObject.FindGameObjectWithTag("SceneSettings").GetComponent<Volume>();
		}


		if (isFadingOut && fadeDuration > 0)
		{
			fadeDuration -= Time.deltaTime;
			if (fadeDuration < 0)
			{
				fadeDuration = 0;
			}
		}
		else if (!isFadingOut && fadeDuration < 1)
		{
			fadeDuration += Time.deltaTime;
			if (fadeDuration > 1)
			{
				fadeDuration = 1f;
			}
		}

		if (sceneSettings != null && deathSettings != null)
		{
			sceneSettings.weight = fadeDuration;
			deathSettings.weight = 1 - fadeDuration;
		}
	}
}
