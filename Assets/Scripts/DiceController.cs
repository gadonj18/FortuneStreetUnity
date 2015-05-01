using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiceController : MonoBehaviour {
	public delegate void DiceSpinHandler();
	public static event DiceSpinHandler DiceSpun;
	private List<Quaternion> rotations = new List<Quaternion>() {
		Quaternion.Euler(new Vector3(60f, 0f, 0f)),
		Quaternion.Euler(new Vector3(0f, 90f, 60f)),
		Quaternion.Euler(new Vector3(240f, 0f, 0f)),
		Quaternion.Euler(new Vector3(0f, 270f, -60f)),
		Quaternion.Euler(new Vector3(0f, 270f, 30f)),
		Quaternion.Euler(new Vector3(150f, 0f, 0f))
	};

	private Vector3 origScale = new Vector3(75f, 75f, 75f);
	private Vector3 recessScale = new Vector3(0f, 0f, 0f);

	private float rollSpeed = 500f;
	private int rollNum;
	private float x = 0f;
	private float y = 1f;
	private float z = 0.5f;
	private bool xUp = true;
	private bool yUp = true;
	private bool zUp = true;

	public IEnumerator Roll(int num) {
		rollNum = num;
		yield return StartCoroutine("Spin");
		yield return StartCoroutine("SpinToRoll");
		yield return new WaitForSeconds(1f);
		yield return StartCoroutine("RecessDice");
		if(DiceSpun != null) DiceSpun();
	}

	private IEnumerator Spin() {
		rollSpeed = 500f;
		float t = 0;
		while(t < 3) {
			transform.RotateAround(transform.position, new Vector3(x, y * 0.2f, z * 0.8f), rollSpeed * Time.deltaTime);
			x += (xUp ? 0.06f : -0.06f);
			y += (yUp ? 0.04f : -0.04f);
			z += (zUp ? 0.02f : -0.02f);
			if(x >= 1.0f) { xUp = false; }
			if(y >= 1.0f) { yUp = false; }
			if(z >= 1.0f) { zUp = false; }
			if(x <= 0f) { xUp = true; }
			if(y <= 0f) { yUp = true; }
			if(z <= 0f) { zUp = true; }
			t += Time.deltaTime;
			yield return null;
		}
	}
	private IEnumerator SpinToRoll() {
		rollSpeed = 200f;
		while(transform.rotation != rotations[rollNum - 1]) {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, rotations[rollNum - 1], Time.deltaTime * rollSpeed);
			if(rollSpeed >= 10f) rollSpeed -= 1f;
			yield return null;
		}
	}

	private IEnumerator RecessDice() {
		float t = 0f;
		float totalTime = 1f;
		while(t < totalTime) {
			transform.localScale = Vector3.Lerp(origScale, recessScale, t / totalTime);
			t += Time.deltaTime;
			yield return null;
		}
	}
}