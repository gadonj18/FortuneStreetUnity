using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {
	public GameObject Character;
	// Update is called once per frame
	void Update () {
		//transform.RotateAround (new Vector3(7.6f, 0f, 7.6f), Vector3.up, 10 * Time.deltaTime);
		transform.position = new Vector3 (Character.transform.position.x, 5.5f, Character.transform.position.z - 3f);
	}
}