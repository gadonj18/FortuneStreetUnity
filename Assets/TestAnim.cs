using UnityEngine;
using System.Collections;

public class TestAnim : MonoBehaviour {
	public float HopSpeed = 2f;
	private Animator anim;
	private AnimatorStateInfo stateInfo;
	private int hopLeftHash = Animator.StringToHash("Base Layer.HopLeft");
	private int hopRightHash = Animator.StringToHash("Base Layer.HopRight");
	private Vector3 targetPos = Vector3.zero;
	private bool moving = false;
	private bool leftHopNext = true;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		if (Input.GetMouseButtonDown(0) && !moving && stateInfo.nameHash != hopLeftHash && stateInfo.nameHash != hopRightHash) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
				Transform tile = hit.transform.Find("Base");
				if(tile) {
					targetPos = tile.position;
					targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z - 0.491f);
					if(transform.position != targetPos) {
						transform.rotation = Quaternion.LookRotation(targetPos - transform.position) * Quaternion.Euler(-15f, 0f, 0f);
						anim.Play(leftHopNext ? hopLeftHash : hopRightHash);
						leftHopNext = !leftHopNext;
						moving = true;
					}
				} else {
					targetPos = Vector3.zero;
				}
			}
		}
		if (targetPos != Vector3.zero && transform.position != targetPos) {
			transform.position = Vector3.MoveTowards (transform.position, targetPos, Time.deltaTime * HopSpeed);
		} else {
			moving = false;
			targetPos = Vector3.zero;
		}
	}

	void OnHopEnd() {
		transform.rotation = Quaternion.Euler(-15f, 180f, 0f);
	}
}