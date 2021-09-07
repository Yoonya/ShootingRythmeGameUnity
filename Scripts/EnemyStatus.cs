using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatus : MonoBehaviour
{
    //���� ���� ����
    [SerializeField]
    private Image imgHP; //���� ü�¹�
    [SerializeField] 
    private Text txtLv; //����
    [SerializeField]
    private Text txtName; //�̸�
    [SerializeField]
    private Text txtOverDamage; //ü���� ���� �� �ް� �߰� ����
    [SerializeField]
    private Text txtOverDamageScore; //ü���� ���� �� �ް� �߰� ����

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

        //�ʱ�ȭ
        if (currentSongNumber == 0) //������ �������ְ�, ���� �� ������ ���� ������ �̷��� ����
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

    private void UpdateHP() //ü�°���
    {
        imgHP.fillAmount = (float)currentHP / maxHP;
        if (overDamage > 0)//���� �������� �־��� ��
        {
            txtOverDamageScore.text = overDamage.ToString() + "%";
            txtOverDamageScore.enabled = true;
            txtOverDamage.enabled = true;
        }
    }

    public void DecreaseHP(int damage) //���� ��� �ѱ�
    {
        if (currentHP > damage)
        {
            currentHP -= damage;
            score.SetScore(damage, 0f);
        }
        else if(currentHP <= damage && currentHP != 0) //���� 0���� ���� �Ŀ� ���������� ���
        {
            currentHP = 0;
            score.SetScore(damage, 0f);
        }  
        else if(currentHP == 0)
        {
            overDamage += (float)damage / maxHP * 100f; //�ִ�ü���� ����ؼ� ���
            
            overDamage = Mathf.Floor(overDamage * 10) / 10; //�Ҽ��� ���ڸ��������
            score.SetScore(damage, overDamage);
        }
                
    }

    public void IncreaseHP(int heal) //ü�� ���� �޼���
    {
        if (currentHP + heal > maxHP)
            currentHP = maxHP;
        else
            currentHP += heal;
    }

    /*
    private void ConfirmSong() //�������� song DB�κ��� ���� �޾ƿ���, ���⼭�� songNumber���� �ʿ�
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
            string tempSongNumber = bro.Rows()[i]["songNumber"]["S"].ToString();

            //������ ����
            currentSongNumber = int.Parse(tempSongNumber);
        }
    }
    */
}
