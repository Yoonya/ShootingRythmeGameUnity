using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class go_LongNote //노트설정, 롱노트 함수랑 같음
{
    public int longNoteSpeed = 0; //롱노트 스피드
    public int longNoteType = 0; //롱노트 타입
    public int longNoteLocation = 0; 
    public float appearTime1 = 0f; //앞등장시간
    public float appearTime2 = 0f; //뒷등장시간
    public int fb = 0; //뭐였지
    public go_LongNote(int longNoteType, int longNoteLocation, float appearTime1, float appearTime2)
    {
        this.longNoteType = longNoteType;
        this.longNoteLocation = longNoteLocation;
        this.appearTime1 = appearTime1;
        this.appearTime2 = appearTime2;
    }
}

//롱노트는 첫부분을 클릭하고 마지막 부분에 떼었을 때, 판정이 있는 것으로 했으며, 중간에 몸체는 두 노트의 시간차이에 맞춰 높이를 키워서 롱노트처럼 보이게 하였다.
public class LongNoteManager : MonoBehaviour
{
    private int longNoteSpeed;
    [SerializeField] private float sync0 = 0f; //추가 싱크(개발자 전용)
    private float sync1; //추가 싱크(사용자 설정)
    private float sync2; //기본싱크
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
        sync2 = longNoteLocation[0].transform.localPosition.y / longNoteSpeed;//기본싱크

        float tempSync = (longNoteSpeed - 2000.0f) / 5000.0f; //세밀계산
        sync0 += tempSync;
        sync0 += 0.6f; //추가계산

        ReadNoteInfo();
        //모든 노트를 정해진 시간에 출발하도록 설정
        for (int i = 0; i < longNotes.Count; i++)
        {
            StartCoroutine(StartMakeNote(longNotes[i]));
        }
    }

    private void ReadNoteInfo()   //리소스에서 노트 텍스트 파일을 불러오기
    {
        if (songNoteManager.songNotes[gameManager.currentSongNumber].longNotes != "")
        {
            string[] texts = songNoteManager.songNotes[gameManager.currentSongNumber].longNotes.Split('\n');

            if (texts.Length > 0) //메모장에 있는 노트 리스트로 정리
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    int tempType = Convert.ToInt32(texts[i].Split(' ')[0]);
                    int tempLocation = Convert.ToInt32(texts[i].Split(' ')[1]);
                    //판정라인에 도달할 시간을 빼서 메모장에 적은 시간에 도착하도록 계산(sync로 조절)
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
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(longNote.appearTime1); //첫등장
        CreateNote(longNote, 0);
        GameObject temp = CreateNoteBody(longNote); //롱노트 바디도 구현
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(longNote.appearTime2 - longNote.appearTime1);//뒷등장
        CreateNote(longNote, 1);
        temp.GetComponent<LongNoteBody>().IsreSize = false; //롱노트 바디 길이 증가 멈춤
    }

    private void CreateNote(go_LongNote longNote, int fb) //노트생성, 뒤 인수는 앞인지 뒤인지 판별
    {
        GameObject tempNote = ObjectPool.instance.queue[18 + longNote.longNoteType].Dequeue(); //오브젝트 풀링 사용, 롱노트는 18번부터

        tempNote.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f); //스케일을 2.5로, 왜 커지는지 모르겠음
        tempNote.transform.localRotation = Quaternion.Euler(Vector3.zero); //롱노트는 다시 트랙과 평면으로 맞춰야한다.

        tempNote.GetComponent<LongNote>().SetNoteSpeed(longNoteSpeed); //노트 속도 설정
        tempNote.GetComponent<LongNote>().SetNoteLocation(longNote.longNoteLocation); //노트 위치 설정    
        tempNote.GetComponent<LongNote>().SetNoteType(longNote.longNoteType); //노트 타입 설정
        tempNote.GetComponent<LongNote>().fb = fb;

        tempNote.transform.position = longNoteLocation[longNote.longNoteLocation].position;

        if (fb == 0) //롱노트 앞부분
        {
            tempNote.GetComponent<LongNote>().SetAppearTime(longNote.appearTime1); //노트 시간 설정
            longNoteJudgement.noteListFront.Add(tempNote);
        }
        else //롱노트 뒷부분
        {
            tempNote.GetComponent<LongNote>().SetAppearTime(longNote.appearTime2); //노트 시간 설정
            longNoteJudgement.noteListBack.Add(tempNote);
        }
       
        tempNote.SetActive(true);       
    }

    private GameObject CreateNoteBody(go_LongNote longNote) //노트생성
    {
        GameObject tempNote = ObjectPool.instance.queue[21 + longNote.longNoteType].Dequeue(); //오브젝트 풀링 사용, 롱노트 바디는 21번부터

        tempNote.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f); //스케일을 2.5로, 왜 커지는지 모르겠음
        tempNote.transform.localRotation = Quaternion.Euler(Vector3.zero); //롱노트는 다시 트랙과 평면으로 맞춰야한다.

        tempNote.GetComponent<LongNoteBody>().SetNoteSpeed(longNoteSpeed); //노트 속도 설정
        tempNote.GetComponent<LongNoteBody>().SetNoteLocation(longNote.longNoteLocation); //노트 위치 설정
        tempNote.GetComponent<LongNoteBody>().SetAppearTime(longNote.appearTime1); //노트 시간 설정
        tempNote.GetComponent<LongNoteBody>().SetReSizeTime(longNote.appearTime2 - longNote.appearTime1); //노트 시간 설정
        tempNote.GetComponent<LongNoteBody>().SetNoteType(longNote.longNoteType); //노트 타입 설정
        tempNote.GetComponent<LongNoteBody>().IsreSize = true; //길이 증가

        tempNote.transform.position = longNoteLocation[longNote.longNoteLocation].position;
        tempNote.SetActive(true);

        return tempNote;
    }

    private void OnTriggerEnter(Collider other) //쓰레기통
    {
        if (other.CompareTag("LongNote"))
        {
            ObjectPool.instance.queue[18 + other.GetComponent<LongNote>().longNoteType].Enqueue(other.gameObject);//오브젝트풀링 집어넣기

            //롱노트는 판정시 사라지지 않게 연출하며 그렇기에 아예 안칠시 miss가 없음

            if (other.gameObject.GetComponent<LongNote>().fb == 0) //롱노트 앞부분
            {
                longNoteJudgement.noteListFront.Remove(other.gameObject);
            }
            else
            {
                longNoteJudgement.noteListBack.Remove(other.gameObject);
                longNoteJudgement.isLongNoting[other.gameObject.GetComponent<LongNote>().longNoteType] = false; //뒷노트가 지나가도 누르고있으면 계속 점수가 올라가는 것을 막기 위해
            }
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("LongNoteBody"))
        {
            StartCoroutine(EnqueueLongNoteBody(other.gameObject));
        }
    }

    private IEnumerator EnqueueLongNoteBody(GameObject other) //롱노트몸통 돌려넣기
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(other.GetComponent<LongNoteBody>().resizeTime + 0.1f); //대기시간에 여유줘서 기다린 후 파괴
        ObjectPool.instance.queue[21 + other.GetComponent<LongNoteBody>().longNoteType].Enqueue(other.gameObject);//오브젝트풀링 집어넣기
        other.gameObject.GetComponent<LongNoteBody>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0); // 높이만 0으로 초기화
        other.gameObject.GetComponent<LongNoteBody>().height = 0; // 높이만 0으로 초기화
        other.gameObject.SetActive(false);
    }
}
