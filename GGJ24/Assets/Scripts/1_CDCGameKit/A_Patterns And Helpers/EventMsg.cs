using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDCGameKit
{
    public abstract class EventMsg
    {
        public delegate void Handler(EventMsg e);
    }
}