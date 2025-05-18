using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Creotly_Studios
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Animation Rigging/Extract Transform Constraint")]
    public class ExtractTransformConstraint : RigConstraint<ExtractTransformConstraintJob,
        ExtractTransformConstraintData, ExtractTransformConstraintJobBinder>
    {

    }
}
