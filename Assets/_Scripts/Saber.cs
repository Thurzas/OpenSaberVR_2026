using UnityEngine;
using UnityEngine.XR;

public class Saber : MonoBehaviour
{
    public LayerMask layer;
    [SerializeField] private XRNode hand = XRNode.RightHand;

    private Vector3 previousPos;
    private Slice slicer;

    private float impactMagnifier = 120f;
    private float collisionForce = 0f;
    private float maxCollisionForce = 4000f;

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
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, layer))
        {
            var swingDirection = transform.position - previousPos;

            if (!string.IsNullOrWhiteSpace(hit.transform.tag) && hit.transform.CompareTag("CubeNonDirection"))
            {
                var bestAngle = Mathf.Max(
                    Vector3.Angle(swingDirection, hit.transform.up),
                    Vector3.Angle(swingDirection, hit.transform.right),
                    Vector3.Angle(swingDirection, -hit.transform.up),
                    Vector3.Angle(swingDirection, -hit.transform.right));

                if (bestAngle > 130)
                {
                    SliceObject(hit, bestAngle);
                }
            }
            else
            {
                var angle = Vector3.Angle(swingDirection, hit.transform.up);
                if (angle > 130)
                {
                    SliceObject(hit, angle);
                }
            }
        }

        previousPos = transform.position;
    }

    private void SliceObject(RaycastHit hit, float angle)
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
        ScoreManager.Instance?.RegisterHit(angle, centerDistance, hit.point);

        Pulse();

        Destroy(hittedObject.gameObject);
        Destroy(go, 2f);
    }
}
