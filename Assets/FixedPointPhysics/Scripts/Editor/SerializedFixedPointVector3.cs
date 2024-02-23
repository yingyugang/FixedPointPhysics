using BlueNoah.Math.FixedPoint;
using UnityEditor;
using UnityEngine;

namespace BlueNoah.PhysicsEngine.Editor
{
    public sealed class SerializedFixedPointVector3
    {
        private readonly SerializedProperty x;
        private readonly SerializedProperty y;
        private readonly SerializedProperty z;
        public Vector3 vector3Value;
        private readonly string name;

        public SerializedFixedPointVector3(string name,SerializedProperty property)
        {
            x = property.FindPropertyRelative("x._serializedValue");
            y = property.FindPropertyRelative("y._serializedValue");
            z = property.FindPropertyRelative("z._serializedValue");
            this.name = name;
            vector3Value = new Vector3(FixedPoint64.FromRaw(x.longValue).AsFloat(),FixedPoint64.FromRaw(y.longValue).AsFloat(),FixedPoint64.FromRaw(z.longValue).AsFloat());
        }

        public void PropertyField()
        {
            vector3Value = EditorGUILayout.Vector3Field(name, vector3Value);
        }

        public void Apply()
        {
            var fixedPointVector3 = new FixedPointVector3(vector3Value);
            x.longValue = fixedPointVector3.x._serializedValue;
            y.longValue = fixedPointVector3.y._serializedValue;
            z.longValue = fixedPointVector3.z._serializedValue;
        }
    }
}