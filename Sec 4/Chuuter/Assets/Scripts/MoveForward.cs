using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{

    public float speed;//Velocidad 

    // Update is called once per frame
    void Update()
    {
        //trasladar la bala en el eje z a la velocidad y 60 fm
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
