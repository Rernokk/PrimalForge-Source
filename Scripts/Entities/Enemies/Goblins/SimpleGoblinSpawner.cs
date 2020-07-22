using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGoblinSpawner : MonoBehaviour {
	[SerializeField]
	GameObject[] goblinPrefabs;
	float timer = 0f;
	void Update () {
		Collider2D[] objInRange = Physics2D.OverlapCircleAll(transform.position, 15f, LayerMask.GetMask("Enemies"));
		if (objInRange.Length < 12f && timer <= 0f)
		{
			Instantiate(goblinPrefabs[Random.Range(0, goblinPrefabs.Length)], transform.position, Quaternion.identity);
			timer = Random.Range(1f, 5f);
		}

		if (timer > 0)
		{
			timer -= Time.deltaTime;
		}
	}
}
