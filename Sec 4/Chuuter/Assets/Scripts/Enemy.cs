using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Cantidad de puntos que se obtienen al derrotar al ememigo")]
    public int pointsAmount = 10;

    private void Awake()
    {
        var life = GetComponent<life>();
        life.onDeath.AddListener(DestroyEnemy);//listener o viculacion evento muerte enemigo
    }


    private void Start()
    {
        EnemyManager.SharedInstance.AddEnemy(this);//añademe a la lista de enemigos
    }

    //funcion para destruir el enemigo si no tiene vida
    private void DestroyEnemy()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Play Die");

        Invoke("PlayDestruction", 0.2f);

        var life = GetComponent<life>();
        //life.onDeath.RemoveListener(DestroyEnemy);//Matar el listener

        Destroy(gameObject, 1);

        EnemyManager.SharedInstance.RemoveEnemy(this);//quitame de la lista de enemigos
        ScoreManager.ShareInstance.Amount += pointsAmount;
    }

    //funcion para ejecutar una explosion en pantalla
    void PlayDestruction()
    {
        ParticleSystem explosion = gameObject.GetComponentInChildren<ParticleSystem>();
        explosion.Play();
    }
}
