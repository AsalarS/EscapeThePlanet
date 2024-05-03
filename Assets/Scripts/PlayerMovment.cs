using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 4f; //character's speed
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance =0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isOnGround;

    Animator animator; // an animator object to reference in unity
    //private PlayerMovment playerMovment;
    private MouseLook mouseLook;
    void Start()
    {
        animator = GetComponent<Animator>(); // a referance for unity's component
        //playerMovment = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovment>();
        mouseLook = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MouseLook>();
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
        //playerMovment.enabled = false;
        animator.applyRootMotion = true;
        controller.enabled = false;
        Vector3 cameraEulerAngles = mouseLook.transform.eulerAngles;
        cameraEulerAngles.x = 0f;
        mouseLook.transform.eulerAngles = cameraEulerAngles;
        mouseLook.enabled = false;
    }
    public void OnTakedownAnimationEnd()
    {
        //playerMovment.enabled = true;
        animator.applyRootMotion = false;
        controller.enabled = true;
        mouseLook.enabled = true;
    }
}
