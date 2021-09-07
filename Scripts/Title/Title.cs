using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd; //뒤끝 서버 연결

public class Title : MonoBehaviour //메인타이틀
{
    [SerializeField]
    private VideoPlayer videoPlayer0; //메인타이틀
    [SerializeField]
    private VideoPlayer videoPlayer1; //타이틀 입장
    [SerializeField]
    private AudioSource audioSourceBGM; //브금
    [SerializeField]
    private AudioSource audioSourceSFX; //버튼 sfx

    [SerializeField]
    private GameObject loginUI; //로그인창
    [SerializeField]
    private InputField id = null; //아이디 인풋
    [SerializeField]
    private InputField pwd = null;//비밀번호 인풋

    private bool isMain = true;

    public string sceneName = "SongSelect"; //다음으로 넘어갈 씬

    private void Start()
    {
        Backend.Initialize(InitializeCallback); //통신 초기화
    }

    void Update()
    {
        if (!isMain && videoPlayer1.isPlaying && audioSourceBGM.volume != 0f) //씬 이동에 맞춰 볼륨이 줄어들게
        {
            audioSourceBGM.volume -= 0.01f;
            if (audioSourceBGM.volume <= 0.005f)
                audioSourceBGM.volume = 0f;
        }
    }

    void InitializeCallback()
    {
        if (Backend.IsInitialized)
        {
            Debug.Log(Backend.Utils.GetServerTime());//현재 시간
            Debug.Log(Backend.Utils.GetGoogleHash());//뒤끝 서버 통신에 필요한 구글 해시키
        }
        else
            Debug.Log("초기화 실패");
    }

    public void TouchToPlay() //클릭으로 로그인 창 불러오기
    {
        if (isMain) //중복 클릭 막기
        {
            audioSourceSFX.Play();
            isMain = false;

            loginUI.SetActive(true);         
        }
    }

    private IEnumerator SceneLoad()
    {
        videoPlayer1.Play(); //비디오 체인지
        videoPlayer0.Stop();
        audioSourceSFX.Play();
        yield return new WaitForSeconds(1.5f); //비디오 재생 끝나는거 보고 이동

        LoadingSceneManager.LoadScene("SongSelect");
    }

    public void BtnSignUp()//회원가입
    {
        string tempID = id.text;
        string tempPWD = pwd.text;

        BackendReturnObject bro = Backend.BMember.CustomSignUp(tempID, tempPWD);//회원가입 호출

        if (bro.IsSuccess()) //회원가입 성공 메서드
        {
            Debug.Log("회원가입 성공");
            loginUI.SetActive(false);
            SignUpInsert(tempID);
            StartCoroutine(SceneLoad());
        }
        else
        {
            Debug.Log("회원가입 실패");
        }
    }

    public void BtnLogin()
    {
        string tempID = id.text;
        string tempPWD = pwd.text;

        BackendReturnObject bro = Backend.BMember.CustomLogin(tempID, tempPWD);//로그인 호출

        if (bro.IsSuccess()) //로그인 성공 메서드
        {
            Debug.Log("로그인 성공");
            loginUI.SetActive(false);
            StartCoroutine(SceneLoad());
        }
        else
        {
            Debug.Log("로그인 실패");
        }
    }

    private void SignUpInsert(string id) //최초 회원가입
    {
        //rankScore DB와 다르게 이들은 1컬럼으로 계속해서 update만 사용하기 때문에 여기서 미리 생성해둔다.
        
        //전부 null비허용 default 값 설정했기 때문에 값 설정할 id만 param에 저장해둠        
        Param param = new Param();
        param.Add("ID", id);

        //유저의 정보 및 환경설정 정보 defalut컬럼 생성
        Backend.GameData.Insert("status", param);
        //선택한 곡 정보 defalut컬럼 생성
        Backend.GameData.Insert("song", param);
        //결과창 defalut컬럼 생성
        Backend.GameData.Insert("result", param);
    }

}
