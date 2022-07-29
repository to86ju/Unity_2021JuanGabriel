using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager SharedInstance;

    [SerializeField]
    private List<Enemy> enemies;

    public UnityEvent onEnemyChyanged;//evento

    public int Enemycount
    {
        get => enemies.Count;
    }

    private void Awake()
    {
        if(SharedInstance == null)
        {
            SharedInstance = this;
            enemies = new List<Enemy>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddEnemy(Enemy enemyadd)
    {
        enemies.Add(enemyadd);
        onEnemyChyanged.Invoke();//Lanzar evento
    }

    public void RemoveEnemy(Enemy enemyremo)
    {
        enemies.Remove(enemyremo);
        onEnemyChyanged.Invoke();//Lanzar evento
    }
}
