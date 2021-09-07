using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour //������ � ���(3��)
{
    Vector3[] point = new Vector3[4]; //�ʱ���ġ �ڵ� 2�� Ÿ����ġ

    [SerializeField] [Range(0, 1)] private float t = 0; //�ð�
    [SerializeField] [Range(0, 1)] private float t2 = 0; //�ð�
    [SerializeField] public float spd = 3; //���ǵ�
    public float posA = 0.3f; //�ڵ��Ŀ
    public float posB = 0.7f;
    public float posC = 0.5f;

    public int hit = 0; //��������

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

        master = shooter.start; //���������δ� ä�� �������⶧���� �ʱ⿡ �޾ƿͼ� ����
        enemy = shooter.target;

        point[0] = master.transform.position; // P0
        point[1] = PointSetting(master.transform.position); // P1
        point[2] = PointSetting(enemy.transform.position); // P2
        point[3] = enemy.transform.position; // P3

        transform.position = shooter.gameObject.transform.position; //�ʱ� ��ġ �ʱ�ȭ
    }

    //Ȱ��ȭ�� ������ ����
    void OnEnable()
    {
        if (t2 >= 1) //���� ������ �ʹ� ���� �� �ֱ�
        {
            //ĳ���� ��ġ�� ��� �ٲ�Ƿ�
            point[0] = master.transform.position; // P0
            point[1] = PointSetting(master.transform.position); // P1
            point[2] = PointSetting(enemy.transform.position); // P2
            point[3] = enemy.transform.position; // P3

            transform.position = shooter.gameObject.transform.position; //�ʱ� ��ġ �ʱ�ȭ
        }
    }

    private void Update()
    {
        if (t2 < 1) t2 += Time.deltaTime;

        if (t > 1) return;//�ð� ���� �׸���
        
        t += Time.deltaTime * spd;
        DrawTrajectory();
    }

    private Vector2 PointSetting(Vector3 origin) //�̻��ϰ� Vector2�� �����°� ���������� ȿ���� �� ����, �Ͻ������� ���
    {
        float x, y, z;

        x = posA * Mathf.Cos(Random.Range(0, 360) * Mathf.Deg2Rad)
            + origin.x * 3; //x�࿡�� �� �������� ���Ͽ�
        y = posB * Mathf.Sin(Random.Range(0, 360) * Mathf.Deg2Rad)
            + origin.y * 0.5f; //y�࿡�� �� �������� ���Ͽ�
        z = posC * Mathf.Tan(Random.Range(0, 360) * Mathf.Deg2Rad)
            + origin.z;
        return new Vector3(x, y, z);
    }

    private void DrawTrajectory() //�׸��ºκ�
    {
        transform.position = new Vector3(
            FourPointBezier(point[0].x, point[1].x, point[2].x, point[3].x),
            FourPointBezier(point[0].y, point[1].y, point[2].y, point[3].y),
            FourPointBezier(point[0].z, point[1].z, point[2].z, point[3].z)
        );
       
    }

    private float FourPointBezier(float a, float b, float c, float d) //������ ����
    {
        return Mathf.Pow((1 - t), 3) * a
            + Mathf.Pow((1 - t), 2) * 3 * t * b
            + Mathf.Pow(t, 2) * 3 * (1 - t) * c
            + Mathf.Pow(t, 3) * d;
    }

    public IEnumerator EnqueueObject()
    {
        yield return new WaitForSeconds(0.3f); //0.5���Ŀ� ������Ʈ Ǯ���� �ٽ� �ְ� �ʱ�ȭ
        enemyStatus.DecreaseHP(playerStatus.hitDamage[hit]); //�� ü�� ó��
        yield return new WaitForSeconds(0.2f); //0.5���Ŀ� ������Ʈ Ǯ���� �ٽ� �ְ� �ʱ�ȭ
        ObjectPool.instance.queue[11].Enqueue(gameObject);//������ƮǮ�� ����ֱ�   
        gameObject.SetActive(false);
        t = 0;
        transform.position = shooter.gameObject.transform.position;
    }

}
