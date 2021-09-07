using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    //ź���� �����ֱ⸸ �ϴ� Ŭ����
    [SerializeField] public GameObject missile;
    [SerializeField] public GameObject start;
    [SerializeField] public GameObject target;

    public void Shot(int hit)
    {
        CreateMissile(hit);
    }

    private void CreateMissile(int hit) //�̻��� ����
    {
       
        GameObject bullet = ObjectPool.instance.queue[11].Dequeue();
        bullet.SetActive(true);

        StartCoroutine(bullet.GetComponent<PlayerBullet>().EnqueueObject());

        bullet.GetComponent<PlayerBullet>().hit = hit;
        bullet.GetComponent<PlayerBullet>().posA = Random.Range(-1.0f, 1.0f); //��Ŀ�� ��� �������� ����
        bullet.GetComponent<PlayerBullet>().posB = Random.Range(-1.0f, 1.0f);
        bullet.GetComponent<PlayerBullet>().posC = Random.Range(-1.0f, 1.0f);
    }
}
