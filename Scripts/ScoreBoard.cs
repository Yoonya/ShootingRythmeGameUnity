using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd; //뒤끝 서버 연결

public class ScoreBoard : MonoBehaviour
{
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

    private int maxCombo = 0;
    private int[] comboCount = new int[4] { 0, 0, 0, 0 }; //크리티컬, 오버, 히트, 미스
    private int star = 0;
    private int arrow = 0;
    private int counter = 0;
    private int defend = 0;

    private Score score;

    private void Start()
    {
        score = FindObjectOfType<Score>();
    }

    //점수판 설정
    public void SetMaxCombo(int combo)
    {
        maxCombo = combo;
        maxComboText.text = maxCombo.ToString();
    }

    public void SetComboCount(int judge)
    {
        comboCount[judge]++;
        comboCountText[judge].text = comboCount[judge].ToString();
    }

    public void SetStar()
    {
        star++;
        starText.text = star.ToString();
    }

    public void SetArrow()
    {
        arrow++;
        arrowText.text = arrow.ToString();
    }

    public void SetCounter()
    {
        counter++;
        counterText.text = counter.ToString();
    }

    public void SetDefend()
    {
        defend++;
        defendText.text = defend.ToString();
    }

    public void ResultUpdate(string id, string songNumber, string songName) //설정 데이터 서버에 저장
    {
        Param param = new Param();
        param.Add("songNumber", songNumber);
        param.Add("songName", songName);
        param.Add("maxCombo", maxCombo.ToString());
        param.Add("critical", comboCount[0].ToString());
        param.Add("over", comboCount[1].ToString());
        param.Add("hit", comboCount[2].ToString());
        param.Add("miss", comboCount[3].ToString());
        param.Add("star", star.ToString());
        param.Add("arrow", arrow.ToString());
        param.Add("counter", counter.ToString());
        param.Add("defend", defend.ToString());
        param.Add("topScore", score.scorePoint.ToString());

        Where where = new Where(); //id가 같은 곳에
        where.Equal("ID", id);

        Backend.GameData.Update("result", where, param); //update
    }
}
