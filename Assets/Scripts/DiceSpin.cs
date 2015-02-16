using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiceSpin : MonoBehaviour {
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

	private float rollSpeed = 500f;
	private int rollNum;
	private bool spinning = false;
	private float x = 0f;
	private float y = 1f;
	private float z = 0.5f;
	private bool xUp = true;
	private bool yUp = true;
	private bool zUp = true;

	public void Roll(int num) {
		rollNum = num;
		spinning = true;
		StartCoroutine("Spin");
		Invoke("StopSpin", 3f);
	}

	private IEnumerator Spin() {
		rollSpeed = 500f;
		while(spinning) {
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
			yield return null;
		}
	}
	
	private void StopSpin() {
		spinning = false;
		StopCoroutine("Spin");
		StartCoroutine("SpinToRoll");
	}
	
	private IEnumerator SpinToRoll() {
		rollSpeed = 200f;
		while(transform.rotation != rotations[rollNum - 1]) {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, rotations[rollNum - 1], Time.deltaTime * rollSpeed);
			if(rollSpeed >= 10f) rollSpeed -= 1f;
			yield return null;
		}
		spinning = false;
		yield return new WaitForSeconds(1f);
		if(DiceSpun != null) DiceSpun();
		//StartCoroutine("RecessDice");
	}

	private IEnumerator RecessDice() {
		Vector3 targetPos = new Vector3();
		Vector3 targetScale = new Vector3();
		while(transform.position != targetPos) {
			yield return null;
		}
	}
}