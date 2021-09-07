using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour //Arrow객체에 쓰임
{
    public int arrowSpeed = 0; //Arrow 속도
    public int arrowType = 0; //0 = short, 1 = long
    public int arrowDirect = 0; //0 = left, 1 = right
    public int arrowLocation = 0; //0~4
    public float appearTime = 0f; //Arrow노트 등장 시간

    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveArrow();
    }

    private void MoveArrow() //노트 움직임
    {
        transform.localPosition += Vector3.down * arrowSpeed * Time.deltaTime;
    }

    public void SetArrowSpeed(int arrowSpeed)
    {
        this.arrowSpeed = arrowSpeed;
    }

    public void SetArrowType(int arrowType)
    {
        this.arrowType = arrowType;
    }

    public void SetArrowDirect(int arrowDirect)
    {
        this.arrowDirect = arrowDirect;
    }

    public void SetArrowLocation(int arrowLocation)
    {
        this.arrowLocation = arrowLocation;
    }

    public void SetAppearTime(float appearTime)
    {
        this.appearTime = appearTime;
    }

}
