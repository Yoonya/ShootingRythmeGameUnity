using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd; //�ڳ� ���� ����

public class PlayerStatus : MonoBehaviour
{
    //ĳ���� ��
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
        ConfirmStatus(); //�÷��̾� ���� �޾ƿ���
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
            int tempDamage = playerLv - enemyStatus.enemyLv; //���������
            if (tempDamage < 0) tempDamage = 0; //�������� �ʹ� �����ų� ������ �Ǽ� ������ �̻��ϰ� �����ʰ�(�ӽ�)
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

    public void DecreaseHP(int damage) //ü�� ����
    {
        if (currentHP > damage)
            currentHP -= damage;
        else
        {
            currentHP = 0;
        }
            
    }

    public void IncreaseHP(int heal) //ü�� ����
    {
        if (currentHP + heal > maxHP)
            currentHP = maxHP;
        else
            currentHP += heal;
    }

    private void ConfirmStatus() //�������� status DB�κ��� ���� �޾ƿ���, ���⼭�� lv���� �ʿ�
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
            string tempLV = bro.Rows()[i]["LV"]["S"].ToString();
            string tempName = bro.Rows()[i]["ID"]["S"].ToString();

            //������ ����
            playerLv = int.Parse(tempLV);
            playerName = tempName;
        }
    }

}
