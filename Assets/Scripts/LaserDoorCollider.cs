using UnityEngine;

public class LaserDoorCollider : MonoBehaviour
{
    public LaserDoor laserDoor;
    public SuperHeroTycoonMan baseman;

    private void Awake()
    {
        if (laserDoor == null)
            laserDoor = GetComponentInParent<LaserDoor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Look for Player component on root or parent
        GorillaLocomotion.Player player = other.GetComponentInParent<GorillaLocomotion.Player>();
        if (player != null)
        {
            laserDoor.CheckDoor(baseman);
        }
    }
}
