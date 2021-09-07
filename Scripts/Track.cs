using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    //트랙이 움직이나 안움직이나 생긴게 같아서 현재 쓰지않음
    [SerializeField] private int trackSpeed = 0;
    private int BPM = 100; //음악 BPM
    private int currentSpeed = 0;

    void Start()
    {
        currentSpeed = trackSpeed * BPM;
    }

    // Update is called once per frame
    void Update()
    {
        MoveTrack();
    }

    private void MoveTrack() //트랙과 노트는 같은 속도로
    {
        transform.localPosition += Vector3.down * currentSpeed * Time.deltaTime;
    }

    public void SetTrackSpeed(int trackSpeed)
    {
        this.trackSpeed = trackSpeed;
    }
}
