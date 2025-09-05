#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class KeosLightprobeGenerator : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(10f, 10f, 10f);
    public Vector3 gridSpacing = new Vector3(2f, 2f, 2f);
    public List<GameObject> staticObjects = new List<GameObject>();
    public LayerMask staticObjectLayerMask;
    public bool showGizmo = true;

    public LightProbeGroup lp;

    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = new Color(0, 1, 0, 0.25f);
            Gizmos.DrawCube(transform.position, boxSize);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, boxSize);
        }
    }

    [ContextMenu("Generate Light Probes")]
    public void GenerateLightProbes()
    {
        if (!lp)
        {
            lp = gameObject.AddComponent<LightProbeGroup>();
        }

        Vector3[] probePositions = CalculateProbePositions();
        lp.probePositions = probePositions;

        Debug.Log($"Generated {probePositions.Length} light probes successfully.");
    }

    private Vector3[] CalculateProbePositions()
    {
        List<Vector3> probePositions = new List<Vector3>();

        Vector3 startPos = transform.position - (boxSize / 2) + (gridSpacing / 2);
        Vector3 endPos = transform.position + (boxSize / 2);

        for (float x = startPos.x; x <= endPos.x; x += gridSpacing.x)
        {
            for (float y = startPos.y; y <= endPos.y; y += gridSpacing.y)
            {
                for (float z = startPos.z; z <= endPos.z; z += gridSpacing.z)
                {
                    Vector3 probePosition = new Vector3(x, y, z);

                    if (IsValidProbePosition(ref probePosition))
                    {
                        probePositions.Add(probePosition);
                    }
                }
            }
        }

        return probePositions.ToArray();
    }

    private bool IsValidProbePosition(ref Vector3 position)
    {
        bool isValid = false;

        Vector3[] directions = new Vector3[]
        {
                Vector3.down, Vector3.up,
                Vector3.left, Vector3.right,
                Vector3.forward, Vector3.back
        };

        foreach (Vector3 direction in directions)
        {
            if (Physics.Raycast(position, direction, out RaycastHit hit, gridSpacing.magnitude, staticObjectLayerMask))
            {
                position = hit.point + (hit.normal * 0.1f);
                isValid = true;
                break;
            }
        }

        if (!isValid)
        {
            foreach (GameObject obj in staticObjects)
            {
                Collider objCollider = obj.GetComponent<Collider>();
                if (objCollider != null && objCollider.bounds.Contains(position))
                {
                    isValid = false;
                    break;
                }
            }
        }

        return isValid;
    }
}

[CustomEditor(typeof(KeosLightprobeGenerator))]
public class KeosLightprobeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        KeosLightprobeGenerator script = (KeosLightprobeGenerator)target;
        if (GUILayout.Button("Generate Light Probes"))
        {
            script.GenerateLightProbes();
        }
    }
}
#endif
