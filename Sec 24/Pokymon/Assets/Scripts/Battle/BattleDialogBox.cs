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

    public float timetoWaitAfterText = 1f;

    [SerializeField] private Color selectedColor = Color.blue;

    public bool isWriting= false;

    public AudioClip[] characterSounds;

    public IEnumerator SetDialog(string message)
    {
        isWriting = true;
        dialogoText.text = "";
        foreach (var character in message)
        {
            if (character != ' ')
            {
                SoundManager.sharedInstance.RandomSoundEffect(characterSounds);
            }
            dialogoText.text += character;
            yield return new WaitForSeconds(1/charactersPerSecond);
        }
        yield return new WaitForSeconds(timetoWaitAfterText);
        isWriting = false;
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

    public void SelectAction(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            actionTexts[i].color = (i == selectedAction ? selectedColor : Color.black);
            
        }
    }

    public void SelectMovement(int selectedMovement,Move move)
    {
        for (int i = 0; i < moventTexts.Count; i++)
        {
            moventTexts[i].color = (i == selectedMovement ? selectedColor : Color.black);

        }

        ppText.text = $"PP {move.Pp} / {move.Base.Pp}";
        typeText.text = move.Base.Type.ToString();

        ppText.color = (move.Pp <= 0 ? Color.red : Color.black);
    }

    public void SetPokemonMovements(List<Move> moves)
    {
        for (int i = 0; i < moventTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moventTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                moventTexts[i].text = "---";
            }
        }
    }
}
