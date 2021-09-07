using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowJudgement : MonoBehaviour
{
    //playercolider�� ��ġ
    //���콺 ��ư�� Ŭ������ �� ������ �ݶ��̴� �浹���� Ȯ��, ������ �� ���� �浹���� Ȯ������ �����Ѵ�.

    //�浹 ���� Ȯ��
    bool rightJudge = false; 
    bool leftJudge = false;

    private ScoreBoard scoreBoard;
    private Score score;
    private PlayerStatus playerStatus;
    private EnemyStatus enemyStatus;
    private CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        scoreBoard = FindObjectOfType<ScoreBoard>();
        score = FindObjectOfType<Score>();
        playerStatus = FindObjectOfType<PlayerStatus>();
        enemyStatus = FindObjectOfType<EnemyStatus>();
        characterController = FindObjectOfType<CharacterController>();
    }

    private void OnTriggerStay(Collider other) //���� ���� ���� �� ��¦ ����ؼ�, enter�� ���� �� �ʹ� ������ ª�� stay�� ������ ����
    {
        if (other.CompareTag("RightArrowColider")) //���⿡ ���� rot���� ������ױ� ������ ���콺��ư Ŭ���� �����̴�.
        {
            rightJudge = true;
        }
        if (other.CompareTag("LeftArrowColider")) //������ ������ ���� true�� �Ǿ��־���Ѵ�.
        {
            if (rightJudge)
            {
                leftJudge = true;
            }

            if (rightJudge && leftJudge) //���� ������
            {
                JudgeClear();

                ObjectPool.instance.queue[16 + other.transform.parent.GetComponent<Arrow>().arrowType].Enqueue(other.transform.parent.gameObject);//������ƮǮ�� ����ֱ�           
                other.transform.parent.gameObject.SetActive(false); //�ش� ��ü �����
            }
        }
    }

    private void JudgeClear() //���������̺�Ʈ
    {
        int tempPoint = playerStatus.playerLv - enemyStatus.enemyLv; //���� ����
        if (tempPoint < 1) tempPoint = 1; //�������� �ʹ� �����ų� ������ �Ǽ� ������ �̻��ϰ� �����ʰ�(�ӽ�)
        if (tempPoint > 3) tempPoint = 3;
        score.SetScore(tempPoint * 50); //���� �߰�
        scoreBoard.SetArrow(); //������ ����
        rightJudge = false;//�ʱ�ȭ
        leftJudge = false;

        AudioManager.instance.PlaySFX("Counter");//ȿ����
        StartCoroutine(CharacterAnimate());
    }

    private IEnumerator CharacterAnimate() //ĳ���� ���� ȿ��
    {
        characterController.motionAnimator.SetInteger("AnimIndex", 16);

        yield return new WaitForSeconds(1f); //ĳ���� ������� ���ð�
        characterController.motionAnimator.SetInteger("AnimIndex", 10);
        characterController.queryBodyParts.transform.localRotation = characterController.originRot; //�ʱ�ȭ
        characterController.queryBodyParts.transform.localPosition = Vector3.MoveTowards(characterController.queryBodyParts.transform.localPosition,
                                                                                  new Vector3(characterController.queryBodyParts.transform.localPosition.x,
                                                                                              characterController.originPos.y,
                                                                                              characterController.originPos.z),
                                                                                  1f); //�ִϸ��̼����� ���� ������ ���ͼ� �ʱ�ȭ
    }
}
