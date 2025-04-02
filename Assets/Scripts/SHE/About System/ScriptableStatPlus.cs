using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StatType
{
    HP = 0,
    Atk,
    AtkSpeed,
    Def,
    Critical,
    CirtDamage,
    LifeSteel,
    MoveSpeed,
    AbilityHaste
}



[CreateAssetMenu(fileName = "Augment", menuName = "Augment System/Augment")]
public class ScriptableStatPlus : ScriptableObject
{
    public string statName;
    public int statPlusInt;
    public Sprite icon;
    public string statDescription;
    public StatType statType;
    
   
}
