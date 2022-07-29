using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Sight_ai))]
public class EnmeyFSM_ai : MonoBehaviour
{
    public enum EnemyState { Gotobase, AttackBase, ChasePlayer, AttackPlayer}


    public EnemyState currentState;

    private Sight_ai _sight;

    private Transform baseTransform;

    public float baseAttackDistance, playerAttackDistance;

    private NavMeshAgent agent;

    private Animator animator;

    private float lastShootTime;
    public float shootRate;

    public GameObject shootingPoint;

    private void Awake()
    {
        _sight = GetComponent<Sight_ai>();
        baseTransform = GameObject.FindWithTag("Base").transform;

        agent = GetComponentInParent<NavMeshAgent>();
        animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Gotobase:
                GotoBase();
                break;
            case EnemyState.AttackBase:
                AttackBase();
                break;
            case EnemyState.ChasePlayer:
                ChasePlayer();
                break;
            case EnemyState.AttackPlayer:
                AttackPlayer();
                break;

            default:
                break;
        }

    }

    void GotoBase()
    {
        print("Ir a base");

        agent.isStopped = false;
        agent.SetDestination(baseTransform.position);

        if (_sight.detectedTarget != null)
        {
            currentState = EnemyState.ChasePlayer;
        }

        float distaceToBase = Vector3.Distance(transform.position, baseTransform.position);
        if (distaceToBase < baseAttackDistance)
        {
            currentState = EnemyState.AttackBase;
        }
    }

    void AttackBase()
    {
        print("Atacar la base enemiga");
        agent.isStopped = true;
        LookAt(baseTransform.position);
        ShootTarget();
    }

    void ChasePlayer()
    {
        print("Persiguir al jugador");
        if (_sight.detectedTarget == null)
        {
            currentState = EnemyState.Gotobase;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(_sight.detectedTarget.transform.position);

        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);
        if (distanceToPlayer < playerAttackDistance)
        {
            currentState = EnemyState.AttackPlayer;
        }
    }

    void AttackPlayer()
    {
        print("Atacar al jugador");
        agent.isStopped = true;

        if (_sight.detectedTarget == null)
        {
            currentState = EnemyState.Gotobase;
            return;
        }

        LookAt(_sight.detectedTarget.transform.position);

        ShootTarget();

        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);
        if (distanceToPlayer > playerAttackDistance*1.1f)
        {
            currentState = EnemyState.ChasePlayer;
        }
    }

    void ShootTarget()
    {
        if (Time.timeScale > 0)
        {
            var timeSinceLastShoot = Time.time - lastShootTime;

            if (timeSinceLastShoot < shootRate)
            {
                return;
            }

            animator.SetTrigger("Shot Bullet");

            lastShootTime = Time.time;
            var bulllet = ObjectPool.SharedInstance.GetFirstPooledObject();
            bulllet.layer = LayerMask.NameToLayer("Enemy Bullet");

            bulllet.transform.position = shootingPoint.transform.position;
            bulllet.transform.rotation = shootingPoint.transform.rotation;

            bulllet.SetActive(true);
        }
    }

    void LookAt(Vector3 targetPos)
    {
        var directionToLook = Vector3.Normalize( targetPos - transform.position);
        directionToLook.y = 0;
        transform.parent.forward = directionToLook;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, playerAttackDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, baseAttackDistance);
    }
}
