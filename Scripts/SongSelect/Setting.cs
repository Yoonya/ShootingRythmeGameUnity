using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd; //뒤끝 서버 연결

public class Setting : MonoBehaviour
{   
    //환경설정 창 처리
    [SerializeField] private Text idTxT; //아이디 쪽
    [SerializeField] private Text lvTxT; //레벨 쪽
    [SerializeField] private Text speedTxT; //스피드
    [SerializeField] private Text syncTxT; //싱크
    [SerializeField] private Text expTxT; //스피드
    [SerializeField] private Text maxexpTxT; //싱크

    //기본 초기화
    public string id = "Player";
    public int lv = 0;
    public float speed = 2.0f;
    public float sync = 1.0f;
    public int exp = 0;
    public int maxexp = 30;

    //스피드와 싱크의 최소 최대 수치
    private float maxSpeed = 4.0f;
    private float minSpeed = 1.0f;
    private float maxSync = 2.0f;
    private float minSync = -2.0f;

    private SettingBtn settingBtn;

    void Start()
    {
        settingBtn = FindObjectOfType<SettingBtn>();
        ConfirmStatus();
       
        idTxT.text = id;
        lvTxT.text = lv.ToString();
        speedTxT.text = speed.ToString("F1"); //소수점표시
        syncTxT.text = sync.ToString("F1");
        expTxT.text = string.Format("{0:#,###}", exp).ToString(); //1000단위 끊기
        maxexpTxT.text = string.Format("{0:#,###}", maxexp).ToString();
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
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();
            string tempLV = bro.Rows()[i]["LV"]["S"].ToString();
            string tempSpeed = bro.Rows()[i]["speed"]["S"].ToString();
            string tempSync = bro.Rows()[i]["sync"]["S"].ToString();
            string tempEXP = bro.Rows()[i]["EXP"]["S"].ToString();
            string tempMaxEXP = bro.Rows()[i]["MaxEXP"]["S"].ToString();

            //데이터 저장
            id = tempID;
            lv = int.Parse(tempLV);
            speed = float.Parse(tempSpeed);
            sync = float.Parse(tempSync);
            exp = int.Parse(tempEXP);
            maxexp = int.Parse(tempMaxEXP);
        }
    }

    public void BtnSpeedUp()//스피드쪽 up버튼
    {
        speed += 0.5f;
        speed = Mathf.Round(speed * 10) * 0.1f; //가끔 0.99999가된다.

        if (speed > maxSpeed)
            speed = maxSpeed;

        speedTxT.text = speed.ToString("F1");
        AudioManager.instance.PlaySFX("Button");
    }

    public void BtnSpeedDown()//스피드쪽 down버튼
    {
        speed -= 0.5f;
        speed = Mathf.Round(speed * 10) * 0.1f; //가끔 0.99999가된다.

        if (speed < minSpeed)
            speed = minSpeed;

        speedTxT.text = speed.ToString("F1");
        AudioManager.instance.PlaySFX("Button");
    }

    public void BtnSyncUp()//싱크쪽 up버튼
    {
        sync += 0.1f;
        sync = Mathf.Round(sync * 10) * 0.1f; //가끔 0.99999가된다.

        if (sync > maxSync)
            sync = maxSync;

        syncTxT.text = sync.ToString("F1");
        AudioManager.instance.PlaySFX("Button");
    }

    public void BtnSyncDown()//스피드쪽 down버튼
    {
        sync -= 0.1f;
        sync = Mathf.Round(sync * 10) * 0.1f; //가끔 0.99999가된다.

        if (sync < minSync)
            sync = minSync;

        syncTxT.text = sync.ToString("F1");
        AudioManager.instance.PlaySFX("Button");
    }

    public void CloseSetting() //close버튼 클릭 이벤트
    {
        settingBtn.IsSetting = false;
        CloseSettingUpdate();
        this.gameObject.SetActive(false);
        AudioManager.instance.PlaySFX("Button");
    }

    private void CloseSettingUpdate() //설정 데이터 서버에 저장
    {
        Param param = new Param();
        param.Add("speed", speed.ToString("F1"));
        param.Add("sync", sync.ToString("F1"));

        Where where = new Where(); //id가 같은 곳에
        where.Equal("ID", id);

        Backend.GameData.Update("status", where, param); //update
    }
}
