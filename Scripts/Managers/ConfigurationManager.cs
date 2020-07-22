using Facepunch.Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ConfigurationManager : MonoBehaviour
{
	public static ConfigurationManager instance;
	private const string BUG_REPORT_URL = "https://script.google.com/macros/s/AKfycbyAaD9jTEFoes5AE7nTTri98sYCbab9eaMdE-2Oq0wYySYhi4Dy/exec";
	private static readonly HttpClient client = new HttpClient();

	[SerializeField]
	AudioMixer primaryMixer;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		} else if (instance != this)
		{
			Destroy(gameObject);
			return;
		}
		UpdateMasterAudio();
		DontDestroyOnLoad(gameObject);
	}

	public void UpdateMasterAudio()
	{
		primaryMixer.SetFloat("Master Volume", PlayerPrefs.GetFloat("MasterVolume"));
	}

	public void PostReport()
	{
		PushBugReport();
	}

	private async Task PushBugReport()
	{
		print("Composing Data...");
		GameObject bugReporter = GameObject.FindGameObjectWithTag("BugReporter");
		CanvasGroup pGrp = bugReporter.transform.Find("Pause").GetComponent<CanvasGroup>();
		CanvasGroup cGrp = bugReporter.transform.Find("Confirmation").GetComponent<CanvasGroup>();
		CanvasGroup dGrp = bugReporter.transform.Find("Description").GetComponent<CanvasGroup>();
		dGrp.alpha = 0;
		pGrp.alpha = 1;

		Dictionary<string, string> myDict = new Dictionary<string, string>();
		myDict.Add("Player", Client.Instance.Username);
		myDict.Add("BuildID", Client.Instance.BuildId.ToString());
		myDict.Add("Description", bugReporter.GetComponentInChildren<InputField>().text);

		print("Starting post...");
		print("Encoding...");
		var content = new FormUrlEncodedContent(myDict);
		print("Posting...");
		var response = await client.PostAsync(BUG_REPORT_URL, content);
		print("Waiting...");
		var responseString = await response.Content.ReadAsStringAsync();
		print("Post completed!");
		cGrp.alpha = 1;
		pGrp.alpha = 0;
		StartCoroutine(HideBugReporter());
	}

	private IEnumerator HideBugReporter()
	{
		yield return new WaitForSeconds(1f);
		GameObject bugReporter = GameObject.FindGameObjectWithTag("BugReporter");
		CanvasGroup bGrp = bugReporter.GetComponent<CanvasGroup>();
		CanvasGroup dGrp = bugReporter.transform.Find("Description").GetComponent<CanvasGroup>();
		CanvasGroup cGrp = bugReporter.transform.Find("Confirmation").GetComponent<CanvasGroup>();

		cGrp.alpha = 0;
		cGrp.blocksRaycasts = false;
		cGrp.interactable = false;

		dGrp.alpha = 1;
		dGrp.blocksRaycasts = true;
		dGrp.interactable = true;

		bGrp.alpha = 0;
		bGrp.blocksRaycasts = false;
		bGrp.interactable = false;
	}
}
