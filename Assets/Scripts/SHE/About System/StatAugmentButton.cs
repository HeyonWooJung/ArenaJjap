using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatAugmentButton : MonoBehaviour
{
    [SerializeField]Text nameText;
    [SerializeField] Text statDescription;
    [SerializeField] Image image;
    [SerializeField] ScriptableStatPlus statSO;

    private void OnEnable()
    {
        nameText.text = statSO.statName;
        image.sprite = statSO.icon;
        statDescription.text = statSO.statDescription;
    }

    



}
