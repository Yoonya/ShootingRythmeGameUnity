using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    //��Ÿ ��ü�� ���
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

    private void MoveStar() //��Ʈ ������
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

    private void OnTriggerEnter(Collider other)  //��������
    {
        if (other.CompareTag("Player")) //rot���� ���� �ݶ��̴� ȸ������ ���� �÷��̾ ����ٴϴ� ��ü�� �ε����� ��
        {
            if (starType == 0) //������̶�� ü������
            {
                playerStatus.IncreaseHP(1 + (int)Mathf.Round(playerStatus.playerLv / 5));
                scoreBoard.SetStar();
            }
            else //�������̶�� ü�� ����
            {
                playerStatus.DecreaseHP(1 + (int)Mathf.Round(playerStatus.playerLv / 3));
                StartCoroutine(playerHit());
            } 
               

            AudioManager.instance.PlaySFX("Star");
            ObjectPool.instance.queue[starType + 9].Enqueue(this.gameObject);//������ƮǮ�� ����ֱ� 
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator playerHit() //������ ĳ���� �ǰ� ó��
    {
        characterController.motionAnimator.SetInteger("AnimIndex", 18);
        gameObject.GetComponent<Image>().enabled = false; //�Ͻ������� �̹����� ����

        yield return CoroutineManager.YieldInstructionCache.WaitForSeconds(0.3f); //ĳ���� �ǰݸ�� ���ð�
        gameObject.GetComponent<Image>().enabled = true; 
        characterController.motionAnimator.SetInteger("AnimIndex", 10);
        characterController.queryBodyParts.transform.localRotation = characterController.originRot; //�ʱ�ȭ
    }
}
