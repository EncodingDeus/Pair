using UnityEngine;

namespace Dobrozaur.Utility
{
    public static  class AnimatorUtils
    {
        public static bool IsPlaying(this Animator anim)
        {
            return anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
        }
        
        
        public static bool IsSetState(this Animator anim, string stateName)
        {
            var t = anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
            var n = anim.GetCurrentAnimatorStateInfo(0);
            return t;
        }

    }
}