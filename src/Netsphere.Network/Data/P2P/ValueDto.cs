﻿using BlubLib.Serialization;

namespace Netsphere.Network.Data.P2P
{
    [BlubContract]
    public class ValueDto
    {
        [BlubMember(0)]
        public float Value1 { get; set; }

        [BlubMember(1)]
        public float Value2 { get; set; }
    }
}
