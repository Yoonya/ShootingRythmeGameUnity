using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public int noteSpeed = 0; //��Ʈ���ǵ�
    public int noteType = 0; //��ƮŸ��
    public int noteLocation = 0;//��Ʈ��ġ
    public float appearTime = 0f; //����ð�

    // Update is called once per frame
    void Update()
    {
        MoveNote();
    }

    private void MoveNote() //��Ʈ ������
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
