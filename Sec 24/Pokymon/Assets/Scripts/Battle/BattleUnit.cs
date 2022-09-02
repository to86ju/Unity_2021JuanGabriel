using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    public PokemonBase _base;
    public int _level;
    public Pokemon Pokemon { get; set; }
    public bool isPlayer;

    private Image pokemonImage;
    private Vector3 inicialPosition;
    private Color InitialColor;

    [SerializeField] float startTimeAnam = 1.0f, attackTimeAnim= 0.3f,hitTimeAnim= 0.15f ,dieTimeAnim = 1f;

    private void Awake()
    {
        pokemonImage = GetComponent<Image>();
        inicialPosition = pokemonImage.transform.localPosition;
        InitialColor = pokemonImage.color;
    }

    public void SetupPokemon()
    {
        Pokemon = new Pokemon(_base, _level);

        pokemonImage.sprite =
            (isPlayer ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite);

        PlayStartAnimatiocion();
    }

    //-------------- animaciones ------------------------------
    public void PlayStartAnimatiocion()
    {
       
        pokemonImage.transform.localPosition = 
            new Vector3(inicialPosition.x+(isPlayer?-1:-1)*400, inicialPosition.y);

        pokemonImage.transform.DOLocalMoveX(inicialPosition.x, startTimeAnam);
      
    }

    public void PlayAttackAnimation()
    {
        var seq = DOTween.Sequence();

        seq.Append(pokemonImage.transform.DOLocalMoveX(inicialPosition.x + (isPlayer ? 1 : -1) + 60, attackTimeAnim));
        seq.Append(pokemonImage.transform.DOLocalMoveX(inicialPosition.x, attackTimeAnim));
    }

    public void PlayReciveAttackAnimation()
    {
        var seq = DOTween.Sequence();

        seq.Append(pokemonImage.DOColor(Color.grey, hitTimeAnim));
        seq.Append(pokemonImage.DOColor(InitialColor, hitTimeAnim));
    }

    public void PlayFaintAnimation()
    {
        var seq = DOTween.Sequence();

        seq.Append(pokemonImage.transform.DOLocalMoveY(inicialPosition.y - 200, dieTimeAnim));
        seq.Join(pokemonImage.DOFade(0f, dieTimeAnim));
    }

}
