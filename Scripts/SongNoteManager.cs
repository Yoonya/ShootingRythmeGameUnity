using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd; //뒤끝 서버 연결

public class go_SongNote //곡 전체 노트설정
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

public class SongNoteManager : MonoBehaviour //여기서 곡 전체 노트 정보를 서버에서 받아 뿌림
{
    private int songCount = 3; //현재 곡 임시 3개
    public List<go_SongNote> songNotes = new List<go_SongNote>(); //리스트로 저장

    void Awake()
    {
        for (int i = 0; i < songCount; i++) //전체 곡 갯수만큼 반복
        {
            ConfirmNote(i);
        }
    }

    private void ConfirmNote(int songNumber) //서버통신
    {
        Where where = new Where();
        where.Equal("songNumber", songNumber.ToString());

        // 조건 없이 모든 데이터 조회하기
        var bro = Backend.GameData.Get("songNote", where, 10);
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
