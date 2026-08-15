using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Saber : MonoBehaviour
{
    public LayerMask layer;
    [SerializeField] private XRNode hand = XRNode.RightHand;
    [SerializeField] private float preSwingWindow = 0.2f;
    [SerializeField] private float postSwingWindow = 0.15f;

    private Vector3 previousPos;
    private Slice slicer;

    private float impactMagnifier = 120f;
    private float collisionForce = 0f;
    private float maxCollisionForce = 4000f;

    private readonly Queue<(float time, Vector3 position)> positionHistory = new Queue<(float, Vector3)>();

    private void Start()
    {
        slicer = GetComponentInChildren<Slice>(true);
    }

    private void Pulse()
    {
        var device = InputDevices.GetDeviceAtXRNode(hand);
        if (!device.isValid || !device.TryGetFeatureValue(CommonUsages.deviceVelocity, out var velocity))
        {
            return;
        }

        collisionForce = velocity.magnitude * impactMagnifier;
        var hapticStrength = Mathf.Clamp01(collisionForce / maxCollisionForce);

        if (device.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
        {
            device.SendHapticImpulse(0u, hapticStrength, 0.5f);
        }
    }

    void Update()
    {
        positionHistory.Enqueue((Time.time, transform.position));
        while (positionHistory.Count > 0 && Time.time - positionHistory.Peek().time > preSwingWindow)
        {
            positionHistory.Dequeue();
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, layer))
        {
            var swingDirection = transform.position - previousPos;

            if (!string.IsNullOrWhiteSpace(hit.transform.tag) && hit.transform.CompareTag("CubeNonDirection"))
            {
                var cutAxis = GetBestAxis(swingDirection, hit.transform);
                var bestAngle = Vector3.Angle(swingDirection, cutAxis);

                if (bestAngle > 130)
                {
                    SliceObject(hit, cutAxis);
                }
            }
            else
            {
                var angle = Vector3.Angle(swingDirection, hit.transform.up);
                if (angle > 130)
                {
                    SliceObject(hit, hit.transform.up);
                }
            }
        }

        previousPos = transform.position;
    }

    private Vector3 GetBestAxis(Vector3 swingDirection, Transform cube)
    {
        Vector3[] axes = { cube.up, cube.right, -cube.up, -cube.right };
        var best = axes[0];
        var bestAngle = -1f;

        foreach (var axis in axes)
        {
            var angle = Vector3.Angle(swingDirection, axis);
            if (angle > bestAngle)
            {
                bestAngle = angle;
                best = axis;
            }
        }

        return best;
    }

    private void SliceObject(RaycastHit hit, Vector3 cutAxis)
    {
        var hittedObject = hit.transform;
        var cutted = slicer.SliceObject(hittedObject.gameObject);
        var go = Instantiate(hittedObject.gameObject);

        go.GetComponent<CubeHandling>().enabled = false;
        go.GetComponentInChildren<BoxCollider>().enabled = false;
        go.layer = 0;

        foreach (var renderer in go.transform.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = false;
        }

        foreach (var cut in cutted)
        {
            cut.transform.SetParent(go.transform);
            cut.AddComponent<BoxCollider>();
            var rigid = cut.AddComponent<Rigidbody>();
            rigid.useGravity = true;
        }

        go.transform.SetPositionAndRotation(hittedObject.position, hittedObject.rotation);

        var centerDistance = Vector3.Distance(hit.point, hittedObject.position);
        var preSwingAngle = ComputePreSwingAngle(cutAxis);
        var hitPosition = hit.point;

        Pulse();

        Destroy(hittedObject.gameObject);
        Destroy(go, 2f);

        StartCoroutine(ResolvePostSwingAndScore(cutAxis, preSwingAngle, centerDistance, hitPosition));
    }

    private float ComputePreSwingAngle(Vector3 cutAxis)
    {
        if (positionHistory.Count == 0)
        {
            return 0f;
        }

        var windupStart = positionHistory.Peek().position;
        var preSwingDirection = transform.position - windupStart;

        return preSwingDirection.sqrMagnitude > 0.0001f ? Vector3.Angle(preSwingDirection, cutAxis) : 0f;
    }

    private IEnumerator ResolvePostSwingAndScore(Vector3 cutAxis, float preSwingAngle, float centerDistance, Vector3 hitPosition)
    {
        var startPos = transform.position;

        yield return new WaitForSeconds(postSwingWindow);

        var postSwingDirection = transform.position - startPos;
        var postSwingAngle = postSwingDirection.sqrMagnitude > 0.0001f
            ? Vector3.Angle(postSwingDirection, cutAxis)
            : 0f;

        ScoreManager.Instance?.RegisterHit(preSwingAngle, postSwingAngle, centerDistance, hitPosition);
    }
}