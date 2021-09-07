using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judgement : MonoBehaviour
{
    //판정 관리
    public List<GameObject> noteList = new List<GameObject>();

    [SerializeField]
    private Transform[] judgementLineY = null;//판정라인Y 모체

    [SerializeField]
    private RectTransform[] judgementLineRectY = null;//판정라인 너비

    private Vector2[] judgementLineLocationY = null; //판정Y적용

    //키의 고유 숫자들
    private int keyNumberA = 0;
    private int keyNumberS = 1;
    private int keyNumberD = 2;

    private JudgeEffect judgeEffect;
    private NoteManager noteManager;
    private PlayerStatus playerStatus;
    private PlayerShooter shooter;

    // Start is called before the first frame update
    void Start()
    {
        shooter = FindObjectOfType<PlayerShooter>();
        judgeEffect = FindObjectOfType<JudgeEffect>();
        noteManager = FindObjectOfType<NoteManager>();
        playerStatus = FindObjectOfType<PlayerStatus>();

        judgementLineLocationY = new Vector2[judgementLineY.Length];

        //판정 영역Y (크기의 최소위치<-범위-> 크기의 최대위치)
        for (int i = 0; i < judgementLineY.Length; i++)
        {
            //기준점이 노트가 0이 아니라 이 위치(rot돌리기 전 최초 로케이션 위치)이기 때문에 더해준다.
            judgementLineLocationY[i].Set(judgementLineY[0].localPosition.y - judgementLineRectY[i].rect.height / 2 + 1200, 
                                          judgementLineY[0].localPosition.y + judgementLineRectY[i].rect.height / 2 + 1200);
        }
    }

    public void CheckLine(string key) //키에 따른 호출
    {
        if (key == "A")
        {
            CheckTiming(keyNumberA);
        }
        else if (key == "S")
        {
            CheckTiming(keyNumberS);
        }
        else if (key == "D")
        {
            CheckTiming(keyNumberD);
        }
    }
    
    private void CheckTiming(int key) //판정부분
    {
        for (int i = 0; i < noteList.Count; i++)
        {
            float noteY = noteList[i].transform.localPosition.y;
            GameObject tempNote = noteList[i];

            if (noteList[i].GetComponent<Note>().noteType == key) //노트타입과 버튼이 일치해야함
            {
                for (int j = judgementLineLocationY.Length - 1; j >= 0; j--)
                {
                    if (judgementLineLocationY[j].x <= noteY && judgementLineLocationY[j].y > noteY)//Y위치 파악, j = 판정라인
                    {
                        //판정이펙트 위치 설정(판정된 노트 위치) 
                        Transform tempNoteLocation = noteManager.noteLocation[tempNote.GetComponent<Note>().noteLocation];
                        judgeEffect.SetNoteTransform(tempNoteLocation); //판정 효과 위치 설정
                        StartCoroutine(judgeEffect.NoteHitEffect(j, false)); //노트 히트 처리

                        noteList.Remove(tempNote); //판정 후 노트 삭제

                        ObjectPool.instance.queue[key].Enqueue(tempNote);//오브젝트풀링 집어넣기
                        tempNote.SetActive(false);

                        AudioManager.instance.PlaySFX("NoteTap");
                        shooter.Shot(j); //미사일 슈팅
                        
                        
                        return;
                    }
                }
            }

        }
        //판정이펙트 위치 설정(판정된 노트 위치), 미스
        Transform missNoteLocation = noteManager.noteLocation[key];
        judgeEffect.SetNoteTransform(missNoteLocation);
        StartCoroutine(judgeEffect.NoteHitEffect(3, false)); //이펙트 호출

    }
    
}
