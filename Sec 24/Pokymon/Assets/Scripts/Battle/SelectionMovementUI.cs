using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMovementUI : MonoBehaviour
{
    [SerializeField] Text[] movementTexts;

    private void Start()
    {
        movementTexts = GetComponentsInChildren<Text>();
    }

    public void SetMovements(List<MoveBase> pokemonMoves, MoveBase newMove)
    {
        for (int i = 0; i < pokemonMoves.Count; i++)
        {
            movementTexts[i].text = pokemonMoves[i].name;
        }

        movementTexts[pokemonMoves.Count].text = newMove.name;
    }
}
