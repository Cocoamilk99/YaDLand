
using UnityEngine;

public class HunterCatchTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER CAUGHT!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerCaught();
            }
            else
            {
                Debug.LogError("GameManager.Instance is NULL!");
            }
        }
    }
}
