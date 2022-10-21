using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private bool isMoving;
    public float speed;
    private Vector2 input;

    private Animator __animator;

    public LayerMask solidObjectsLayer, pokemonLayer;

    public event Action OnPokemonEncountered;

    [SerializeField] float verticaloffset = 0.7f;

    private void Awake()
    {
        __animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
            {
                input.y = 0;
            }

            if (input != Vector2.zero)
            {
                __animator.SetFloat("Move x", input.x);
                __animator.SetFloat("Move y", input.y);

                var targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                if (IsAvailable(targetPosition))
                {
                    StartCoroutine(MoveTowards(targetPosition));
                }
                
            }
        }
    }

    private void LateUpdate()
    {
        __animator.SetBool("Is moving", isMoving);
    }

    IEnumerator MoveTowards(Vector3 destination)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, destination) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;

        isMoving = false;

        CheckForPokemon();
    }


    /// <summary>
    /// El método comprueba que la zona a la que  queremos acceder, esté  disponible
    /// </summary>
    /// <param name="target"> Zona a la que queremos acceder</param>
    /// <returns> Devuelve  true, si el target está  disponible y false en caso contrario</returns>
    private bool IsAvailable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target,0.25f,solidObjectsLayer) !=null)
        {
            return false;
        }

        return true;
    }


    private void CheckForPokemon()
    {
        if (Physics2D.OverlapCircle(transform.position- new Vector3(0,verticaloffset,0), 0.25f, pokemonLayer) !=null)
        {
            if (Random.Range(0,100) < 10)
            {
                //Trasmite el evento
                OnPokemonEncountered();
            }
        }
    }
}
