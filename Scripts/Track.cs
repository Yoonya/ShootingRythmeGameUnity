using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    //Ʈ���� �����̳� �ȿ����̳� ����� ���Ƽ� ���� ��������
    [SerializeField] private int trackSpeed = 0;
    private int BPM = 100; //���� BPM
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

    private void MoveTrack() //Ʈ���� ��Ʈ�� ���� �ӵ���
    {
        transform.localPosition += Vector3.down * currentSpeed * Time.deltaTime;
    }

    public void SetTrackSpeed(int trackSpeed)
    {
        this.trackSpeed = trackSpeed;
    }
}
