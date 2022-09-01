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

    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private List<Text> moventTexts;

    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;

    public IEnumerator SetDialog(string message)
    {
        dialogoText.text = "";
        foreach (var character in message)
        {
            dialogoText.text += character;
            yield return new WaitForSeconds(1/charactersPerSecond);
        }
    }

    public void ToggleDialogText(bool activated)
    {
        dialogoText.enabled = activated;
    }

    public void ToggleActions(bool activated)
    {
        actionSelect.SetActive(activated);
    }

    public void ToggleMovements(bool activated)
    {
        movementSelect.SetActive(activated);
        movementDesc.SetActive(activated);
    }
}
