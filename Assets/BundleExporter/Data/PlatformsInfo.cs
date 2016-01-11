using System.Collections.Generic;
using ProtoBuf;

namespace Assets.BundleExporter.Data
{
    [ProtoContract]
    public class PlatformsInfo
    {
        [ProtoMember(1)] 
        public int Version;
        [ProtoMember(2)]
        public List<PlatformInfo> Infos;
    }
}