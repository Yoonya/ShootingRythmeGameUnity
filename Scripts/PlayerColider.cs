using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColider : MonoBehaviour
{
    //이 스크립트는 캐릭터의 box collider가 애니메이션 모션 중에 rot가 도는 것으로 인해 판정이 이상하기 때문에 대체로 만들어짐

    [SerializeField]
    private Transform player;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = player.position;
    }
}
