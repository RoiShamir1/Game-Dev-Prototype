using UnityEngine;
public class DetectionZone : MonoBehaviour
{
    EnemyShooting owner;
    void Awake() { owner = GetComponentInParent<EnemyShooting>(); }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) owner.SetTarget(other.transform);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) owner.ClearTarget(other.transform);
    }
}
