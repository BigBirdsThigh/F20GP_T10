using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
    public Animator animator;

    private Collider doorCollider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider>();

        // Wait a frame to let Animator settle before checking state
        StartCoroutine(InitializeColliderState());
    }

    private System.Collections.IEnumerator InitializeColliderState()
    {
        yield return null;

        UpdateColliderBasedOnState();
    }

    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Opening finished
        if (stateInfo.IsName("Opening Door") && stateInfo.normalizedTime >= 1f)
        {
            AnimationStateSet(2); // Set to Open Idle
        }

        // Closing finished
        if (stateInfo.IsName("Closing Door") && stateInfo.normalizedTime >= 1f)
        {
            AnimationStateSet(4); // Set to Closed Idle
        }

        UpdateColliderBasedOnState();
    }

    private void UpdateColliderBasedOnState()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Closed Idle") || stateInfo.IsName("Closing Door"))
        {
            if (!doorCollider.enabled)
            {
                doorCollider.enabled = true;
                Debug.Log("Collider ENABLED (Closed)");
            }
        }
        else if (stateInfo.IsName("Open Idle") || stateInfo.IsName("Opening Door"))
        {
            if (doorCollider.enabled)
            {
                doorCollider.enabled = false;
                Debug.Log("Collider DISABLED (Open)");
            }
        }
    }

    public void OpenDoor()
    {
        AnimationStateSet(1); // Opening Door
    }

    public void CloseDoor()
    {
        AnimationStateSet(3); // Closing Door
    }

    private void AnimationStateSet(int value)
    {
        animator.SetInteger("doorState", value);
    }
}
