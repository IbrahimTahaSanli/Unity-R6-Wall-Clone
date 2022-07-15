using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree : MonoBehaviour
{
    [SerializeField] Vector3 objectScale;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject DebugCube;

    [SerializeField] float radius;
    [SerializeField] int samplePoint;

    [SerializeField] bool DebugBox;

    QuadTreeStruct quadTree;

    public static QuadTree staTree;

    private void Awake()
    {
        QuadTree.staTree = this; 

        this.quadTree = new QuadTreeStruct(objectScale);

        CreateObjectStruct();
    }

    public void ObjectHit(Vector3 Pos)
    {
        QuadTreeStruct.DestroyCircle(this.quadTree.RootNode, this.transform.InverseTransformPoint(Pos), radius, samplePoint);

        foreach(Transform obj in this.transform)
        {
            Destroy(obj.gameObject);
        }
        QuadTreeStruct.OptimizeTree(this.quadTree.RootNode);

        CreateObjectStruct();

    }





    public void CreateObjectStruct(Node node = null, GameObject parentObj = null)
    {
        if (node == null)
            node = this.quadTree.RootNode;

        if (parentObj == null)
            parentObj = this.gameObject;

        if (!node.isDestroy)
        {
            if (node.IsNodeContainer())
            {
                GameObject containerObj = new GameObject("cont");
                containerObj.transform.parent = parentObj.transform;
                containerObj.transform.localRotation = Quaternion.identity;
                containerObj.transform.localPosition = Vector3.zero;

                if (node.leftTop != null) CreateObjectStruct(node.leftTop, containerObj);
                if (node.rightTop != null) CreateObjectStruct(node.rightTop, containerObj);
                if (node.leftBottom != null) CreateObjectStruct(node.leftBottom, containerObj);
                if (node.rightBottom != null) CreateObjectStruct(node.rightBottom, containerObj);
            }
            else
            {
                GameObject wallPiece = Instantiate(wallPrefab, parentObj.transform);
                wallPiece.transform.name = node.side.ToString();
                wallPiece.transform.localScale = (Vector3)node;
                wallPiece.transform.localRotation = wallPrefab.transform.localRotation;


                wallPiece.transform.localPosition = node.Position;
            }
        }

    }
}
