using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OctreeNode {
    // objects in space
    public List<GameObject> areaObjects;
    // center of space
    public Vector3 center;
    // size of space
    public float size;

    private const int KIDNUMBERS = 8;

    private OctreeNode[] kids;

    public OctreeNode(Vector3 center, float size) {
        this.center = center;
        this.size = size;

        kids = new OctreeNode[KIDNUMBERS];
        areaObjects = new List<GameObject>();
    }

    public OctreeNode[] GetKids { get { return kids; } }
    public int objectCount => areaObjects.Count;

    public void DrawGizmos() {
        Gizmos.DrawWireCube(center, Vector3.one * size);
    }

    public bool IsContains(Vector3 position) {
        var halfSize = size * 0.5f;
        return Mathf.Abs(position.x - center.x) < halfSize &&
               Mathf.Abs(position.y - center.y) < halfSize &&
               Mathf.Abs(position.z - center.z) < halfSize;
    }

    public void ClearArea() { areaObjects.Clear(); }
    public void AddGameObject(GameObject gameObject) { areaObjects.Add(gameObject); }

    public OctreeNode top0 {
        get {
            return kids[0];
        }
        set {
            kids[0] = value;
        }
    }

    public OctreeNode top1 {
        get {
            return kids[1];
        }
        set {
            kids[1] = value;
        }
    }

    public OctreeNode top2 {
        get {
            return kids[2];
        }
        set {
            kids[2] = value;
        }
    }

    public OctreeNode top3 {
        get {
            return kids[3];
        }
        set {
            kids[3] = value;
        }
    }

    public OctreeNode bottom0 {
        get {
            return kids[4];
        }
        set {
            kids[4] = value;
        }
    }

    public OctreeNode bottom1 {
        get {
            return kids[5];
        }
        set {
            kids[5] = value;
        }
    }

    public OctreeNode bottom2 {
        get {
            return kids[6];
        }
        set {
            kids[6] = value;
        }
    }

    public OctreeNode bottom3 {
        get {
            return kids[7];
        }
        set {
            kids[7] = value;
        }
    }
}
