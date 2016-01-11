using ProtoBuf;
using UnityEngine;

namespace Assets.BundleExporter.Data
{
    [ProtoContract]
    public class LanguageInfo
    {
        [ProtoMember(1)]
        public SystemLanguage Language;
        [ProtoMember(2)]
        public string Url;
        [ProtoMember(3)] 
        public string IconPath;
    }
}