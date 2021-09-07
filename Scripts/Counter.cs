using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour //카운터 객체에 쓰임
{
    public float counterSpeed = 0f; //카운터의 스피드
    public int counterType = 0; //0 왼쪽 1 오른쪽
    public float appearTime = 0f; //등장시간

    private PlayerStatus playerStatus;
    private EnemyStatus enemyStatus;
    private EnemyController enemyController;

    void Start()
    {
        playerStatus = FindObjectOfType<PlayerStatus>();
        enemyStatus = FindObjectOfType<EnemyStatus>();
        enemyController = FindObjectOfType<EnemyController>();
    }

    public void SetCounter(float counterSpeed, int counterType, float appearTime) //카운터 설정
    {
        this.counterSpeed = counterSpeed;
        this.counterType = counterType;
        this.appearTime = appearTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CounterJudge")) //카운터 판정하는 곳
        {
            transform.position = new Vector3(0f, 0f, 0f); //충돌 후에 그 자리에 안 있도록 임시로보냄
            ObjectPool.instance.queue[12].Enqueue(gameObject);//오브젝트풀링 집어넣기
            gameObject.SetActive(false);
            playerStatus.DecreaseHP(enemyStatus.enemyLv * 2); //닿았을 경우는 카운터에 실패하는 것이므로
        }
        else if (other.CompareTag("Enemy")) //카운터 성공할 경우 적에게 날아았을 때 처리
        {
            StartCoroutine(EnemyHit());
        }
    }

    private IEnumerator EnemyHit() //적 공격
    {
        enemyController.motionAnimator.SetInteger("AnimIndex", 18); //적 맞았을 때 모션 처리
        gameObject.GetComponent<Image>().enabled = false; //일시적으로 이미지만 숨김
        transform.position = new Vector3(0f, 0f, 0f); //그자리에 안 있도록 임시로보냄
        
        enemyStatus.DecreaseHP(enemyStatus.enemyLv * 2); //적 체력 감소

        yield return new WaitForSeconds(0.3f); //임시 대기
        gameObject.GetComponent<Image>().enabled = true;
        enemyController.motionAnimator.SetInteger("AnimIndex", 9); //다시 idle
        enemyController.queryBodyParts.transform.localRotation = enemyController.originRot; //초기화
        gameObject.SetActive(false);
    }
}
