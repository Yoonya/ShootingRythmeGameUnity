using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd; //�ڳ� ���� ����

public class Setting : MonoBehaviour
{   
    //ȯ�漳�� â ó��
    [SerializeField] private Text idTxT; //���̵� ��
    [SerializeField] private Text lvTxT; //���� ��
    [SerializeField] private Text speedTxT; //���ǵ�
    [SerializeField] private Text syncTxT; //��ũ
    [SerializeField] private Text expTxT; //���ǵ�
    [SerializeField] private Text maxexpTxT; //��ũ

    //�⺻ �ʱ�ȭ
    public string id = "Player";
    public int lv = 0;
    public float speed = 2.0f;
    public float sync = 1.0f;
    public int exp = 0;
    public int maxexp = 30;

    //���ǵ�� ��ũ�� �ּ� �ִ� ��ġ
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
        speedTxT.text = speed.ToString("F1"); //�Ҽ���ǥ��
        syncTxT.text = sync.ToString("F1");
        expTxT.text = string.Format("{0:#,###}", exp).ToString(); //1000���� ����
        maxexpTxT.text = string.Format("{0:#,###}", maxexp).ToString();
    }

    private void ConfirmStatus() //�������� status DB�κ��� ���� �޾ƿ���
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
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();
            string tempLV = bro.Rows()[i]["LV"]["S"].ToString();
            string tempSpeed = bro.Rows()[i]["speed"]["S"].ToString();
            string tempSync = bro.Rows()[i]["sync"]["S"].ToString();
            string tempEXP = bro.Rows()[i]["EXP"]["S"].ToString();
            string tempMaxEXP = bro.Rows()[i]["MaxEXP"]["S"].ToString();

            //������ ����
            id = tempID;
            lv = int.Parse(tempLV);
            speed = float.Parse(tempSpeed);
            sync = float.Parse(tempSync);
            exp = int.Parse(tempEXP);
            maxexp = int.Parse(tempMaxEXP);
        }
    }

    public void BtnSpeedUp()//���ǵ��� up��ư
    {
        speed += 0.5f;
        speed = Mathf.Round(speed * 10) * 0.1f; //���� 0.99999���ȴ�.

        if (speed > maxSpeed)
            speed = maxSpeed;

        speedTxT.text = speed.ToString("F1");
        AudioManager.instance.PlaySFX("Button");
    }

    public void BtnSpeedDown()//���ǵ��� down��ư
    {
        speed -= 0.5f;
        speed = Mathf.Round(speed * 10) * 0.1f; //���� 0.99999���ȴ�.

        if (speed < minSpeed)
            speed = minSpeed;

        speedTxT.text = speed.ToString("F1");
        AudioManager.instance.PlaySFX("Button");
    }

    public void BtnSyncUp()//��ũ�� up��ư
    {
        sync += 0.1f;
        sync = Mathf.Round(sync * 10) * 0.1f; //���� 0.99999���ȴ�.

        if (sync > maxSync)
            sync = maxSync;

        syncTxT.text = sync.ToString("F1");
        AudioManager.instance.PlaySFX("Button");
    }

    public void BtnSyncDown()//���ǵ��� down��ư
    {
        sync -= 0.1f;
        sync = Mathf.Round(sync * 10) * 0.1f; //���� 0.99999���ȴ�.

        if (sync < minSync)
            sync = minSync;

        syncTxT.text = sync.ToString("F1");
        AudioManager.instance.PlaySFX("Button");
    }

    public void CloseSetting() //close��ư Ŭ�� �̺�Ʈ
    {
        settingBtn.IsSetting = false;
        CloseSettingUpdate();
        this.gameObject.SetActive(false);
        AudioManager.instance.PlaySFX("Button");
    }

    private void CloseSettingUpdate() //���� ������ ������ ����
    {
        Param param = new Param();
        param.Add("speed", speed.ToString("F1"));
        param.Add("sync", sync.ToString("F1"));

        Where where = new Where(); //id�� ���� ����
        where.Equal("ID", id);

        Backend.GameData.Update("status", where, param); //update
    }
}
