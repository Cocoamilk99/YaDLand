using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTrigger : MonoBehaviour
{
    public string endingSceneName = "Ending";
    public float delay = 1f;

    private bool done = false;

    private void OnTriggerEnter(Collider other)
    {
        if (done || !other.CompareTag("Player")) return;
        done = true;
        StartCoroutine(GoToEnding());
    }

    private IEnumerator GoToEnding()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(endingSceneName);
    }
}