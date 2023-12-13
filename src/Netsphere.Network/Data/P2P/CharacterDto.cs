﻿using System;
using System.Numerics;
using BlubLib.Serialization;
using BlubLib.Serialization.Serializers;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.P2P
{
    [BlubContract]
    public class CharacterDto
    {
        [BlubMember(0)]
        public LongPeerId Id { get; set; }

        [BlubMember(1)]
        public TeamId Team { get; set; }

        [BlubMember(2)]
        public Vector3 Position { get; set; }

        [BlubMember(3)]
        public Vector2 Rotation { get; set; }

        [BlubMember(4)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Costumes { get; set; }

        [BlubMember(5)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Skills { get; set; }

        [BlubMember(6)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Weapons { get; set; }

        [BlubMember(7)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ItemDto[] Weapons2 { get; set; }

        [BlubMember(8)]
        [BlubSerializer(typeof(EnumSerializer), typeof(uint))]
        public WeaponSlot CurrentWeapon { get; set; }

        [BlubMember(9)]
        public CharacterGender Gender { get; set; }

        [BlubMember(10)]
        public string Name { get; set; }

        [BlubMember(11)]
        public byte Unk2 { get; set; }

        [BlubMember(12)]
        public string Country { get; set; }

        [BlubMember(13)]
        [BlubSerializer(typeof(CompressedFloatSerializer))]
        public float CurrentHP { get; set; }

        [BlubMember(14)]
        [BlubSerializer(typeof(CompressedFloatSerializer))]
        public float MaxHP { get; set; }

        [BlubMember(15)]
        [BlubSerializer(typeof(CompressedFloatSerializer))]
        public float Unk3 { get; set; }

        [BlubMember(16)]
        [BlubSerializer(typeof(ArrayWithIntPrefixSerializer))]
        public ValueDto[] Values { get; set; }

        [BlubMember(17)]
        public byte Unk4 { get; set; }

        [BlubMember(18)]
        public byte Unk5 { get; set; }

        [BlubMember(19)]
        public byte Unk6 { get; set; }

        [BlubMember(20)]
        public byte Unk7 { get; set; }

        public CharacterDto()
        {
            Id = 0;
            CurrentWeapon = WeaponSlot.None;
            Position = Vector3.Zero;
            Costumes = Array.Empty<ItemDto>();
            Skills = Array.Empty<ItemDto>();
            Weapons = Array.Empty<ItemDto>();
            Name = "";
            Country = "";
            Values = Array.Empty<ValueDto>();
        }
    }
}
