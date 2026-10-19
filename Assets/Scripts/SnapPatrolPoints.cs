using UnityEngine;
using UnityEngine.AI;

public class SnapPatrolPoints : MonoBehaviour
{
    void Start()
    {
        foreach (Transform p in transform)
        {
            if (NavMesh.SamplePosition(p.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                p.position = hit.position;
            else
                Debug.LogWarning(p.name + " อยู่ไกล NavMesh เกิน 10 เมตร ต้องย้ายเข้ามา");
        }
    }
}