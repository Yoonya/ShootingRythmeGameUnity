using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judgement : MonoBehaviour
{
    //���� ����
    public List<GameObject> noteList = new List<GameObject>();

    [SerializeField]
    private Transform[] judgementLineY = null;//��������Y ��ü

    [SerializeField]
    private RectTransform[] judgementLineRectY = null;//�������� �ʺ�

    private Vector2[] judgementLineLocationY = null; //����Y����

    //Ű�� ���� ���ڵ�
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

        //���� ����Y (ũ���� �ּ���ġ<-����-> ũ���� �ִ���ġ)
        for (int i = 0; i < judgementLineY.Length; i++)
        {
            //�������� ��Ʈ�� 0�� �ƴ϶� �� ��ġ(rot������ �� ���� �����̼� ��ġ)�̱� ������ �����ش�.
            judgementLineLocationY[i].Set(judgementLineY[0].localPosition.y - judgementLineRectY[i].rect.height / 2 + 1200, 
                                          judgementLineY[0].localPosition.y + judgementLineRectY[i].rect.height / 2 + 1200);
        }
    }

    public void CheckLine(string key) //Ű�� ���� ȣ��
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
    
    private void CheckTiming(int key) //�����κ�
    {
        for (int i = 0; i < noteList.Count; i++)
        {
            float noteY = noteList[i].transform.localPosition.y;
            GameObject tempNote = noteList[i];

            if (noteList[i].GetComponent<Note>().noteType == key) //��ƮŸ�԰� ��ư�� ��ġ�ؾ���
            {
                for (int j = judgementLineLocationY.Length - 1; j >= 0; j--)
                {
                    if (judgementLineLocationY[j].x <= noteY && judgementLineLocationY[j].y > noteY)//Y��ġ �ľ�, j = ��������
                    {
                        //��������Ʈ ��ġ ����(������ ��Ʈ ��ġ) 
                        Transform tempNoteLocation = noteManager.noteLocation[tempNote.GetComponent<Note>().noteLocation];
                        judgeEffect.SetNoteTransform(tempNoteLocation); //���� ȿ�� ��ġ ����
                        StartCoroutine(judgeEffect.NoteHitEffect(j, false)); //��Ʈ ��Ʈ ó��

                        noteList.Remove(tempNote); //���� �� ��Ʈ ����

                        ObjectPool.instance.queue[key].Enqueue(tempNote);//������ƮǮ�� ����ֱ�
                        tempNote.SetActive(false);

                        AudioManager.instance.PlaySFX("NoteTap");
                        shooter.Shot(j); //�̻��� ����
                        
                        
                        return;
                    }
                }
            }

        }
        //��������Ʈ ��ġ ����(������ ��Ʈ ��ġ), �̽�
        Transform missNoteLocation = noteManager.noteLocation[key];
        judgeEffect.SetNoteTransform(missNoteLocation);
        StartCoroutine(judgeEffect.NoteHitEffect(3, false)); //����Ʈ ȣ��

    }
    
}
