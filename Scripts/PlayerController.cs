using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 40; //높을수록 느려짐
    [SerializeField]
    private float moveSpeed2 = 3; //등속운동 값, 높을수록 빨라짐

    private Vector3 currentLocation = new Vector3(0f,0f,0f);
    private Vector3 movingLocation;
    private Vector3 preMousePosition;

    private float leftLimit = -22f;
    private float righttLimit = 22f;

    // Start is called before the first frame update
    void Start()
    {
        currentLocation = transform.position;
        preMousePosition = new Vector3(0, 0, 0); //마우스 가운데로 초기화 시킬것
        movingLocation = new Vector3(preMousePosition.x, transform.position.y, transform.position.z);
    }

    public void Move() //캐릭터 움직임
    {
        //마우스로 실시간으로 움직이게 하는 대신에 좌표가 잘 안맞아 계산식이 조금 이상함
        movingLocation.x = Input.mousePosition.x / moveSpeed - 22; //창모드로 가둘시 좌표가 0부터 시작? 

        if (movingLocation.x > currentLocation.x)
        {
            if (movingLocation.x > righttLimit) //좌우 범위 제한
                movingLocation.x = righttLimit;

            transform.position = Vector3.MoveTowards(transform.position, movingLocation, moveSpeed2);//등속운동
            currentLocation = movingLocation;
        }
        else if(movingLocation.x < currentLocation.x)
        {
            if (movingLocation.x < leftLimit) //좌우 범위 제한
                movingLocation.x = leftLimit;

            transform.position = Vector3.MoveTowards(transform.position, movingLocation, moveSpeed2);
            currentLocation = movingLocation;
        }      

    }
}
