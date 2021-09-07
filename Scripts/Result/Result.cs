using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BackEnd; //뒤끝 서버 연결

public class Result : MonoBehaviour
{
    //결과창
    [SerializeField]
    private VideoPlayer[] videoPlayer0; //등장, {ss, s, a, b}
    [SerializeField]
    private VideoPlayer[] videoPlayer1; //루프, {ss, s, a, b}

    private VideoPlayer currentVideoPlayer0; //현재 선택 중
    private VideoPlayer currentVideoPlayer1;

    [SerializeField]
    private AudioSource audioSourceBGM; //브금
    [SerializeField]
    private AudioSource audioSourceSFX; //버튼 sfx
    [SerializeField]
    private Text songNameText; //노래 이름

    private bool isResult = true; //결과상태인지

    private string sceneName = "SongSelect"; //씬 이동할때 사용

    private float t = 0f; //시간

    //점수창
    [SerializeField]
    private Text maxComboText; //최대콤보
    [SerializeField]
    private Text[] comboCountText; //판정당 갯수 
    [SerializeField]
    private Text starText; //별
    [SerializeField]
    private Text arrowText; //화살
    [SerializeField]
    private Text counterText; //카운터
    [SerializeField]
    private Text defendText; //방어
    [SerializeField]
    private Text scoreText; //방어
    //경험치창
    [SerializeField]
    private Text expText; //현재 경험치
    [SerializeField]
    private Text expPlusText; //추가된 경험치

    private string id;
    private string songName;
    private string maxCombo;
    private string[] comboCount = new string[4] { "0", "0", "0", "0" }; //크리티컬, 오버, 히트, 미스
    private string star;
    private string arrow;
    private string counter;
    private string defend;
    private string score;

    //status exp, lv 처리관련
    private int exp;
    private int maxexp;
    private int lv;

    //exp에 추가되는 경험치
    public int expSS = 100;
    public int expS = 70;
    public int expA = 50;
    public int expB = 30;

    void Awake()
    {
        ConfirmStatus(); //exp 계산을 위해 호출
        SetResultUI();
        SetRank();
    }

    // Update is called once per frame
    void Update()
    {
        if (t < 1) //아래 조건만 사용하니 시작 몇프레임 동안 바로 동영상이 재생되지 않을 틈에 코딩이 들어가 비디오1이 바로 재생된다. 하여 시간 텀을 준다.
            t += Time.deltaTime;

        if (t >= 0.5) // +너무 빨리 실행됨
            if (!currentVideoPlayer0.isPlaying) //등장 후에 루프로 교체
                if (!currentVideoPlayer1.isPlaying)
                    currentVideoPlayer1.Play();
    }

    public void TouchToMain()//버튼을 눌러 돌아가기
    {
        if (isResult) //중복 클릭 막기
        {
            audioSourceSFX.Play();
            isResult = false;

            StartCoroutine(SceneLoad());
        }
    }

    private IEnumerator SceneLoad()//돌아가기
    {
        yield return new WaitForSeconds(0.5f);

        LoadingSceneManager.LoadScene(sceneName);
    }

    private void SetResultUI() // 결과창 세부 적용
    {
        ConfirmResult();
        songNameText.text = songName;
        maxComboText.text = maxCombo;
        comboCountText[0].text = comboCount[0];
        comboCountText[1].text = comboCount[1];
        comboCountText[2].text = comboCount[2];
        comboCountText[3].text = comboCount[3];
        starText.text = star;
        arrowText.text = arrow;
        counterText.text = counter;
        defendText.text = defend;
        scoreText.text = score;
    }

    private void SetRank() // 랭크 비디오 적용
    {
        //그냥 미스 갯수로 측정
        int tempMiss = int.Parse(comboCount[3]);

        //현재 임시로 관대
        if (tempMiss < 20) //SS
        {
            currentVideoPlayer0 = videoPlayer0[0];
            currentVideoPlayer1 = videoPlayer1[0];
            exp += expSS; //원래는 더 복잡해야겠지만 지금은 간단하게
            expPlusText.text = "(+" + expSS.ToString() + ")";
        }
        else if (tempMiss < 50) //S
        {
            currentVideoPlayer0 = videoPlayer0[1];
            currentVideoPlayer1 = videoPlayer1[1];
            exp += expS;
            expPlusText.text = "(+" + expS.ToString() + ")";
        }
        else if (tempMiss < 80) //A
        {
            currentVideoPlayer0 = videoPlayer0[2];
            currentVideoPlayer1 = videoPlayer1[2];
            exp += expA;
            expPlusText.text = "(+" + expA.ToString() + ")";
        }
        else // B
        {
            currentVideoPlayer0 = videoPlayer0[3];
            currentVideoPlayer1 = videoPlayer1[3];
            exp += expB;
            expPlusText.text = "(+" + expB.ToString() + ")";
        }

        currentVideoPlayer0.Play(); //뒷배경(현재 오류 중)

        expText.text = exp.ToString();

        SetEXP(); //경험치 부여 후 레벨 조정
        EXPUpdate(); //경험치 레벨 계산 후 업데이트
    }

    private void SetEXP()
    {
        while (exp >= maxexp) //exp가 maxexp보다 높을 경우 계속 레벨업
        {
            maxexp += 30 * lv;
            lv++;
        }
    }

    private void ConfirmResult() //서버에서 Result DB로부터 정보 받아오기
    {
        var bro = Backend.GameData.GetMyData("result", new Where(), 10);
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
            string tempSongName = bro.Rows()[i]["songName"]["S"].ToString();
            string tempMaxCombo = bro.Rows()[i]["maxCombo"]["S"].ToString();
            string tempCritical = bro.Rows()[i]["critical"]["S"].ToString();
            string tempOver = bro.Rows()[i]["over"]["S"].ToString();
            string tempHit = bro.Rows()[i]["hit"]["S"].ToString();
            string tempMiss = bro.Rows()[i]["miss"]["S"].ToString();
            string tempStar = bro.Rows()[i]["star"]["S"].ToString();
            string tempArrow = bro.Rows()[i]["arrow"]["S"].ToString();
            string tempCounter = bro.Rows()[i]["counter"]["S"].ToString();
            string tempDefend = bro.Rows()[i]["defend"]["S"].ToString();
            string tempTopScore = bro.Rows()[i]["topScore"]["S"].ToString();

            //데이터 저장
            id = tempID;
            songName = tempSongName;
            maxCombo = tempMaxCombo;
            comboCount[0] = tempCritical;
            comboCount[1] = tempOver;
            comboCount[2] = tempHit;
            comboCount[3] = tempMiss;
            star = tempStar;
            arrow = tempArrow;
            counter = tempCounter;
            defend = tempDefend;
            score = tempTopScore;
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
            string tempLV = bro.Rows()[i]["LV"]["S"].ToString();
            string tempEXP = bro.Rows()[i]["EXP"]["S"].ToString();
            string tempMaxEXP = bro.Rows()[i]["MaxEXP"]["S"].ToString();

            //데이터 저장
            lv = int.Parse(tempLV);
            exp = int.Parse(tempEXP);
            maxexp = int.Parse(tempMaxEXP);
        }
    }

    private void EXPUpdate() //설정 데이터 서버에 저장
    {
        Param param = new Param();
        param.Add("LV", lv.ToString());
        param.Add("EXP", exp.ToString());
        param.Add("MaxEXP", maxexp.ToString());

        Where where = new Where(); //id가 같은 곳에
        where.Equal("ID", id);

        Backend.GameData.Update("status", where, param); //update
    }

}
