using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageoncontact : MonoBehaviour
{
    public float damage;//daño

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        gameObject.SetActive(false);//Desactiva la bala

        /*
        if(other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            Destroy(other.gameObject);//Destruye el otro objeto
        }
        */

        life _life = other.GetComponent<life>();

        //si tiene un componete live
        if(_life != null)
        {

            _life.Amount -= damage;//Quitar vida
        }
    }
}
