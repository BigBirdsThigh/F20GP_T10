using UnityEngine;

public class Player_Camera : MonoBehaviour
{

    public float rotationSpeed = 5f; //rotation speed
    public float rotX = 0f; //x axis from the mouse input
    public float rotY = 0f; //y axis from the mouse input

    private GameObject player;
    
    public Vector3 TP_Offset = Vector3.zero; //set this in the inspector tab- it is an vector where we can modify where the camera's position will be
    public float fov = -4f; //how close the third person camera is to the player
    public float focusSpeed = 5f; //how fast the camera will move

    public float maxLookupAngle = 80f; //how far ontop of the player we can see (e.g. how close to the player's head/top)
    public float minLookupAngle = -25f; //how below the player we can see (e.g. how close to the floor)

    public float maxFOV = -1f;
    public float minFOV = -4.5f;

    public float zoomInOffset = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); //get the player object from the scene
    }

    //late update to prevent glitchy camera- as movement is done in fixed update
    void LateUpdate()
    {
        ThirdPersonCamera();
        AdjustFOV();
    }

    void ThirdPersonCamera()
    {
        //get the mouse movement to move around the camera
        float mX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mY = Input.GetAxis("Mouse Y") * rotationSpeed;

        rotY += mX; //left and right
        rotX -= mY; //up and down

        rotX = Mathf.Clamp(rotX, minLookupAngle, maxLookupAngle); //limit the x rotation, should be modifiable in inspector
        
        Quaternion cameraRotation = Quaternion.Euler(rotX, rotY, 0f); //rotate the camera position along the y and x axis
        Vector3 targetOffset = TP_Offset;
        Vector3 correctCamPos = player.transform.position + (cameraRotation * targetOffset);
        //Vector3 cameraRotationFinal = cameraRotation * targetOffset; //add the offset to the initial camera position to get the final position

        RaycastHit hit;
        if (Physics.Linecast(player.transform.position, correctCamPos, out hit))
        {
            if(!hit.collider.CompareTag("Player") || !hit.collider.CompareTag("Ground"))
            {
                this.transform.position = Vector3.Slerp(this.transform.position, correctCamPos, Time.deltaTime * focusSpeed);
            }
            else
            {
                this.transform.position = hit.point + hit.normal * zoomInOffset; //offset to stop clipping
            }
        } 
        else 
        {
            this.transform.position = Vector3.Slerp(this.transform.position, correctCamPos, Time.deltaTime * focusSpeed);
        }

        //this.transform.position = Vector3.Slerp(this.transform.position, player.transform.position + cameraRotationFinal, Time.deltaTime * focusSpeed);
        this.transform.LookAt(player.transform.position); //look at player 
    }

    void AdjustFOV()
    {
        fov = Mathf.Clamp(fov, minFOV, maxFOV); //minimum and maximum distances for the fov
        TP_Offset = new Vector3(TP_Offset.x, TP_Offset.y, fov); //update the camera offset position
    }
}