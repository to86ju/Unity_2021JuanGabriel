using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{

    public Text nameText, lvlTExt, typeText, hpText;
    public HealthBar healthBar;
    public Image pokemonImage;

    private Pokemon _pokemon;    

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        lvlTExt.text = $"Lv {pokemon.Level}";
        typeText.text = pokemon.Base.Type1.ToString().ToUpper();
        hpText.text = $"{pokemon.Hp} / {pokemon.MaxHp}";
        healthBar.SetHP((float)pokemon.Hp / pokemon.MaxHp);
        pokemonImage.sprite = pokemon.Base.FrontSprite;

        GetComponent<Image>().color = ColorManager.TypeColor.GetColorFromType(pokemon.Base.Type1);
    }

    public void SetSelectedPokemon(bool selected)
    {
        if (selected)
        {
            nameText.color = ColorManager.sharedInstance.selectecColor;
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}
