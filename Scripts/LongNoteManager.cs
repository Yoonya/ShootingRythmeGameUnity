using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class go_LongNote //��Ʈ����, �ճ�Ʈ �Լ��� ����
{
    public int longNoteSpeed = 0; //�ճ�Ʈ ���ǵ�
    public int longNoteType = 0; //�ճ�Ʈ Ÿ��
    public int longNoteLocation = 0; 
    public float appearTime1 = 0f; //�յ���ð�
    public float appearTime2 = 0f; //�޵���ð�
    public int fb = 0; //������
    public go_LongNote(int longNoteType, int longNoteLocation, float appearTime1, float appearTime2)
    {
        this.longNoteType = longNoteType;
        this.longNoteLocation = longNoteLocation;
        this.appearTime1 = appearTime1;
        this.appearTime2 = appearTime2;
    }
}

//�ճ�Ʈ�� ù�κ��� Ŭ���ϰ� ������ �κп� ������ ��, ������ �ִ� ������ ������, �߰��� ��ü�� �� ��Ʈ�� �ð����̿� ���� ���̸� Ű���� �ճ�Ʈó�� ���̰� �Ͽ���.
public class LongNoteManager : MonoBehaviour
{
    private int longNoteSpeed;
    [SerializeField] private float sync0 = 0f; //�߰� ��ũ(������ ����)
    private float sync1; //�߰� ��ũ(����� ����)
    private float sync2; //�⺻��ũ
    private int count = 0;

    [SerializeField]
    public Transform[] longNoteLocation = null;

    private List<go_LongNote> longNotes = new List<go_LongNote>();

    [SerializeField] public GameObject prefab;
    [SerializeField] public GameObject longNoteBody;

    private LongNoteJudgement longNoteJudgement;
    private JudgeEffect judgeEffect;
    private GameManager gameManager;
    private SongNoteManager songNoteManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        longNoteJudgement = FindObjectOfType<LongNoteJudgement>();
        judgeEffect = FindObjectOfType<JudgeEffect>();
        songNoteManager = FindObjectOfType<SongNoteManager>();

        longNoteSpeed = (int)(gameManager.speed * 1000);
        sync1 = gameManager.sync;
        sync2 = longNoteLocation[0].transform.localPosition.y / longNoteSpeed;//�⺻��ũ

        float tempSync = (longNoteSpeed - 2000.0f) / 5000.0f; //���а��
        sync0 += tempSync;
        sync0 += 0.6f; //�߰����

        ReadNoteInfo();
        //��� ��Ʈ�� ������ �ð��� ����ϵ��� ����
        for (int i = 0; i < longNotes.Count; i++)
        {
            StartCoroutine(StartMakeNote(longNotes[i]));
        }
    }

    private void ReadNoteInfo()   //���ҽ����� ��Ʈ �ؽ�Ʈ ������ �ҷ�����
    {
        if (songNoteManager.songNotes[gameManager.currentSongNumber].longNotes != "")
        {
            string[] texts = songNoteManager.songNotes[gameManager.currentSongNumber].longNotes.Split('\n');

            if (texts.Length > 0) //�޸��忡 �ִ� ��Ʈ ����Ʈ�� ����
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    int tempType = Convert.ToInt32(texts[i].Split(' ')[0]);
                    int tempLocation = Convert.ToInt32(texts[i].Split(' ')[1]);
                    //�������ο� ������ �ð��� ���� �޸��忡 ���� �ð��� �����ϵ��� ���(sync�� ����)
                    float tempAppear1 = Convert.ToSingle(texts[i].Split(' ')[2]) - sync2 + sync1 + sync0;
                    float tempAppear2 = Convert.ToSingle(texts[i].Split(' ')[3]) - sync2 + sync1 + sync0;

                    go_LongNote tempNote = new go_LongNote(tempType, tempLocation, tempAppear1, tempAppear2);
                    longNotes.Add(tempNote);
                }
            }
        }
    }

    IEnumerator StartMakeNote(go_LongNote longNote)
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(longNote.appearTime1); //ù����
        CreateNote(longNote, 0);
        GameObject temp = CreateNoteBody(longNote); //�ճ�Ʈ �ٵ� ����
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(longNote.appearTime2 - longNote.appearTime1);//�޵���
        CreateNote(longNote, 1);
        temp.GetComponent<LongNoteBody>().IsreSize = false; //�ճ�Ʈ �ٵ� ���� ���� ����
    }

    private void CreateNote(go_LongNote longNote, int fb) //��Ʈ����, �� �μ��� ������ ������ �Ǻ�
    {
        GameObject tempNote = ObjectPool.instance.queue[18 + longNote.longNoteType].Dequeue(); //������Ʈ Ǯ�� ���, �ճ�Ʈ�� 18������

        tempNote.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f); //�������� 2.5��, �� Ŀ������ �𸣰���
        tempNote.transform.localRotation = Quaternion.Euler(Vector3.zero); //�ճ�Ʈ�� �ٽ� Ʈ���� ������� ������Ѵ�.

        tempNote.GetComponent<LongNote>().SetNoteSpeed(longNoteSpeed); //��Ʈ �ӵ� ����
        tempNote.GetComponent<LongNote>().SetNoteLocation(longNote.longNoteLocation); //��Ʈ ��ġ ����    
        tempNote.GetComponent<LongNote>().SetNoteType(longNote.longNoteType); //��Ʈ Ÿ�� ����
        tempNote.GetComponent<LongNote>().fb = fb;

        tempNote.transform.position = longNoteLocation[longNote.longNoteLocation].position;

        if (fb == 0) //�ճ�Ʈ �պκ�
        {
            tempNote.GetComponent<LongNote>().SetAppearTime(longNote.appearTime1); //��Ʈ �ð� ����
            longNoteJudgement.noteListFront.Add(tempNote);
        }
        else //�ճ�Ʈ �޺κ�
        {
            tempNote.GetComponent<LongNote>().SetAppearTime(longNote.appearTime2); //��Ʈ �ð� ����
            longNoteJudgement.noteListBack.Add(tempNote);
        }
       
        tempNote.SetActive(true);       
    }

    private GameObject CreateNoteBody(go_LongNote longNote) //��Ʈ����
    {
        GameObject tempNote = ObjectPool.instance.queue[21 + longNote.longNoteType].Dequeue(); //������Ʈ Ǯ�� ���, �ճ�Ʈ �ٵ�� 21������

        tempNote.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f); //�������� 2.5��, �� Ŀ������ �𸣰���
        tempNote.transform.localRotation = Quaternion.Euler(Vector3.zero); //�ճ�Ʈ�� �ٽ� Ʈ���� ������� ������Ѵ�.

        tempNote.GetComponent<LongNoteBody>().SetNoteSpeed(longNoteSpeed); //��Ʈ �ӵ� ����
        tempNote.GetComponent<LongNoteBody>().SetNoteLocation(longNote.longNoteLocation); //��Ʈ ��ġ ����
        tempNote.GetComponent<LongNoteBody>().SetAppearTime(longNote.appearTime1); //��Ʈ �ð� ����
        tempNote.GetComponent<LongNoteBody>().SetReSizeTime(longNote.appearTime2 - longNote.appearTime1); //��Ʈ �ð� ����
        tempNote.GetComponent<LongNoteBody>().SetNoteType(longNote.longNoteType); //��Ʈ Ÿ�� ����
        tempNote.GetComponent<LongNoteBody>().IsreSize = true; //���� ����

        tempNote.transform.position = longNoteLocation[longNote.longNoteLocation].position;
        tempNote.SetActive(true);

        return tempNote;
    }

    private void OnTriggerEnter(Collider other) //��������
    {
        if (other.CompareTag("LongNote"))
        {
            ObjectPool.instance.queue[18 + other.GetComponent<LongNote>().longNoteType].Enqueue(other.gameObject);//������ƮǮ�� ����ֱ�

            //�ճ�Ʈ�� ������ ������� �ʰ� �����ϸ� �׷��⿡ �ƿ� ��ĥ�� miss�� ����

            if (other.gameObject.GetComponent<LongNote>().fb == 0) //�ճ�Ʈ �պκ�
            {
                longNoteJudgement.noteListFront.Remove(other.gameObject);
            }
            else
            {
                longNoteJudgement.noteListBack.Remove(other.gameObject);
                longNoteJudgement.isLongNoting[other.gameObject.GetComponent<LongNote>().longNoteType] = false; //�޳�Ʈ�� �������� ������������ ��� ������ �ö󰡴� ���� ���� ����
            }
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("LongNoteBody"))
        {
            StartCoroutine(EnqueueLongNoteBody(other.gameObject));
        }
    }

    private IEnumerator EnqueueLongNoteBody(GameObject other) //�ճ�Ʈ���� �����ֱ�
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(other.GetComponent<LongNoteBody>().resizeTime + 0.1f); //���ð��� �����༭ ��ٸ� �� �ı�
        ObjectPool.instance.queue[21 + other.GetComponent<LongNoteBody>().longNoteType].Enqueue(other.gameObject);//������ƮǮ�� ����ֱ�
        other.gameObject.GetComponent<LongNoteBody>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0); // ���̸� 0���� �ʱ�ȭ
        other.gameObject.GetComponent<LongNoteBody>().height = 0; // ���̸� 0���� �ʱ�ȭ
        other.gameObject.SetActive(false);
    }
}
