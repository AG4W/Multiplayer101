using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject target = null;

    void LateUpdate()
    {
        if (target == null)
            return;

        this.transform.SetPositionAndRotation(
            target.transform.position, 
            target.transform.rotation);
    }

    public void SetTarget(GameObject target) => this.target = target;
}
