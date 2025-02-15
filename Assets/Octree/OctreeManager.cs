using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public enum OctreeDebugMode {
    AllDepth,
    TargetDepth
}

public class OctreeManager : MonoBehaviour {
    // the number of generating objects;
    [Range(0, 500)]
    public int genCount = 100;

    // the depth of octree
    [Range(1, 8)]
    public int buildDepth = 3;

    // the root of octree
    public OctreeNode root;

    // the space of generating objects;
    [Range(1, 300)]
    public float spaceRange = 100f;

    public bool showOctree = true;
    public OctreeDebugMode octreeDebugMode;
    [Range(0, 8)]
    public int displayDepth = 3;
    // visual color in different depth
    public Color[] displayColor;

    // save scene objects
    private List<GameObject> sceneObjects;

    private void Start() {
        GenSceneObjects();
        OctreePartion();
    }

    // generate Cubes
    private void GenSceneObjects() {
        var genRange = spaceRange * 0.5f;
        sceneObjects = new List<GameObject>();

        for (int i = 0; i < genCount; i++) {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = new Vector3(
                Random.Range(-genRange, genRange),
                Random.Range(-genRange, genRange),
                Random.Range(-genRange, genRange));
            obj.hideFlags = HideFlags.HideInHierarchy;
            sceneObjects.Add(obj);
        }
    }

    private void OctreePartion() {
        var initialOrigin = Vector3.zero;
        root = new OctreeNode(initialOrigin, spaceRange);
        root.areaObjects = sceneObjects;
        GenerateOctree(root, spaceRange, buildDepth);
    }

    private void GenerateOctree(OctreeNode root, float space, int depth) {
        if (depth <= 0) return; // do nothing if depth <= 0;

        // calc the center and size of grid;
        var halfRange = space / 2.0f;
        var rootOffset = halfRange / 2.0f;
        var rootCenter = root.center;

        var origin = rootCenter + new Vector3(-1, 1, -1) * rootOffset;
        root.top0 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(1, 1, -1) * rootOffset;
        root.top1 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(1, 1, 1) * rootOffset;
        root.top2 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(-1, 1, 1) * rootOffset;
        root.top3 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(-1, -1, -1) * rootOffset;
        root.bottom0 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(1, -1, -1) * rootOffset;
        root.bottom1 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(1, -1, 1) * rootOffset;
        root.bottom2 = new OctreeNode(origin, halfRange);

        origin = rootCenter + new Vector3(-1, -1, 1) * rootOffset;
        root.bottom3 = new OctreeNode(origin, halfRange);

        // traverse objects in current space, and allocate object to subnode
        PartitionSceneObjects(root);

        // generate octree if there are too many objects in current node
        if (root.top0.objectCount >= 2)
            GenerateOctree(root.top0, halfRange, depth - 1);

        if (root.top1.objectCount >= 2)
            GenerateOctree(root.top1, halfRange, depth - 1);

        if (root.top2.objectCount >= 2)
            GenerateOctree(root.top2, halfRange, depth - 1);

        if (root.top3.objectCount >= 2)
            GenerateOctree(root.top3, halfRange, depth - 1);

        if (root.bottom0.objectCount >= 2)
            GenerateOctree(root.bottom0, halfRange, depth - 1);

        if (root.bottom1.objectCount >= 2)
            GenerateOctree(root.bottom1, halfRange, depth - 1);

        if (root.bottom2.objectCount >= 2)
            GenerateOctree(root.bottom2, halfRange, depth - 1);

        if (root.bottom3.objectCount >= 2)
            GenerateOctree(root.bottom3, halfRange, depth - 1);


    }

    private void PartitionSceneObjects(OctreeNode root) {
        var objcets = root.areaObjects;
        foreach (var obj in objcets) {
            if (root.top0.IsContains(obj.transform.position)) {
                root.top0.AddGameObject(obj);
            }
            else if (root.top1.IsContains(obj.transform.position)) {
                root.top1.AddGameObject(obj);
            }
            else if (root.top2.IsContains(obj.transform.position)) {
                root.top2.AddGameObject(obj);
            }
            else if (root.top3.IsContains(obj.transform.position)) {
                root.top3.AddGameObject(obj);
            }
            else if (root.bottom0.IsContains(obj.transform.position)) {
                root.bottom0.AddGameObject(obj);
            }
            else if (root.bottom1.IsContains(obj.transform.position)) {
                root.bottom1.AddGameObject(obj);
            }
            else if (root.bottom2.IsContains(obj.transform.position)) {
                root.bottom2.AddGameObject(obj);
            }
            else if (root.bottom3.IsContains(obj.transform.position)) {
                root.bottom3.AddGameObject(obj);
            }
        }
    }

    private void OnDrawGizmos() {
        if (root == null) return;

        if (showOctree && displayDepth <= buildDepth) {
            // show space range in all depth
            if (octreeDebugMode == OctreeDebugMode.AllDepth) {
                Gizmos.color = new Color(1, 1, 1, 0.2f);
            }
            // show space range in specified depth
            else if (octreeDebugMode == OctreeDebugMode.TargetDepth) { 
                if (displayColor.Length > displayDepth) {
                    var color = displayColor[displayDepth];
                    color.a = 0.2f;
                    Gizmos.color = color;
                    DrawTargetDepth(root, displayDepth);
                }
            }
        }
    }

    // show something in specified depth
    private void DrawTargetDepth(OctreeNode node, int depth) {
        if (node == null) return;

        if (depth < 0) {
            node.DrawGizmos();
            return;
        }

        var nextDepth = depth - 1;
        var kid = node.top0;
        DrawTargetDepth(kid, nextDepth);

        kid = node.top1;
        DrawTargetDepth(kid, nextDepth);

        kid = node.top2;
        DrawTargetDepth(kid, nextDepth);

        kid = node.top3;
        DrawTargetDepth(kid, nextDepth);

        kid = node.bottom0;
        DrawTargetDepth(kid, nextDepth);

        kid = node.bottom1;
        DrawTargetDepth(kid, nextDepth);

        kid = node.bottom2;
        DrawTargetDepth(kid, nextDepth);

        kid = node.bottom3;
        DrawTargetDepth(kid, nextDepth);
    }

    // show everything
    private void DrawNode(OctreeNode node, int depth) {
        if (node == null) return;

        if (depth > 0 && depth < displayColor.Length) {
            var color = displayColor[depth];
            color.a = 0.5f;
            Gizmos.color = color;
            node.DrawGizmos();
        }

        var kid = node.top0;
        DrawNode(kid, depth - 1);

        kid = node.top1;
        DrawNode(kid, depth - 1);

        kid = node.top2;
        DrawNode(kid, depth - 1);

        kid = node.top3;
        DrawNode(kid, depth - 1);

        kid = node.bottom0;
        DrawNode(kid, depth - 1);

        kid = node.bottom1;
        DrawNode(kid, depth - 1);

        kid = node.bottom2;
        DrawNode(kid, depth - 1);

        kid = node.bottom3;
        DrawNode(kid, depth - 1);
    }
}

