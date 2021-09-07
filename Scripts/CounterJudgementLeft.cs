using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterJudgementLeft : MonoBehaviour
{
    //이전에 만들어뒀던 판정클래스 구성이 왼쪽 오른쪽 한꺼번에 관리하기 마땅치않아 그냥 편하게 둘을 나눔
    [SerializeField]
    private Transform[] judgementLineY = null;//판정라인Y 모체
    [SerializeField]
    private Transform parentTransform = null; //실제 적용 포지션
    [SerializeField]
    private RectTransform[] judgementLineRectY = null;//판정라인 너비

    private Vector2[] judgementLineLocationY = null; //판정Y적용

    private int keyNumberShift = 0; //shift 고유숫자

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

        judgementLineLocationY = new Vector2[judgementLineY.Length];

        //판정 영역Y (크기의 최소위치<-범위-> 크기의 최대위치)
        for (int i = 0; i < judgementLineY.Length; i++)
        {
            judgementLineLocationY[i].Set(parentTransform.localPosition.y - judgementLineRectY[i].rect.height / 2,
                                          parentTransform.localPosition.y + judgementLineRectY[i].rect.height / 2);
        }
    }

    public void CheckLine(string key) //키에 따른 호출
    {
        if (key == "Shift")
        {
            StartCoroutine(CheckTiming(keyNumberShift));
        }
    }
    
    IEnumerator CheckTiming(int key) //판정부분
    {
        for (int i = 0; i < counters.Count; i++)
        {
            float counterY = counters[i].transform.localPosition.y; //위치설정
            GameObject tempCounter = counters[i];

            if (counters[i].GetComponent<Counter>().counterType == key) //카운터타입과 버튼이 일치해야함
            {
                for (int j = judgementLineLocationY.Length - 1; j >= 0; j--)
                {
                    if (judgementLineLocationY[j].x <= counterY && judgementLineLocationY[j].y > counterY)//Y위치 파악, j = 판정라인
                    {
                        StartCoroutine(judgeEffect.CounterEffect(j, 0)); //카운터 이펙트
                        if (j == 1)
                        {
                            AudioManager.instance.PlaySFX("Counter");
                            scoreBoard.SetCounter(); //스코어보드 갱신
                            characterController.motionAnimator.SetInteger("AnimIndex", 107); //캐릭터 성공모션
                            counters[i].SetActive(false); //활성화 비활성화를 이용해서 애니메이션 중단
                            counters[i].SetActive(true);
                            counters[i].GetComponent<Animator>().SetTrigger("LeftReturn");//카운터백
                            yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(0.4f); //캐릭터 성공모션 시간
                            characterController.motionAnimator.SetInteger("AnimIndex", 10); //캐릭터 idle
                            characterController.queryBodyParts.transform.localRotation = characterController.originRot; //초기화
                            characterController.queryBodyParts.transform.localPosition = Vector3.MoveTowards(characterController.queryBodyParts.transform.localPosition,
                                                                                                      new Vector3(characterController.queryBodyParts.transform.localPosition.x,
                                                                                                                  characterController.originPos.y,
                                                                                                                  characterController.originPos.z),
                                                                                                      0.2f); //점점 앞으로 나옴
                            yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(2.1f); //뭐였지, 카운터 성공시 적에게 날아가는 시간?
                        }
                        else
                        {
                            AudioManager.instance.PlaySFX("Defend");
                            counters[i].SetActive(false); //활성화 비활성화를 이용해서 애니메이션 중단->캐릭터에게 데미지x
                            scoreBoard.SetDefend();
                        }

                        counters.Remove(tempCounter); //판정 후 카운터 삭제
                        ObjectPool.instance.queue[12].Enqueue(tempCounter);//오브젝트풀링 집어넣기
                        yield return null;
                        break;
                    }
                }
            }

        }
    }   
}
