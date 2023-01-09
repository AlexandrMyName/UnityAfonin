using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_MovementBehavior : MonoBehaviour
{

    Rigidbody rb;
    Animator animator;
    CapsuleCollider capsuleCollider;
    public float speedForward = 5f;
    public float speedRotation = 2f;

    private float horizontal;
    private float vertical;

    private float mouseX;
    private float mouseY;

    Transform cm;
    Transform camera_Behavior;
     
     public LayerMask layerForCamera;


    public LayerMask noPlayer;
    public LayerMask everything;


    Quaternion InitotationCM;

    Quaternion rotation;
    Vector3 inputs;

    public float speedCM = 10f;
    public Vector3 cameraOFFset/* = new Vector3(0f, 3.5f, -5f)*/;
    private Quaternion rotationCM;

    public float clampingX_MIN;
    public float clampingX_MAX;
    public float clampingY_MIN;
    public float clampingY_MAX;

    void Awake()
    {
         rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        cm = GameObject.Find("Camera").GetComponent<Transform>();
        camera_Behavior = GameObject.Find("CameraPOSITION").GetComponent<Transform>();
        cm.position = camera_Behavior.transform.TransformPoint(cameraOFFset);


        InitotationCM = cm.localRotation;
    }

    
    void Update()
    {
        camera_Behavior.position = this.transform.position + Vector3.up * 2f;

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");


       

        animator.SetFloat("Vertical", vertical);
        if(!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.S) && Mathf.Abs(vertical) !=0)
        animator.SetFloat("Horizontal", horizontal);
        else
        {
            animator.SetFloat("Horizontal", 0);
        }


        inputs.Set(horizontal, 0f, vertical);

        PlayerRotation();


      
    }
    private void LateUpdate()
    {
        CameraBehavior();
    }

    private void PlayerRotation()
    {
       

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            Quaternion cameraRotation = cm.rotation;
            cameraRotation.x = 0f;
            cameraRotation.z = 0f;


            rotation = Quaternion.Slerp(transform.rotation, cameraRotation, speedRotation * Time.fixedDeltaTime);
        }
        else
        {
            rotation = transform.rotation;
        }
    }

    private void OnAnimatorMove()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? 5f : speedForward;
        if (Input.GetKey(KeyCode.S) /*&& speed != 5f*/)
        {
            rb.MovePosition(transform.position + -transform.forward * speedForward * animator.deltaPosition.magnitude);
        }
       else if (Input.GetKey(KeyCode.W))
        {
            rb.MovePosition(transform.position + transform.forward * speed * animator.deltaPosition.magnitude);
            if(speed == 5f && !Input.GetKey(KeyCode.S))
            {
                animator.SetBool("Sprint", true);
            }
            else
            {
                animator.SetBool("Sprint", false);
            }
        }
         if (Mathf.Abs(vertical) == 1 && horizontal == 1 && speed != 5f && !Input.GetKey(KeyCode.S))
        {
            rb.MovePosition(transform.position + (transform.forward + transform.right).normalized * speed * animator.deltaPosition.magnitude);
        }
        else if (Mathf.Abs(vertical) == 1 && horizontal == -1 && speed != 5f && !Input.GetKey(KeyCode.S))
        {
            rb.MovePosition(transform.position + (transform.forward - transform.right).normalized * speed * animator.deltaPosition.magnitude);
        }
      


        rb.MoveRotation(rotation);
    }

    public void CameraBehavior()
    {
        rotationCM.y += mouseX * speedCM;
        rotationCM.x += -mouseY * speedCM;
        rotationCM.z = 0f;

       
        rotationCM.x = Mathf.Clamp(rotationCM.x, clampingX_MIN, clampingX_MAX);



        float distance = Vector3.Distance(cm.position, transform.position);
        float maxDistance = Vector3.Distance(transform.position, camera_Behavior.transform.TransformPoint(cameraOFFset));

       
            camera_Behavior.rotation = Quaternion.Euler(rotationCM.x, rotationCM.y, rotationCM.z);
        cm.LookAt(camera_Behavior.position);


        RaycastHit hitObstacle;
        if(
            Physics.Raycast(camera_Behavior.transform.position, 
            cm.transform.position - camera_Behavior.transform.position,
            out hitObstacle, maxDistance,
            layerForCamera
            )){

            cm.position = hitObstacle.point;
            cm.LookAt(camera_Behavior.position);
            Debug.Log("Ray");
#if UNITY_EDITOR
            Debug.DrawRay(camera_Behavior.transform.position, cm.transform.position - camera_Behavior.transform.position, Color.red);
#endif

        }
        //Возможно это и есть ошибка дергания.. -transform.pos ]
         if (distance < maxDistance && !Physics.Raycast(cm.position, -cm.transform.forward, 0.1f, layerForCamera))
        {

            cm.transform.position -= cm.transform.forward * 0.1f;
            cm.LookAt(camera_Behavior.position);
        }
       
        

        if(Vector3.Distance(cm.transform.position,transform.position) < 2f)
        {
          
            Camera.main.cullingMask = noPlayer;
        }
        else { Camera.main.cullingMask = everything;  }
    }

}
