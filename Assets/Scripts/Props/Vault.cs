using UnityEngine;
using DLIFR.Data;

namespace DLIFR.Props
{
    public class Vault : Grabbable
    {
        private void FixedUpdate() 
        {
            beingGrabbed = false;
        }
    }
}