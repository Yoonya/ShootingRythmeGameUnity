using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using BackEnd; //뒤끝 서버 연결

[System.Serializable]
public class Song //곡 정보
{
    public int number; //고유숙자
    public string name; //이름
    public Sprite sprite; //이미지
    public AudioClip audioClip; //미리듣기 곡
    public Song(int number, string name, Sprite sprite, AudioClip audioClip)
    {
        this.number = number;
        this.name = name;
        this.sprite = sprite;
        this.audioClip = audioClip;
    }
}

public class SongSelect : MonoBehaviour
{
    [SerializeField] private Song[] songList; //곡 클래스 리스트
    [SerializeField] private Image[] songImages; //왼, 오, 중간 이미지
    [SerializeField] public Text currentSongText; //제목 텍스트
    [SerializeField] public Text currentTopScoreText; //최고점수 텍스트
    private int[] rankScore = new int[3]; //자신의 곡 최고점수, 3은 현재 곡 갯수->계속 업데이트

    [SerializeField] private AudioSource audioSource; //배경음악
    [SerializeField] private VideoPlayer videoPlayer; //뒷배경

    private string id;

    private SettingBtn settingBtn;

    public int currentNumber = 0; //현재 선택되고 있는 인덱스 

    void Start()
    {
        settingBtn = FindObjectOfType<SettingBtn>();

        for (int i = 0; i < songList.Length; i++) //최초 초기화
        {
            rankScore[i] = 0;
        }
        ConfirmRankScore(); //곡 선택에서 자신의 최고점수 출력을 위해 정보 받아오기
        currentTopScoreText.text = rankScore[0].ToString(); //최고점수 텍스트

        //최초 초기화
        currentSongText.text = songList[0].name;
        songImages[0].sprite = songList[0].sprite; //가운데
        songImages[1].sprite = songList[1].sprite; //오른쪽
        songImages[2].sprite = songList[songList.Length-1].sprite; //왼쪽
        audioSource.clip = songList[0].audioClip; //현재 곡 설정
        audioSource.Play(); //미리듣기 곡
        videoPlayer.Play(); //배경 비디오
    }

    public void BtnRightArrow() //오른쪽 화살표버튼
    {
        if (!settingBtn.IsSetting) //환경설정 창이 꺼져 있는 상태에서만
        {
            currentNumber++;
            if (currentNumber > songList.Length - 1)
                currentNumber = 0;

            currentSongText.text = songList[currentNumber].name;
            songImages[0].sprite = songList[currentNumber].sprite;
            currentTopScoreText.text = rankScore[currentNumber].ToString(); //최고점수 텍스트

            if (currentNumber == songList.Length - 1)
                songImages[1].sprite = songList[0].sprite;
            else
                songImages[1].sprite = songList[currentNumber + 1].sprite;

            if (currentNumber == 0)
                songImages[2].sprite = songList[songList.Length - 1].sprite;
            else
                songImages[2].sprite = songList[currentNumber - 1].sprite;

            audioSource.clip = songList[currentNumber].audioClip; //미리듣기 재설정
            audioSource.Play();
            AudioManager.instance.PlaySFX("Slide");
        }
    }

    public void BtnLeftArrow() //왼쪽 화살표 버튼
    {
        if (!settingBtn.IsSetting) //환경설정 창이 꺼져 있는 상태에서만
        {
            currentNumber--;
            if (currentNumber < 0)
                currentNumber = songList.Length - 1;

            currentSongText.text = songList[currentNumber].name;
            songImages[0].sprite = songList[currentNumber].sprite;
            currentTopScoreText.text = rankScore[currentNumber].ToString(); //최고점수 텍스트

            if (currentNumber == songList.Length - 1)
                songImages[1].sprite = songList[0].sprite;
            else
                songImages[1].sprite = songList[currentNumber + 1].sprite;

            if (currentNumber == 0)
                songImages[2].sprite = songList[songList.Length - 1].sprite;
            else
                songImages[2].sprite = songList[currentNumber - 1].sprite;

            audioSource.clip = songList[currentNumber].audioClip; //미리듣기 재설정
            audioSource.Play();
            AudioManager.instance.PlaySFX("Slide");
        }
    }

    public void BtnSongSelect()//곡 선택 버튼 이벤트
    {
        if (!settingBtn.IsSetting) //환경설정 창이 꺼져 있는 상태에서만
        {
            BtnSongSelectUpdate();
            AudioManager.instance.PlaySFX("Button");
            StartCoroutine(SceneLoad());
        }
    }

    private void BtnSongSelectUpdate() //선택한 곡 정보 데이터 서버에 저장
    {
        ConfirmStatus();

        Param param = new Param();
        param.Add("songName", songList[currentNumber].name);
        param.Add("songNumber", currentNumber.ToString());

        Where where = new Where(); //id가 같은 곳에
        where.Equal("ID", id);

        Backend.GameData.Update("song", where, param); //update
    }

    private void ConfirmStatus() //서버에서 status DB로부터 정보 받아오기, 여기서는 id값만 필요
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
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();

            //데이터 저장
            id = tempID;
        }
    }

    private void ConfirmRankScore() //서버에서 rankScore DB로부터 정보 받아오기
    {
        var bro = Backend.GameData.GetMyData("rankScore", new Where(), 10);
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
            string tempSongNumber = bro.Rows()[i]["songNumber"]["S"].ToString();
            string tempTopScore = bro.Rows()[i]["topScore"]["S"].ToString();

            //데이터 저장
            rankScore[int.Parse(tempSongNumber)] = int.Parse(tempTopScore);
        }
    }

    private IEnumerator SceneLoad() //게임창 불러오기
    {
        yield return new WaitForSeconds(1f);

        LoadingSceneManager.LoadScene("Game");
        gameObject.GetComponentInChildren<VideoPlayer>().enabled = false; //파괴되지 않고 가져가기 때문에 비활성화 임시
        gameObject.GetComponentInChildren<AudioSource>().enabled = false;
        gameObject.GetComponentInChildren<Text>().enabled = false;
    }

    public void SceneBack()
    {
        //다른 곳에서 돌아올 경우
        gameObject.GetComponentInChildren<VideoPlayer>().enabled = true; //파괴되지 않고 가져가기 때문에 활성화  임시
        gameObject.GetComponentInChildren<AudioSource>().enabled = true;
        gameObject.GetComponentInChildren<Text>().enabled = true;
        audioSource.Play();
        videoPlayer.Play();
    }

}
