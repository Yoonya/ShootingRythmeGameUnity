using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; //공유자원으로 설정

    [SerializeField] Sound[] sfx = null; //효과음
    [SerializeField] Sound[] bgm = null; //bgm

    [SerializeField] AudioSource[] sfxPlayers = null;//효과음
    [SerializeField] AudioSource bgmPlayer = null;//bgm


    void Start()
    {
        instance = this; //공유자원으로 설정
    }

    public void PlayBGM(string bgmName) //이름을 받아 그 곡이 저장되어있는지 확인
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgmName == bgm[i].name)
            {
                bgmPlayer.clip = bgm[i].clip;
                bgmPlayer.Play();
            }
        }
    }

    public bool IsPlayingBGM() //현재 재생중인지 확인
    {
        return bgmPlayer.isPlaying;
    }

    public void StopBGM() //브금 스탑
    {
        bgmPlayer.Stop();
    }

    public void PauseBGM() //브금 일시정지
    {
        bgmPlayer.Pause();
    }

    public void ReplayBGM() //브금 시작
    {
        bgmPlayer.Play();
    }

    public void PlaySFX(string sfxName) //이름을 받아 그 곡이 저장되어있는지 확인
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (sfxName == sfx[i].name)
            {
                for (int j = 0; j < sfxPlayers.Length; j++) //효과음 소스 넣을 공간들이 있는지 확인
                {
                    if (!sfxPlayers[j].isPlaying) //재생 중인지 확인
                    {
                        sfxPlayers[j].clip = sfx[i].clip;
                        sfxPlayers[j].Play(); //play
                        return;
                    }
                }
                Debug.Log("모든 오디오 플레이중");
                return;
            }
        }
    }
}
