using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer clip;
    public Toggle muteTG;
    public Toggle aniStop;
    public RawImage loginImage;
    public void OnMuteToggleIsOn()
    {
        if(muteTG.isOn)
        {
            clip.SetDirectAudioMute(0, true);
        }
        else
        {
            clip.SetDirectAudioMute(0, false);
        }
    }
    public void OnAniStopIsOn()
    {
        if (aniStop.isOn)
        {
            loginImage.gameObject.SetActive(true);
        }
        else
        {
            loginImage.gameObject.SetActive(false);
        }
    }

}
