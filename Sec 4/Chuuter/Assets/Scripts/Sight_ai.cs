using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight_ai : MonoBehaviour
{

    public float distance;//Distancia de vision
    public float angle;//angulo de vision

    public LayerMask targetLayers;//Enemigos(player y base)
    public LayerMask obstacleLayers;//obtaculos 

    public Collider detectedTarget;

    private void Update()
    {
        //colecion de enemigos(player o base)
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance, targetLayers);

        detectedTarget = null;

        //Hemos pasado el primer filtro: La distancia
        foreach (Collider collider in colliders)
        {
            //direción al collider
            Vector3 directionToCollider = Vector3.Normalize(collider.bounds.center - transform.position);

            //Angulo que forma el vector visión con el vector objetivo
            float angleToCollider = Vector3.Angle(transform.forward, directionToCollider);
            //cos(angle) = u.v / ||u||.||v||

            //Si el ángulo es menor que el de visión
            if (angleToCollider < angle)
            {
                
                //Comprobaomos que en la line de visión emenmigo -> objetivo no hay obtáculos
                if (!Physics.Linecast(transform.position,collider.bounds.center, out RaycastHit hit,obstacleLayers))
                {
                    //Guardamos la referncia del objetivo detectado
                    Debug.DrawLine(transform.position, collider.bounds.center, Color.green);
                    Debug.Log("player");
                    detectedTarget = collider;

                    break;//salir en el primero que se cumpla
                }
                else
                {
                    
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        //------------ distancia --------------------------
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distance);
        //-------------------------------------------------

        //---------- cono de vision ---------------------
        Gizmos.color = Color.magenta;
        Vector3 rightDir = Quaternion.Euler(0, angle, 0)*transform.forward;
        Vector3 leftDit = Quaternion.Euler(0,-angle,0)*transform.forward;
        Gizmos.DrawRay(transform.position, rightDir * distance);
        Gizmos.DrawRay(transform.position, leftDit * distance);
        //-----------------------------------------------
    }

}
