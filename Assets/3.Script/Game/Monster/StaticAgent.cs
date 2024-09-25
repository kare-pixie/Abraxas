using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StaticAgent : MonoBehaviour
{
    [SerializeField] private Transform[] targets;
    private int targetIdx = -1;
    private NavMeshAgent agent;
    private Action onComplete;

    private Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        TryGetComponent(out animator);
        onComplete += SetTarget;
        SetTarget();
    }
    private void Update()
    {
        if(agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance - agent.stoppingDistance < 0.1f)
        {
            onComplete.Invoke();
        }
    }
    private void LateUpdate()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
    private void SetTarget()
    {
        targetIdx = (targetIdx + 1) % targets.Length;
        agent.SetDestination(targets[targetIdx].position);
    }
}
