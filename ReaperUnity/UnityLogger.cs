using System;
using ReaperCore;
using UnityEngine;

namespace ReaperUnity
{
    public class UnityLogger : ILogger
    {
        public void LogError(Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
}
