using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	public static GameManager instance;

	[SerializeField]
	private static string prevScene;

	private void Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += DetachPlayer;
		prevScene = SceneManager.GetActiveScene().name;
	}

	private void DetachPlayer(Scene scene, LoadSceneMode mode)
	{
		GameObject player = transform.Find("Player").gameObject;
		GameObject interf = transform.Find("Interface").gameObject;
		GameObject cam = transform.Find("Main Camera").gameObject;

		player.transform.parent = null;
		interf.transform.parent = null;
		cam.transform.parent = null;

		Transform spawnPos = GameObject.FindGameObjectWithTag("PlayerSpawnPoints").transform;
		Transform tarPos = spawnPos.Find(prevScene);
		if (tarPos != null)
		{
			player.transform.position = tarPos.position;
		}
		cam.GetComponent<Camera_Controller_Script>().CorrectCameraPosition();
		AudioManager.instance.Stop(prevScene + "Music");
		AudioManager.instance.Play(scene.name + "Music");
		prevScene = scene.name;
	}
}
