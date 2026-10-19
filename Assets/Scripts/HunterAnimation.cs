using UnityEngine;

public class HunterAnimation : MonoBehaviour
{
    public Animator animator;
    public float smoothTime = 0.1f;
    public float runThreshold = 0.1f;

    Vector3 lastPos;
    float speed;
    float speedVel;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        lastPos = transform.position;
    }

    void Update()
    {
        if (Time.deltaTime <= 0f) return;

        float current = (transform.position - lastPos).magnitude / Time.deltaTime;
        lastPos = transform.position;

        speed = Mathf.SmoothDamp(speed, current, ref speedVel, smoothTime);
        animator.SetBool("runing", speed > runThreshold);
    }
}