using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour //베지어 곡선 기법(3차)
{
    Vector3[] point = new Vector3[4]; //초기위치 핸들 2개 타겟위치

    [SerializeField] [Range(0, 1)] private float t = 0; //시간
    [SerializeField] [Range(0, 1)] private float t2 = 0; //시간
    [SerializeField] public float spd = 3; //스피드
    public float posA = 0.3f; //핸들앵커
    public float posB = 0.7f;
    public float posC = 0.5f;

    public int hit = 0; //판정종류

    private GameObject master;
    private GameObject enemy;

    private PlayerShooter shooter;
    private EnemyStatus enemyStatus;
    private PlayerStatus playerStatus;

    void Start()
    {
        shooter = FindObjectOfType<PlayerShooter>();
        enemyStatus = FindObjectOfType<EnemyStatus>();
        playerStatus = FindObjectOfType<PlayerStatus>();

        master = shooter.start; //프리펩으로는 채울 수가없기때문에 초기에 받아와서 설정
        enemy = shooter.target;

        point[0] = master.transform.position; // P0
        point[1] = PointSetting(master.transform.position); // P1
        point[2] = PointSetting(enemy.transform.position); // P2
        point[3] = enemy.transform.position; // P3

        transform.position = shooter.gameObject.transform.position; //초기 위치 초기화
    }

    //활성화될 때마다 실행
    void OnEnable()
    {
        if (t2 >= 1) //실행 순서상 너무 빨라서 텀 주기
        {
            //캐릭터 위치는 계속 바뀌므로
            point[0] = master.transform.position; // P0
            point[1] = PointSetting(master.transform.position); // P1
            point[2] = PointSetting(enemy.transform.position); // P2
            point[3] = enemy.transform.position; // P3

            transform.position = shooter.gameObject.transform.position; //초기 위치 초기화
        }
    }

    private void Update()
    {
        if (t2 < 1) t2 += Time.deltaTime;

        if (t > 1) return;//시간 동안 그리기
        
        t += Time.deltaTime * spd;
        DrawTrajectory();
    }

    private Vector2 PointSetting(Vector3 origin) //이상하게 Vector2로 보내는게 가시적으로 효과가 더 좋음, 일시적으로 사용
    {
        float x, y, z;

        x = posA * Mathf.Cos(Random.Range(0, 360) * Mathf.Deg2Rad)
            + origin.x * 3; //x축에서 더 벌어지기 위하여
        y = posB * Mathf.Sin(Random.Range(0, 360) * Mathf.Deg2Rad)
            + origin.y * 0.5f; //y축에서 덜 벌어지기 위하여
        z = posC * Mathf.Tan(Random.Range(0, 360) * Mathf.Deg2Rad)
            + origin.z;
        return new Vector3(x, y, z);
    }

    private void DrawTrajectory() //그리는부분
    {
        transform.position = new Vector3(
            FourPointBezier(point[0].x, point[1].x, point[2].x, point[3].x),
            FourPointBezier(point[0].y, point[1].y, point[2].y, point[3].y),
            FourPointBezier(point[0].z, point[1].z, point[2].z, point[3].z)
        );
       
    }

    private float FourPointBezier(float a, float b, float c, float d) //베지어곡선 공식
    {
        return Mathf.Pow((1 - t), 3) * a
            + Mathf.Pow((1 - t), 2) * 3 * t * b
            + Mathf.Pow(t, 2) * 3 * (1 - t) * c
            + Mathf.Pow(t, 3) * d;
    }

    public IEnumerator EnqueueObject()
    {
        yield return new WaitForSeconds(0.3f); //0.5초후에 오브젝트 풀링을 다시 넣고 초기화
        enemyStatus.DecreaseHP(playerStatus.hitDamage[hit]); //적 체력 처리
        yield return new WaitForSeconds(0.2f); //0.5초후에 오브젝트 풀링을 다시 넣고 초기화
        ObjectPool.instance.queue[11].Enqueue(gameObject);//오브젝트풀링 집어넣기   
        gameObject.SetActive(false);
        t = 0;
        transform.position = shooter.gameObject.transform.position;
    }

}
