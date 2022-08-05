using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    
   
    

    private Animator _animator;

    public int bulletsAmount;//cantidad de balas

    public Weapon weapon;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //-------------Disparar la bala----------------

        //Si pulsamos el boton de raton izquierdo y el juego no esta parado
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.timeScale > 0)
        {
            //_animator.SetTrigger("Shot Bullet");
            _animator.SetBool("Shot Bullet Bool", true);

            if (bulletsAmount > 0 && weapon.ShootBullet("Player Bullet", 0.25f))
            {

                bulletsAmount--;//restar balas disparadas

                //si no tengo balas
                if (bulletsAmount < 0)
                {
                    //balas igual a 0
                    bulletsAmount = 0;
                }
            }
        }
        else
        {
            _animator.SetBool("Shot Bullet Bool", false);
        }
        //-----------------------------------------------
    }

    //funcion para instanciar la bala

}
