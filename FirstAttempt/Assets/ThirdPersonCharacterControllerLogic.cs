using UnityEngine;
using System.Collections;

public class ThirdPersonCharacterControllerLogic : MonoBehaviour {

	[SerializeField]
	private Animator animator;
	[SerializeField]
	private float directionDampTime = 0.25f;
	[SerializeField]
	private ThirdPersonCamera gamecam;
	[SerializeField]
	private float directionSpeed = 3.0f;
	[SerializeField]
	private float rotationDegreePerSecond = 120f;
	
	private float speed = 0.0f;
	private float direction = 0.0f;
	private float horizon = 0.0f;
	private float vertic = 0.0f;
	private AnimatorStateInfo stateInfo;
	
	/* HASHES */
	private int m_LocomotionId = 0;
	
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		if (animator.layerCount >= 2) {
			animator.SetLayerWeight(1, 1);
		}
		
		m_LocomotionId = Animator.StringToHash ("Base Layer.Locomotion");
	}
	
	// Update is called once per frame
	void Update () {
		if (animator) {
			stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			
			horizon = Input.GetAxis("Horizontal");
			vertic = Input.GetAxis("Vertical");
			
			StickToWorldspace(this.transform, gamecam.transform, ref direction, ref speed);
			
			animator.SetFloat("Speed", speed);
			animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);
		}
	}
	
	void FixedUpdate() {							
		/* Rotate character model if stick is tilted right or left, but only if character is moving in that direction */
		if (IsInLocomotion() && ((direction >= 0 && horizon >= 0) || (direction < 0 && horizon < 0))) {
			Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (horizon < 0f ? -1f : 1f), 0f), Mathf.Abs(horizon));
			Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
			this.transform.rotation = (this.transform.rotation * deltaRotation);
		}		
	}
	
	public void StickToWorldspace(Transform root, Transform camera, ref float directionOut, ref float speedOut) {
		Vector3 rootDirection = root.forward;
		
		Vector3 stickDirection = new Vector3(horizon, 0, vertic);
		
		speedOut = stickDirection.sqrMagnitude;		
		
		// Get camera rotation
		Vector3 CameraDirection = camera.forward;
		CameraDirection.y = 0.0f; // kill Y
		Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(CameraDirection));
		
		// Convert joystick input in Worldspace coordinates
		Vector3 moveDirection = referentialShift * stickDirection;
		Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);
		
		Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
		Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
		Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);
		Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2.5f, root.position.z), axisSign, Color.red);
		
		float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
		angleRootToMove /= 180f;
		
		directionOut = angleRootToMove * directionSpeed;
	}	
	
	public bool IsInLocomotion () {
		return stateInfo.nameHash == m_LocomotionId;
	}
}
