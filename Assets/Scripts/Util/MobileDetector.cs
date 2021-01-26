using System.Runtime.InteropServices;
using UnityEngine;

public class MobileDetector : MonoBehaviour
{
    [DllImport("__Internal")]
     private static extern bool IsMobile();
     
     public bool isMobile()
     {
         #if !UNITY_EDITOR && UNITY_WEBGL
             return IsMobile();
         #endif
         return false;
     }
}
