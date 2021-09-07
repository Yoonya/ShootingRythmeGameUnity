using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTypeJudge : MonoBehaviour
{
    //키 입력 직전 노트 타입 구별, 노트와 롱노트의 판정 중복 때문에
    public string currentType = ""; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Note"))
            currentType = "Note";
        else if (other.CompareTag("LongNote"))
            currentType = "LongNote";
    }
}
