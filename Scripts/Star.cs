using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    //스타 객체에 사용
    public int starSpeed = 0;
    public int starType = 0;
    public int starLocation = 0;
    public float appearTime = 0f;

    private ScoreBoard scoreBoard;
    private PlayerStatus playerStatus;
    private CharacterController characterController;

    void Start()
    {
        scoreBoard = FindObjectOfType<ScoreBoard>();
        playerStatus = FindObjectOfType<PlayerStatus>();
        characterController = FindObjectOfType<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveStar();
    }

    private void MoveStar() //노트 움직임
    {
        transform.localPosition += Vector3.down * starSpeed * Time.deltaTime;
    }

    public void SetStarSpeed(int noteSpeed)
    {
        this.starSpeed = noteSpeed;
    }

    public void SetStarType(int starType)
    {
        this.starType = starType;
    }

    public void SetStarLocation(int starLocation)
    {
        this.starLocation = starLocation;
    }


    public void SetAppearTime(float appearTime)
    {
        this.appearTime = appearTime;
    }

    private void OnTriggerEnter(Collider other)  //쓰레기통
    {
        if (other.CompareTag("Player")) //rot으로 인한 콜라이더 회전으로 인해 플레이어를 따라다니는 객체에 부딪히게 됨
        {
            if (starType == 0) //노란별이라면 체력증가
            {
                playerStatus.IncreaseHP(1 + (int)Mathf.Round(playerStatus.playerLv / 5));
                scoreBoard.SetStar();
            }
            else //붉은별이라면 체력 감소
            {
                playerStatus.DecreaseHP(1 + (int)Mathf.Round(playerStatus.playerLv / 3));
                StartCoroutine(playerHit());
            } 
               

            AudioManager.instance.PlaySFX("Star");
            ObjectPool.instance.queue[starType + 9].Enqueue(this.gameObject);//오브젝트풀링 집어넣기 
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator playerHit() //붉은별 캐릭터 피격 처리
    {
        characterController.motionAnimator.SetInteger("AnimIndex", 18);
        gameObject.GetComponent<Image>().enabled = false; //일시적으로 이미지만 숨김

        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(0.3f); //캐릭터 피격모션 대기시간
        gameObject.GetComponent<Image>().enabled = true; 
        characterController.motionAnimator.SetInteger("AnimIndex", 10);
        characterController.queryBodyParts.transform.localRotation = characterController.originRot; //초기화
    }
}
