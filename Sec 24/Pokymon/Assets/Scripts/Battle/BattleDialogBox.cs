using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField]private Text dialogoText;
    public float charactersPerSecond= 10.0f;

    [SerializeField] private GameObject actionSelect;
    [SerializeField] private GameObject movementSelect;
    [SerializeField] private GameObject movementDesc;

    public IEnumerator SetDialog(string message)
    {
        dialogoText.text = "";
        foreach (var character in message)
        {
            dialogoText.text += character;
            yield return new WaitForSeconds(1/charactersPerSecond);
        }
    }
}
