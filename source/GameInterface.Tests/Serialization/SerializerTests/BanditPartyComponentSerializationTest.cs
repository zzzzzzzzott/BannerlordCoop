﻿using GameInterface.Serialization.External;
using GameInterface.Serialization;
using System.Collections.Generic;
using Xunit;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using System.Runtime.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using System.Reflection;
using TaleWorlds.CampaignSystem.Party;
using GameInterface.Tests.Bootstrap;
using TaleWorlds.Library;
using Autofac;
using GameInterface.Tests.Bootstrap.Modules;
using Common.Serialization;

namespace GameInterface.Tests.Serialization.SerializerTests
{
    public class BanditPartyComponentSerializationTest
    {
        IContainer container;
        public BanditPartyComponentSerializationTest()
        {
            GameBootStrap.Initialize();

            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule<SerializationTestModule>();

            container = builder.Build();
        }

        [Fact]
        public void BanditPartyComponent_Serialize()
        {
            BanditPartyComponent item = (BanditPartyComponent)FormatterServices.GetUninitializedObject(typeof(BanditPartyComponent));

            var factory = container.Resolve<IBinaryPackageFactory>();
            BanditPartyComponentBinaryPackage package = new BanditPartyComponentBinaryPackage(item, factory);

            package.Pack();

            byte[] bytes = BinaryFormatterSerializer.Serialize(package);

            Assert.NotEmpty(bytes);
        }

        private static readonly FieldInfo Campaign_hideouts = typeof(Campaign).GetField("_hideouts", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        private static readonly PropertyInfo PartyComponent_MobileParty = typeof(PartyComponent).GetProperty(nameof(PartyComponent.MobileParty));
        private static readonly PropertyInfo BanditPartyComponent_Hideout = typeof(BanditPartyComponent).GetProperty(nameof(BanditPartyComponent.Hideout));
        private static readonly PropertyInfo BanditPartyComponent_IsBossParty = typeof(BanditPartyComponent).GetProperty(nameof(BanditPartyComponent.IsBossParty));
        private static readonly PropertyInfo PartyBase_MobileParty = typeof(PartyBase).GetProperty(nameof(PartyBase.MobileParty));
        private static readonly FieldInfo MobileParty_actualClan = typeof(MobileParty).GetField("_actualClan", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly PropertyInfo MobileParty_Party = typeof(MobileParty).GetProperty(nameof(MobileParty.Party));

        [Fact]
        public void BanditPartyComponent_Full_Serialization()
        {
            Hideout hideout = (Hideout)FormatterServices.GetUninitializedObject(typeof(Hideout));

            MBList<Hideout> allhideouts = (MBList<Hideout>)Campaign_hideouts.GetValue(Campaign.Current) ?? new MBList<Hideout>();

            allhideouts.Add(hideout);

            Campaign_hideouts.SetValue(Campaign.Current, allhideouts);

            MobileParty mobileParty = (MobileParty)FormatterServices.GetUninitializedObject(typeof(MobileParty));
            PartyBase party = (PartyBase)FormatterServices.GetUninitializedObject(typeof(PartyBase));
            Clan clan = (Clan)FormatterServices.GetUninitializedObject(typeof(Clan));

            mobileParty.StringId = "MyMobileParty";
            clan.StringId = "myClan";

            PartyBase_MobileParty.SetValue(party, mobileParty);
            MobileParty_Party.SetValue(mobileParty, party);
            MobileParty_actualClan.SetValue(mobileParty, clan);

            BanditPartyComponent item = (BanditPartyComponent)FormatterServices.GetUninitializedObject(typeof(BanditPartyComponent));
            BanditPartyComponent_Hideout.SetValue(item, hideout);
            BanditPartyComponent_IsBossParty.SetValue(item, true);
            PartyComponent_MobileParty.SetValue(item, mobileParty);

            var factory = container.Resolve<IBinaryPackageFactory>();
            BanditPartyComponentBinaryPackage package = new BanditPartyComponentBinaryPackage(item, factory);

            package.Pack();

            byte[] bytes = BinaryFormatterSerializer.Serialize(package);

            Assert.NotEmpty(bytes);

            object obj = BinaryFormatterSerializer.Deserialize(bytes);

            Assert.IsType<BanditPartyComponentBinaryPackage>(obj);

            BanditPartyComponentBinaryPackage returnedPackage = (BanditPartyComponentBinaryPackage)obj;

            var deserializeFactory = container.Resolve<IBinaryPackageFactory>();
            BanditPartyComponent newBanditPartyComponent = returnedPackage.Unpack<BanditPartyComponent>(deserializeFactory);

            Assert.Equal(item.IsBossParty, newBanditPartyComponent.IsBossParty);
            Assert.NotNull(newBanditPartyComponent.Hideout);
        }
    }
}
