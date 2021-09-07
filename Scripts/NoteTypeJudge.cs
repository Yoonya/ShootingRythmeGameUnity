using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTypeJudge : MonoBehaviour
{
    //Ű �Է� ���� ��Ʈ Ÿ�� ����, ��Ʈ�� �ճ�Ʈ�� ���� �ߺ� ������
    public string currentType = ""; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Note"))
            currentType = "Note";
        else if (other.CompareTag("LongNote"))
            currentType = "LongNote";
    }
}
