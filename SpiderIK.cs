using UnityEngine;

public class SpiderIK : MonoBehaviour
{
    private Animator animator;
    public Transform target;  // Target for the spider to walk towards (player position)
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // Set the positions of each foot using IK
            SetFootIK(AvatarIKGoal.LeftFoot, target.position);  // Adjust foot position
            SetFootIK(AvatarIKGoal.RightFoot, target.position);
            SetFootIK(AvatarIKGoal.LeftHand, target.position);  // Optional for additional touchpoints
            SetFootIK(AvatarIKGoal.RightHand, target.position);
        }
    }

    void SetFootIK(AvatarIKGoal foot, Vector3 targetPos)
    {
        // Here we use raycasting or other techniques to get a valid position for the foot
        RaycastHit hit;
        Vector3 footPos = targetPos; // default to player position
        if (Physics.Raycast(targetPos + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity))
        {
            footPos = hit.point; // Set foot on the ground
        }

        // Set the IK goal position
        animator.SetIKPositionWeight(foot, 1f);
        animator.SetIKPosition(foot, footPos);
        animator.SetIKRotationWeight(foot, 1f);
        animator.SetIKRotation(foot, Quaternion.LookRotation(targetPos - transform.position));
    }

    void Update()
    {
        // Move towards the player (target)
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.Translate(direction * Time.deltaTime * 2f);  // Speed = 2f, adjust to your needs
    }
}
