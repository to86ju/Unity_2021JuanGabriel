using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight_ai : MonoBehaviour
{

    public float distance;
    public float angle;

    public LayerMask targetLayers;
    public LayerMask obstacleLayers;

    public Collider detectedTarget;

    private void Update()
    {
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance, targetLayers);

        detectedTarget = null;

        //Hemos pasado el primer filtro: La distancia
        foreach (Collider collider in colliders)
        {
            Vector3 directionToCollider = Vector3.Normalize(collider.bounds.center - transform.position);

            //Angulo que forma el vector vsión con el vector objetivo
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

                    break;
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distance);

        Gizmos.color = Color.magenta;
        Vector3 rightDir = Quaternion.Euler(0, angle, 0)*transform.forward;
        Vector3 leftDit = Quaternion.Euler(0,-angle,0)*transform.forward;
        Gizmos.DrawRay(transform.position, rightDir * distance);
        Gizmos.DrawRay(transform.position, leftDit * distance);
    }

}
