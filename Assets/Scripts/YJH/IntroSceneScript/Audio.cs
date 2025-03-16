using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Audio : MonoBehaviour
{
    public VideoPlayer clip;
    public Toggle muteTG;
    public void OnToggleIsOn()
    {
        if(muteTG.isOn)
        {
            clip.playbackSpeed = 0;
        }
        else
        {
            clip.playbackSpeed = 1;
        }
    }
}
