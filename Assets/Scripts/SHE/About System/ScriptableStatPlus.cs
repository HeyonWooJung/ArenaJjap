using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    AbilityHaste,
    ArmorPen,
    Lethal
}



[CreateAssetMenu(fileName = "Augment", menuName = "Augment System/Augment")]
public class ScriptableStatPlus : ScriptableObject
{
    public string statName;
    public float statPlusFloat;
    public Sprite icon;
    public string statDescription;
    public StatType statType;
    
   
}
