using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class go_Arrow //��Ʈ����
{
    public int arrowType = 0; //0 = short, 1 = long
    public int arrowDirect = 0; //0 = left, 1 = right
    public int arrowLocation = 0; //0~4
    public float appearTime = 0f; //��Ʈ ����ð�
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
    private int arrowSpeed;//��Ʈ �ӵ�
    [SerializeField] private float sync0 = 0f; //�߰� ��ũ(������ ����)
    private float sync1; //�߰� ��ũ(����� ����)
    private float sync2; //�⺻��ũ
    [SerializeField] 
    public Transform[] arrowLocation = null; //arrow��ġ

    private List<go_Arrow> arrows = new List<go_Arrow>(); //arrow�� ���� ����Ʈ

    private GameManager gameManager;
    private SongNoteManager songNoteManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        songNoteManager = FindObjectOfType<SongNoteManager>();

        arrowSpeed = (int)(gameManager.speed * 1000);
        sync1 = gameManager.sync;
        sync2 = arrowLocation[0].transform.localPosition.y / arrowSpeed; //�⺻ ��ũ����

        float tempSync = (arrowSpeed - 2000.0f) / 5000.0f; //���а��
        sync0 += tempSync;
        sync0 += 0.6f; //�߰����

        ReadArrowInfo();
        //��� ��Ʈ�� ������ �ð��� ����ϵ��� ����
        for (int i = 0; i < arrows.Count; i++)
        {
            StartCoroutine(StartMakeArrow(arrows[i]));
        }

    }

    private void ReadArrowInfo()   //���ҽ����� ��Ʈ �ؽ�Ʈ ������ �ҷ�����
    {
        if (songNoteManager.songNotes[gameManager.currentSongNumber].arrows != "")
        {
            string[] texts = songNoteManager.songNotes[gameManager.currentSongNumber].arrows.Split('\n');

            if (texts.Length > 0) //�޸��忡 �ִ� ��Ʈ ����Ʈ�� ����
            {
                for (int i = 0; i < texts.Length; i++) //arrow������ �ϳ��� ����
                {
                    int tempType = Convert.ToInt32(texts[i].Split(' ')[0]);
                    int tempDirect = Convert.ToInt32(texts[i].Split(' ')[1]);
                    int tempLocation = Convert.ToInt32(texts[i].Split(' ')[2]);
                    //�������ο� ������ �ð��� ���� �޸��忡 ���� �ð��� �����ϵ��� ���(sync�� ����)
                    float tempAppear = Convert.ToSingle(texts[i].Split(' ')[3]) - sync2 + sync1 + sync0;

                    go_Arrow tempArrow = new go_Arrow(tempType, tempDirect, tempLocation, tempAppear);
                    arrows.Add(tempArrow);
                }
            }
        }
    }

    IEnumerator StartMakeArrow(go_Arrow arrow)//���� �ð��� ���� arrow ����
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(arrow.appearTime);
        CreateArrow(arrow);
    }

    private void CreateArrow(go_Arrow arrow) //��Ʈ����
    {
        GameObject tempArrow = ObjectPool.instance.queue[16 + arrow.arrowType].Dequeue(); //������Ʈ Ǯ�� ���

        tempArrow.transform.localScale = new Vector3(1f, 1f, 1f); //�������� 1��, �� Ŀ������ �𸣰���

        tempArrow.GetComponent<Arrow>().SetArrowSpeed(arrowSpeed); //ȭ�� �ӵ� ����
        tempArrow.GetComponent<Arrow>().SetArrowType(arrow.arrowType); //ȭ�� Ÿ�� ����
        tempArrow.GetComponent<Arrow>().SetArrowDirect(arrow.arrowDirect); //ȭ�� ���� ����
        tempArrow.GetComponent<Arrow>().SetArrowLocation(arrow.arrowLocation); //ȭ�� ��ġ ����
        tempArrow.GetComponent<Arrow>().SetAppearTime(arrow.appearTime); //ȭ�� �ð� ����

        tempArrow.transform.position = arrowLocation[arrow.arrowLocation].position; //������ ��ġ�� ����
        tempArrow.SetActive(true); //on

        //Arrow ��� ����
        if (arrow.arrowType == 0 && arrow.arrowDirect == 0)
            tempArrow.GetComponent<Arrow>().animator.SetTrigger("ArrowShortLeft");
        else if (arrow.arrowType == 0 && arrow.arrowDirect == 1)
            tempArrow.GetComponent<Arrow>().animator.SetTrigger("ArrowShortRight");
        else if (arrow.arrowType == 1 && arrow.arrowDirect == 0)
            tempArrow.GetComponent<Arrow>().animator.SetTrigger("ArrowLongLeft");
        else if (arrow.arrowType == 1 && arrow.arrowDirect == 1)
            tempArrow.GetComponent<Arrow>().animator.SetTrigger("ArrowLongRight");
    }

    private void OnTriggerEnter(Collider other) //��������
    {
        if (other.CompareTag("Arrow"))
        {
            ObjectPool.instance.queue[16 + other.GetComponent<Arrow>().arrowType].Enqueue(other.gameObject);//������ƮǮ�� ����ֱ�           
            other.gameObject.SetActive(false);//off
        }
    }
}
