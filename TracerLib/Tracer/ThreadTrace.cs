﻿using System;
 using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TracerLib.Tracer
{
    
    [XmlType("thread")]
    public class ThreadTrace
    {
        [JsonProperty,XmlAttribute("id")] public int ThreadId { get; set; }
        [JsonProperty,XmlAttribute("time")] public long ThreadTime { get; set; }
        [JsonProperty,XmlElement("methods")] public List<MethodInfo> MethodInfo { get; set; }

        private int RootMethodCounter { get; set;}
        public ThreadTrace(int threadId)
        {
            ThreadId = threadId;
            MethodInfo = new List<MethodInfo>();
            RootMethodCounter = 0;
        }
        public ThreadTrace () {}

        public void PushMethod(string methodName, string className, string allMethodPath)
        {
            MethodInfo.Add(new MethodInfo(methodName, className, allMethodPath));
        }

        public void PopMethod(string allMethodPath)
        {
            var index = MethodInfo.FindLastIndex(item => item.GetAllMethodPath() == allMethodPath);
            if (index != MethodInfo.Count - 1)
            {
                var size = MethodInfo.Count - index - 1;
                
                var childMethods = MethodInfo.GetRange(index + 1, size);

                for (var i = 0; i < size; i++)
                    MethodInfo.RemoveAt(MethodInfo.Count - 1);

                MethodInfo[index].SetChildMethods(childMethods);
                MethodInfo[index].CalculateTime();
            }
            else
            {
                MethodInfo[index].CalculateTime();
            }

            if (index == RootMethodCounter)
            {
                RootMethodCounter++;
                ThreadTime += MethodInfo[index].GetTime();
            }
        }

    }
    
}