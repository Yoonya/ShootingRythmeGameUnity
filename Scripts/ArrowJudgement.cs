using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowJudgement : MonoBehaviour
{
    //playercolider에 위치
    //마우스 버튼을 클릭했을 때 오른쪽 콜라이더 충돌상태 확인, 떼었을 때 왼쪽 충돌상태 확인으로 판정한다.

    //충돌 상태 확인
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

    private void OnTriggerStay(Collider other) //양쪽 끝에 있을 시 살짝 곤란해서, enter로 했을 때 너무 판정이 짧아 stay로 판정을 좋게
    {
        if (other.CompareTag("RightArrowColider")) //방향에 따라 rot값만 변경시켰기 때문에 마우스버튼 클릭이 고정이다.
        {
            rightJudge = true;
        }
        if (other.CompareTag("LeftArrowColider")) //오른쪽 판정이 먼저 true가 되어있어야한다.
        {
            if (rightJudge)
            {
                leftJudge = true;
            }

            if (rightJudge && leftJudge) //판정 성공시
            {
                JudgeClear();

                ObjectPool.instance.queue[16 + other.transform.parent.GetComponent<Arrow>().arrowType].Enqueue(other.transform.parent.gameObject);//오브젝트풀링 집어넣기           
                other.transform.parent.gameObject.SetActive(false); //해당 객체 숨기기
            }
        }
    }

    private void JudgeClear() //판정성공이벤트
    {
        int tempPoint = playerStatus.playerLv - enemyStatus.enemyLv; //점수 설정
        if (tempPoint < 1) tempPoint = 1; //데미지가 너무 세지거나 음수가 되서 점수가 이상하게 되지않게(임시)
        if (tempPoint > 3) tempPoint = 3;
        score.SetScore(tempPoint * 50); //점수 추가
        scoreBoard.SetArrow(); //보드판 갱신
        rightJudge = false;//초기화
        leftJudge = false;

        AudioManager.instance.PlaySFX("Counter");//효과음
        StartCoroutine(CharacterAnimate());
    }

    private IEnumerator CharacterAnimate() //캐릭터 성공 효과
    {
        characterController.motionAnimator.SetInteger("AnimIndex", 16);

        yield return new WaitForSeconds(1f); //캐릭터 성공모션 대기시간
        characterController.motionAnimator.SetInteger("AnimIndex", 10);
        characterController.queryBodyParts.transform.localRotation = characterController.originRot; //초기화
        characterController.queryBodyParts.transform.localPosition = Vector3.MoveTowards(characterController.queryBodyParts.transform.localPosition,
                                                                                  new Vector3(characterController.queryBodyParts.transform.localPosition.x,
                                                                                              characterController.originPos.y,
                                                                                              characterController.originPos.z),
                                                                                  1f); //애니메이션으로 점점 앞으로 나와서 초기화
    }
}
