using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	public float dps = 720f;

	public float Speed {
		set { dps = value; }
		get { return dps; }
	}

	// Update is called once per frame
	void Update () {
		transform.RotateAround(transform.position, new Vector3(0f, 1f, 0f), dps * Time.deltaTime);
	}
}
