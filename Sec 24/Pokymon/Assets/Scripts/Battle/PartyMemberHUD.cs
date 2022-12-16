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

        //Actualizar cunado corresponda
        hpText.text = $"{pokemon.Hp} / {pokemon.MaxHp}";
        healthBar.SetHP(pokemon);
        
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
