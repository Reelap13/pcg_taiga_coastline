using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Objects
{
    public class ObjectData
    {
        private GameObject _prefab;
        private Vector3 _position;
        private Quaternion _rotation;

        public ObjectData(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            _prefab = prefab;
            _position = position;
            _rotation = rotation;
        }

        public GameObject Prefab { get { return _prefab; } }
        public Vector3 Position { get { return _position; } }
        public Quaternion Rotation { get { return _rotation; } }
    }
}