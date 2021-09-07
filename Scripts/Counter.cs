using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour //ī���� ��ü�� ����
{
    public float counterSpeed = 0f; //ī������ ���ǵ�
    public int counterType = 0; //0 ���� 1 ������
    public float appearTime = 0f; //����ð�

    private PlayerStatus playerStatus;
    private EnemyStatus enemyStatus;
    private EnemyController enemyController;

    void Start()
    {
        playerStatus = FindObjectOfType<PlayerStatus>();
        enemyStatus = FindObjectOfType<EnemyStatus>();
        enemyController = FindObjectOfType<EnemyController>();
    }

    public void SetCounter(float counterSpeed, int counterType, float appearTime) //ī���� ����
    {
        this.counterSpeed = counterSpeed;
        this.counterType = counterType;
        this.appearTime = appearTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CounterJudge")) //ī���� �����ϴ� ��
        {
            transform.position = new Vector3(0f, 0f, 0f); //�浹 �Ŀ� �� �ڸ��� �� �ֵ��� �ӽ÷κ���
            ObjectPool.instance.queue[12].Enqueue(gameObject);//������ƮǮ�� ����ֱ�
            gameObject.SetActive(false);
            playerStatus.DecreaseHP(enemyStatus.enemyLv * 2); //����� ���� ī���Ϳ� �����ϴ� ���̹Ƿ�
        }
        else if (other.CompareTag("Enemy")) //ī���� ������ ��� ������ ���ƾ��� �� ó��
        {
            StartCoroutine(EnemyHit());
        }
    }

    private IEnumerator EnemyHit() //�� ����
    {
        enemyController.motionAnimator.SetInteger("AnimIndex", 18); //�� �¾��� �� ��� ó��
        gameObject.GetComponent<Image>().enabled = false; //�Ͻ������� �̹����� ����
        transform.position = new Vector3(0f, 0f, 0f); //���ڸ��� �� �ֵ��� �ӽ÷κ���
        
        enemyStatus.DecreaseHP(enemyStatus.enemyLv * 2); //�� ü�� ����

        yield return new WaitForSeconds(0.3f); //�ӽ� ���
        gameObject.GetComponent<Image>().enabled = true;
        enemyController.motionAnimator.SetInteger("AnimIndex", 9); //�ٽ� idle
        enemyController.queryBodyParts.transform.localRotation = enemyController.originRot; //�ʱ�ȭ
        gameObject.SetActive(false);
    }
}
