using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PlayerMovment : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 4f; //character's speed
    Animator animator; // an animator object to reference in unity
    //private PlayerMovment playerMovment;
    private MouseLook mouseLook;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform virtualCameraSource;
    [SerializeField] private Transform virtualCameraLook;
    [HideInInspector] public static bool IsAnimating { get; set; }// to prevent multiple takedowns in the same location and time
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private Transform rockTarget;
    void Start()
    {
        animator = GetComponent<Animator>(); // a referance for unity's component
        //playerMovment = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovment>();
        mouseLook = GameObject.FindGameObjectWithTag("FPSCamera").GetComponent<MouseLook>();
    }

    // Update is called once per frame
    void Update()
    {
        
        float x = Input.GetAxis("Horizontal"); //get horizontal movement
        float z = Input.GetAxis("Vertical"); //get vertical movement

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
        Animate(x,z); // animate the character based on movement
    }
    
    void Animate(float x, float y)
    {
        //cap values in range of -1 to 1
        float clmapetHorizontal = Mathf.Clamp(x, -1f, 1f);
        float clmapetVertical = Mathf.Clamp(y, -1f, 1f);

        //change velocities in the blend tree
        this.animator.SetFloat("x",clmapetHorizontal);
        this.animator.SetFloat("y",clmapetVertical);
    }
    public void OnTakedownAnimationStart()
    {
        IsAnimating = true; //a repeated step - to avoid potential exploits
        virtualCamera.transform.position = virtualCameraSource.position; //transform the VCamera to the source near the player
        virtualCamera.transform.LookAt(virtualCameraLook); //rotate the camera to the source
        animator.applyRootMotion = true; //enable root motion
        controller.enabled = false; //disable movement
        //reset fps camera position
        Vector3 cameraEulerAngles = mouseLook.transform.eulerAngles;
        cameraEulerAngles.x = 0f;
        mouseLook.transform.eulerAngles = cameraEulerAngles;
        mouseLook.enabled = false; //disable camera rotation
        virtualCamera.Priority = 21; //transfer view to VCamera
    }
    public void OnTakedownAnimationEnd()
    {
        IsAnimating = false; //enable takedowns 
        animator.applyRootMotion = false; //disable root motion
        controller.enabled = true; //enable movement
        mouseLook.enabled = true; //enable camera rotation
    }
    public void OnAnimationReturnCamera()
    {
        virtualCamera.Priority = 10; //return view to fps camera
    }
    public void OnAnimationSpwanRock()
    {
        GameObject newRock = Instantiate(rockPrefab, rockTarget.position, rockTarget.rotation);
        newRock.transform.SetParent(rockTarget);
    }
    public void OnAnimationThrowRock()
    {
        GameObject rock = GameObject.FindWithTag("AniRock");
        Rigidbody rockRigidbody = rock.gameObject.AddComponent<Rigidbody>();

            // Adjust Rigidbody properties as needed
            rockRigidbody.mass = 50f;
            rockRigidbody.drag = 0.5f;
            rockRigidbody.angularDrag = 0.5f;
        
        rock.transform.SetParent(null);
        float force = 500f;
        Vector3 launchDirection = rock.transform.forward;
        rockRigidbody.AddForce(launchDirection * force, ForceMode.Impulse);
    }
}
