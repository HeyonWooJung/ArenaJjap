using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    [SerializeField] Character localPlayer;
    [SerializeField] Dictionary<StatType, Action<float>> augmentActions;
    [SerializeField] Dictionary<StatType, Action<int>> augmentIntActions;
    [SerializeField] GameObject[] statAugmentPrefabs;
    [SerializeField] int count;
    [SerializeField] GameObject augmentPanel;

    private void Start()
    {
        FindLocalPlayer();


        augmentActions = new Dictionary<StatType, Action<float>>()
        {
            { StatType.Atk, localPlayer.AdjustATK},
            { StatType.AtkSpeed, localPlayer.AdjustAtkSpeed},
            { StatType.Critical, localPlayer.AdjustCritChance },
            { StatType.CirtDamage, localPlayer.AdjustCritDmg },
            { StatType.HP, localPlayer.AdjustHP },
            { StatType.Def, localPlayer.AdjustDef },
            { StatType.LifeSteel, localPlayer.AdjustLifeSteal },
        };
        augmentIntActions = new Dictionary<StatType, Action<int>>()
        {
            {StatType.MoveSpeed, localPlayer.AdjustMoveSpeed},
            {StatType.AbilityHaste, localPlayer.AdjustAbilityHaste},
        };


        
    }

    public void FindLocalPlayer()
    {
        // 현재 로컬 플레이어의 PhotonView 찾기
        localPlayer = PhotonNetwork.LocalPlayer.TagObject as Character;
        if(localPlayer != null )
        {
           
            Debug.Log($"{localPlayer.name} 등록!");

        }
        else
        {
            Debug.LogWarning("로컬 플레이어를 찾을 수 없습니다!");
        }
    }

    public void ApplyStatAugment<T>(StatType statType, T value)
    {
       if(typeof(T)  == typeof(float) && augmentActions.TryGetValue(statType, out var actionFloat))
        {
            actionFloat.Invoke(Convert.ToSingle(value));
        }
       else if(typeof(T) == typeof(int) && augmentIntActions.TryGetValue(statType, out var actionInt))
        {
            actionInt.Invoke(Convert.ToInt32(value));
            
        }
        else
        {
            Debug.Log("그런 건 등록되지 않았어");
        }
    }

    public void SetUpAugment()
    {
        
        StartCoroutine(AugmentAppear());
    }
    //증강 띄우고 고르면 여기서 패널 끈 걸로 알아차린 것처럼 똑같이 스탯 증강 부르면 됨
    IEnumerator StatAugmentInstantiate()
    {
        int num = 0;
        int x = 0;
        count = 0;
        while(num < 2)
        {
            augmentPanel.SetActive(true);
            while (count < 3)
            {
                x = UnityEngine.Random.Range(0, 11);
                if (!statAugmentPrefabs[x].activeSelf)
                {
                    statAugmentPrefabs[x].SetActive(true);
                    count++;
                }
                yield return null;
            }

            yield return new WaitUntil(() => augmentPanel.activeSelf != true);
            num++;
        }
        
        
    }

    IEnumerator AugmentAppear()
    {


        yield return new WaitUntil(() => augmentPanel.activeSelf != true);
        StartCoroutine(StatAugmentInstantiate());
    }
    



}

