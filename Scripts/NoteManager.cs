using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class go_Note //노트설정
{
    public int noteType = 0; //노트타입
    public int noteLocation = 0;//노트위치
    public float appearTime = 0f;//등장시간
    public go_Note(int noteType, int noteLocation, float appearTime)
    {
        this.noteType = noteType;
        this.noteLocation = noteLocation;
        this.appearTime = appearTime;
    }
}

public class NoteManager : MonoBehaviour
{
    private int noteSpeed; //게임 스피드, 노트 떨어지는 속도
    [SerializeField] private float sync0 = 0f; //추가 싱크(개발자 전용)
    private float sync1; //추가 싱크(사용자 설정)
    private float sync2; //기본싱크
    
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
        sync2 = noteLocation[0].transform.localPosition.y / noteSpeed; //기본 싱크

        float tempSync = (noteSpeed - 2000.0f) / 5000.0f; //속도에 따른 세밀계산(5천은 맵의 크기)
        sync0 += tempSync;
        sync0 += 0.6f; //추가계산

        ReadNoteInfo();
        //모든 노트를 정해진 시간에 출발하도록 설정
        for (int i = 0; i < notes.Count; i++)
        {
            StartCoroutine(StartMakeNote(notes[i]));
        }

    }

    private void ReadNoteInfo()   //리소스에서 노트 텍스트 파일을 불러오기
    {
        if (songNoteManager.songNotes[gameManager.currentSongNumber].notes != "") //해당 값이 있을 경우에만
        {
            string[] texts = songNoteManager.songNotes[gameManager.currentSongNumber].notes.Split('\n');

            if (texts.Length > 0) //메모장에 있는 노트 리스트로 정리
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    int tempType = Convert.ToInt32(texts[i].Split(' ')[0]);
                    int tempLocation = Convert.ToInt32(texts[i].Split(' ')[1]);
                    //판정라인에 도달할 시간을 빼서 메모장에 적은 시간에 도착하도록 계산(sync로 조절)
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

    private void CreateNote(go_Note note) //노트생성
    {
        GameObject tempNote = ObjectPool.instance.queue[note.noteType].Dequeue(); //오브젝트 풀링 사용

        tempNote.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f); //스케일을 2.5로, 왜 커지는지 모르겠음
        tempNote.GetComponent<Note>().SetNoteSpeed(noteSpeed); //노트 속도 설정
        tempNote.GetComponent<Note>().SetNoteLocation(note.noteLocation); //노트 위치 설정
        tempNote.GetComponent<Note>().SetAppearTime(note.appearTime); //노트 시간 설정
        tempNote.GetComponent<Note>().SetNoteType(note.noteType); //노트 타입 설정

        tempNote.transform.position = noteLocation[note.noteLocation].position;
        tempNote.SetActive(true);

        judgement.noteList.Add(tempNote);
    }

    private void OnTriggerEnter(Collider other) //쓰레기통
    {
        if (other.CompareTag("Note"))
        {
            Transform missNoteLocation = noteLocation[1];
            judgeEffect.SetNoteTransform(missNoteLocation); //판정효과 위치설정
            StartCoroutine(judgeEffect.NoteHitEffect(3, true));//판정효과

            judgement.noteList.Remove(other.gameObject);

            ObjectPool.instance.queue[other.GetComponent<Note>().noteType].Enqueue(other.gameObject);//오브젝트풀링 집어넣기
            other.gameObject.SetActive(false);
        }
    }

    public Transform[] GetNoteLocation()
    {
        return noteLocation;
    }

}
