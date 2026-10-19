using UnityEngine;
using UnityEngine.AI;

public class HunterTrigger : MonoBehaviour
{
    public GameObject hunter;
    public Transform spawnPoint;

    private bool triggered = false;

    public void SpawnHunter()
    {
        if (triggered || hunter == null || spawnPoint == null) return;
        triggered = true;

        hunter.SetActive(true);

        NavMeshAgent agent = hunter.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (NavMesh.SamplePosition(spawnPoint.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogWarning("spawnPoint ไม่อยู่ใกล้ NavMesh");
            }
        }
        else
        {
            hunter.transform.position = spawnPoint.position;
        }
    }

    // ให้เก็บของครั้งใหม่แล้วผีโผล่ได้อีก (เรียกจาก HitReset)
    public void ResetTrigger()
    {
        triggered = false;
    }
}