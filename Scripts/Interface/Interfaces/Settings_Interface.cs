using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings_Interface : MonoBehaviour
{
  Coroutine saveRoutine;
  Transform quitButton;
  float delay = 0;
  string message = "Saving";
  // Use this for initialization
  void Start()
  {
    saveRoutine = null;
    quitButton = transform.Find("QuitGame");
  }

  private void Update()
  {
    if (delay > .21)
    {
      delay = 0;
      message += ".";
      if (message.LastIndexOf('.') > 8)
      {
        message = "Saving";
      }
    }
    if (saveRoutine != null)
    {
      quitButton.GetChild(0).GetComponent<Text>().text = message;
      delay += Time.unscaledDeltaTime;
    }
  }

	public void ExitToMain()
	{
		Player_Accessor_Script.SaveGame();
		SceneManager.LoadScene("MainMenu");
	}

  public void ExitGame()
  {
    if (saveRoutine == null)
    {
      Player_Accessor_Script.SaveGame();
      saveRoutine = StartCoroutine(SaveAndQuit());
    }
  }

  IEnumerator SaveAndQuit()
  {
    quitButton.GetComponent<Button>().interactable = false;
    yield return new WaitForSeconds(1f);
    quitButton.GetChild(0).GetComponent<Text>().text = "Quitting";
    quitButton.GetComponent<Button>().interactable = true;
    saveRoutine = null;
    Application.Quit();
  }
}
