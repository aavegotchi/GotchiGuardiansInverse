using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if __MICROVERSE_VEGETATION__
namespace JBooth.MicroVerseCore
{
    [ExecuteAlways]
    [RequireComponent(typeof(ClearStamp))]
    public class BindHeightFilterRangeToTransform : MonoBehaviour
    {
        public Stamp target;
        public enum BindTarget
        {
            Minimum,
            Maximum
        }

        public enum ValueMode
        {
            Absolute,
            Relative
        }

        public BindTarget bindTarget = BindTarget.Minimum;
        public ValueMode valueMode = ValueMode.Absolute;

#if UNITY_EDITOR
        Stamp stamp;
        public void OnEnable()
        {
            stamp = GetComponent<Stamp>();
            OnMoved();
            cachedMtx = transform.localToWorldMatrix;
        }

        public void OnMoved()
        {
            cachedMtx = transform.localToWorldMatrix;
            if (stamp)
            {
                var fs = stamp.GetFilterSet();
                if (fs != null)
                {
                    var range = fs.heightFilter.range;
                    if (bindTarget == BindTarget.Minimum)
                    {
                        if (valueMode == ValueMode.Relative)
                        {
                            float rel = range.y - range.x;
                            range.x = transform.position.y;
                            range.y = range.x + rel;
                        }
                        else
                        {
                            range.x = transform.position.y;
                        }
                    }
                    else
                    {
                        if (valueMode == ValueMode.Relative)
                        {
                            float rel = range.y - range.x;
                            range.y = transform.position.y;
                            range.x = range.y - rel;
                        }
                        else
                        {
                            range.y = transform.position.y;
                        }
                    }
                    fs.heightFilter.range = range;
                    UnityEditor.EditorUtility.SetDirty(stamp);
                }
            }

        }

        Matrix4x4 cachedMtx;
        void Update()
        {
            if (cachedMtx != transform.localToWorldMatrix)
            {
                OnMoved();
            }
        }
#endif
    }
}
#endif
