using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherVFXController : MonoBehaviour {

	public EnemyObject objRef1, objRef2;
	LineRenderer MLR;
	Material mat;
	void Start () {
		MLR = GetComponent<LineRenderer>();
		mat = new Material(MLR.material);
		MLR.material = mat;
	}
	
	// Update is called once per frame
	void Update () {
		if (objRef2 == null || objRef1 == null){
			Destroy(gameObject);
			return;
		}
		MLR.SetPositions(new Vector3[] { objRef1.transform.position, .5f * (objRef1.transform.position + objRef2.transform.position), objRef2.transform.position });
		mat.SetTextureOffset("_MainTex", new Vector2(Time.deltaTime*2, 0) + mat.GetTextureOffset("_MainTex"));

	}
}
