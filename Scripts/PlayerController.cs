using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 40; //�������� ������
    [SerializeField]
    private float moveSpeed2 = 3; //��ӿ ��, �������� ������

    private Vector3 currentLocation = new Vector3(0f,0f,0f);
    private Vector3 movingLocation;
    private Vector3 preMousePosition;

    private float leftLimit = -22f;
    private float righttLimit = 22f;

    // Start is called before the first frame update
    void Start()
    {
        currentLocation = transform.position;
        preMousePosition = new Vector3(0, 0, 0); //���콺 ����� �ʱ�ȭ ��ų��
        movingLocation = new Vector3(preMousePosition.x, transform.position.y, transform.position.z);
    }

    public void Move() //ĳ���� ������
    {
        //���콺�� �ǽð����� �����̰� �ϴ� ��ſ� ��ǥ�� �� �ȸ¾� ������ ���� �̻���
        movingLocation.x = Input.mousePosition.x / moveSpeed - 22; //â���� ���ѽ� ��ǥ�� 0���� ����? 

        if (movingLocation.x > currentLocation.x)
        {
            if (movingLocation.x > righttLimit) //�¿� ���� ����
                movingLocation.x = righttLimit;

            transform.position = Vector3.MoveTowards(transform.position, movingLocation, moveSpeed2);//��ӿ
            currentLocation = movingLocation;
        }
        else if(movingLocation.x < currentLocation.x)
        {
            if (movingLocation.x < leftLimit) //�¿� ���� ����
                movingLocation.x = leftLimit;

            transform.position = Vector3.MoveTowards(transform.position, movingLocation, moveSpeed2);
            currentLocation = movingLocation;
        }      

    }
}
