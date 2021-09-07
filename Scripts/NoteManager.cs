using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class go_Note //��Ʈ����
{
    public int noteType = 0; //��ƮŸ��
    public int noteLocation = 0;//��Ʈ��ġ
    public float appearTime = 0f;//����ð�
    public go_Note(int noteType, int noteLocation, float appearTime)
    {
        this.noteType = noteType;
        this.noteLocation = noteLocation;
        this.appearTime = appearTime;
    }
}

public class NoteManager : MonoBehaviour
{
    private int noteSpeed; //���� ���ǵ�, ��Ʈ �������� �ӵ�
    [SerializeField] private float sync0 = 0f; //�߰� ��ũ(������ ����)
    private float sync1; //�߰� ��ũ(����� ����)
    private float sync2; //�⺻��ũ
    
    [SerializeField]
    public Transform[] noteLocation = null;

    private Judgement judgement;
    private JudgeEffect judgeEffect;
    private List<go_Note> notes = new List<go_Note>();
    private GameManager gameManager;
    private SongNoteManager songNoteManager;

    void Start()
    {
        judgement = FindObjectOfType<Judgement>();
        judgeEffect = FindObjectOfType<JudgeEffect>();
        gameManager = FindObjectOfType<GameManager>();
        songNoteManager = FindObjectOfType<SongNoteManager>();

        noteSpeed = (int)(gameManager.speed * 1000);
        sync1 = gameManager.sync;
        sync2 = noteLocation[0].transform.localPosition.y / noteSpeed; //�⺻ ��ũ

        float tempSync = (noteSpeed - 2000.0f) / 5000.0f; //�ӵ��� ���� ���а��(5õ�� ���� ũ��)
        sync0 += tempSync;
        sync0 += 0.6f; //�߰����

        ReadNoteInfo();
        //��� ��Ʈ�� ������ �ð��� ����ϵ��� ����
        for (int i = 0; i < notes.Count; i++)
        {
            StartCoroutine(StartMakeNote(notes[i]));
        }

    }

    private void ReadNoteInfo()   //���ҽ����� ��Ʈ �ؽ�Ʈ ������ �ҷ�����
    {
        if (songNoteManager.songNotes[gameManager.currentSongNumber].notes != "") //�ش� ���� ���� ��쿡��
        {
            string[] texts = songNoteManager.songNotes[gameManager.currentSongNumber].notes.Split('\n');

            if (texts.Length > 0) //�޸��忡 �ִ� ��Ʈ ����Ʈ�� ����
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    int tempType = Convert.ToInt32(texts[i].Split(' ')[0]);
                    int tempLocation = Convert.ToInt32(texts[i].Split(' ')[1]);
                    //�������ο� ������ �ð��� ���� �޸��忡 ���� �ð��� �����ϵ��� ���(sync�� ����)
                    float tempAppear = Convert.ToSingle(texts[i].Split(' ')[2]) - sync2 + sync1 + sync0;

                    go_Note tempNote = new go_Note(tempType, tempLocation, tempAppear);
                    notes.Add(tempNote);
                }
            }
        }
    }

    IEnumerator StartMakeNote(go_Note note)
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(note.appearTime);
        CreateNote(note);
    }

    private void CreateNote(go_Note note) //��Ʈ����
    {
        GameObject tempNote = ObjectPool.instance.queue[note.noteType].Dequeue(); //������Ʈ Ǯ�� ���

        tempNote.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f); //�������� 2.5��, �� Ŀ������ �𸣰���
        tempNote.GetComponent<Note>().SetNoteSpeed(noteSpeed); //��Ʈ �ӵ� ����
        tempNote.GetComponent<Note>().SetNoteLocation(note.noteLocation); //��Ʈ ��ġ ����
        tempNote.GetComponent<Note>().SetAppearTime(note.appearTime); //��Ʈ �ð� ����
        tempNote.GetComponent<Note>().SetNoteType(note.noteType); //��Ʈ Ÿ�� ����

        tempNote.transform.position = noteLocation[note.noteLocation].position;
        tempNote.SetActive(true);

        judgement.noteList.Add(tempNote);
    }

    private void OnTriggerEnter(Collider other) //��������
    {
        if (other.CompareTag("Note"))
        {
            Transform missNoteLocation = noteLocation[1];
            judgeEffect.SetNoteTransform(missNoteLocation); //����ȿ�� ��ġ����
            StartCoroutine(judgeEffect.NoteHitEffect(3, true));//����ȿ��

            judgement.noteList.Remove(other.gameObject);

            ObjectPool.instance.queue[other.GetComponent<Note>().noteType].Enqueue(other.gameObject);//������ƮǮ�� ����ֱ�
            other.gameObject.SetActive(false);
        }
    }

    public Transform[] GetNoteLocation()
    {
        return noteLocation;
    }

}
