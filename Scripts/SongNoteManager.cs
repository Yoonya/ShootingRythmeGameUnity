using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd; //�ڳ� ���� ����

public class go_SongNote //�� ��ü ��Ʈ����
{
    public int songNumber;
    public string notes;
    public string longNotes;
    public string arrows;
    public string counters;
    public string stars;

    public go_SongNote(int songNumber, string notes, string longNotes, string arrows, string counters, string stars)
    {
        this.songNumber = songNumber;
        this.notes = notes;
        this.longNotes = longNotes;
        this.arrows = arrows;
        this.counters = counters;
        this.stars = stars;
    }
}

public class SongNoteManager : MonoBehaviour //���⼭ �� ��ü ��Ʈ ������ �������� �޾� �Ѹ�
{
    private int songCount = 3; //���� �� �ӽ� 3��
    public List<go_SongNote> songNotes = new List<go_SongNote>(); //����Ʈ�� ����

    void Awake()
    {
        for (int i = 0; i < songCount; i++) //��ü �� ������ŭ �ݺ�
        {
            ConfirmNote(i);
        }
    }

    private void ConfirmNote(int songNumber) //�������
    {
        Where where = new Where();
        where.Equal("songNumber", songNumber.ToString());

        // ���� ���� ��� ������ ��ȸ�ϱ�
        var bro = Backend.GameData.Get("songNote", where, 10);
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
            string tempNotes = bro.Rows()[i]["notes"]["S"].ToString();
            string tempLongNotes = bro.Rows()[i]["longNotes"]["S"].ToString();
            string tempArrows = bro.Rows()[i]["arrows"]["S"].ToString();
            string tempCounters = bro.Rows()[i]["counters"]["S"].ToString();
            string tempStars = bro.Rows()[i]["stars"]["S"].ToString();

            go_SongNote tempSongNote = new go_SongNote(int.Parse(tempSongNumber), tempNotes, tempLongNotes, tempArrows, tempCounters, tempStars);
            songNotes.Add(tempSongNote);
        }
    }
}
