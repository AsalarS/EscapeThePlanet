using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
public class PlayerMovment : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 4f; //character's speed
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance =0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isOnGround;

    Animator animator; // an animator object to reference in unity
    //private PlayerMovment playerMovment;
    private MouseLook mouseLook;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform virtualCameraSource;
    [SerializeField] private Transform virtualCameraLook;
    [HideInInspector] public static bool IsAnimating { get; set; }// to prevent multiple takedowns in the same location and time
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private Transform rockTarget;
    [SerializeField] public GameObject WeaponInventory;
    [SerializeField] private Camera cinemachineBrain;
    private int playerLayer, defaultCullingMask;
    public AudioSource takedownInitial, punch, kick;
    public bool HasToken { get; set; } = false;

    void Start()
    {
        animator = GetComponent<Animator>(); // a referance for unity's component
        //playerMovment = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovment>();
        mouseLook = GameObject.FindGameObjectWithTag("FPSCamera").GetComponent<MouseLook>();
        playerLayer = LayerMask.NameToLayer("Player");
        defaultCullingMask = cinemachineBrain.cullingMask;
    }

    // Update is called once per frame
    void Update()
    {
        isOnGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isOnGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal"); //get horizontal movement
        float z = Input.GetAxis("Vertical"); //get vertical movement

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButton("Jump") && isOnGround)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

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
        takedownInitial.Play();
        IsAnimating = true; //a repeated step - to avoid potential exploits
        EnablePlayerVisibilty();
        virtualCamera.transform.position = virtualCameraSource.position; //transform the VCamera to the source near the player
        virtualCamera.transform.LookAt(virtualCameraLook); //rotate the camera to the source
        animator.applyRootMotion = true; //enable root motion
        DisableMovement();
        //reset fps camera position
        Vector3 cameraEulerAngles = mouseLook.transform.eulerAngles;
        cameraEulerAngles.x = 0f;
        mouseLook.transform.eulerAngles = cameraEulerAngles;
        
        virtualCamera.Priority = 21; //transfer view to VCamera

        //Disable gun
        if (WeaponInventory.GetComponent<WeaponSwitching>() != null)
        {
            WeaponSwitching weaponInventory = WeaponInventory.GetComponent<WeaponSwitching>();
            weaponInventory.DisableGun();
        }
    }
    public void EnablePlayerVisibilty()
    {
        cinemachineBrain.cullingMask |= (1 << playerLayer);
    }
    public void OnTakedownAnimationEnd()
    {
        IsAnimating = false; //enable takedowns 
        animator.applyRootMotion = false; //disable root motion
        EnableMovement();
        //Disable gun
        if (WeaponInventory.GetComponent<WeaponSwitching>() != null)
        {
            WeaponSwitching weaponInventory = WeaponInventory.GetComponent<WeaponSwitching>();
            weaponInventory.EnableGun();
        }
        DisablePlayerVisibilty();
    }
    public void DisablePlayerVisibilty()
    {
        cinemachineBrain.cullingMask = defaultCullingMask;

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
            rockRigidbody.mass = 70f;
            rockRigidbody.drag = 0.5f;
            rockRigidbody.angularDrag = 0.5f;
        
        rock.transform.SetParent(null);
        float force = 400f;
        Vector3 launchDirection = rock.transform.forward;
        rockRigidbody.AddForce(launchDirection * force, ForceMode.Impulse);
        Destroy(rock,5f);
    }

    public void playPunchSFX()
    {
        punch.Play();
    }

    public void OnAnimationDeath()
    {
        EnablePlayerVisibilty();
        virtualCamera.transform.position = virtualCameraSource.position; //transform the VCamera to the source near the player
        virtualCamera.transform.LookAt(virtualCameraLook); //rotate the camera to the source
        animator.applyRootMotion = true; //enable root motion
        DisableMovement();
        virtualCamera.Priority = 21; //transfer view to VCamera
    }

    public void DisableMovement()
    {
        controller.enabled = false; //disable movement
        mouseLook.enabled = false; //disable camera rotation
    }

    public void EnableMovement()
    {
        controller.enabled = true;
        mouseLook.enabled = true;
    }

    public void PlayKickSFX()
    {
        kick.Play();
    }
}
