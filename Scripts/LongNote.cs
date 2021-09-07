using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNote : MonoBehaviour
{
    //�ճ�Ʈ ��ü�� ����
    public int longNoteSpeed = 0; //�ճ�Ʈ�� ���ǵ�
    public int longNoteType = 0; //�ճ�Ʈ�� Ÿ��
    public int longNoteLocation = 0; //�ճ�Ʈ ��ġ
    public float appearTime = 0f; //���� �ð�
    public int fb = 0; //�յڱ��� 0�� ��

    // Update is called once per frame
    void Update()
    {
        MoveNote();
    }

    private void MoveNote() //��Ʈ ������
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
