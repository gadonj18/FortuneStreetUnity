using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {
	private Transform target;
	private bool panning = false;

	// Update is called once per frame
	void Update () {
		//transform.RotateAround (new Vector3(7.6f, 0f, 7.6f), Vector3.up, 10 * Time.deltaTime);
		if(target != null && !panning) transform.position = new Vector3 (target.position.x, 5.5f, target.position.z - 3f);
	}

	public void SwitchTarget(Transform newTarget) {
		if(target == null) {
			transform.position = new Vector3(newTarget.position.x, 5.5f, newTarget.position.z - 3f);
		} else {
			StartCoroutine(PanToTarget(target, newTarget));
		}
		target = newTarget;
	}

	private IEnumerator PanToTarget(Transform from, Transform to) {
		float t = 0f;
		float totalTime = 0.1f * Vector3.Distance(from.position, to.position);
		Vector3 startPos =  new Vector3(from.position.x, 5.5f, from.position.z - 3f);
		Vector3 endPos = new Vector3(to.position.x, 5.5f, to.position.z - 3f);
		while(t < totalTime) {
			panning = true;
			transform.position = Vector3.Lerp(startPos, endPos, t / totalTime);
			t += Time.deltaTime;
			yield return null;
		}
		panning = false;
		yield break;
	}
}