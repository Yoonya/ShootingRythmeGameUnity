using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteJudgement : MonoBehaviour
{
    //롱노트 판정부분
    public List<GameObject> noteListFront = new List<GameObject>(); //앞노트
    public List<GameObject> noteListBack = new List<GameObject>(); //뒷노트

    [SerializeField]
    private Transform[] judgementLineY = null;//판정라인Y 모체

    [SerializeField]
    private RectTransform[] judgementLineRectY = null;//판정라인 너비

    private Vector2[] judgementLineLocationY = null; //판정Y적용

    private int keyNumberA = 0;
    private int keyNumberS = 1;
    private int keyNumberD = 2;

    //롱노팅 필요, update문에서 한번에 관리하기 때문에 키마다 따로 관리(A, S, D)
    public bool[] isLongNoting = { false,false,false };
    private GameObject[] go = { null, null, null }; //노트 뒷부분 객체
    private float[] timer = { 0.0f, 0.0f, 0.0f }; //노트 길이를 시간으로
    private float waitingTime = 0.2f; //이거뭐였지, 최소대기시간?

    private JudgeEffect judgeEffect;
    private LongNoteManager longNoteManager;
    private PlayerStatus playerStatus;
    private PlayerShooter shooter;

    // Start is called before the first frame update
    void Start()
    {
        shooter = FindObjectOfType<PlayerShooter>();
        judgeEffect = FindObjectOfType<JudgeEffect>();
        longNoteManager = FindObjectOfType<LongNoteManager>();
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

    void Update()
    {
        if (isLongNoting[0] == true) //첫번째 타입 롱노트
        {
            timer[0] += Time.deltaTime;

            if (timer[0] > waitingTime && go[0] != null) //뒷객체가 있을 때 -> 롱노트일 때
            {
                Transform tempNoteLocation = longNoteManager.longNoteLocation[go[0].GetComponent<LongNote>().longNoteLocation];
                judgeEffect.SetNoteTransform(tempNoteLocation); //판정효과 
                StartCoroutine(judgeEffect.NoteHitEffect(2, false)); //노트 히트 처리
                timer[0] = 0.0f; //초기화
                shooter.Shot(2);//탄막
            }               
        }
        if (isLongNoting[1] == true && go[1] != null)
        {
            timer[1] += Time.deltaTime;

            if (timer[1] > waitingTime)
            {
                Transform tempNoteLocation = longNoteManager.longNoteLocation[go[1].GetComponent<LongNote>().longNoteLocation];
                judgeEffect.SetNoteTransform(tempNoteLocation);
                StartCoroutine(judgeEffect.NoteHitEffect(2, false)); //노트 히트 처리
                timer[1] = 0.0f;
                shooter.Shot(2);
            }
        }
        if (isLongNoting[2] == true && go[2] != null)
        {
            timer[2] += Time.deltaTime;

            if (timer[2] > waitingTime)
            {
                Transform tempNoteLocation = longNoteManager.longNoteLocation[go[2].GetComponent<LongNote>().longNoteLocation];
                judgeEffect.SetNoteTransform(tempNoteLocation);
                StartCoroutine(judgeEffect.NoteHitEffect(2, false)); //노트 히트 처리
                timer[2] = 0.0f;
                shooter.Shot(2);
            }
        }
    }

    public void CheckLineFront(string key) //키에 따른 호출
    {
        if (key == "A")
        {
            CheckTimingFront(keyNumberA);
        }
        else if (key == "S")
        {
            CheckTimingFront(keyNumberS);
        }

        else if (key == "D")
        {
            CheckTimingFront(keyNumberD);
        }
    }

    public void CheckLineBack(string key) //키에 따른 호출
    {
        if (key == "A")
        {
            CheckTimingBack(keyNumberA);
        }
        else if (key == "S")
        {
            CheckTimingBack(keyNumberS);
        }

        else if (key == "D")
        {
            CheckTimingBack(keyNumberD);
        }
    }

    private void CheckTimingFront(int key) //앞부분 판정부분
    {
        for (int i = 0; i < noteListFront.Count; i++)
        {
            float noteY = noteListFront[i].transform.localPosition.y;
            GameObject tempNote = noteListFront[i];
            if (noteListFront[i].GetComponent<LongNote>().longNoteType == key) //노트타입과 버튼이 일치해야함
            {
                for (int j = judgementLineLocationY.Length - 1; j >= 0; j--)
                {
                    if (judgementLineLocationY[j].x <= noteY && judgementLineLocationY[j].y > noteY)//Y위치 파악, j = 판정라인
                    {
                        //판정이펙트 위치 설정(판정된 노트 위치) 
                        Transform tempNoteLocation = longNoteManager.longNoteLocation[tempNote.GetComponent<LongNote>().longNoteLocation];
                        judgeEffect.SetNoteTransform(tempNoteLocation); //판정효과
                        StartCoroutine(judgeEffect.NoteHitEffect(j, false)); //노트 히트 처리

                        //이때 객체를 저장
                        if(tempNote.GetComponent<LongNote>().longNoteLocation == 0)
                            go[0] = tempNote;
                        else if (tempNote.GetComponent<LongNote>().longNoteLocation == 1)
                            go[1] = tempNote;
                        else if (tempNote.GetComponent<LongNote>().longNoteLocation == 2)
                            go[2] = tempNote;

                        isLongNoting[key] = true; //롱노트 점수중

                        noteListFront.Remove(tempNote); //판정 후 노트 삭제

                        ObjectPool.instance.queue[18 + key].Enqueue(tempNote);//오브젝트풀링 집어넣기

                        AudioManager.instance.PlaySFX("NoteTap");
                        shooter.Shot(j);

                        return;
                    }
                }
            }

        }
        //판정이펙트 위치 설정(판정된 노트 위치), 미스
        Transform missNoteLocation = longNoteManager.longNoteLocation[key];
        isLongNoting[key] = false; //초기화
        judgeEffect.SetNoteTransform(missNoteLocation);
        StartCoroutine(judgeEffect.NoteHitEffect(3, false));
        AudioManager.instance.PlaySFX("NoteTap");
    }

    private void CheckTimingBack(int key) //뒷부분 판정부분
    {
        for (int i = 0; i < noteListBack.Count; i++)
        {
            float noteY = noteListBack[i].transform.localPosition.y;
            GameObject tempNote = noteListBack[i];

            if (noteListBack[i].GetComponent<LongNote>().longNoteType == key) //노트타입과 버튼이 일치해야함
            {
                for (int j = judgementLineLocationY.Length - 1; j >= 0; j--)
                {
                    if (judgementLineLocationY[j].x <= noteY && judgementLineLocationY[j].y > noteY)//Y위치 파악, j = 판정라인
                    {
                        //판정이펙트 위치 설정(판정된 노트 위치) 
                        Transform tempNoteLocation = longNoteManager.longNoteLocation[tempNote.GetComponent<LongNote>().longNoteLocation];
                        judgeEffect.SetNoteTransform(tempNoteLocation);
                        StartCoroutine(judgeEffect.NoteHitEffect(j, false)); //노트 히트 처리

                        if (tempNote.GetComponent<LongNote>().longNoteLocation == 0)
                            go[0] = tempNote;
                        else if (tempNote.GetComponent<LongNote>().longNoteLocation == 1)
                            go[1] = tempNote;
                        else if (tempNote.GetComponent<LongNote>().longNoteLocation == 2)
                            go[2] = tempNote;

                        isLongNoting[key] = false;//롱노트 점수중단
                        
                        noteListBack.Remove(tempNote); //판정 후 노트 삭제

                        ObjectPool.instance.queue[18 + key].Enqueue(tempNote);//오브젝트풀링 집어넣기

                        AudioManager.instance.PlaySFX("NoteTap");
                        shooter.Shot(j);

                        return;
                    }
                }
            }

        }
        //판정이펙트 위치 설정(판정된 노트 위치), 미스
        Transform missNoteLocation = longNoteManager.longNoteLocation[key];
        isLongNoting[key] = false;
        judgeEffect.SetNoteTransform(missNoteLocation);
        StartCoroutine(judgeEffect.NoteHitEffect(3, false));
    }
}
