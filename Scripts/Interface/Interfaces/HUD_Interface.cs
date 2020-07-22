using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Interface : MonoBehaviour {
  Player_Details_Script playerDetails;
  Slider hpBar, resourceBar;

	[SerializeField]
	Text healthText, resourceText;

	void Start () {
    hpBar = transform.GetChild(0).GetComponent<Slider>();
		resourceBar = transform.GetChild(1).GetComponent<Slider>();
    playerDetails = Player_Accessor_Script.DetailsScript;
	}
	
	// Update is called once per frame
	void Update () {
    hpBar.value = playerDetails.HealthPercent;
		resourceBar.value = playerDetails.ResourcePercent;
		healthText.text = playerDetails.CurrentHealth.ToString("#") + "/" + playerDetails.MaxHealth + " (" + ((100 * playerDetails.HealthPercent).ToString("#.#")) + "%)";
		resourceText.text = playerDetails.CurrentResource.ToString("#") + "/" + playerDetails.MaxResource + " (" + ((100 * playerDetails.ResourcePercent).ToString("#.#")) + "%)";
	}
}
