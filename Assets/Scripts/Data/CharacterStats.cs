using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Antigravity/Data/Character")]
public class CharacterStats : ScriptableObject
{
    [Header("Character Info")]
    public string CharacterID;
    public string CharacterName;

    [Header("Visuals & Prefab")]
    public Sprite Portrait;
    public Sprite FullBody;
    public GameObject CharacterPrefab;

    [Header("Base Attributes")]
    public int BaseHP;
    public int BaseATK;
    public int BaseDEF;
    public int APContribution;

    [Header("Character Cards (0: BasicAttack, 1: Skill, 2: Ultimate)")]
    public List<CardData> Cards = new List<CardData>(3);
}
