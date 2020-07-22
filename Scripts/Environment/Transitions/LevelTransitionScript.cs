using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionScript : MonoBehaviour {
	[SerializeField]
	string targetScene;

	[SerializeField]
	bool playerInteracting = false;

	protected CanvasGroup interactionHUD;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.transform.tag == "Player")
		{
			playerInteracting = true;
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.transform.tag == "Player")
		{
			playerInteracting = false;
		}
	}

	protected virtual void Start()
	{
		interactionHUD = transform.Find("Display/Interact").GetComponent<CanvasGroup>();
	}

	protected virtual void Update()
	{
		if (playerInteracting)
		{
			interactionHUD.alpha = 1;
		} else
		{
			interactionHUD.alpha = 0;
		}

		if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.I_INTERACT]) && playerInteracting)
		{
			GameObject.FindGameObjectWithTag("Player").transform.parent = GameManager.instance.transform;
			GameObject.FindGameObjectWithTag("PrimaryInterface").transform.parent = GameManager.instance.transform;
			GameObject.FindGameObjectWithTag("MainCamera").transform.parent = GameManager.instance.transform;
			SceneManager.LoadScene(targetScene);
		}
	}
}
