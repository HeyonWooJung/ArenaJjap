using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    [SerializeField] PlayerController localPlayer;
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
            { StatType.Atk, localPlayer.character.AdjustATK},
            { StatType.AtkSpeed, localPlayer.character.AdjustAtkSpeed},
            { StatType.Critical, localPlayer.character.AdjustCritChance },
            { StatType.CirtDamage, localPlayer.character.AdjustCritDmg },
            { StatType.HP, localPlayer.character.AdjustHP },
            { StatType.Def, localPlayer.character.AdjustDef },
            { StatType.LifeSteel, localPlayer.character.AdjustLifeSteal },
            
        };
        augmentIntActions = new Dictionary<StatType, Action<int>>()
        {
            {StatType.MoveSpeed, localPlayer.character.AdjustMoveSpeed},
            {StatType.AbilityHaste, localPlayer.character.AdjustAbilityHaste},
            {StatType.ArmorPen, localPlayer.character.AdjustArmorPen},
            {StatType.Lethal, localPlayer.character.AdjustLethality},
        };


        
    }

    public void FindLocalPlayer()
    {
        // 현재 로컬 플레이어의 PhotonView 찾기
        localPlayer = PhotonNetwork.LocalPlayer.TagObject as PlayerController;
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
        
        StartCoroutine(StatAugmentInstantiate());
    }
    //증강 띄우고 고르면 여기서 패널 끈 걸로 알아차린 것처럼 똑같이 스탯 증강 부르면 됨
    IEnumerator StatAugmentInstantiate()
    {
        int num = 0;
        int x = 0;
        count = 0;
        while(num < 3)
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
            foreach (GameObject activated in statAugmentPrefabs)
            {
                if (activated.activeSelf)
                {
                    activated.SetActive(false);
                }
            }
            num++;
        }
        
        
    }

    //IEnumerator AugmentAppear()
    //{
    //
    //
    //    yield return new WaitUntil(() => augmentPanel.activeSelf != true);
    //    StartCoroutine(StatAugmentInstantiate());
    //}
    



}

