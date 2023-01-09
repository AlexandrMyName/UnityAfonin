using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class IK_Controller : MonoBehaviour
{
    Animator animator;
    [Range(0,5)] public float offSetIK;
    [Range(0f, 1f)] public float DistanceToGround;
    public LayerMask layerMask;

    private int leftFootHash;
    private int rightFootHash;
    private float leftFootWeight;
    private float rightFootWeight;

    private float horizontal;
    private float vertical;

    public Transform headTarget;
    [Range(0,1)] public float bodyWeight;
    [Range(0, 1)] public float eyesWeight;
    [Range(0, 1)] public float headWeight;
    [Range(0, 1)] public float clampWeight;
    [Range(0, 1)] public float weightMain;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        leftFootHash = Animator.StringToHash("LeftFootWeight");
        rightFootHash = Animator.StringToHash("RightFootWeight");

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }



    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            if (isWalking())
            {
                leftFootWeight = animator.GetFloat(leftFootHash);
                rightFootWeight = animator.GetFloat(rightFootHash);
            }
            else
            {
                leftFootWeight = 1f;
                rightFootWeight = 1f;
            }
            #region Left Foot IK

            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, rightFootWeight);

            RaycastHit hit;
            Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) /*+ Vector3.up*/, Vector3.down);

            if (Physics.Raycast(ray, out hit, DistanceToGround + offSetIK, layerMask))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += DistanceToGround;
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);

                    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(forward,hit.normal));
                }
            }
            #endregion

            #region Right Foot IK

            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

            ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) /*+ Vector3.up*/, Vector3.down);

            if (Physics.Raycast(ray, out hit, DistanceToGround + offSetIK, layerMask))
            {
                if (hit.transform.tag == "Walkable")
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += DistanceToGround;
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);

                    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);

                    animator.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.LookRotation(forward, hit.normal));
                }
            }
            #endregion


            if (headTarget)
            {
                animator.SetLookAtWeight(weightMain, bodyWeight, headWeight, eyesWeight, clampWeight);
                animator.SetLookAtPosition(headTarget.position);
            }
        }
    }

    private bool isWalking()
    {
        bool isHorizontalInput = Mathf.Approximately(Mathf.Abs(horizontal), 1f);
        bool isVerticalInput = Mathf.Approximately(Mathf.Abs(vertical), 1f);
        bool isWalking = isHorizontalInput || isVerticalInput ? true : false;
        return isWalking;
    }
}
