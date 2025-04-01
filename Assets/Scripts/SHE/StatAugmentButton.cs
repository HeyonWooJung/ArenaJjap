using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatAugmentButton : MonoBehaviour
{
    [SerializeField]Text nameText;
    [SerializeField] Text statInt;
    [SerializeField] Text statDescription;
    [SerializeField]Sprite image;
    [SerializeField] ScriptableStatPlus statSO;

    private void OnEnable()
    {
        nameText.text = statSO.statName;
        statInt.text = statSO.statPlusInt.ToString();
        image = statSO.icon;
        statDescription.text = statSO.statDescription;
    }
}
