using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAnimator_ai : MonoBehaviour
{
    private NavMeshAgent _anget;
    private Animator _animator;

    private void Awake()
    {
        _anget = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetFloat("Velocity", _anget.velocity.magnitude);
    }

}
