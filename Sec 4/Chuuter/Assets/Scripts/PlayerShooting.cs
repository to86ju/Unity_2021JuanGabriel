using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    
    public GameObject shootingPoint;

    public ParticleSystem fireEffect;
    public AudioSource shootSound;

    private Animator _animator;

    public int bulletsAmount;//cantidad de balas

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
            _animator.SetTrigger("Shot Bullet");

            //Si nos quenda balas
            if (bulletsAmount > 0)
            {
                Invoke("FireBullet", 0.25f);//disparar
            }
            
        }
        //-----------------------------------------------
    }

    //funcion para instanciar la bala
    void FireBullet()
    {
        //bala = piscina de balas unica
        GameObject bullet = ObjectPool.SharedInstance.GetFirstPooledObject();

        bullet.layer = LayerMask.NameToLayer("Player Bullet");//cambiar la capa de la bala

        //posicion y rotacion para la bala
        bullet.transform.position = shootingPoint.transform.position;
        bullet.transform.rotation = shootingPoint.transform.rotation;

        bullet.SetActive(true);//activar la bala en pantalla

        bulletsAmount--;//restar balas disparadas

        fireEffect.Play();
        Instantiate(shootSound, transform.position, transform.rotation).GetComponent<AudioSource>().Play();


        if (bulletsAmount <0)
        {
            bulletsAmount = 0;
        }
    }
}
