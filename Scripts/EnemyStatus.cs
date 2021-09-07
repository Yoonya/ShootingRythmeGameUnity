using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatus : MonoBehaviour
{
    //적의 상태 정보
    [SerializeField]
    private Image imgHP; //빨간 체력바
    [SerializeField] 
    private Text txtLv; //레벨
    [SerializeField]
    private Text txtName; //이름
    [SerializeField]
    private Text txtOverDamage; //체력이 전부 다 달고 추가 점수
    [SerializeField]
    private Text txtOverDamageScore; //체력이 전부 다 달고 추가 점수

    [SerializeField]
    public int enemyLv = 1;
    public string enemyName;
    private int maxHP;
    private int currentHP;
    private float overDamage = 0f;
    public int currentSongNumber;

    private Score score;

    // Start is called before the first frame update
    void Start()
    {
        score = FindObjectOfType<Score>();

        //초기화
        if (currentSongNumber == 0) //적마다 정해져있고, 현재 곡 수량이 적기 때문에 이렇게 관리
        {
            maxHP = 400;
            enemyLv = 1;
        }
        else if (currentSongNumber == 1)
        {
            maxHP = 600;
            enemyLv = 2;
        }
        else if (currentSongNumber == 2)
        {
            maxHP = 800;
            enemyLv = 3;
        }
        else
        {
            maxHP = 400;
            enemyLv = 1;
        }

        currentHP = maxHP;
        txtLv.text = "Lv." + enemyLv.ToString();
        txtName.text = enemyName;
        txtOverDamageScore.text = overDamage.ToString() + "%";
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHP();
    }

    private void UpdateHP() //체력관리
    {
        imgHP.fillAmount = (float)currentHP / maxHP;
        if (overDamage > 0)//오버 데미지로 넣었을 때
        {
            txtOverDamageScore.text = overDamage.ToString() + "%";
            txtOverDamageScore.enabled = true;
            txtOverDamage.enabled = true;
        }
    }

    public void DecreaseHP(int damage) //점수 계산 넘김
    {
        if (currentHP > damage)
        {
            currentHP -= damage;
            score.SetScore(damage, 0f);
        }
        else if(currentHP <= damage && currentHP != 0) //먼저 0으로 만든 후에 오버데미지 계산
        {
            currentHP = 0;
            score.SetScore(damage, 0f);
        }  
        else if(currentHP == 0)
        {
            overDamage += (float)damage / maxHP * 100f; //최대체력의 비례해서 계산
            
            overDamage = Mathf.Floor(overDamage * 10) / 10; //소수점 한자리까지계산
            score.SetScore(damage, overDamage);
        }
                
    }

    public void IncreaseHP(int heal) //체력 증가 메서드
    {
        if (currentHP + heal > maxHP)
            currentHP = maxHP;
        else
            currentHP += heal;
    }

    /*
    private void ConfirmSong() //서버에서 song DB로부터 정보 받아오기, 여기서는 songNumber값만 필요
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
            string tempSongNumber = bro.Rows()[i]["songNumber"]["S"].ToString();

            //데이터 저장
            currentSongNumber = int.Parse(tempSongNumber);
        }
    }
    */
}
