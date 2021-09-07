using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class go_Arrow //노트설정
{
    public int arrowType = 0; //0 = short, 1 = long
    public int arrowDirect = 0; //0 = left, 1 = right
    public int arrowLocation = 0; //0~4
    public float appearTime = 0f; //노트 등장시간
    public go_Arrow(int arrowType, int arrowDirect, int arrowLocation, float appearTime)
    {
        this.arrowType = arrowType;
        this.arrowDirect = arrowDirect;
        this.arrowLocation = arrowLocation;
        this.appearTime = appearTime;
    }
}

public class ArrowManager : MonoBehaviour //arrow parent
{
    private int arrowSpeed;//노트 속도
    [SerializeField] private float sync0 = 0f; //추가 싱크(개발자 전용)
    private float sync1; //추가 싱크(사용자 설정)
    private float sync2; //기본싱크
    [SerializeField] 
    public Transform[] arrowLocation = null; //arrow위치

    private List<go_Arrow> arrows = new List<go_Arrow>(); //arrow를 담을 리스트

    private GameManager gameManager;
    private SongNoteManager songNoteManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        songNoteManager = FindObjectOfType<SongNoteManager>();

        arrowSpeed = (int)(gameManager.speed * 1000);
        sync1 = gameManager.sync;
        sync2 = arrowLocation[0].transform.localPosition.y / arrowSpeed; //기본 싱크차이

        float tempSync = (arrowSpeed - 2000.0f) / 5000.0f; //세밀계산
        sync0 += tempSync;
        sync0 += 0.6f; //추가계산

        ReadArrowInfo();
        //모든 노트를 정해진 시간에 출발하도록 설정
        for (int i = 0; i < arrows.Count; i++)
        {
            StartCoroutine(StartMakeArrow(arrows[i]));
        }

    }

    private void ReadArrowInfo()   //리소스에서 노트 텍스트 파일을 불러오기
    {
        if (songNoteManager.songNotes[gameManager.currentSongNumber].arrows != "")
        {
            string[] texts = songNoteManager.songNotes[gameManager.currentSongNumber].arrows.Split('\n');

            if (texts.Length > 0) //메모장에 있는 노트 리스트로 정리
            {
                for (int i = 0; i < texts.Length; i++) //arrow정보를 하나씩 저장
                {
                    int tempType = Convert.ToInt32(texts[i].Split(' ')[0]);
                    int tempDirect = Convert.ToInt32(texts[i].Split(' ')[1]);
                    int tempLocation = Convert.ToInt32(texts[i].Split(' ')[2]);
                    //판정라인에 도달할 시간을 빼서 메모장에 적은 시간에 도착하도록 계산(sync로 조절)
                    float tempAppear = Convert.ToSingle(texts[i].Split(' ')[3]) - sync2 + sync1 + sync0;

                    go_Arrow tempArrow = new go_Arrow(tempType, tempDirect, tempLocation, tempAppear);
                    arrows.Add(tempArrow);
                }
            }
        }
    }

    IEnumerator StartMakeArrow(go_Arrow arrow)//등장 시간에 맞춰 arrow 생성
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(arrow.appearTime);
        CreateArrow(arrow);
    }

    private void CreateArrow(go_Arrow arrow) //노트생성
    {
        GameObject tempArrow = ObjectPool.instance.queue[16 + arrow.arrowType].Dequeue(); //오브젝트 풀링 사용

        tempArrow.transform.localScale = new Vector3(1f, 1f, 1f); //스케일을 1로, 왜 커지는지 모르겠음

        tempArrow.GetComponent<Arrow>().SetArrowSpeed(arrowSpeed); //화살 속도 설정
        tempArrow.GetComponent<Arrow>().SetArrowType(arrow.arrowType); //화살 타입 설정
        tempArrow.GetComponent<Arrow>().SetArrowDirect(arrow.arrowDirect); //화살 방향 설정
        tempArrow.GetComponent<Arrow>().SetArrowLocation(arrow.arrowLocation); //화살 위치 설정
        tempArrow.GetComponent<Arrow>().SetAppearTime(arrow.appearTime); //화살 시간 설정

        tempArrow.transform.position = arrowLocation[arrow.arrowLocation].position; //설정된 위치에 등장
        tempArrow.SetActive(true); //on

        //Arrow 모양 설정
        if (arrow.arrowType == 0 && arrow.arrowDirect == 0)
            tempArrow.GetComponent<Arrow>().animator.SetTrigger("ArrowShortLeft");
        else if (arrow.arrowType == 0 && arrow.arrowDirect == 1)
            tempArrow.GetComponent<Arrow>().animator.SetTrigger("ArrowShortRight");
        else if (arrow.arrowType == 1 && arrow.arrowDirect == 0)
            tempArrow.GetComponent<Arrow>().animator.SetTrigger("ArrowLongLeft");
        else if (arrow.arrowType == 1 && arrow.arrowDirect == 1)
            tempArrow.GetComponent<Arrow>().animator.SetTrigger("ArrowLongRight");
    }

    private void OnTriggerEnter(Collider other) //쓰레기통
    {
        if (other.CompareTag("Arrow"))
        {
            ObjectPool.instance.queue[16 + other.GetComponent<Arrow>().arrowType].Enqueue(other.gameObject);//오브젝트풀링 집어넣기           
            other.gameObject.SetActive(false);//off
        }
    }
}
