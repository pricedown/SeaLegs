using System;
using UnityEngine;

namespace SeaLegs
{
    
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Water : MonoBehaviour
    {
       private MeshFilter _meshFilter;
       public float density = 1000;

       private void Awake()
       {
           _meshFilter = GetComponent<MeshFilter>();
       }

       private void Update()
       {
           Vector3[] vertices = _meshFilter.mesh.vertices;
           for (int i = 0; i < vertices.Length; i++)
               vertices[i].y = Waves.Instance.GetHeight(transform.position.x + (vertices[i].x * transform.localScale.x)) / transform.localScale.y ;
           
           _meshFilter.mesh.vertices = vertices;
           _meshFilter.mesh.RecalculateBounds();
           _meshFilter.mesh.RecalculateNormals();
       }
    }
}
