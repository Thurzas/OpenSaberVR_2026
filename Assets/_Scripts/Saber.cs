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
            if (!string.IsNullOrWhiteSpace(hit.transform.tag) && hit.transform.CompareTag("CubeNonDirection"))
            {
                if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130 ||
                    Vector3.Angle(transform.position - previousPos, hit.transform.right) > 130 ||
                    Vector3.Angle(transform.position - previousPos, -hit.transform.up) > 130 ||
                    Vector3.Angle(transform.position - previousPos, -hit.transform.right) > 130)
                {
                    SliceObject(hit.transform);
                }
            }
            else
            {
                if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130)
                {
                    SliceObject(hit.transform);
                }
            }
        }

        previousPos = transform.position;
    }

    private void SliceObject(Transform hittedObject)
    {
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

        Pulse();

        Destroy(hittedObject.gameObject);
        Destroy(go, 2f);
    }
}
