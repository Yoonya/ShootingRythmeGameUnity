using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd; //뒤끝 서버 연결

public class PlayerStatus : MonoBehaviour
{
    //캐릭터 상세
    [SerializeField]
    private Image imgHP;
    [SerializeField]
    private Text txtLv;
    [SerializeField]
    private Text txtName;

    [SerializeField]
    public int playerLv = 1;
    private string playerName;
    private int maxHP;
    public int currentHP;
    public int[] hitDamage = new int[3];

    private EnemyStatus enemyStatus;

    private void Awake()
    {
        ConfirmStatus(); //플레이어 레벨 받아오기
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyStatus = FindObjectOfType<EnemyStatus>();

        maxHP = 5 + playerLv * 5;
        currentHP = maxHP;
        txtLv.text = "Lv." + playerLv.ToString();
        txtName.text = playerName;
        for (int i = 0; i < hitDamage.Length; i++)
        {
            int tempDamage = playerLv - enemyStatus.enemyLv; //데미지계산
            if (tempDamage < 0) tempDamage = 0; //데미지가 너무 세지거나 음수가 되서 점수가 이상하게 되지않게(임시)
            if (tempDamage > 3) tempDamage = 3;
            hitDamage[i] = tempDamage + i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHP();
    }

    private void UpdateHP()
    {
        imgHP.fillAmount = (float)currentHP / maxHP;
    }

    public void DecreaseHP(int damage) //체력 감소
    {
        if (currentHP > damage)
            currentHP -= damage;
        else
        {
            currentHP = 0;
        }
            
    }

    public void IncreaseHP(int heal) //체력 증가
    {
        if (currentHP + heal > maxHP)
            currentHP = maxHP;
        else
            currentHP += heal;
    }

    private void ConfirmStatus() //서버에서 status DB로부터 정보 받아오기, 여기서는 lv값만 필요
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
            string tempName = bro.Rows()[i]["ID"]["S"].ToString();

            //데이터 저장
            playerLv = int.Parse(tempLV);
            playerName = tempName;
        }
    }

}
