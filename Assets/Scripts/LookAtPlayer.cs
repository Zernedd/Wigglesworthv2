using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform cameraTransform;
    public bool onlyYRotation = false;

    void Update()
    {
        if (cameraTransform == null)
        {
            if (Camera.main != null)
                cameraTransform = Camera.main.transform;
            else
                return;
        }

        if (onlyYRotation)
        {
            Vector3 lookDirection = cameraTransform.position - transform.position;
            lookDirection.y = 0;
            if (lookDirection.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(lookDirection);
        }
        else
        {
           
            transform.LookAt(cameraTransform);
           
            transform.Rotate(0, 180f, 0);
        }
    }
}
