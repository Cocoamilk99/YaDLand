using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Teleport : MonoBehaviour
{
    public Transform destination;
    public GameObject hunter;          // ลาก GameObject ผีมาใส่ (ไม่ใส่ = ผีไม่ตาม)
    public float hunterDelay = 1.5f;   // ผีตามมาช้ากว่าผู้เล่นกี่วินาที

    private static bool canTeleport = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canTeleport || !other.CompareTag("Player"))
            return;

        StartCoroutine(TeleportPlayer(other));
    }

    private IEnumerator TeleportPlayer(Collider other)
    {
        canTeleport = false;

        CharacterController controller = other.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        other.transform.position = destination.position;

        if (controller != null) controller.enabled = true;

        // ผีโผล่แล้วเท่านั้นถึงจะตามมา
        if (hunter != null && hunter.activeInHierarchy)
        {
            yield return new WaitForSeconds(hunterDelay);

            NavMeshAgent agent = hunter.GetComponent<NavMeshAgent>();
            if (agent != null)
                agent.Warp(destination.position);
            else
                hunter.transform.position = destination.position;
        }

        yield return new WaitForSeconds(0.5f);
        canTeleport = true;
    }
}