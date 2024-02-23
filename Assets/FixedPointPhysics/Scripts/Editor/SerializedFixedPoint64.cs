using BlueNoah.Math.FixedPoint;
using UnityEditor;

namespace BlueNoah.PhysicsEngine.Editor
{
    public sealed class SerializedFixedPoint64
    {
        private readonly SerializedProperty v;
        public float floatValue;
        private readonly string name;

        public SerializedFixedPoint64(string name,SerializedProperty property)
        {
            v = property.FindPropertyRelative("_serializedValue");
            this.name = name;
            floatValue = FixedPoint64.FromRaw(v.longValue).AsFloat();
        }

        public void PropertyField()
        {
            floatValue = EditorGUILayout.FloatField(name, floatValue);
        }

        public void Apply()
        {
            // Can't be Undo.
            FixedPoint64 fixedPointFloat = floatValue;
            v.longValue = fixedPointFloat._serializedValue;
        }
    }
}