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
        if (doorCollider == null){
            Debug.Log("NO DOOR COLLIDER");
        }
        animator.SetBool("open", true);
        
    }



    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // // Transition to idle states after animation completes
        // if (stateInfo.IsName("Opening Door") && stateInfo.normalizedTime >= 1f)
        //     AnimationStateSet(4); // Open Idle

        // if (stateInfo.IsName("Closing Door") && stateInfo.normalizedTime >= 1f)
        //     AnimationStateSet(2); // Closed Idle

        // UpdateColliderState();
    }

   

    public void OpenDoor()
    {
        animator.SetBool("open", true);
        doorCollider.enabled = false; // Door is open -> no blocking
    }

    public void CloseDoor()
    {
        animator.SetBool("open", false);
        doorCollider.enabled = true; // Door is closed -> should block
    }


    private void AnimationStateSet(int state)
    {
        animator.SetInteger("doorState", state);
    }
}
