using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Move",menuName ="Pokemon/Nuevo Movimiento")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private PokemonType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int pp;

    public string Name => name;
    public string Descrption => description;
    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int Pp => pp;
}
