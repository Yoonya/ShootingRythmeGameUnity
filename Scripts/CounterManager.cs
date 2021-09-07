using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class go_Counter //��Ʈ����
{
    public float counterSpeed = 0f; //ī���� ���ǵ�
    public int counterType = 0; //ī���� Ÿ�� �޿� 01
    public float appearTime = 0f; //ī���� ���� �ð�
    public go_Counter(float counterSpeed, int counterType, float appearTime)
    {
        this.counterSpeed = counterSpeed;
        this.counterType = counterType;
        this.appearTime = appearTime;
    }
}

public class CounterManager : MonoBehaviour //ī���ʹ� �ӵ� x
{
    [SerializeField] private float counterSpeed = 1f; //ī���� ���ǵ�

    [SerializeField] private float sync0 = 0f; //�߰� ��ũ(������ ����)
    private float sync1; //�߰� ��ũ(����� ����)


    [SerializeField]
    public Transform[] counterLocation; //ī���� ��ġ ����

    public List<go_Counter> counters = new List<go_Counter>(); //ī���� ����Ʈ

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
        sync0 += 0.6f; //�߰����

        ReadCounterInfo();
        //��� ī���͸� ������ �ð��� ����ϵ��� ����
        for (int i = 0; i < counters.Count; i++)
        {
            StartCoroutine(StartMakeCounter(counters[i]));
            StartCoroutine(StartAttackMotion(counters[i])); //�� ���� ��� ����
        }
    }

    private void ReadCounterInfo()
    {
        if (songNoteManager.songNotes[gameManager.currentSongNumber].counters != "")
        {
            string[] texts = songNoteManager.songNotes[gameManager.currentSongNumber].counters.Split('\n');

            if (texts.Length > 0) //�޸��忡 �ִ� ��Ʈ ����Ʈ�� ����
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    int tempType = Convert.ToInt32(texts[i].Split(' ')[0]);
                    //�������ο� ������ �ð��� ���� �޸��忡 ���� �ð��� �����ϵ��� ���(��ũ)
                    float tempAppear = Convert.ToSingle(texts[i].Split(' ')[1]) + sync1 + sync0;

                    go_Counter tempCounter = new go_Counter(counterSpeed, tempType, tempAppear);

                    counters.Add(tempCounter);
                }
            }
        }
    }

    IEnumerator StartAttackMotion(go_Counter counter) //�� ���� ���
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(counter.appearTime - 0.3f); //ī���Ϳ� ��︮�� ���� ����
        enemyController.motionAnimator.SetInteger("AnimIndex", 107); //attack
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(0.5f); //��� ���� �ð�
        enemyController.motionAnimator.SetInteger("AnimIndex", 9); //idle
        enemyController.queryBodyParts.transform.localRotation = enemyController.originRot; //�ʱ�ȭ
        enemyController.queryBodyParts.transform.localPosition = new Vector3(enemyController.originPos.x,enemyController.originPos.y,enemyController.originPos.z + 3f); //���� ��ġ �̻�����, z���� ����� ���ƿ�������?
    }

    IEnumerator StartMakeCounter(go_Counter counter)
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(counter.appearTime);
        CreateCounter(counter);
    }

    private void CreateCounter(go_Counter counter) //ī���ͻ���
    {
        GameObject tempCounter = ObjectPool.instance.queue[12].Dequeue(); //������Ʈ Ǯ�� ���, ī���� ��ġ�� 12

        tempCounter.transform.localScale = new Vector3(1f, 1f, 1f); //���� �ʱ�ȭ
        tempCounter.transform.position = counterLocation[counter.counterType].position;
        tempCounter.GetComponent<Counter>().SetCounter(counterSpeed, counter.counterType, counter.appearTime); //ī���� ��ü�� �� �Ҵ�
        tempCounter.SetActive(true);

        counterJudgementLeft.counters.Add(tempCounter); //����Ʈ�� �߰�
        counterJudgementRight.counters.Add(tempCounter);

        if (counter.counterType == 0) //SetInteger�� �ϴ� ���� ����� ������ ����� ����ȵǰ� ����, animator�� Counter�������� ���� ���ϰ�����
            tempCounter.GetComponent<Animator>().SetTrigger("Left");
        if (counter.counterType == 1)
            tempCounter.GetComponent<Animator>().SetTrigger("Right");
        tempCounter.GetComponent<Animator>().SetFloat("Speed", counter.counterSpeed);
        
    }
}
