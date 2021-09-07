using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class go_Counter //노트설정
{
    public float counterSpeed = 0f; //카운터 스피드
    public int counterType = 0; //카운터 타입 왼오 01
    public float appearTime = 0f; //카운터 등장 시간
    public go_Counter(float counterSpeed, int counterType, float appearTime)
    {
        this.counterSpeed = counterSpeed;
        this.counterType = counterType;
        this.appearTime = appearTime;
    }
}

public class CounterManager : MonoBehaviour //카운터는 속도 x
{
    [SerializeField] private float counterSpeed = 1f; //카운터 스피드

    [SerializeField] private float sync0 = 0f; //추가 싱크(개발자 전용)
    private float sync1; //추가 싱크(사용자 설정)


    [SerializeField]
    public Transform[] counterLocation; //카운터 위치 저장

    public List<go_Counter> counters = new List<go_Counter>(); //카운터 리스트

    private CounterJudgementLeft counterJudgementLeft;
    private CounterJudgementRight counterJudgementRight;
    private EnemyController enemyController;
    private GameManager gameManager;
    private SongNoteManager songNoteManager;

    // Start is called before the first frame update
    void Start()
    {
        counterJudgementLeft = FindObjectOfType<CounterJudgementLeft>();
        counterJudgementRight = FindObjectOfType<CounterJudgementRight>();
        enemyController = FindObjectOfType<EnemyController>();
        gameManager = FindObjectOfType<GameManager>();
        songNoteManager = FindObjectOfType<SongNoteManager>();

        sync1 = gameManager.sync;
        sync0 += 0.6f; //추가계산

        ReadCounterInfo();
        //모든 카운터를 정해진 시간에 출발하도록 설정
        for (int i = 0; i < counters.Count; i++)
        {
            StartCoroutine(StartMakeCounter(counters[i]));
            StartCoroutine(StartAttackMotion(counters[i])); //적 공격 모션 설정
        }
    }

    private void ReadCounterInfo()
    {
        if (songNoteManager.songNotes[gameManager.currentSongNumber].counters != "")
        {
            string[] texts = songNoteManager.songNotes[gameManager.currentSongNumber].counters.Split('\n');

            if (texts.Length > 0) //메모장에 있는 노트 리스트로 정리
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    int tempType = Convert.ToInt32(texts[i].Split(' ')[0]);
                    //판정라인에 도달할 시간을 빼서 메모장에 적은 시간에 도착하도록 계산(싱크)
                    float tempAppear = Convert.ToSingle(texts[i].Split(' ')[1]) + sync1 + sync0;

                    go_Counter tempCounter = new go_Counter(counterSpeed, tempType, tempAppear);

                    counters.Add(tempCounter);
                }
            }
        }
    }

    IEnumerator StartAttackMotion(go_Counter counter) //적 공격 모션
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(counter.appearTime - 0.3f); //카운터와 어울리기 위해 조정
        enemyController.motionAnimator.SetInteger("AnimIndex", 107); //attack
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(0.5f); //모션 중인 시간
        enemyController.motionAnimator.SetInteger("AnimIndex", 9); //idle
        enemyController.queryBodyParts.transform.localRotation = enemyController.originRot; //초기화
        enemyController.queryBodyParts.transform.localPosition = new Vector3(enemyController.originPos.x,enemyController.originPos.y,enemyController.originPos.z + 3f); //점점 위치 이상해짐, z축이 제대로 돌아오지못해?
    }

    IEnumerator StartMakeCounter(go_Counter counter)
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(counter.appearTime);
        CreateCounter(counter);
    }

    private void CreateCounter(go_Counter counter) //카운터생성
    {
        GameObject tempCounter = ObjectPool.instance.queue[12].Dequeue(); //오브젝트 풀링 사용, 카운터 위치는 12

        tempCounter.transform.localScale = new Vector3(1f, 1f, 1f); //값들 초기화
        tempCounter.transform.position = counterLocation[counter.counterType].position;
        tempCounter.GetComponent<Counter>().SetCounter(counterSpeed, counter.counterType, counter.appearTime); //카운터 객체에 값 할당
        tempCounter.SetActive(true);

        counterJudgementLeft.counters.Add(tempCounter); //리스트에 추가
        counterJudgementRight.counters.Add(tempCounter);

        if (counter.counterType == 0) //SetInteger로 하니 루프 현상과 정수가 제대로 적용안되고 있음, animator를 Counter내에서는 참조 못하고있음
            tempCounter.GetComponent<Animator>().SetTrigger("Left");
        if (counter.counterType == 1)
            tempCounter.GetComponent<Animator>().SetTrigger("Right");
        tempCounter.GetComponent<Animator>().SetFloat("Speed", counter.counterSpeed);
        
    }
}
