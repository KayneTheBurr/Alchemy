using System.Collections.Generic;
using UnityEngine;

public class OcclusionFader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform target;               
    [SerializeField] Camera cam;                     

    [Header("Cast")]
    [SerializeField] LayerMask occluderMask = ~0;
    [SerializeField] float castRadius = 0.6f;
    [SerializeField] Vector3 targetOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] float stopShortOfTarget = 0.5f;
    [SerializeField] int maxHits = 32;

    RaycastHit[] hits;
    readonly HashSet<FadeableObject> current = new HashSet<FadeableObject>();
    readonly HashSet<FadeableObject> previous = new HashSet<FadeableObject>();
    readonly Dictionary<Collider, FadeableObject> lookup = new Dictionary<Collider, FadeableObject>();

    void Awake()
    {
        if (!cam) cam = Camera.main;
        hits = new RaycastHit[maxHits];
    }

    void LateUpdate()
    {
        if (!target || !cam) return;

        Vector3 origin = cam.transform.position;
        Vector3 end = target.position + targetOffset;
        Vector3 toTarget = end - origin;
        float fullDist = toTarget.magnitude;
        float castDist = fullDist - stopShortOfTarget;
        if (castDist <= 0f) return;

        Vector3 dir = toTarget / fullDist;
        int count = Physics.SphereCastNonAlloc(origin, castRadius, dir, hits, castDist,
                                               occluderMask, QueryTriggerInteraction.Ignore);

        current.Clear();
        for (int i = 0; i < count; i++)
        {
            FadeableObject f = GetFadeable(hits[i].collider);
            if (f) current.Add(f);
        }

        foreach (FadeableObject f in current)
            f.SetOccluding(true);

        foreach (FadeableObject f in previous)
            if (f && !current.Contains(f))
                f.SetOccluding(false);

        previous.Clear();
        previous.UnionWith(current);
    }

    // Cache collider -> FadeableObject so we don't GetComponentInParent every frame.
    FadeableObject GetFadeable(Collider col)
    {
        if (!lookup.TryGetValue(col, out FadeableObject f))
        {
            f = col.GetComponentInParent<FadeableObject>();
            lookup[col] = f; // caches nulls too
        }
        return f;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!target) return;
        Camera c = cam ? cam : Camera.main;
        if (!c) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(c.transform.position, target.position + targetOffset);
        Gizmos.DrawWireSphere(target.position + targetOffset, castRadius);
    }
#endif
}