using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {

	[SerializeField]
	private float distanceAway;
	[SerializeField]
	private float distanceUp;
	[SerializeField]
	private float smooth;
	[SerializeField]
	private Transform followXForm;
	[SerializeField]
	private Vector3 offset = new Vector3 (0f, 1.5f, 0f);
	[SerializeField]
	private float camSmoothDampTime = 0.1f;
	
	private Vector3 targetPosition;
	private Vector3 lookDirection;
	private Vector3 velocityCamSmooth = Vector3.zero;
	
	// Use this for initialization
	void Start () {
		followXForm = GameObject.FindWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void LateUpdate () {
		Vector3 characterOffset = followXForm.position + offset;
		
		/* create direction from camera to player, kill Y-axis move, normalize to give valid direction with unit magnitude */
		lookDirection = characterOffset - this.transform.position;
		lookDirection.y = 0;
		lookDirection.Normalize ();
		
		/* 
		 * set target position to have correct offset from hovercraft 
		 * so, we take current move up and backwards to get it correct
		 */
		// targetPosition = followXForm.position + followXForm.up * distanceUp - followXForm.forward * distanceAway;
		targetPosition = characterOffset + followXForm.up * distanceUp - lookDirection * distanceAway;
		
		Debug.DrawRay(followXForm.position, Vector3.up * distanceUp, Color.red);
		Debug.DrawRay(followXForm.position, -1f * followXForm.forward * distanceAway, Color.blue);
		Debug.DrawLine(followXForm.position, targetPosition, Color.magenta);
		
		/* making smooth transaction between current position and position it needs to be at */
		// transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * smooth);
		smoothPosition (this.transform.position, targetPosition);
		
		/* point camera in right direction */
		transform.LookAt(followXForm);
	}
	
	private void smoothPosition(Vector3 fromPosition, Vector3 toPosition) {
		/* making smooth transaction between current position and position it needs to be at */
		this.transform.position = Vector3.SmoothDamp (fromPosition, toPosition, ref velocityCamSmooth, camSmoothDampTime);
	}

}
