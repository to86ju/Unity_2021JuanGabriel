using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;//null,Singleton 

    public GameObject prefab;//objeto para la piscina de balas
    public List<GameObject> pooledObjects;//Piscina de balas
    public int amountToPool;//Cantidad de objetos a la piscina de balas

    private void Awake()
    {
        //si la variable SharedInstance es nula
        if (SharedInstance == null)
        {
            SharedInstance = this;//la creo
        }
        //si la variable SharedInstance esta creada
        else
        {
            Debug.LogError("Ya hay otro pool en pantalla");
            Destroy(gameObject);//destruir todos los SharedInstance creados despues
        }
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;

        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(prefab);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }


    //funcion para encontrar el primer objeto activo
    public GameObject GetFirstPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
}
