using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColider : MonoBehaviour
{
    //�� ��ũ��Ʈ�� ĳ������ box collider�� �ִϸ��̼� ��� �߿� rot�� ���� ������ ���� ������ �̻��ϱ� ������ ��ü�� �������

    [SerializeField]
    private Transform player;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = player.position;
    }
}
