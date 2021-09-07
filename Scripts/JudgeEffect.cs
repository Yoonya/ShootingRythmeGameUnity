using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgeEffect : MonoBehaviour
{
    //웬만한 판정들 관리
    //0 = Hit, 1 = Over, 2 = Critical, 3 = Miss 통일

    private Transform noteTransform;

    private int comboCount = 0; //콤보 숫자 처리
    private int maxCombo = 0; //맥스 콤보 저장

    private ScoreBoard scoreBoard;
    private CharacterController characterController;

    void Start()
    {
        scoreBoard = FindObjectOfType<ScoreBoard>();
        characterController = FindObjectOfType<CharacterController>();
    }

    public IEnumerator NoteHitEffect(int judge, bool notPause) //연출 애니메이션 처리
    {
        //현재 노트 위치 위에 생성,
        GameObject tempNoteJudgeEffect = ObjectPool.instance.queue[judge + 3].Dequeue(); //오브젝트 풀링 사용
        tempNoteJudgeEffect.SetActive(true);
        tempNoteJudgeEffect.transform.position = noteTransform.position + Vector3.up + Vector3.back * 180;//위치설정
        tempNoteJudgeEffect.transform.SetParent(this.transform); //캔버스 안에 들어오도록 부모설정
        tempNoteJudgeEffect.transform.localScale = new Vector3(1f, 1f, 1f); //스케일을 1로, 왜 달라지는지 모르겠음

        tempNoteJudgeEffect.GetComponent<Animator>().SetTrigger("Hit");

        GameObject tempNoteHitEffect = null;
        if (!notPause) //플레이어가 노트를 아예 못누르고 쓰레기통으로 노트가 가서 미스가 뜰 때는 나오지 않도록
        {
            tempNoteHitEffect = ObjectPool.instance.queue[8].Dequeue(); //오브젝트 풀링 사용
            tempNoteHitEffect.SetActive(true);
            tempNoteHitEffect.transform.position = noteTransform.position + Vector3.down * 125 + Vector3.back * 180;//위치설정
            tempNoteHitEffect.transform.SetParent(this.transform); //캔버스 안에 들어오도록 부모설정
            tempNoteHitEffect.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); //스케일을 1로, 왜 달라지는지 모르겠음

            tempNoteHitEffect.GetComponent<Animator>().SetTrigger("Hit");

            characterController.motionAnimator.SetInteger("AnimIndex", 106); //이때에만 반응
        }

        if (judge != 3) comboCount++; //Miss를 제외하고 콤보처리
        else comboCount = 0;

        if (comboCount > maxCombo)
        {
            maxCombo = comboCount;
            scoreBoard.SetMaxCombo(maxCombo);
        } 

        GameObject tempComboEffect = null;
        if (comboCount != 0)
        {
            tempComboEffect = ObjectPool.instance.queue[7].Dequeue(); //오브젝트 풀링 사용
            tempComboEffect.SetActive(true);
            tempComboEffect.transform.position = noteTransform.position - Vector3.up * 100 + Vector3.back * 180;//위치설정          
            tempComboEffect.transform.SetParent(this.transform); //캔버스 안에 들어오도록 부모설정
            tempComboEffect.transform.localScale = new Vector3(1f, 1f, 1f); //스케일을 1로, 왜 커지는지 모르겠음
            comboEffect(tempComboEffect);
        }

        scoreBoard.SetComboCount(judge);

        yield return new WaitForSeconds(0.6f); //애니메이션 끝나는시간 대기

        characterController.queryBodyParts.transform.localRotation = characterController.originRot; //초기화
        characterController.queryBodyParts.transform.localPosition = Vector3.Lerp(characterController.queryBodyParts.transform.localPosition, 
                                                                                  new Vector3(characterController.queryBodyParts.transform.localPosition.x, 
                                                                                              characterController.originPos.y,
                                                                                              characterController.originPos.z), 
                                                                                  0.2f); //점점 앞으로 나옴
        characterController.motionAnimator.SetInteger("AnimIndex", 10);
        ObjectPool.instance.queue[judge+3].Enqueue(tempNoteJudgeEffect);//오브젝트풀링 집어넣기
        tempNoteJudgeEffect.SetActive(false);
        if (tempNoteHitEffect != null)
        {
            ObjectPool.instance.queue[8].Enqueue(tempNoteHitEffect);//오브젝트풀링 집어넣기
            tempNoteHitEffect.SetActive(false);
        }
        if (tempComboEffect != null)
        {
            ObjectPool.instance.queue[7].Enqueue(tempComboEffect);//오브젝트풀링 집어넣기
            tempComboEffect.SetActive(false);
        }
    }

    public IEnumerator CounterEffect(int judge, int direct) //판정과, 카운터 방향
    {
        //카운터판정이펙트
        GameObject tempCounterJudgeEffect = ObjectPool.instance.queue[judge + 13].Dequeue(); //오브젝트 풀링 사용
        tempCounterJudgeEffect.SetActive(true);
        tempCounterJudgeEffect.transform.SetParent(this.transform); //캔버스 안에 들어오도록 부모설정
        tempCounterJudgeEffect.transform.localScale = new Vector3(1f, 1f, 1f); //스케일을 1로, 왜 달라지는지 모르겠음

        Vector3 originPos = new Vector3(-290f, -190f, -650f); //어쩌다보니 아직 고정좌표
        if (direct == 1) //0은 그대로
            tempCounterJudgeEffect.transform.localPosition = new Vector3(originPos.x * -1, originPos.y, originPos.z);//위치설정
        else
            tempCounterJudgeEffect.transform.localPosition = new Vector3(originPos.x, originPos.y, originPos.z);//위치설정

        GameObject tempCounterFontEffect = null;
        Vector3 originPos2 = Vector3.zero;
        if (judge == 1) //카운터폰트이벤트
        {
            tempCounterFontEffect = ObjectPool.instance.queue[15].Dequeue(); //오브젝트 풀링 사용
            tempCounterFontEffect.SetActive(true);
            tempCounterFontEffect.transform.SetParent(this.transform); //캔버스 안에 들어오도록 부모설정
            tempCounterFontEffect.transform.localScale = new Vector3(1f, 1f, 1f); //스케일을 1로, 왜 달라지는지 모르겠음
            
            originPos2 = new Vector3(-400f, -120f, 0f);
            if (direct == 1) //0은 그대로
                tempCounterFontEffect.transform.localPosition = new Vector3(originPos2.x * -1, originPos2.y, originPos2.z);//위치설정
            else
                tempCounterFontEffect.transform.localPosition = new Vector3(originPos2.x, originPos2.y, originPos2.z);//위치설정
        }

        yield return new WaitForSeconds(1f); //애니메이션 끝나는시간 대기

        tempCounterJudgeEffect.transform.localPosition = originPos; //카운터판정이펙트 제자리로
        ObjectPool.instance.queue[judge + 13].Enqueue(tempCounterJudgeEffect);//오브젝트풀링 집어넣기
        tempCounterJudgeEffect.SetActive(false);

        if (tempCounterFontEffect != null)
        {
            tempCounterFontEffect.transform.localPosition = originPos2; //카운터폰트 이벤트 제자리로
            ObjectPool.instance.queue[15].Enqueue(tempCounterFontEffect);//오브젝트풀링 집어넣기
            tempCounterFontEffect.SetActive(false);
        }

    }

    private void comboEffect(GameObject combo) //콤보숫자연출
    {
        combo.GetComponentInChildren<Text>().text = comboCount.ToString();
        combo.GetComponent<Animator>().SetTrigger("Hit");       
    }

    public void SetNoteTransform(Transform noteTransform)
    {
        this.noteTransform = noteTransform;
    }
}
