using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugReporter_Controller : MonoBehaviour
{
	private GameObject bugReporterRoot;

	[SerializeField]
	private bool closeScreen = false;

	private void Start()
	{
		if (closeScreen)
		{
			bugReporterRoot = transform.parent.parent.gameObject;
		}
		else
		{
			bugReporterRoot = transform.parent.Find("BugReporter").gameObject;
		}

		CanvasGroup tarGroup = bugReporterRoot.GetComponent<CanvasGroup>();
		tarGroup.alpha = 0;
		tarGroup.blocksRaycasts = false;
		tarGroup.interactable = false;
	}

	public void ToggleBugReporter()
	{
		CanvasGroup tarGroup = bugReporterRoot.GetComponent<CanvasGroup>();
		tarGroup.alpha = Mathf.Abs(1 - tarGroup.alpha);
		tarGroup.blocksRaycasts = !tarGroup.blocksRaycasts;
		tarGroup.interactable = !tarGroup.interactable;
	}
}
