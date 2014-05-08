using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

	[SerializeField]
	private Animator animator;
	[SerializeField]
	private float directionDampTime = 0.25f;

	private float speed = 0.0f;
	private float horizon = 0.0f;
	private float vertic = 0.0f;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		if (animator.layerCount >= 2) {
			animator.SetLayerWeight(1, 1);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (animator) {
			horizon = Input.GetAxis("Horizontal");
			vertic = Input.GetAxis("Vertical");
			speed = new Vector2(horizon, vertic).sqrMagnitude;

			animator.SetFloat("Speed", speed);
			animator.SetFloat("Direction", horizon, directionDampTime, Time.deltaTime);
		}
	}
}
