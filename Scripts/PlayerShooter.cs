using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    //탄막을 날려주기만 하는 클래스
    [SerializeField] public GameObject missile;
    [SerializeField] public GameObject start;
    [SerializeField] public GameObject target;

    public void Shot(int hit)
    {
        CreateMissile(hit);
    }

    private void CreateMissile(int hit) //미사일 생성
    {
       
        GameObject bullet = ObjectPool.instance.queue[11].Dequeue();
        bullet.SetActive(true);

        StartCoroutine(bullet.GetComponent<PlayerBullet>().EnqueueObject());

        bullet.GetComponent<PlayerBullet>().hit = hit;
        bullet.GetComponent<PlayerBullet>().posA = Random.Range(-1.0f, 1.0f); //앵커를 계속 랜덤으로 설정
        bullet.GetComponent<PlayerBullet>().posB = Random.Range(-1.0f, 1.0f);
        bullet.GetComponent<PlayerBullet>().posC = Random.Range(-1.0f, 1.0f);
    }
}
