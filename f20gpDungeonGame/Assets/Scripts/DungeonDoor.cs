using UnityEngine;
using System.Collections;

public class DungeonDoor : MonoBehaviour
{
    private Animator animator;
    private Collider doorCollider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider>();

        StartCoroutine(InitializeColliderState());
    }

    private IEnumerator InitializeColliderState()
    {
        yield return null; // Let Animator initialize
        UpdateColliderState();
    }

    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Transition to idle states after animation completes
        if (stateInfo.IsName("Opening Door") && stateInfo.normalizedTime >= 1f)
            AnimationStateSet(4); // Open Idle

        if (stateInfo.IsName("Closing Door") && stateInfo.normalizedTime >= 1f)
            AnimationStateSet(2); // Closed Idle

        UpdateColliderState();
    }

    private void UpdateColliderState()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        bool isClosed = stateInfo.IsName("Closed Idle") || stateInfo.IsName("Closing Door");
        bool isOpen = stateInfo.IsName("Open Idle") || stateInfo.IsName("Opening Door");

        if (isClosed && !doorCollider.enabled)
        {
            doorCollider.enabled = true;
            Debug.Log("Door is closed: Collider ENABLED");
        }
        else if (isOpen && doorCollider.enabled)
        {
            doorCollider.enabled = false;
            Debug.Log("Door is open: Collider DISABLED");
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

    private void AnimationStateSet(int state)
    {
        animator.SetInteger("doorState", state);
    }
}
