using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectiveLightNode : MonoBehaviour
{
  public float remainingDuration = 1f;
  bool playerHasBuff = false;
  bool applyDamage = false;
  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (applyDamage){
      applyDamage = false;
    }

    remainingDuration -= Time.deltaTime;
    if (remainingDuration <= 0)
    {
      Destroy(gameObject);
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (!playerHasBuff && collision.transform.tag == "Player"){
      print("Player entered.");
      playerHasBuff = true;
      Player_Accessor_Script.DetailsScript.ResistanceMultiplier += .5f;
    }
  }

  private void OnTriggerStay2D(Collider2D collision)
  {
    if (collision.transform.tag == "Player" && !playerHasBuff){
      playerHasBuff = true;
      Player_Accessor_Script.DetailsScript.ResistanceMultiplier += .5f;
      print("Player is inside");
    }
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
    if (playerHasBuff && collision.transform.tag == "Player"){
      playerHasBuff = false;
      print("Player has left.");
      Player_Accessor_Script.DetailsScript.ResistanceMultiplier -= .5f;
    }
  }
}
