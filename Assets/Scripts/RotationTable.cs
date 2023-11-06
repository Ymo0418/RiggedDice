using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RotationTableData")]
public class RotationTable : ScriptableObject
{
    [System.Serializable]
    public class cl
    {
        public Vector3[] to = new Vector3[6];
    }

    public cl[] needRotation = new cl[6];
}
