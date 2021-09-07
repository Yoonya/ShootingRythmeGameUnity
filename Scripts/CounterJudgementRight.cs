using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterJudgementRight : MonoBehaviour
{
    [SerializeField]
    private Transform[] judgementLineY = null;//��������Y ��ü
    [SerializeField]
    private Transform parentTransform = null; //���� ���� ������
    [SerializeField]
    private RectTransform[] judgementLineRectY = null;//�������� �ʺ�

    private Vector2[] judgementLineLocationY = null; //����Y����

    private int keyNumberSpace = 1; //���� ���� �����̽�

    public List<GameObject> counters = new List<GameObject>(); //ī���� ����Ʈ

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

        judgementLineLocationY = new Vector2[judgementLineY.Length]; //��ġ ����

        //���� ����Y (ũ���� �ּ���ġ<-����-> ũ���� �ִ���ġ)
        for (int i = 0; i < judgementLineY.Length; i++)
        {
            judgementLineLocationY[i].Set(parentTransform.localPosition.y - judgementLineRectY[i].rect.height / 2,
                                          parentTransform.localPosition.y + judgementLineRectY[i].rect.height / 2);
        }
    }

    public void CheckLine(string key) //Ű�� ���� ȣ��
    {
        if (key == "Space")
        {
            StartCoroutine(CheckTiming(keyNumberSpace));
        }
    }

    IEnumerator CheckTiming(int key) //�����κ�
    {
        for (int i = 0; i < counters.Count; i++)
        {
            float counterY = counters[i].transform.localPosition.y;
            GameObject tempCounter = counters[i];

            if (counters[i].GetComponent<Counter>().counterType == key) //ī����Ÿ�԰� ��ư�� ��ġ�ؾ���
            {
                for (int j = judgementLineLocationY.Length - 1; j >= 0; j--)
                {
                    if (judgementLineLocationY[j].x <= counterY && judgementLineLocationY[j].y > counterY)//Y��ġ �ľ�, j = ��������
                    {
                        StartCoroutine(judgeEffect.CounterEffect(j, 1)); //ī��������Ʈ �ҷ�����
                        if (j == 1) //ī���� ����
                        {
                            AudioManager.instance.PlaySFX("Counter");//ȿ����
                            scoreBoard.SetCounter(); //���ھ�� ����
                            characterController.motionAnimator.SetInteger("AnimIndex", 107); //ĳ���� �������
                            counters[i].SetActive(false); //Ȱ��ȭ ��Ȱ��ȭ�� �̿��ؼ� �ִϸ��̼� �ߴ�
                            counters[i].SetActive(true);
                            counters[i].GetComponent<Animator>().SetTrigger("RightReturn");//ī���͹�
                            yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(0.4f); //ĳ���� ������� ���ð�
                            characterController.motionAnimator.SetInteger("AnimIndex", 10);
                            characterController.queryBodyParts.transform.localRotation = characterController.originRot; //�ʱ�ȭ
                            characterController.queryBodyParts.transform.localPosition = Vector3.MoveTowards(characterController.queryBodyParts.transform.localPosition,
                                                                                                      new Vector3(characterController.queryBodyParts.transform.localPosition.x,
                                                                                                                  characterController.originPos.y,
                                                                                                                  characterController.originPos.z),
                                                                                                      1f); //���� ������ ����
                            yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(2.5f);//������, ī���� ������ ������ ���ư��� �ð�? �ٵ� �� �޿��ð��� �ٸ�
                        }
                        else
                        {
                            counters[i].SetActive(false); //Ȱ��ȭ ��Ȱ��ȭ�� �̿��ؼ� �ִϸ��̼� �ߴ�->ĳ���Ϳ��� ������x
                            AudioManager.instance.PlaySFX("Defend");
                            scoreBoard.SetDefend();
                        }
                        counters.Remove(tempCounter); //���� �� ��Ʈ ����
                        ObjectPool.instance.queue[12].Enqueue(tempCounter);//������ƮǮ�� ����ֱ�

                        yield return null;
                        break;
                    }
                }
            }

        }
    }
}
