using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteBody : MonoBehaviour
{
    public int longNoteSpeed = 0;
    public int longNoteType = 0;
    public int longNoteLocation = 0;
    public float appearTime = 0f;
    public float resizeTime = 0f;

    public RectTransform rectTransform;
    public float height = 0f;
    public bool IsreSize = true;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0); // ���̸� 0���� �ʱ�ȭ
    }

    // Update is called once per frame
    void Update()
    {
        MoveNote();
        if(IsreSize)
            ReSize();
    }

    private void MoveNote() //��Ʈ ������
    {
        transform.localPosition += Vector3.down * longNoteSpeed * Time.deltaTime;
    }

    public void ReSize()
    {
        height += longNoteSpeed * 0.4f * Time.deltaTime; //60���� ���� �ٽ� 0���� ���߸鼭 ������ �ӵ��� ������Ѵ�.
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height); //���� ������ ��� ���������
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

    public void SetReSizeTime(float resizeTime)
    {
        this.resizeTime = resizeTime;
    }
}
