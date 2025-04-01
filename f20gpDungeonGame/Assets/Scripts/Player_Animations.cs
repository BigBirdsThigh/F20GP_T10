using UnityEngine;

public class Player_Animations : MonoBehaviour
{

    public Player_Movement player_Movement;
    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_Movement = GetComponent<Player_Movement>(); //get player movement script from the object
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player_Movement.animator.GetInteger("JumpTrans") != 0) 
        {
            //If jumping or falling, stop any movements animations
            return;
        }
        AnimationCyclesMovement();
    }

    void AnimationCyclesMovement()
    {
        //idle, walk, run
        //if grounded and is not moving, play idle
        if (player_Movement.isGrounded && !player_Movement.isMoving)
        {
            animator.SetInteger("MoveTrans",0);
        }
        //if grounded and moving, play either walking or running
        else if(player_Movement.isGrounded && player_Movement.isMoving)
        {
            //if walking
            if(player_Movement.currentMovementSpeed == player_Movement.walkSpeed)
            {
                animator.SetInteger("MoveTrans",1);
            }
            else if(player_Movement.currentMovementSpeed == player_Movement.runSpeed)
            {
                animator.SetInteger("MoveTrans",2);
            }
        }
    }
}
