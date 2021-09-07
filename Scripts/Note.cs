using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public int noteSpeed = 0; //노트스피드
    public int noteType = 0; //노트타입
    public int noteLocation = 0;//노트위치
    public float appearTime = 0f; //등장시간

    // Update is called once per frame
    void Update()
    {
        MoveNote();
    }

    private void MoveNote() //노트 움직임
    {
        transform.localPosition += Vector3.down * noteSpeed * Time.deltaTime;
    }

    public void SetNoteSpeed(int noteSpeed)
    {
        this.noteSpeed = noteSpeed;
    }

    public void SetNoteType(int noteType)
    {
        this.noteType = noteType;
    }

    public void SetNoteLocation(int noteLocation)
    {
        this.noteLocation = noteLocation;
    }

    public void SetAppearTime(float appearTime)
    {
        this.appearTime = appearTime;
    }
}
