using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class go_Star //��Ʈ����
{
    public int starType = 0;
    public int starLocation = 0;
    public float appearTime = 0f;
    public go_Star(int starType, int starLocation, float appearTime)
    {
        this.starType = starType;
        this.starLocation = starLocation;
        this.appearTime = appearTime;
    }
}

public class StarManager : MonoBehaviour
{
    private int starSpeed;
    [SerializeField] private float sync0 = 0f; //�߰� ��ũ(������ ����)
    private float sync1; //�߰� ��ũ(����� ����)
    private float sync2; //�⺻��ũ

    [SerializeField]
    public Transform[] starLocation = null;

    private List<go_Star> stars = new List<go_Star>();
    private GameManager gameManager;
    private SongNoteManager songNoteManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        songNoteManager = FindObjectOfType<SongNoteManager>();

        starSpeed = (int)(gameManager.speed * 1000);
        sync1 = gameManager.sync;
        sync2 = starLocation[0].transform.localPosition.y / starSpeed;

        float tempSync = (starSpeed - 2000.0f) / 5000.0f; //���а��
        sync0 += tempSync;
        sync0 += 0.6f; //�߰����

        ReadStarInfo();
        //��� ��Ʈ�� ������ �ð��� ����ϵ��� ����
        for (int i = 0; i < stars.Count; i++)
        {
            StartCoroutine(StartMakeStar(stars[i]));
        }
    }

    private void ReadStarInfo()   //���ҽ����� ��Ʈ �ؽ�Ʈ ������ �ҷ�����
    {
        if (songNoteManager.songNotes[gameManager.currentSongNumber].stars != "")
        {
            string[] texts = songNoteManager.songNotes[gameManager.currentSongNumber].stars.Split('\n');

            if (texts.Length > 0) //�޸��忡 �ִ� ��Ʈ ����Ʈ�� ����
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    int tempType = Convert.ToInt32(texts[i].Split(' ')[0]);
                    int tempLocation = Convert.ToInt32(texts[i].Split(' ')[1]);
                    //�������ο� ������ �ð��� ���� �޸��忡 ���� �ð��� �����ϵ��� ���(��ũ)
                    float tempAppear = Convert.ToSingle(texts[i].Split(' ')[2]) - sync2 + sync1 + sync0;

                    go_Star tempStar = new go_Star(tempType, tempLocation, tempAppear);
                    stars.Add(tempStar);
                }
            }
        }
    }

    IEnumerator StartMakeStar(go_Star star)
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(star.appearTime);
        CreateStar(star);
    }

    private void CreateStar(go_Star star) //��Ÿ����
    {
        GameObject tempStar = ObjectPool.instance.queue[star.starType + 9].Dequeue(); //������Ʈ Ǯ�� ���, ��Ÿ��ġ�� 9����

        tempStar.transform.localScale = new Vector3(1f, 1f, 1f); 
        tempStar.GetComponent<Star>().SetStarSpeed(starSpeed); //��Ÿ �ӵ� ����
        tempStar.GetComponent<Star>().SetStarLocation(star.starLocation); //��Ÿ ��ġ ����
        tempStar.GetComponent<Star>().SetAppearTime(star.appearTime); //��Ÿ �ð� ����
        tempStar.GetComponent<Star>().SetStarType(star.starType); //��Ÿ Ÿ�� ����
        tempStar.GetComponent<Image>().enabled = true; //ĳ���� �ǰݸ�� �߿� false�� ���� �ִµ� Ȥ�� �𸣴� �ʱ�ȭ

        tempStar.transform.position = starLocation[star.starLocation].position;
        tempStar.SetActive(true);
    }

    private void OnTriggerEnter(Collider other) //��������
    {
        if (other.CompareTag("Star"))
        {
            ObjectPool.instance.queue[other.GetComponent<Star>().starType + 9].Enqueue(other.gameObject);//������ƮǮ�� ����ֱ�
            other.gameObject.SetActive(false);
        }
    }
}
