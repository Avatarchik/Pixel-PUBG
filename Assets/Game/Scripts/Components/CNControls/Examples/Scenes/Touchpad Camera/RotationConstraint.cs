using UnityEngine;
using System.Collections;

namespace Examples.Scenes.TouchpadCamera
{
    public class RotationConstraint : MonoBehaviour
    {
        public float Min = -15f;
        public float Max = 15f;

        private Transform _transformCache;
        private Quaternion _minQuaternion;
        private Quaternion _maxQuaternion;
        private Vector3 _rotateAround;
        private float _range;

        private void Awake()
        {
            _transformCache = transform;

            _rotateAround = Vector3.right;
            var axisRotation = Quaternion.AngleAxis(_transformCache.localRotation.eulerAngles[0], _rotateAround);
            _minQuaternion = axisRotation * Quaternion.AngleAxis(Min, _rotateAround);
            _maxQuaternion = axisRotation * Quaternion.AngleAxis(Max, _rotateAround);
            _range = Max - Min;
        }

        private void LateUpdate()
        {
            var localRotation = _transformCache.localRotation;
            var axisRotation = Quaternion.AngleAxis(localRotation.eulerAngles[0], _rotateAround);
            var angleFromMin = Quaternion.Angle(axisRotation, _minQuaternion);
            var angleFromMax = Quaternion.Angle(axisRotation, _maxQuaternion);

            if (angleFromMin <= _range && angleFromMax <= _range)
            {
                return; // within range
            }
            else
            {
                // Let's keep the current rotations around other axes and only
                // correct the axis that has fallen out of range.
                var euler = localRotation.eulerAngles;
                if (angleFromMin > angleFromMax)
                    euler[0] = _maxQuaternion.eulerAngles[0];
                else
                    euler[0] = _minQuaternion.eulerAngles[0];

                _transformCache.localEulerAngles = euler;
            }
        }
    }
}
