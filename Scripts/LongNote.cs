using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNote : MonoBehaviour
{
    //롱노트 객체에 쓰임
    public int longNoteSpeed = 0; //롱노트의 스피드
    public int longNoteType = 0; //롱노트의 타입
    public int longNoteLocation = 0; //롱노트 위치
    public float appearTime = 0f; //등장 시간
    public int fb = 0; //앞뒤구별 0이 앞

    // Update is called once per frame
    void Update()
    {
        MoveNote();
    }

    private void MoveNote() //노트 움직임
    {
        transform.localPosition += Vector3.down * longNoteSpeed * Time.deltaTime;
    }

    public void SetNoteSpeed(int longNoteSpeed)
    {
        this.longNoteSpeed = longNoteSpeed;
    }

    public void SetNoteType(int longNoteType)
    {
        this.longNoteType = longNoteType;
    }

    public void SetNoteLocation(int longNoteLocation)
    {
        this.longNoteLocation = longNoteLocation;
    }

    public void SetAppearTime(float appearTime)
    {
        this.appearTime = appearTime;
    }
}
