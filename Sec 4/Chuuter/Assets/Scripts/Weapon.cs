using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private float lastShootTime;
    public float shootRate;

    public GameObject shootingPoint;

    public ParticleSystem fireEffect;
    public AudioSource shootSound;

    private string layerNameP;

    public bool ShootBullet(string layername, float delay)
    {
        //Si no estamos en pausa
        if (Time.timeScale > 0)
        {
            //Momento desde el ultimo disparo
            var timeSinceLastShoot = Time.time - lastShootTime;

            if (timeSinceLastShoot < shootRate)
            {
                return false;
            }

            lastShootTime = Time.time;

            this.layerNameP = layername;
            Invoke("FireBullet",delay);

           
            return true;
        }

        return false;
    }

    void FireBullet()
    {
        //-------- instancia de la bala-------------------------
        var bulllet = ObjectPool.SharedInstance.GetFirstPooledObject();
        bulllet.layer = LayerMask.NameToLayer(layerNameP);

        bulllet.transform.position = shootingPoint.transform.position;
        bulllet.transform.rotation = shootingPoint.transform.rotation;

        bulllet.SetActive(true);

        //----- efecto de disparo y sonido--------------
        if (fireEffect != null)
        {
            fireEffect.Play();
        }

        if (shootSound != null)
        {
            Instantiate(shootSound, transform.position, transform.rotation).GetComponent<AudioSource>().Play();
        }
        //------------------------------------

        //-------------------------------------------------------
    }
}
