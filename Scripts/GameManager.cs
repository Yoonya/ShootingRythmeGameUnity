using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using BackEnd; //�ڳ� ���� ����

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
    public float speed; //����ڼ��� ���ǵ�
    public float sync; //����ڼ��� ��ũ

    public bool isPause = false;
    public bool isGameOver = false;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gameoverAnimator;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ�� �߾ӿ� ����, Ŀ���� �Ⱥ���
        //Cursor.lockState = CursorLockMode.None; // ���콺Ŀ�� ����
        Cursor.lockState = CursorLockMode.Confined; //���콺 ���α�
        ConfirmSong(); //�ε����� ������ �̿��� �뷡 ���� �޾ƿ���
        ConfirmStatus(); //��ũ, ���ǵ� ���� �޾ƿ���

        enemyStatus = FindObjectOfType<EnemyStatus>();
        playerStatus = FindObjectOfType<PlayerStatus>();

        enemyStatus.currentSongNumber = currentSongNumber; //�� ����
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
        Cursor.lockState = CursorLockMode.Confined; //���콺 ���α�
        Time.timeScale = 1f;
        AudioManager.instance.ReplayBGM();
    }

    public void BtnPauseMain()
    {
        //�ʱ�ȭ
        pause.SetActive(false);
        Cursor.lockState = CursorLockMode.None; // ���콺Ŀ�� ����
        Time.timeScale = 1f;
        AudioManager.instance.ReplayBGM();

        LoadingSceneManager.LoadScene("SongSelect");
    }

    private void ConfirmSong() //�������� song DB�κ��� ���� �޾ƿ���
    {
        var bro = Backend.GameData.GetMyData("song", new Where(), 10);
        if (bro.IsSuccess() == false)
        {
            // ��û ���� ó��
            Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // ��û�� �����ص� where ���ǿ� �����ϴ� �����Ͱ� ���� �� �ֱ� ������
            // �����Ͱ� �����ϴ��� Ȯ��
            // ���� ���� new Where() ������ ��� ���̺� row�� �ϳ��� ������ Count�� 0 ���� �� �� �ִ�.
            Debug.Log(bro);
            return;
        }
        // �˻��� �������� ��� row�� inDate �� Ȯ��
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();
            string tempSongNumber = bro.Rows()[i]["songNumber"]["S"].ToString();
            string tempSongName = bro.Rows()[i]["songName"]["S"].ToString();

            //������ ����
            id = tempID;
            currentSongNumber = int.Parse(tempSongNumber);
            currentSongName = tempSongName;
        }
    }

    private void ConfirmStatus() //�������� status DB�κ��� ���� �޾ƿ���
    {
        var bro = Backend.GameData.GetMyData("status", new Where(), 10);
        if (bro.IsSuccess() == false)
        {
            // ��û ���� ó��
            Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // ��û�� �����ص� where ���ǿ� �����ϴ� �����Ͱ� ���� �� �ֱ� ������
            // �����Ͱ� �����ϴ��� Ȯ��
            // ���� ���� new Where() ������ ��� ���̺� row�� �ϳ��� ������ Count�� 0 ���� �� �� �ִ�.
            Debug.Log(bro);
            return;
        }
        // �˻��� �������� ��� row�� inDate �� Ȯ��
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            string tempSpeed = bro.Rows()[i]["speed"]["S"].ToString();
            string tempSync = bro.Rows()[i]["sync"]["S"].ToString();

            //������ ����
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
    void Update() //��������
    {
        if (!AudioManager.instance.IsPlayingBGM() && !isPause && !isGameOver) //�뷡�� �� ������ ���� ��
        {
            Cursor.lockState = CursorLockMode.None; // ���콺Ŀ�� ����
            Cursor.visible = true;
            LoadingSceneManager.LoadScene("Result");
            scoreBoard.ResultUpdate(id, currentSongNumber.ToString(), currentSongName);
        }

        if (playerStatus.currentHP <= 0) //ü���� ���� �޾� ���� ��
        {
            if(!isGameOver) //�ߺ��������
                isGameOver = !isGameOver; //���� ����
            playerStatus.currentHP = 1; //�ߺ� ���� ����

            if (isGameOver)
            {
                AudioManager.instance.PauseBGM();
                enemy.SetActive(false);
                player.SetActive(false);
                gameoverAnimator.SetActive(true);

                Cursor.lockState = CursorLockMode.None; // ���콺Ŀ�� ����
                Cursor.visible = true;
                StartCoroutine(GameOverCoroutine());          
            }
        }


        //�Ͻ����� ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = !isPause;

            if (isPause && !isGameOver)
            {
                pause.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None; // ���콺Ŀ�� ����
                Time.timeScale = 0f;
                AudioManager.instance.PauseBGM();
            }
            else
            {
                pause.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined; //���콺 ���α�
                Time.timeScale = 1f;
                AudioManager.instance.ReplayBGM();
            }
        }

        if (!isPause && !isGameOver)//�Ͻ����� ���°� �ƴϰ� ���� ���ᰡ �ƴ� ��
        {
            if (noteTypeJudge.currentType == "Note")
            { //���ϳ�Ʈ
                if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //5Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //4Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //4Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4Ű �����Է�
                {
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift)) //3Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //3Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //3Ű �����Է�
                {
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    judgement.CheckLine("A");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    judgement.CheckLine("S");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D)) //3Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftShift)) //2Ű �����Է�
                {
                    judgement.CheckLine("A");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.Space)) //2Ű �����Է�
                {
                    judgement.CheckLine("A");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift)) //2Ű �����Է�
                {
                    judgement.CheckLine("S");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space)) //2Ű �����Է�
                {
                    judgement.CheckLine("S");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //2Ű �����Է�
                {
                    judgement.CheckLine("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //2Ű �����Է�
                {
                    judgement.CheckLine("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S)) //2Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("S");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D)) //2Ű �����Է�
                {
                    judgement.CheckLine("A");
                    judgement.CheckLine("D");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D)) //2Ű �����Է�
                {
                    judgement.CheckLine("S");
                    judgement.CheckLine("D");
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //2Ű �����Է�
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
            else if (noteTypeJudge.currentType == "LongNote") //�ճ�Ʈ
            {
                if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //5Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //4Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //4Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    longNoteJudgement.CheckLineFront("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftShift)) //4Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift)) //3Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //3Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //3Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //3Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D)) //3Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftShift)) //2Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.Space)) //2Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift)) //2Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.Space)) //2Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("S");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.LeftShift)) //2Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementLeft.CheckLine("Shift");
                }
                else if (Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.Space)) //2Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("D");
                    counterJudgementRight.CheckLine("Space");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.S)) //2Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("S");
                }
                else if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D)) //2Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("A");
                    longNoteJudgement.CheckLineFront("D");
                }
                else if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.D)) //2Ű �����Է�
                {
                    longNoteJudgement.CheckLineFront("S");
                    longNoteJudgement.CheckLineFront("D");
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space)) //2Ű �����Է�
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
                //�ճ�Ʈ ������ ��
                else if (Input.GetKeyUp(KeyCode.A) && Input.GetKeyUp(KeyCode.S) && Input.GetKeyUp(KeyCode.D)) //3Ű �����Է�
                {
                    longNoteJudgement.CheckLineBack("A");
                    longNoteJudgement.CheckLineBack("S");
                    longNoteJudgement.CheckLineBack("D");
                }
                else if (Input.GetKeyUp(KeyCode.A) && Input.GetKeyUp(KeyCode.S)) //2Ű �����Է�
                {
                    longNoteJudgement.CheckLineBack("A");
                    longNoteJudgement.CheckLineBack("S");
                }
                else if (Input.GetKeyUp(KeyCode.A) && Input.GetKeyUp(KeyCode.D)) //2Ű �����Է�
                {
                    longNoteJudgement.CheckLineBack("A");
                    longNoteJudgement.CheckLineBack("D");
                }
                else if (Input.GetKeyUp(KeyCode.S) && Input.GetKeyUp(KeyCode.D)) //2Ű �����Է�
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
