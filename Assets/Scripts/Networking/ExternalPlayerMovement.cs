using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExternalPlayerMovement : MonoBehaviour
{
    //navigator and current navigation target
    private Vector3 TargetPosition;
    private NavMeshAgent Navigator;

    //previous position used to calculate distance travelled
    //sent to animation controller to automatically blend between
    //idle and walking animations during movement
    private Vector3 PreviousPosition;
    private Animator AnimationController;

    private void Awake()
    {
        TargetPosition = transform.position;
        PreviousPosition = transform.position;
        Navigator = GetComponent<NavMeshAgent>();
        AnimationController = GetComponent<Animator>();
    }

    private void Update()
    {
        float DistanceTravelled = Vector3.Distance(transform.position, PreviousPosition);
        PreviousPosition = transform.position;
        AnimationController.SetFloat("Movement", DistanceTravelled);
    }

    public void UpdatePosition(Vector3 NewTarget)
    {
        Navigator.SetDestination(NewTarget);
    }
}
