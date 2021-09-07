using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//star와 note가 사용
[System.Serializable]
public class ObjectInfo //오브젝트 내용 클래스
{
    public GameObject prefab;
    public int maxCount;
    public Transform parent;
}

//오브젝트 풀링기법
public class ObjectPool : MonoBehaviour
{
    [SerializeField] ObjectInfo[] objectInfos = null;

    public Queue<GameObject>[] queue = new Queue<GameObject>[24];

    public static ObjectPool instance; //공유자원으로 설정

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        queue[0] = InsertQueue(objectInfos[0]);
        queue[1] = InsertQueue(objectInfos[1]);
        queue[2] = InsertQueue(objectInfos[2]);
        queue[3] = InsertQueue(objectInfos[3]);
        queue[4] = InsertQueue(objectInfos[4]);
        queue[5] = InsertQueue(objectInfos[5]);
        queue[6] = InsertQueue(objectInfos[6]);
        queue[7] = InsertQueue(objectInfos[7]);
        queue[8] = InsertQueue(objectInfos[8]);
        queue[9] = InsertQueue(objectInfos[9]);
        queue[10] = InsertQueue(objectInfos[10]);
        queue[11] = InsertQueue(objectInfos[11]);
        queue[12] = InsertQueue(objectInfos[12]);
        queue[13] = InsertQueue(objectInfos[13]);
        queue[14] = InsertQueue(objectInfos[14]);
        queue[15] = InsertQueue(objectInfos[15]);
        queue[16] = InsertQueue(objectInfos[16]);
        queue[17] = InsertQueue(objectInfos[17]);
        queue[18] = InsertQueue(objectInfos[18]);
        queue[19] = InsertQueue(objectInfos[19]);
        queue[20] = InsertQueue(objectInfos[20]);
        queue[21] = InsertQueue(objectInfos[21]);
        queue[22] = InsertQueue(objectInfos[22]);
        queue[23] = InsertQueue(objectInfos[23]);
    }

    private Queue<GameObject> InsertQueue(ObjectInfo objectInfo) //오브젝트 풀링 생성
    {
        Queue<GameObject> tempQueue = new Queue<GameObject>();
        for (int i = 0; i < objectInfo.maxCount; i++) //갯수만큼 채워두기
        {
            GameObject clone = Instantiate(objectInfo.prefab, transform.position, Quaternion.identity); //객체생성
            clone.SetActive(false);

            //부모객체설정
            if (objectInfo.parent != null)
                clone.transform.SetParent(objectInfo.parent);
            else
                clone.transform.SetParent(this.transform);

            tempQueue.Enqueue(clone);
        }

        return tempQueue;
    }
}
