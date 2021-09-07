using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterJudgementRight : MonoBehaviour
{
    [SerializeField]
    private Transform[] judgementLineY = null;//판정라인Y 모체
    [SerializeField]
    private Transform parentTransform = null; //실제 적용 포지션
    [SerializeField]
    private RectTransform[] judgementLineRectY = null;//판정라인 너비

    private Vector2[] judgementLineLocationY = null; //판정Y적용

    private int keyNumberSpace = 1; //고유 숫자 스페이스

    public List<GameObject> counters = new List<GameObject>(); //카운터 리스트

    private CounterManager counterManager;
    private JudgeEffect judgeEffect;
    private CharacterController characterController;
    private ScoreBoard scoreBoard;

    // Start is called before the first frame update
    void Start()
    {
        counterManager = FindObjectOfType<CounterManager>();
        judgeEffect = FindObjectOfType<JudgeEffect>();
        characterController = FindObjectOfType<CharacterController>();
        scoreBoard = FindObjectOfType<ScoreBoard>();

        judgementLineLocationY = new Vector2[judgementLineY.Length]; //위치 설정

        //판정 영역Y (크기의 최소위치<-범위-> 크기의 최대위치)
        for (int i = 0; i < judgementLineY.Length; i++)
        {
            judgementLineLocationY[i].Set(parentTransform.localPosition.y - judgementLineRectY[i].rect.height / 2,
                                          parentTransform.localPosition.y + judgementLineRectY[i].rect.height / 2);
        }
    }

    public void CheckLine(string key) //키에 따른 호출
    {
        if (key == "Space")
        {
            StartCoroutine(CheckTiming(keyNumberSpace));
        }
    }

    IEnumerator CheckTiming(int key) //판정부분
    {
        for (int i = 0; i < counters.Count; i++)
        {
            float counterY = counters[i].transform.localPosition.y;
            GameObject tempCounter = counters[i];

            if (counters[i].GetComponent<Counter>().counterType == key) //카운터타입과 버튼이 일치해야함
            {
                for (int j = judgementLineLocationY.Length - 1; j >= 0; j--)
                {
                    if (judgementLineLocationY[j].x <= counterY && judgementLineLocationY[j].y > counterY)//Y위치 파악, j = 판정라인
                    {
                        StartCoroutine(judgeEffect.CounterEffect(j, 1)); //카운터이펙트 불러오기
                        if (j == 1) //카운터 성공
                        {
                            AudioManager.instance.PlaySFX("Counter");//효과음
                            scoreBoard.SetCounter(); //스코어보드 갱신
                            characterController.motionAnimator.SetInteger("AnimIndex", 107); //캐릭터 성공모션
                            counters[i].SetActive(false); //활성화 비활성화를 이용해서 애니메이션 중단
                            counters[i].SetActive(true);
                            counters[i].GetComponent<Animator>().SetTrigger("RightReturn");//카운터백
                            yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(0.4f); //캐릭터 성공모션 대기시간
                            characterController.motionAnimator.SetInteger("AnimIndex", 10);
                            characterController.queryBodyParts.transform.localRotation = characterController.originRot; //초기화
                            characterController.queryBodyParts.transform.localPosition = Vector3.MoveTowards(characterController.queryBodyParts.transform.localPosition,
                                                                                                      new Vector3(characterController.queryBodyParts.transform.localPosition.x,
                                                                                                                  characterController.originPos.y,
                                                                                                                  characterController.originPos.z),
                                                                                                      1f); //점점 앞으로 나옴
                            yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(2.5f);//뭐였지, 카운터 성공시 적에게 날아가는 시간? 근데 왜 왼오시간이 다름
                        }
                        else
                        {
                            counters[i].SetActive(false); //활성화 비활성화를 이용해서 애니메이션 중단->캐릭터에게 데미지x
                            AudioManager.instance.PlaySFX("Defend");
                            scoreBoard.SetDefend();
                        }
                        counters.Remove(tempCounter); //판정 후 노트 삭제
                        ObjectPool.instance.queue[12].Enqueue(tempCounter);//오브젝트풀링 집어넣기

                        yield return null;
                        break;
                    }
                }
            }

        }
    }
}
