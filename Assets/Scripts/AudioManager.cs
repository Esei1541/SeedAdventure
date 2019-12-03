using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource[] channel;
    public AudioClip[] bgmClip;
    public AudioClip[] seClip;
    public int bgmNum;

    private void Awake()
    {
        channel = GetComponents<AudioSource>();
    }

    private void Start()
    {
        PlayBGM(bgmNum);
    }

    public void Pause(int chanNum)
    {
        if (channel[chanNum].isPlaying)
        {
            channel[chanNum].Pause();
        }
        else
        {
            channel[chanNum].Play();
        }
    }

    public void PlayBGM(int track)
    {
        channel[0].clip = bgmClip[track];
        channel[0].Play();
    }

    public void SetLoop(int chanNum, bool isLoop)
    {
        channel[chanNum].loop = isLoop;
    }

    public void PauseChannel(int chanNum)
    {
        channel[chanNum].Pause();
    }

    public void PlaySE(int track)
    {
        channel[1].clip = seClip[track];
        channel[1].Play();
    }

    public void PlaySE(int track, int chanNum)
    {
        channel[chanNum].clip = seClip[track];
        channel[chanNum].Play();
    }
}
