using UnityEngine;
using UnityEngine.AI;

public class chaseFish : MonoBehaviour
{
    public float chaseDistance = 10f; // �ִ� �ν� �Ÿ�
    public string fishTag = "Fish"; // ���� �±�
    public float updateInterval = 1f; // ��ǥ ���� �ֱ�

    public float offsetY = 0.5f; // Ÿ���� Y�� ������
    private GameObject currentTarget;
    private float timer = 0f;

    public float moveSpeed = 5f; // �̵� �ӵ�
    void Update()
    {
        timer += Time.deltaTime;
        if ( timer >= updateInterval)
        {
            currentTarget = FindNearestFish();
            timer = 0f;
        }

        if( currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (dist <= chaseDistance)
            {
                // Ÿ�� ��ġ (Y�� ������ ����)
                Vector3 targetPos = currentTarget.transform.position + new Vector3(0, offsetY, 0); ;

                // �̵� (�ӵ� = moveSpeed)
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

                // �ٶ󺸴� ���⵵ �ڿ������� ȸ��
                Vector3 direction = (targetPos - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 10f * Time.deltaTime);
                }
            }

        }
    }

    GameObject FindNearestFish()
    {
        GameObject[] fishes = GameObject.FindGameObjectsWithTag(fishTag);
        GameObject nearest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject fish in fishes)
        {
            float dist = Vector3.Distance(currentPos, fish.transform.position);
            if (dist < minDist && dist <= chaseDistance)
            {
                minDist = dist;
                nearest = fish;
            }
        }
        return nearest;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(fishTag))
        {
            Destroy(collision.gameObject);
            currentTarget = null; // �� Ÿ���� ���� Update���� �ڵ� Ž��
        }
    }
}

