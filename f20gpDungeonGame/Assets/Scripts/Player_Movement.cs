using UnityEngine;
using System.Collections;

public class Player_Movement : MonoBehaviour
{

    private Rigidbody rb; //the player rigidbody
    public DungeonGen dg;

    //movement speed variables
    public float currentMovementSpeed = 0f;
    public float walkSpeed = 10f;
    public float runSpeed = 20f;
    public float rotationSpeed = 10f;

    //camera movement variables
    public bool isMoving = false;
    private float stopRotTimer = 0f;
    public float stopRotDur = 0.5f; //time in secs after stopping before camera+player rotation stops
    public float turnRot = 25f;
    public float fixRot = -90f; //fixer for camera movement, fixes bugs

    //jumping and falling variables
    public float raycastMaxDistance = 2f;
    public bool isGrounded = true;
    public float jumpHeight = 50f;
    public float fallMult;

    private GameObject camera; //getting the player's camera from the scene

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get the rigidbody of the player
        rb = this.GetComponent<Rigidbody>();
        camera = GameObject.FindGameObjectWithTag("MainCamera"); //get camera from scene

        //place the player in the spawn room
        gameObject.transform.localPosition = dg.getSpawnRoom();
        Debug.Log(dg.getSpawnRoom());
        SetChildrenRotation();
    }

    // Update is called once per frame
    void Update()
    {
        MovementChecker(); //checks 'isMoving' either True or False
        PlayerSpeed(); //increases player speed if LEFT SHIFT held
        CheckGrounded(); //check if player is on the ground for jumping
    }

    void FixedUpdate() //fixed update for handling in game physics with rigidbody movement
    {
        BasicMovement();
        RotationMovement();
        Jump(); //jumping for the player, done pressing spacebar
    }

    void SetChildrenRotation()
    {
        //used to fix rotation bug
        foreach (Transform child in transform)
        {
            Vector3 currentRotation = child.eulerAngles;
            child.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 90f);//force z axis to be 90
        }
    }

    void BasicMovement()
    {
        //Vector3 moveDirection = transform.right * Input.GetAxis("Vertical") + transform.forward * -Input.GetAxis("Horizontal"); //reverse horizontal movement
        Vector3 moveDirection = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector3(moveDirection.x * currentMovementSpeed, rb.linearVelocity.y, moveDirection.z * currentMovementSpeed);
    }

    void RotationMovement()
    {
        //if camera is still- dont rotate the player
        //if camera moving- rotate the player to face where the camera + player movement is going

        //get the rotation of the camera
        float camYRot = camera.transform.eulerAngles.y;
        float camXRot = camera.transform.eulerAngles.x;

        //if moving, rotate the camera to always be behind the player
        if(isMoving)
        {
            Quaternion goalRotation = Quaternion.Euler(0f, camYRot + fixRot, 0f); //fix rotation ensures camera is always behind the player
            float turnAmountAngle = Input.GetAxis("Horizontal") * turnRot; //turn the player slightly when moving left or right
            transform.rotation = Quaternion.Slerp(transform.rotation, goalRotation, rotationSpeed * Time.deltaTime);
            stopRotTimer = stopRotDur; //reset timer when moving
        }
        else if (stopRotTimer > 0) //if stopping moving, stop camera rotating with the player
        {
            Quaternion goalRotation = Quaternion.Euler(0f, camYRot + fixRot, 0f); //fix rotation ensures camera is always behind the player
            float turnAmountAngle = Input.GetAxis("Horizontal") * turnRot; //turn the player slightly when moving left or right
            goalRotation *= Quaternion.Euler(0f, turnAmountAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, goalRotation, rotationSpeed * Time.deltaTime);
            stopRotTimer -= Time.deltaTime; //reset timer when moving
        }
        else { //if movement stopped completely
            rb.angularVelocity = Vector3.zero; //completely stop all angular movement to prevent bugs
        }
    }

    void MovementChecker()
    {
        //method to check whether or not the player is moving
        if(Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    void PlayerSpeed()
    {
        //if get key down, make player speed set to the running speed, vice versa
        if(Input.GetKey(KeyCode.LeftShift))
        {
            //make running speed
            currentMovementSpeed = runSpeed;
        }
        else 
        {
            //make walking speed
            currentMovementSpeed = walkSpeed;
        }
    }

    void CheckGrounded()
    {
        //shoot a raycast down from the bottom of the player
        RaycastHit hit;
        Vector3 origin = this.transform.position;
        Vector3 direction = Vector3.down;

        if(Physics.Raycast(origin, direction, out hit, raycastMaxDistance)) //should prob change this to height of character + some to get bottom of avatar
        {
            if(hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }
        else //jumping/falling
        {
            isGrounded = false;
        }
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("jump pressed!");
            //rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Sqrt(2 * jumpHeight * fallMult), rb.linearVelocity.z);
            rb.AddForce(Vector3.up * Mathf.Sqrt(2 * jumpHeight * -Physics.gravity.y), ForceMode.Force);
        }
        if(!isGrounded)
        {
            //if falling, add downwards force
            rb.linearVelocity += Vector3.down * (fallMult * Time.deltaTime);
        }
    }
}
