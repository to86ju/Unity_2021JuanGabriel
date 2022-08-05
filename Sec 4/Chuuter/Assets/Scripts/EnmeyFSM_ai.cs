using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Sight_ai))]
public class EnmeyFSM_ai : MonoBehaviour
{
    //Estados de los enemigos
    public enum EnemyState { Gotobase, AttackBase, ChasePlayer, AttackPlayer}


    public EnemyState currentState;//Esado actual

    private Sight_ai _sight;// visor de la ia

    private Transform baseTransform;

    public float baseAttackDistance, playerAttackDistance;

    private NavMeshAgent agent;

    private Animator animator;

    public Weapon weapon;

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
                //ir a por la base
                GotoBase();
                break;
            case EnemyState.AttackBase:
                //atacar a la base
                AttackBase();
                break;
            case EnemyState.ChasePlayer:
                //ir a por el jugador
                ChasePlayer();
                break;
            case EnemyState.AttackPlayer:
                //atacar al jugador
                AttackPlayer();
                break;

            default:
                break;
        }

    }

    //funcion ir a base
    void GotoBase()
    {
        print("Ir a base");

        animator.SetBool("Shot Bullet Bool", false);

        agent.isStopped = false;//seguir el enemigo
        agent.SetDestination(baseTransform.position);//dirigite a la base

        //Si el ia detecta un objetivo(
        if (_sight.detectedTarget != null)
        {
            currentState = EnemyState.ChasePlayer;//Cambio estado: ir a por el jugador
        }

        //distancia hacia la base
        float distaceToBase = Vector3.Distance(transform.position, baseTransform.position);

        //si esto muy cerca de la base
        if (distaceToBase < baseAttackDistance)
        {
            currentState = EnemyState.AttackBase;//cambio estado: Atacar a la base
        }
    }

    //funcion atacar la base
    void AttackBase()
    {
        print("Atacar la base enemiga");
        agent.isStopped = true;//para el enemigo
        LookAt(baseTransform.position);
        ShootTarget();
    }

    //Funcion ir al por el jugador
    void ChasePlayer()
    {
        animator.SetBool("Shot Bullet Bool", false);

        print("Persiguir al jugador");

        //Si el ia no detecta un objetivo
        if (_sight.detectedTarget == null)
        {
            currentState = EnemyState.Gotobase;//Cambiar el estado: ir a base
            return;
        }

        agent.isStopped = false;//seguir el enemigo
        agent.SetDestination(_sight.detectedTarget.transform.position);

        //distancia hacia el jugador
        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);

        //si estoy cerca del jugador
        if (distanceToPlayer < playerAttackDistance)
        {
            //cambiar el estado: Atacar al jugador
            currentState = EnemyState.AttackPlayer;
        }
    }

    //funcion atacar al jugador
    void AttackPlayer()
    {
        print("Atacar al jugador");
        agent.isStopped = true;//Parar el enemigo

        //Si el ia no detecta un objetivo
        if (_sight.detectedTarget == null)
        {
            //cambiar estado: ir a la base
            currentState = EnemyState.Gotobase;
            return;
        }

        LookAt(_sight.detectedTarget.transform.position);

        ShootTarget();

        //distancia hacia el jugador
        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);

        //si estoy cerca del jugador un 10% mas
        if (distanceToPlayer > playerAttackDistance*1.1f)
        {
            currentState = EnemyState.ChasePlayer;//cambia estado. ir a por el jugador
        }
    }

    //metodo para disparar al objetivo(base o jugador)
    void ShootTarget()
    {
        if (weapon.ShootBullet("Enemy Bullet",0))
        {
            animator.SetBool("Shot Bullet Bool", true);
        }
        
    }

    //Metodo para mirar al objetivo
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
