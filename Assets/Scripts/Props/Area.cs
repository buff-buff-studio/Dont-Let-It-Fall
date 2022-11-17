using System;
using System.Collections.Generic;
using UnityEngine;

namespace DLIFR.Props
{
    public class Area : MonoBehaviour
    {
        public List<Cargo> cargoes = new List<Cargo>();
        public Action onChange;
    }
}