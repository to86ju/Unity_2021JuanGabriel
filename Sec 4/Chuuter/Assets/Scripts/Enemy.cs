using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Cantidad de putnos que se obtienen al derrotar al ememigo")]
    public int pointsAmount = 10;

    private void Awake()
    {
        var life = GetComponent<life>();
        life.onDeath.AddListener(DestroyEnemy);//listener o viculacion
    }


    private void Start()
    {
        EnemyManager.SharedInstance.AddEnemy(this);
    }

    private void DestroyEnemy()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Play Die");

        Invoke("PlayDestruction", 0.2f);

        var life = GetComponent<life>();
        //life.onDeath.RemoveListener(DestroyEnemy);//Matar el listener

        Destroy(gameObject, 1);

        EnemyManager.SharedInstance.RemoveEnemy(this);
        ScoreManager.ShareInstance.Amount += pointsAmount;
    }

    void PlayDestruction()
    {
        ParticleSystem explosion = gameObject.GetComponentInChildren<ParticleSystem>();
        explosion.Play();
    }
}
