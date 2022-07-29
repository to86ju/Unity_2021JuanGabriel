using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    //f = m*a
    [Tooltip("Fuerza de Movimiento del personaje en N/s")]
    [Range(0,1000)]
    public float speed;

    [Tooltip("Fuerza de Rotacion del persoje en N/seg")]
    [Range(0,360)]
    public float rotationSpeed;

    private Rigidbody _rb;
    private Animator _animator;

    private void Start()
    {
        //---Quitar el cursor en el juego--
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //-------------------------
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }


    void Update()
    {
        //--- Movimiento del personaje ---
        // s = v*t
        float space = speed * Time.deltaTime;

        float horizontal = Input.GetAxis("Horizontal"); // -1 a 1
        float vertical = Input.GetAxis("Vertical"); // -1 a 1

        Vector3 dir = new Vector3(horizontal, 0, vertical);
        //transform.Translate(dir.normalized*space);
        //fuerza de translacion
        _rb.AddRelativeForce(dir.normalized * space);
        //----------------------------------

        //--Giro del personaje----
        float angle = rotationSpeed * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X");// -1 a 1
        //transform.Rotate(0, mouseX*angle, 0);
        //fuerza de rotacion <-> Torque
        _rb.AddRelativeTorque(0, mouseX * angle, 0);
        //-------------------------

        _animator.SetFloat("Velocity", _rb.velocity.magnitude);

        /*
        _animator.SetFloat("MoveX", horizontal);
        _animator.SetFloat("MoveY", vertical);

        if (_rb.velocity.magnitude > 2.0f)
        {
           
        }
        else
        {
            _animator.SetFloat("Velocity", 0);
        }
        */

        /*
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Translate(0, 0, space);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Translate(0, 0, -space);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Translate(-space, 0, 0);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Translate(space, 0, 0);
        }
        */
    }
}
