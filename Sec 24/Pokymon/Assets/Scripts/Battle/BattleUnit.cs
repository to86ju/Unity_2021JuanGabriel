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

    [SerializeField]private bool isPlayer;

    [SerializeField] private BattleHUD hud;

    private Image pokemonImage;
    private Vector3 inicialPosition;
    private Color InitialColor;

    [SerializeField] float startTimeAnam = 1.0f, attackTimeAnim= 0.3f,hitTimeAnim= 0.15f ,dieTimeAnim = 1f,capturedTimeAnim= 0.6f;

    //---- geter o seter--------
    public bool IsPlayer 
    { 
        get => isPlayer; 
    }
    public BattleHUD Hud 
    { 
        get => hud; 
    }
    //---------------------------

    private void Awake()
    {
        pokemonImage = GetComponent<Image>();
        inicialPosition = pokemonImage.transform.localPosition;
        InitialColor = pokemonImage.color;
    }

    public void SetupPokemon(Pokemon pokemon)
    {
        Pokemon = pokemon;

        pokemonImage.sprite =
            (IsPlayer ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite);
        pokemonImage.color = InitialColor;

        hud.SetPokemonData(pokemon);// configuar el hud

        transform.localScale = new Vector3(1, 1, 1);//escala original

        PlayStartAnimatiocion();
    }

    //-------------- animaciones ------------------------------
    public void PlayStartAnimatiocion()
    {
       
        pokemonImage.transform.localPosition = 
            new Vector3(inicialPosition.x+(IsPlayer?-1:-1)*400, inicialPosition.y);

        pokemonImage.transform.DOLocalMoveX(inicialPosition.x, startTimeAnam);
      
    }

    public void PlayAttackAnimation()
    {
        var seq = DOTween.Sequence();

        seq.Append(pokemonImage.transform.DOLocalMoveX(inicialPosition.x + (IsPlayer ? 1 : -1) + 60, attackTimeAnim));
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

    public IEnumerator PlayCapturedAnimation()
    {
        var seq = DOTween.Sequence();

        seq.Append(pokemonImage.DOFade(0, capturedTimeAnim));
        seq.Join(transform.DOScale(new Vector3(0.25f,025f,1f),capturedTimeAnim));

        seq.Join(transform.DOLocalMoveY(inicialPosition.y + 50f, capturedTimeAnim));

        yield return seq.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var seq = DOTween.Sequence();

        seq.Append(pokemonImage.DOFade(1, capturedTimeAnim));
        seq.Join(transform.DOScale(new Vector3(1f, 1f, 1f), capturedTimeAnim));

        seq.Join(transform.DOLocalMoveY(inicialPosition.y, capturedTimeAnim));

        yield return seq.WaitForCompletion();
    }
    //-----------------------------------------------------------
}
