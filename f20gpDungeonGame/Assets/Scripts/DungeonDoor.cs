using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //check the animator state information
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //if the animation is finished
        if (stateInfo.IsName("Opening Door") && stateInfo.normalizedTime >= 1.0f)
        {
            //set to open idle animation
            AnimationStateSet(2);
        }
        if (stateInfo.IsName("Closing Door") && stateInfo.normalizedTime >= 1.0f)
        {
            //set to closed idle animation
            AnimationStateSet(4);
        }

        //Testing
        // if(Input.GetKeyDown(KeyCode.T))
        // {
        //     OpenDoor();
        // }
        // if(Input.GetKeyDown(KeyCode.Y))
        // {
        //     CloseDoor();
        // }
    }

    public void OpenDoor()
    {
        AnimationStateSet(1);
        //once animation finished, play next one, this is done in the animator
    }

    public void CloseDoor()
    {
        AnimationStateSet(3);
    }

    //helper method to set the animation state
    private void AnimationStateSet(int value)
    {
        animator.SetInteger("doorState", value);
    }
}
