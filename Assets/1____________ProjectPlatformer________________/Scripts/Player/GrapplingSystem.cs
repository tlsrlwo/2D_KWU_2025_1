using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingSystem : MonoBehaviour
{
    [Header("����")]
    [SerializeField]
    private float               range = 10f;

    [Header("������Ʈ")]
    [SerializeField] 
    private LayerMask           hookableLayer;
    [SerializeField] 
    private List<Collider2D>    detectedHits = new List<Collider2D>();
    private LineRenderer        lr;
    private DistanceJoint2D     joint;
    private Vector2             target;
    private Vector2             grappleTarget;

    public bool                 isGrappling { get; private set; }

    void Start()
    {
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    public void TryGrapple()
    {
        detectedHits.Clear();

        // ���� �� ������Ʈ ����
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, hookableLayer);

        if (hits.Length == 0)
            return;

        // ...���� ����� Ÿ�� ã��
        Collider2D nearest = null;
        float shortest = Mathf.Infinity;

        foreach(var hit in hits)
        {
            detectedHits.Add(hit);

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if(distance < shortest)
            {
                shortest = distance;
                nearest = hit;
            }
        }

        if (nearest != null)
        {
            // �ش� ������Ʈ�� �ݶ��̴� ǥ�� �� ���� ����� ���� ���������� ���
            grappleTarget = nearest.ClosestPoint(transform.position);

            // DistanceJoint2D�� ����
            joint.connectedAnchor = grappleTarget;
            joint.autoConfigureDistance = false;
            joint.distance = Vector2.Distance(transform.position, grappleTarget);
            joint.enabled = true;

            // �� �ð�ȭ Ȱ��ȭ
            lr.enabled = true;
            lr.positionCount = 2;

            // ���� ����
            isGrappling = true;
        }
    }

    public void StopGrapple()
    {
        joint.enabled = false;
        lr.enabled = false;
        isGrappling = false;
    }

    public void UpdateGrappleLine()
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, grappleTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
