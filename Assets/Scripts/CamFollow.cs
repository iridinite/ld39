using UnityEngine;

public class CamFollow : MonoBehaviour {

    public Transform parent;
    public float ZDistance;

	private void Start() {
		
	}

	private void FixedUpdate() {
	    const float HALF_MAP_WIDTH = Tuning.MAP_WIDTH / 2f;
	    Vector3 parentPosition = parent.transform.position;
		Vector3 target = new Vector3(parentPosition.x, parentPosition.y + 0.35f, parentPosition.z + ZDistance);
	    target.x = Mathf.Clamp(target.x, -HALF_MAP_WIDTH + 2f, HALF_MAP_WIDTH - 4f);

	    Vector3 delta = (target - this.transform.position) * Tuning.CAMERA_FOLLOW_SPEED * Time.fixedDeltaTime;
	    transform.localPosition += delta;
	}

}
