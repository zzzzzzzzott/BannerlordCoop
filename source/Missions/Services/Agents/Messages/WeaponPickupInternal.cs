﻿using Common.Messaging;
using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace Missions.Services.Agents.Patches
{
    public readonly struct WeaponPickupInternal : IEvent
    {
        public Agent Agent { get; }
        public EquipmentIndex EquipmentIndex { get; }
        public ItemObject WeaponObject { get; }
        public ItemModifier WeaponModifier { get; }
        public Banner Banner { get; }

        public WeaponPickupInternal(Agent agent, EquipmentIndex equipmentIndex, ItemObject weaponObject, ItemModifier itemModifier, Banner banner)
        {
            Agent = agent;
            EquipmentIndex = equipmentIndex;
            WeaponObject = weaponObject;
            WeaponModifier = itemModifier;
            Banner = banner;
        }
    }
}