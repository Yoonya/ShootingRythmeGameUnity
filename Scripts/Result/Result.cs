using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BackEnd; //�ڳ� ���� ����

public class Result : MonoBehaviour
{
    //���â
    [SerializeField]
    private VideoPlayer[] videoPlayer0; //����, {ss, s, a, b}
    [SerializeField]
    private VideoPlayer[] videoPlayer1; //����, {ss, s, a, b}

    private VideoPlayer currentVideoPlayer0; //���� ���� ��
    private VideoPlayer currentVideoPlayer1;

    [SerializeField]
    private AudioSource audioSourceBGM; //���
    [SerializeField]
    private AudioSource audioSourceSFX; //��ư sfx
    [SerializeField]
    private Text songNameText; //�뷡 �̸�

    private bool isResult = true; //�����������

    private string sceneName = "SongSelect"; //�� �̵��Ҷ� ���

    private float t = 0f; //�ð�

    //����â
    [SerializeField]
    private Text maxComboText; //�ִ��޺�
    [SerializeField]
    private Text[] comboCountText; //������ ���� 
    [SerializeField]
    private Text starText; //��
    [SerializeField]
    private Text arrowText; //ȭ��
    [SerializeField]
    private Text counterText; //ī����
    [SerializeField]
    private Text defendText; //���
    [SerializeField]
    private Text scoreText; //���
    //����ġâ
    [SerializeField]
    private Text expText; //���� ����ġ
    [SerializeField]
    private Text expPlusText; //�߰��� ����ġ

    private string id;
    private string songName;
    private string maxCombo;
    private string[] comboCount = new string[4] { "0", "0", "0", "0" }; //ũ��Ƽ��, ����, ��Ʈ, �̽�
    private string star;
    private string arrow;
    private string counter;
    private string defend;
    private string score;

    //status exp, lv ó������
    private int exp;
    private int maxexp;
    private int lv;

    //exp�� �߰��Ǵ� ����ġ
    public int expSS = 100;
    public int expS = 70;
    public int expA = 50;
    public int expB = 30;

    void Awake()
    {
        ConfirmStatus(); //exp ����� ���� ȣ��
        SetResultUI();
        SetRank();
    }

    // Update is called once per frame
    void Update()
    {
        if (t < 1) //�Ʒ� ���Ǹ� ����ϴ� ���� �������� ���� �ٷ� �������� ������� ���� ƴ�� �ڵ��� �� ����1�� �ٷ� ����ȴ�. �Ͽ� �ð� ���� �ش�.
            t += Time.deltaTime;

        if (t >= 0.5) // +�ʹ� ���� �����
            if (!currentVideoPlayer0.isPlaying) //���� �Ŀ� ������ ��ü
                if (!currentVideoPlayer1.isPlaying)
                    currentVideoPlayer1.Play();
    }

    public void TouchToMain()//��ư�� ���� ���ư���
    {
        if (isResult) //�ߺ� Ŭ�� ����
        {
            audioSourceSFX.Play();
            isResult = false;

            StartCoroutine(SceneLoad());
        }
    }

    private IEnumerator SceneLoad()//���ư���
    {
        yield return new WaitForSeconds(0.5f);

        LoadingSceneManager.LoadScene(sceneName);
    }

    private void SetResultUI() // ���â ���� ����
    {
        ConfirmResult();
        songNameText.text = songName;
        maxComboText.text = maxCombo;
        comboCountText[0].text = comboCount[0];
        comboCountText[1].text = comboCount[1];
        comboCountText[2].text = comboCount[2];
        comboCountText[3].text = comboCount[3];
        starText.text = star;
        arrowText.text = arrow;
        counterText.text = counter;
        defendText.text = defend;
        scoreText.text = score;
    }

    private void SetRank() // ��ũ ���� ����
    {
        //�׳� �̽� ������ ����
        int tempMiss = int.Parse(comboCount[3]);

        //���� �ӽ÷� ����
        if (tempMiss < 20) //SS
        {
            currentVideoPlayer0 = videoPlayer0[0];
            currentVideoPlayer1 = videoPlayer1[0];
            exp += expSS; //������ �� �����ؾ߰����� ������ �����ϰ�
            expPlusText.text = "(+" + expSS.ToString() + ")";
        }
        else if (tempMiss < 50) //S
        {
            currentVideoPlayer0 = videoPlayer0[1];
            currentVideoPlayer1 = videoPlayer1[1];
            exp += expS;
            expPlusText.text = "(+" + expS.ToString() + ")";
        }
        else if (tempMiss < 80) //A
        {
            currentVideoPlayer0 = videoPlayer0[2];
            currentVideoPlayer1 = videoPlayer1[2];
            exp += expA;
            expPlusText.text = "(+" + expA.ToString() + ")";
        }
        else // B
        {
            currentVideoPlayer0 = videoPlayer0[3];
            currentVideoPlayer1 = videoPlayer1[3];
            exp += expB;
            expPlusText.text = "(+" + expB.ToString() + ")";
        }

        currentVideoPlayer0.Play(); //�޹��(���� ���� ��)

        expText.text = exp.ToString();

        SetEXP(); //����ġ �ο� �� ���� ����
        EXPUpdate(); //����ġ ���� ��� �� ������Ʈ
    }

    private void SetEXP()
    {
        while (exp >= maxexp) //exp�� maxexp���� ���� ��� ��� ������
        {
            maxexp += 30 * lv;
            lv++;
        }
    }

    private void ConfirmResult() //�������� Result DB�κ��� ���� �޾ƿ���
    {
        var bro = Backend.GameData.GetMyData("result", new Where(), 10);
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
            string tempSongName = bro.Rows()[i]["songName"]["S"].ToString();
            string tempMaxCombo = bro.Rows()[i]["maxCombo"]["S"].ToString();
            string tempCritical = bro.Rows()[i]["critical"]["S"].ToString();
            string tempOver = bro.Rows()[i]["over"]["S"].ToString();
            string tempHit = bro.Rows()[i]["hit"]["S"].ToString();
            string tempMiss = bro.Rows()[i]["miss"]["S"].ToString();
            string tempStar = bro.Rows()[i]["star"]["S"].ToString();
            string tempArrow = bro.Rows()[i]["arrow"]["S"].ToString();
            string tempCounter = bro.Rows()[i]["counter"]["S"].ToString();
            string tempDefend = bro.Rows()[i]["defend"]["S"].ToString();
            string tempTopScore = bro.Rows()[i]["topScore"]["S"].ToString();

            //������ ����
            id = tempID;
            songName = tempSongName;
            maxCombo = tempMaxCombo;
            comboCount[0] = tempCritical;
            comboCount[1] = tempOver;
            comboCount[2] = tempHit;
            comboCount[3] = tempMiss;
            star = tempStar;
            arrow = tempArrow;
            counter = tempCounter;
            defend = tempDefend;
            score = tempTopScore;
        }
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
            string tempLV = bro.Rows()[i]["LV"]["S"].ToString();
            string tempEXP = bro.Rows()[i]["EXP"]["S"].ToString();
            string tempMaxEXP = bro.Rows()[i]["MaxEXP"]["S"].ToString();

            //������ ����
            lv = int.Parse(tempLV);
            exp = int.Parse(tempEXP);
            maxexp = int.Parse(tempMaxEXP);
        }
    }

    private void EXPUpdate() //���� ������ ������ ����
    {
        Param param = new Param();
        param.Add("LV", lv.ToString());
        param.Add("EXP", exp.ToString());
        param.Add("MaxEXP", maxexp.ToString());

        Where where = new Where(); //id�� ���� ����
        where.Equal("ID", id);

        Backend.GameData.Update("status", where, param); //update
    }

}
