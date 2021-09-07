using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgeEffect : MonoBehaviour
{
    //������ ������ ����
    //0 = Hit, 1 = Over, 2 = Critical, 3 = Miss ����

    private Transform noteTransform;

    private int comboCount = 0; //�޺� ���� ó��
    private int maxCombo = 0; //�ƽ� �޺� ����

    private ScoreBoard scoreBoard;
    private CharacterController characterController;

    void Start()
    {
        scoreBoard = FindObjectOfType<ScoreBoard>();
        characterController = FindObjectOfType<CharacterController>();
    }

    public IEnumerator NoteHitEffect(int judge, bool notPause) //���� �ִϸ��̼� ó��
    {
        //���� ��Ʈ ��ġ ���� ����,
        GameObject tempNoteJudgeEffect = ObjectPool.instance.queue[judge + 3].Dequeue(); //������Ʈ Ǯ�� ���
        tempNoteJudgeEffect.SetActive(true);
        tempNoteJudgeEffect.transform.position = noteTransform.position + Vector3.up + Vector3.back * 180;//��ġ����
        tempNoteJudgeEffect.transform.SetParent(this.transform); //ĵ���� �ȿ� �������� �θ���
        tempNoteJudgeEffect.transform.localScale = new Vector3(1f, 1f, 1f); //�������� 1��, �� �޶������� �𸣰���

        tempNoteJudgeEffect.GetComponent<Animator>().SetTrigger("Hit");

        GameObject tempNoteHitEffect = null;
        if (!notPause) //�÷��̾ ��Ʈ�� �ƿ� �������� ������������ ��Ʈ�� ���� �̽��� �� ���� ������ �ʵ���
        {
            tempNoteHitEffect = ObjectPool.instance.queue[8].Dequeue(); //������Ʈ Ǯ�� ���
            tempNoteHitEffect.SetActive(true);
            tempNoteHitEffect.transform.position = noteTransform.position + Vector3.down * 125 + Vector3.back * 180;//��ġ����
            tempNoteHitEffect.transform.SetParent(this.transform); //ĵ���� �ȿ� �������� �θ���
            tempNoteHitEffect.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); //�������� 1��, �� �޶������� �𸣰���

            tempNoteHitEffect.GetComponent<Animator>().SetTrigger("Hit");

            characterController.motionAnimator.SetInteger("AnimIndex", 106); //�̶����� ����
        }

        if (judge != 3) comboCount++; //Miss�� �����ϰ� �޺�ó��
        else comboCount = 0;

        if (comboCount > maxCombo)
        {
            maxCombo = comboCount;
            scoreBoard.SetMaxCombo(maxCombo);
        } 

        GameObject tempComboEffect = null;
        if (comboCount != 0)
        {
            tempComboEffect = ObjectPool.instance.queue[7].Dequeue(); //������Ʈ Ǯ�� ���
            tempComboEffect.SetActive(true);
            tempComboEffect.transform.position = noteTransform.position - Vector3.up * 100 + Vector3.back * 180;//��ġ����          
            tempComboEffect.transform.SetParent(this.transform); //ĵ���� �ȿ� �������� �θ���
            tempComboEffect.transform.localScale = new Vector3(1f, 1f, 1f); //�������� 1��, �� Ŀ������ �𸣰���
            comboEffect(tempComboEffect);
        }

        scoreBoard.SetComboCount(judge);

        yield return new WaitForSeconds(0.6f); //�ִϸ��̼� �����½ð� ���

        characterController.queryBodyParts.transform.localRotation = characterController.originRot; //�ʱ�ȭ
        characterController.queryBodyParts.transform.localPosition = Vector3.Lerp(characterController.queryBodyParts.transform.localPosition, 
                                                                                  new Vector3(characterController.queryBodyParts.transform.localPosition.x, 
                                                                                              characterController.originPos.y,
                                                                                              characterController.originPos.z), 
                                                                                  0.2f); //���� ������ ����
        characterController.motionAnimator.SetInteger("AnimIndex", 10);
        ObjectPool.instance.queue[judge+3].Enqueue(tempNoteJudgeEffect);//������ƮǮ�� ����ֱ�
        tempNoteJudgeEffect.SetActive(false);
        if (tempNoteHitEffect != null)
        {
            ObjectPool.instance.queue[8].Enqueue(tempNoteHitEffect);//������ƮǮ�� ����ֱ�
            tempNoteHitEffect.SetActive(false);
        }
        if (tempComboEffect != null)
        {
            ObjectPool.instance.queue[7].Enqueue(tempComboEffect);//������ƮǮ�� ����ֱ�
            tempComboEffect.SetActive(false);
        }
    }

    public IEnumerator CounterEffect(int judge, int direct) //������, ī���� ����
    {
        //ī������������Ʈ
        GameObject tempCounterJudgeEffect = ObjectPool.instance.queue[judge + 13].Dequeue(); //������Ʈ Ǯ�� ���
        tempCounterJudgeEffect.SetActive(true);
        tempCounterJudgeEffect.transform.SetParent(this.transform); //ĵ���� �ȿ� �������� �θ���
        tempCounterJudgeEffect.transform.localScale = new Vector3(1f, 1f, 1f); //�������� 1��, �� �޶������� �𸣰���

        Vector3 originPos = new Vector3(-290f, -190f, -650f); //��¼�ٺ��� ���� ������ǥ
        if (direct == 1) //0�� �״��
            tempCounterJudgeEffect.transform.localPosition = new Vector3(originPos.x * -1, originPos.y, originPos.z);//��ġ����
        else
            tempCounterJudgeEffect.transform.localPosition = new Vector3(originPos.x, originPos.y, originPos.z);//��ġ����

        GameObject tempCounterFontEffect = null;
        Vector3 originPos2 = Vector3.zero;
        if (judge == 1) //ī������Ʈ�̺�Ʈ
        {
            tempCounterFontEffect = ObjectPool.instance.queue[15].Dequeue(); //������Ʈ Ǯ�� ���
            tempCounterFontEffect.SetActive(true);
            tempCounterFontEffect.transform.SetParent(this.transform); //ĵ���� �ȿ� �������� �θ���
            tempCounterFontEffect.transform.localScale = new Vector3(1f, 1f, 1f); //�������� 1��, �� �޶������� �𸣰���
            
            originPos2 = new Vector3(-400f, -120f, 0f);
            if (direct == 1) //0�� �״��
                tempCounterFontEffect.transform.localPosition = new Vector3(originPos2.x * -1, originPos2.y, originPos2.z);//��ġ����
            else
                tempCounterFontEffect.transform.localPosition = new Vector3(originPos2.x, originPos2.y, originPos2.z);//��ġ����
        }

        yield return new WaitForSeconds(1f); //�ִϸ��̼� �����½ð� ���

        tempCounterJudgeEffect.transform.localPosition = originPos; //ī������������Ʈ ���ڸ���
        ObjectPool.instance.queue[judge + 13].Enqueue(tempCounterJudgeEffect);//������ƮǮ�� ����ֱ�
        tempCounterJudgeEffect.SetActive(false);

        if (tempCounterFontEffect != null)
        {
            tempCounterFontEffect.transform.localPosition = originPos2; //ī������Ʈ �̺�Ʈ ���ڸ���
            ObjectPool.instance.queue[15].Enqueue(tempCounterFontEffect);//������ƮǮ�� ����ֱ�
            tempCounterFontEffect.SetActive(false);
        }

    }

    private void comboEffect(GameObject combo) //�޺����ڿ���
    {
        combo.GetComponentInChildren<Text>().text = comboCount.ToString();
        combo.GetComponent<Animator>().SetTrigger("Hit");       
    }

    public void SetNoteTransform(Transform noteTransform)
    {
        this.noteTransform = noteTransform;
    }
}
