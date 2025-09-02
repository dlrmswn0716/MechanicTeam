using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindEnemy : MonoBehaviour
{
    // ���� �Ÿ�
    [SerializeField] private float detectionDistance = 10f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float rayInterval = 1f;
    //���� ����
    [SerializeField] private float coneAngle = 60f;
    [SerializeField] private bool showArea = true;

    private List<Transform> detectedPlayers = new List<Transform>();
    private Coroutine detectionCoroutine;

    //�ڷ�ƾ ��ȯ
    void Start()
    {
        detectionCoroutine = StartCoroutine(DetectionLoop());
    }

    IEnumerator DetectionLoop()
    {
        while (true)
        {
            DetectAllObjectsInTriangle();
            yield return new WaitForSeconds(rayInterval);
        }
    }

    //�� ���� -> �ﰢ�� ���� -> �÷��̾� ���� Ȯ��
    void DetectAllObjectsInTriangle()
    {
        detectedPlayers.Clear();

        // 1�ܰ�: ���� �� ��� �ݶ��̴� ã�� (�� ������ ��ĵ)
        Collider[] allColliders = Physics.OverlapSphere(transform.position, detectionDistance);

        foreach (var collider in allColliders)
        {
            // 2�ܰ�: �÷��̾� �±� üũ
            if (!collider.CompareTag(playerTag)) continue;

            // 3�ܰ�: �ﰢ�� ���� ���� �ִ��� üũ
            if (IsInTriangleRange(collider.transform.position))
            {
                // 4�ܰ�: ó�� �浹�� ����ĳ��Ʈ�� �˻�
                if (HasClearLineOfSight(collider.transform.position))
                {
                    detectedPlayers.Add(collider.transform);
                    OnPlayerDetected(collider.transform);
                }
            }
        }

        if (detectedPlayers.Count > 0)
            Debug.Log($"�ﰢ�� ���� ��ü �˻� - ������ �÷��̾�: {detectedPlayers[0].name}");
        else
            Debug.Log("�÷��̾ ã�� ����.");
    }

    bool IsInTriangleRange(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // ������ �Ÿ� ��� üũ
        return angleToTarget <= coneAngle * 0.5f && distanceToTarget <= detectionDistance;
    }

    bool HasClearLineOfSight(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget))
        {
            // Ÿ���� �´��� Ȯ�� (��ֹ��� �ƴ�)
            return hit.collider.CompareTag(playerTag);
        }

        return true; // �ƹ��͵� ���� ����
    }

    private void OnPlayerDetected(Transform player)
    {
        float distance = Vector3.Distance(transform.position, player.position);
        Debug.Log($"�÷��̾� �߰�: {player.name}, �Ÿ�: {distance:F2}m");
    }

    //TO-DO : �ﰢ�� �ð�ȭ �ڵ� (�ϼ� �� ����)
    private void OnDrawGizmos()
    {
        if (!showArea) return;

        // �ﰢ�� ���� �ð�ȭ
        float halfAngle = coneAngle * 0.5f;
        Vector3 leftBoundary = Quaternion.AngleAxis(-halfAngle, Vector3.up) * transform.forward * detectionDistance;
        Vector3 rightBoundary = Quaternion.AngleAxis(halfAngle, Vector3.up) * transform.forward * detectionDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position + leftBoundary, transform.position + rightBoundary);

        // ������ �÷��̾�� ǥ��
        Gizmos.color = Color.red;
        foreach (var player in detectedPlayers)
        {
            if (player != null)
            {
                Gizmos.DrawLine(transform.position, player.position);
                Gizmos.DrawWireSphere(player.position, 0.5f);
            }
        }
    }
}
