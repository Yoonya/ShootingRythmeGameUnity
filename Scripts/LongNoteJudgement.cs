using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteJudgement : MonoBehaviour
{
    //�ճ�Ʈ �����κ�
    public List<GameObject> noteListFront = new List<GameObject>(); //�ճ�Ʈ
    public List<GameObject> noteListBack = new List<GameObject>(); //�޳�Ʈ

    [SerializeField]
    private Transform[] judgementLineY = null;//��������Y ��ü

    [SerializeField]
    private RectTransform[] judgementLineRectY = null;//�������� �ʺ�

    private Vector2[] judgementLineLocationY = null; //����Y����

    private int keyNumberA = 0;
    private int keyNumberS = 1;
    private int keyNumberD = 2;

    //�ճ��� �ʿ�, update������ �ѹ��� �����ϱ� ������ Ű���� ���� ����(A, S, D)
    public bool[] isLongNoting = { false,false,false };
    private GameObject[] go = { null, null, null }; //��Ʈ �޺κ� ��ü
    private float[] timer = { 0.0f, 0.0f, 0.0f }; //��Ʈ ���̸� �ð�����
    private float waitingTime = 0.2f; //�̰Ź�����, �ּҴ��ð�?

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

        //���� ����Y (ũ���� �ּ���ġ<-����-> ũ���� �ִ���ġ)
        for (int i = 0; i < judgementLineY.Length; i++)
        {
            //�������� ��Ʈ�� 0�� �ƴ϶� �� ��ġ(rot������ �� ���� �����̼� ��ġ)�̱� ������ �����ش�.
            judgementLineLocationY[i].Set(judgementLineY[0].localPosition.y - judgementLineRectY[i].rect.height / 2 + 1200,
                                          judgementLineY[0].localPosition.y + judgementLineRectY[i].rect.height / 2 + 1200);
        }
    }

    void Update()
    {
        if (isLongNoting[0] == true) //ù��° Ÿ�� �ճ�Ʈ
        {
            timer[0] += Time.deltaTime;

            if (timer[0] > waitingTime && go[0] != null) //�ް�ü�� ���� �� -> �ճ�Ʈ�� ��
            {
                Transform tempNoteLocation = longNoteManager.longNoteLocation[go[0].GetComponent<LongNote>().longNoteLocation];
                judgeEffect.SetNoteTransform(tempNoteLocation); //����ȿ�� 
                StartCoroutine(judgeEffect.NoteHitEffect(2, false)); //��Ʈ ��Ʈ ó��
                timer[0] = 0.0f; //�ʱ�ȭ
                shooter.Shot(2);//ź��
            }               
        }
        if (isLongNoting[1] == true && go[1] != null)
        {
            timer[1] += Time.deltaTime;

            if (timer[1] > waitingTime)
            {
                Transform tempNoteLocation = longNoteManager.longNoteLocation[go[1].GetComponent<LongNote>().longNoteLocation];
                judgeEffect.SetNoteTransform(tempNoteLocation);
                StartCoroutine(judgeEffect.NoteHitEffect(2, false)); //��Ʈ ��Ʈ ó��
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
                StartCoroutine(judgeEffect.NoteHitEffect(2, false)); //��Ʈ ��Ʈ ó��
                timer[2] = 0.0f;
                shooter.Shot(2);
            }
        }
    }

    public void CheckLineFront(string key) //Ű�� ���� ȣ��
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

    public void CheckLineBack(string key) //Ű�� ���� ȣ��
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

    private void CheckTimingFront(int key) //�պκ� �����κ�
    {
        for (int i = 0; i < noteListFront.Count; i++)
        {
            float noteY = noteListFront[i].transform.localPosition.y;
            GameObject tempNote = noteListFront[i];
            if (noteListFront[i].GetComponent<LongNote>().longNoteType == key) //��ƮŸ�԰� ��ư�� ��ġ�ؾ���
            {
                for (int j = judgementLineLocationY.Length - 1; j >= 0; j--)
                {
                    if (judgementLineLocationY[j].x <= noteY && judgementLineLocationY[j].y > noteY)//Y��ġ �ľ�, j = ��������
                    {
                        //��������Ʈ ��ġ ����(������ ��Ʈ ��ġ) 
                        Transform tempNoteLocation = longNoteManager.longNoteLocation[tempNote.GetComponent<LongNote>().longNoteLocation];
                        judgeEffect.SetNoteTransform(tempNoteLocation); //����ȿ��
                        StartCoroutine(judgeEffect.NoteHitEffect(j, false)); //��Ʈ ��Ʈ ó��

                        //�̶� ��ü�� ����
                        if(tempNote.GetComponent<LongNote>().longNoteLocation == 0)
                            go[0] = tempNote;
                        else if (tempNote.GetComponent<LongNote>().longNoteLocation == 1)
                            go[1] = tempNote;
                        else if (tempNote.GetComponent<LongNote>().longNoteLocation == 2)
                            go[2] = tempNote;

                        isLongNoting[key] = true; //�ճ�Ʈ ������

                        noteListFront.Remove(tempNote); //���� �� ��Ʈ ����

                        ObjectPool.instance.queue[18 + key].Enqueue(tempNote);//������ƮǮ�� ����ֱ�

                        AudioManager.instance.PlaySFX("NoteTap");
                        shooter.Shot(j);

                        return;
                    }
                }
            }

        }
        //��������Ʈ ��ġ ����(������ ��Ʈ ��ġ), �̽�
        Transform missNoteLocation = longNoteManager.longNoteLocation[key];
        isLongNoting[key] = false; //�ʱ�ȭ
        judgeEffect.SetNoteTransform(missNoteLocation);
        StartCoroutine(judgeEffect.NoteHitEffect(3, false));
        AudioManager.instance.PlaySFX("NoteTap");
    }

    private void CheckTimingBack(int key) //�޺κ� �����κ�
    {
        for (int i = 0; i < noteListBack.Count; i++)
        {
            float noteY = noteListBack[i].transform.localPosition.y;
            GameObject tempNote = noteListBack[i];

            if (noteListBack[i].GetComponent<LongNote>().longNoteType == key) //��ƮŸ�԰� ��ư�� ��ġ�ؾ���
            {
                for (int j = judgementLineLocationY.Length - 1; j >= 0; j--)
                {
                    if (judgementLineLocationY[j].x <= noteY && judgementLineLocationY[j].y > noteY)//Y��ġ �ľ�, j = ��������
                    {
                        //��������Ʈ ��ġ ����(������ ��Ʈ ��ġ) 
                        Transform tempNoteLocation = longNoteManager.longNoteLocation[tempNote.GetComponent<LongNote>().longNoteLocation];
                        judgeEffect.SetNoteTransform(tempNoteLocation);
                        StartCoroutine(judgeEffect.NoteHitEffect(j, false)); //��Ʈ ��Ʈ ó��

                        if (tempNote.GetComponent<LongNote>().longNoteLocation == 0)
                            go[0] = tempNote;
                        else if (tempNote.GetComponent<LongNote>().longNoteLocation == 1)
                            go[1] = tempNote;
                        else if (tempNote.GetComponent<LongNote>().longNoteLocation == 2)
                            go[2] = tempNote;

                        isLongNoting[key] = false;//�ճ�Ʈ �����ߴ�
                        
                        noteListBack.Remove(tempNote); //���� �� ��Ʈ ����

                        ObjectPool.instance.queue[18 + key].Enqueue(tempNote);//������ƮǮ�� ����ֱ�

                        AudioManager.instance.PlaySFX("NoteTap");
                        shooter.Shot(j);

                        return;
                    }
                }
            }

        }
        //��������Ʈ ��ġ ����(������ ��Ʈ ��ġ), �̽�
        Transform missNoteLocation = longNoteManager.longNoteLocation[key];
        isLongNoting[key] = false;
        judgeEffect.SetNoteTransform(missNoteLocation);
        StartCoroutine(judgeEffect.NoteHitEffect(3, false));
    }
}
