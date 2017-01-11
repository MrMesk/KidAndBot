using UnityEngine;
using System.Collections;

namespace Utility {
    public struct PointData {
        public Vector3 position;
        public Vector3 normal;

        public PointData(Vector3 position, Vector3 normal) {
            this.position = position;
            this.normal = normal;
        }
    }
}