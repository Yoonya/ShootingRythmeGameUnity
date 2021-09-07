using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; //dic ���Ŀ� ���
using BackEnd; //�ڳ� ���� ����

public class ResultRank : MonoBehaviour
{
    //����â
    [SerializeField]
    private Text rankScoreText; //�ִ�����

    [SerializeField]
    private Text[] rankUserIDText; //������ũ���̵�
    [SerializeField]
    private Text[] rankUserScoreText; //������ũ����

    private int rankScore = -1;
    private int topScore = -1;
    private string id;
    private string songNumber;
    private string songName;

    //������ũ�� ������ ���� dic
    Dictionary<string, int> userRankDic = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        ConfirmResult(); //���� �޾ƿ� ������ �ʱ�ȭ
        ConfirmRank();

        if (rankScore == -1) //������ ���� ���� ���
        {
            RankScoreInsert(); //��, �����Ͱ� ������ ����
            ConfirmRank(); //�ٽ�
        }

        if (topScore > rankScore)
            rankScore = topScore;

        RankScoreUpdate(); //��ŷ �� ������Ʈ

        ConfirmUserRank(); //������ŷ�� �޾ƿ� dic�� ����
        SetUserRank(); //text�� ����

        rankScoreText.text = rankScore.ToString(); //UI���
    }

    private void ConfirmResult() //�������� result DB�κ��� ���� �޾ƿ���
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
            string tempSongNumber = bro.Rows()[i]["songNumber"]["S"].ToString(); //���� ���� ���� ���� �� �� ����
            string tempSongName = bro.Rows()[i]["songName"]["S"].ToString();
            string tempTopScore = bro.Rows()[i]["topScore"]["S"].ToString();

            //������ ����
            id = tempID;
            songName = tempSongName;
            songNumber = tempSongNumber;
            topScore = int.Parse(tempTopScore);
        }
    }

    private void ConfirmRank() //�������� rankscore DB�κ��� ���� �޾ƿ���
    {
        //�ش� �� ����
        Where where = new Where(); //�� where�� and�� �ƴ϶� or�̴� ���� -> ID�� �����ϸ� �� �ɸ���.
        where.Equal("songNumber", songNumber);

        var bro = Backend.GameData.GetMyData("rankScore", where, 10);
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
        // �˻��� �������� ��� row�� �� Ȯ��
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            string tempRankScore = bro.Rows()[i]["topScore"]["S"].ToString();

            //������ ����
            rankScore = int.Parse(tempRankScore);
        }
    }

    private void ConfirmUserRank() //�������
    {
        Where where = new Where();
        where.Equal("songNumber", songNumber);

        // ���� ���� ��� ������ ��ȸ�ϱ�
        var bro = Backend.GameData.Get("rankScore", where, 10);
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
        for (int i = 0; i < bro.Rows().Count; i++)
        {
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();
            string tempTopScore = bro.Rows()[i]["topScore"]["S"].ToString();

            userRankDic.Add(tempID, int.Parse(tempTopScore));
        }
    }

    private void RankScoreInsert() //���� ���� ����
    {
        Param param = new Param();
        param.Add("ID", id);
        param.Add("songNumber", songNumber);
        param.Add("songName", songName);
        param.Add("topScore", "0"); //default �Ǿ������� ���ü�

        Backend.GameData.Insert("rankScore", param);
    }

    private void RankScoreUpdate() //���� ������Ʈ
    { 
        Param param = new Param();
        param.Add("topScore", rankScore.ToString());

        Where where = new Where(); //id�� songNumber�� ���� ����
        where.Equal("songNumber", songNumber);

        Backend.GameData.Update("rankScore", where, param); //update
    }

    private void SetUserRank()
    {
        var userRankDesc = userRankDic.OrderByDescending(x => x.Value); //dic�� ����Ǿ� �ִ� value ������������

        int i = 0;
        foreach (var dic in userRankDesc)
        {
            rankUserIDText[i].text = dic.Key;
            rankUserScoreText[i].text = dic.Value.ToString();
            i++;
        }
    }

}
