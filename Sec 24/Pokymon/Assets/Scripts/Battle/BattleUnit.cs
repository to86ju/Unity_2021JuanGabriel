using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    public PokemonBase _base;
    public int _level;
    public Pokemon Pokemon { get; set; }
    public bool isPlayer;

    public void SetupPokemon()
    {
        Pokemon = new Pokemon(_base, _level);

        GetComponent<Image>().sprite =
            (isPlayer ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite);
    }

}
