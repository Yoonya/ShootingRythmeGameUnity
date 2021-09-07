using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using BackEnd; //뒤끝 서버 연결

public class GameManager : MonoBehaviour
{
    private Judgement judgement;
    private LongNoteJudgement longNoteJudgement;
    private CounterJudgementLeft counterJudgementLeft;
    private CounterJudgementRight counterJudgementRight;
    private NoteTypeJudge noteTypeJudge;
    private ScoreBoard scoreBoard;
    private EnemyStatus enemyStatus;
    private PlayerStatus playerStatus;

    public int currentSongNumber = 0;
    public string currentSongName;
    public string id;
    public float speed; //사용자설정 스피드
    public float sync; //사용자설정 싱크

    public bool isPause = false;
    public bool isGameOver = false;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gameoverAnimator;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 중앙에 리셋, 커서가 안보임
        //Cursor.lockState = CursorLockMode.None; // 마우스커서 정상
        Cursor.lockState = CursorLockMode.Confined; //마우스 가두기
        ConfirmSong(); //인덱스와 서버를 이용해 노래 정보 받아오기
        ConfirmStatus(); //싱크, 스피드 설정 받아오기

        enemyStatus = FindObjectOfType<EnemyStatus>();
        playerStatus = FindObjectOfType<PlayerStatus>();

        enemyStatus.currentSongNumber = currentSongNumber; //적 설정
        enemyStatus.enemyName = currentSongName;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        judgement = FindObjectOfType<Judgement>();
        longNoteJudgement = FindObjectOfType<LongNoteJudgement>();
        counterJudgementLeft = FindObjectOfType<CounterJudgementLeft>();
        counterJudgementRight = FindObjectOfType<CounterJudgementRight>();
        noteTypeJudge = FindObjectOfType<NoteTypeJudge>();
        scoreBoard = FindObjectOfType<ScoreBoard>();

        AudioManager.instance.PlayBGM("BGM" + currentSongNumber);
    }

    public void BtnPauseBack()
    {
        isPause = !isPause;
        pause.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined; //마우스 가두기
        Time.timeScale = 1f;
        AudioManager.instance.ReplayBGM();
    }

    public void BtnPauseMain()
    {
        //초기화
        pause.SetActive(false);
        Cursor.lockState = CursorLockMode.None; // 마우스커서 정상
        Time.timeScale = 1f;
        AudioManager.instance.ReplayBGM();

        LoadingSceneManager.LoadScene("SongSelect");
    }

    private void ConfirmSong() //서버에서 song DB로부터 정보 받아오기
    {
        var bro = Backend.GameData.GetMyData("song", new Where(), 10);
        if (bro.IsSuccess() == false)
        {
            // 요청 실패 처리
            Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // 요청이 성공해도 where 조건에 부합하는 데이터가 없을 수 있기 때문에
            // 데이터가 존재하는지 확인
            // 위와 같은 new Where() 조건의 경우 테이블에 row가 하나도 없으면 Count가 0 이하 일 수 있다.
            Debug.Log(bro);
            return;
        }
        // 검색한 데이터의 모든 row의 inDate 값 확인
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();
            string tempSongNumber = bro.Rows()[i]["songNumber"]["S"].ToString();
            string tempSongName = bro.Rows()[i]["songName"]["S"].ToString();

            //데이터 저장
            id = tempID;
            currentSongNumber = int.Parse(tempSongNumber);
            currentSongName = tempSongName;
        }
    }

    private void ConfirmStatus() //서버에서 status DB로부터 정보 받아오기
    {
        var bro = Backend.GameData.GetMyData("status", new Where(), 10);
        if (bro.IsSuccess() == false)
        {
            // 요청 실패 처리
            Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // 요청이 성공해도 where 조건에 부합하는 데이터가 없을 수 있기 때문에
            // 데이터가 존재하는지 확인
            // 위와 같은 new Where() 조건의 경우 테이블에 row가 하나도 없으면 Count가 0 이하 일 수 있다.
            Debug.Log(bro);
            return;
        }
        // 검색한 데이터의 모든 row의 inDate 값 확인
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            string tempSpeed = bro.Rows()[i]["speed"]["S"].ToString();
            string tempSync = bro.Rows()[i]["sync"]["S"].ToString();

            //데이터 저장
            speed = float.Parse(tempSpeed);
            sync = float.Parse(tempSync);
        }
    }

    IEnumerator GameOverCoroutine()
    {
        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(5.0f);
        LoadingSceneManager.LoadScene("SongSelect");
    }

    // Update is called once per frame
    void Update() //게임조작
    {
        if (!AudioManager.instance.IsPlayingBGM() && !isPause && !isGameOver) //노래가 다 끝나서 게임 끝
        {
            Cursor.lockState = CursorLockMode.None; // 마우스커서 정상
            Cursor.visible = true;
            LoadingSceneManager.LoadScene("Result");
            scoreBoard.ResultUpdate(id, currentSongNumber.ToString(), currentSongName);
        }

        if (playerStatus.currentHP <= 0) //체력이 전부 달아 게임 끝
        {
            if(!isGameOver) //중복실행방지
                isGameOver = !isGameOver; //게임 종료
            playerStatus.currentHP = 1; //중복 실행 방지

            if (isGameOver)
            {
                AudioManager.instance.PauseBGM();
                enemy.SetActive(false);
                player.SetActive(false);
                gameoverAnimator.SetActive(true);

                Cursor.lockState = CursorLockMode.None; // 마우스커서 정상
                Cursor.visible = true;
                StartCoroutine(GameOverCoroutine());          
            }
        }


        //일시정지 기능
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = !isPause;

            if (isPause && !isGameOver)
            {
                pause.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None; // 마우스커서 정상
                Time.timeScale = 0f;
                AudioManager.instance.PauseBGM();
            }
            else
            {
                pause.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined; //마우스 가두기
                Time.timeScale = 1f;
                AudioManager.instance.ReplayBGM();
            }
        }

        if (!isPause && !isGameOver)//일시정지 상태가 아니고 게임 종료가 아닐 때
        {
            if (noteTypeJudge.currentType == "Note")
            { //단일노트
                if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //5키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //4키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //4키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4키 동시입력
                {
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift)) //3키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //3키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //3키 동시입력
                {
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    judgement.CheckLine("A");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    judgement.CheckLine("S");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D)) //3키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftShift)) //2키 동시입력
                {
                    judgement.CheckLine("A");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.Space)) //2키 동시입력
                {
                    judgement.CheckLine("A");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift)) //2키 동시입력
                {
                    judgement.CheckLine("S");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space)) //2키 동시입력
                {
                    judgement.CheckLine("S");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //2키 동시입력
                {
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //2키 동시입력
                {
                    judgement.CheckLine("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S)) //2키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D)) //2키 동시입력
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("D");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D)) //2키 동시입력
                {
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //2키 동시입력
                {
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    judgement.CheckLine("A");
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    judgement.CheckLine("S");
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    judgement.CheckLine("D");
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    counterJudgementRight.CheckLine("Space");
                }
            }
            else if (noteTypeJudge.currentType == "LongNote") //롱노트
            {
                if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //5키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //4키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //4키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4키 동시입력
                {
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    longNoteJudgement.CheckLineFront("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift)) //3키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //3키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //3키 동시입력
                {
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3키 동시입력
                {
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D)) //3키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftShift)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.Space)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("D");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //2키 동시입력
                {
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    longNoteJudgement.CheckLineFront("A");
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    longNoteJudgement.CheckLineFront("S");
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    longNoteJudgement.CheckLineFront("D");
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    counterJudgementRight.CheckLine("Space");
                }
                //롱노트 떼었을 때
                else if (Input.GetKeyUp(KeyCode.A) && Input.GetKeyUp(KeyCode.S) && Input.GetKeyUp(KeyCode.D)) //3키 동시입력
                {
                    longNoteJudgement.CheckLineBack("A");
                    longNoteJudgement.CheckLineBack("S");
                    longNoteJudgement.CheckLineBack("D");
                }
                else if (Input.GetKeyUp(KeyCode.A) && Input.GetKeyUp(KeyCode.S)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineBack("A");
                    longNoteJudgement.CheckLineBack("S");
                }
                else if (Input.GetKeyUp(KeyCode.A) && Input.GetKeyUp(KeyCode.D)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineBack("A");
                    longNoteJudgement.CheckLineBack("D");
                }
                else if (Input.GetKeyUp(KeyCode.S) && Input.GetKeyUp(KeyCode.D)) //2키 동시입력
                {
                    longNoteJudgement.CheckLineBack("S");
                    longNoteJudgement.CheckLineBack("D");
                }
                else if (Input.GetKeyUp(KeyCode.A))
                {
                    longNoteJudgement.CheckLineBack("A");
                }
                else if (Input.GetKeyUp(KeyCode.S))
                {
                    longNoteJudgement.CheckLineBack("S");
                }
                else if (Input.GetKeyUp(KeyCode.D))
                {
                    longNoteJudgement.CheckLineBack("D");
                }
            }
        }
    }
}
