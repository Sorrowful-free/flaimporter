using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;

namespace Assets.BundleExporter.Data
{
    [ProtoContract]
    public class PlatformInfo
    {
        [ProtoMember(1)]
        public RuntimePlatform Platform;

        [ProtoMember(2)]
        public List<LanguageInfo> LanguagesInfos = new List<LanguageInfo>();
    }
}