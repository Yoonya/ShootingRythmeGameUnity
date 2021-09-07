using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; //dic 정렬에 사용
using BackEnd; //뒤끝 서버 연결

public class ResultRank : MonoBehaviour
{
    //점수창
    [SerializeField]
    private Text rankScoreText; //최대점수

    [SerializeField]
    private Text[] rankUserIDText; //유저랭크아이디
    [SerializeField]
    private Text[] rankUserScoreText; //유저랭크점수

    private int rankScore = -1;
    private int topScore = -1;
    private string id;
    private string songNumber;
    private string songName;

    //유저랭크를 관리를 위한 dic
    Dictionary<string, int> userRankDic = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        ConfirmResult(); //값을 받아와 데이터 초기화
        ConfirmRank();

        if (rankScore == -1) //데이터 값이 없는 경우
        {
            RankScoreInsert(); //즉, 데이터가 없으니 삽입
            ConfirmRank(); //다시
        }

        if (topScore > rankScore)
            rankScore = topScore;

        RankScoreUpdate(); //랭킹 값 업데이트

        ConfirmUserRank(); //유저랭킹을 받아와 dic에 저장
        SetUserRank(); //text에 설정

        rankScoreText.text = rankScore.ToString(); //UI출력
    }

    private void ConfirmResult() //서버에서 result DB로부터 정보 받아오기
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
            string tempSongNumber = bro.Rows()[i]["songNumber"]["S"].ToString(); //현재 선택 중인 곡을 알 수 있음
            string tempSongName = bro.Rows()[i]["songName"]["S"].ToString();
            string tempTopScore = bro.Rows()[i]["topScore"]["S"].ToString();

            //데이터 저장
            id = tempID;
            songName = tempSongName;
            songNumber = tempSongNumber;
            topScore = int.Parse(tempTopScore);
        }
    }

    private void ConfirmRank() //서버에서 rankscore DB로부터 정보 받아오기
    {
        //해당 곡 설정
        Where where = new Where(); //이 where은 and가 아니라 or이니 주의 -> ID를 설정하면 다 걸린다.
        where.Equal("songNumber", songNumber);

        var bro = Backend.GameData.GetMyData("rankScore", where, 10);
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
        // 검색한 데이터의 모든 row의 값 확인
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            string tempRankScore = bro.Rows()[i]["topScore"]["S"].ToString();

            //데이터 저장
            rankScore = int.Parse(tempRankScore);
        }
    }

    private void ConfirmUserRank() //서버통신
    {
        Where where = new Where();
        where.Equal("songNumber", songNumber);

        // 조건 없이 모든 데이터 조회하기
        var bro = Backend.GameData.Get("rankScore", where, 10);
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
        for (int i = 0; i < bro.Rows().Count; i++)
        {
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();
            string tempTopScore = bro.Rows()[i]["topScore"]["S"].ToString();

            userRankDic.Add(tempID, int.Parse(tempTopScore));
        }
    }

    private void RankScoreInsert() //최초 점수 삽입
    {
        Param param = new Param();
        param.Add("ID", id);
        param.Add("songNumber", songNumber);
        param.Add("songName", songName);
        param.Add("topScore", "0"); //default 되어있지만 가시성

        Backend.GameData.Insert("rankScore", param);
    }

    private void RankScoreUpdate() //점수 업데이트
    { 
        Param param = new Param();
        param.Add("topScore", rankScore.ToString());

        Where where = new Where(); //id와 songNumber가 같은 곳에
        where.Equal("songNumber", songNumber);

        Backend.GameData.Update("rankScore", where, param); //update
    }

    private void SetUserRank()
    {
        var userRankDesc = userRankDic.OrderByDescending(x => x.Value); //dic에 저장되어 있는 value 내림차순정렬

        int i = 0;
        foreach (var dic in userRankDesc)
        {
            rankUserIDText[i].text = dic.Key;
            rankUserScoreText[i].text = dic.Value.ToString();
            i++;
        }
    }

}
