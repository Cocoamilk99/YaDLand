using UnityEngine;
using UnityEngine.AI;

public class HunterDebug : MonoBehaviour
{
    NavMeshAgent agent;
    Transform player;
    float t;

    void Start() { agent = GetComponent<NavMeshAgent>(); }

    void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        t += Time.deltaTime;
        if (t < 1f) return;
        t = 0f;

        float dist = player != null
            ? Vector3.Distance(transform.position, player.position) : -1f;

        Debug.Log("HUNTER dist=" + dist.ToString("F1") +
                  " hasPath=" + agent.hasPath +
                  " status=" + agent.pathStatus +
                  " speed=" + agent.velocity.magnitude.ToString("F1"));
    }
}