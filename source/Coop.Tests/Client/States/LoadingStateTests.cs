﻿using Coop.Core.Client;
using Coop.Core.Client.States;
using GameInterface.Services.GameState.Messages;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Coop.Tests.Client.States
{
    public class LoadingStateTests : CoopTest
    {
        private readonly IClientLogic clientLogic;
        public LoadingStateTests(ITestOutputHelper output) : base(output)
        {
            var mockCoopClient = new Mock<ICoopClient>();
            clientLogic = new ClientLogic(mockCoopClient.Object, StubNetworkMessageBroker);
            clientLogic.State = new LoadingState(clientLogic);
        }

        [Fact]
        public void Dispose_RemovesAllHandlers()
        {
            Assert.NotEqual(0, StubMessageBroker.GetTotalSubscribers());

            clientLogic.State.Dispose();

            Assert.Equal(0, StubMessageBroker.GetTotalSubscribers());
        }

        [Fact]
        public void EnterMainMenu_Publishes_EnterMainMenuEvent()
        {
            var isEventPublished = false;
            StubMessageBroker.Subscribe<EnterMainMenu>((payload) =>
            {
                isEventPublished = true;
            });

            clientLogic.EnterMainMenu();

            Assert.True(isEventPublished);
        }

        [Fact]
        public void EnterMainMenu_Transitions_MainMenuState()
        {
            StubMessageBroker.Publish(this, new MainMenuEntered());

            Assert.IsType<MainMenuState>(clientLogic.State);
        }

        [Fact]
        public void GameLoaded_Transitions_CampaignState()
        {
            StubMessageBroker.Publish(this, new CampaignLoaded());

            Assert.IsType<CampaignState>(clientLogic.State);
        }

        [Fact]
        public void Disconnect_Publishes_EnterMainMenu()
        {
            var isEventPublished = false;
            StubMessageBroker.Subscribe<EnterMainMenu>((payload) =>
            {
                isEventPublished = true;
            });

            clientLogic.Disconnect();

            Assert.True(isEventPublished);
        }

        [Fact]
        public void OtherStateMethods_DoNotAlterState()
        {
            clientLogic.Connect();
            Assert.IsType<LoadingState>(clientLogic.State);

            clientLogic.Disconnect();
            Assert.IsType<LoadingState>(clientLogic.State);

            clientLogic.ExitGame();
            Assert.IsType<LoadingState>(clientLogic.State);

            clientLogic.LoadSavedData();
            Assert.IsType<LoadingState>(clientLogic.State);

            clientLogic.StartCharacterCreation();
            Assert.IsType<LoadingState>(clientLogic.State);

            clientLogic.EnterMissionState();
            Assert.IsType<LoadingState>(clientLogic.State);

            clientLogic.ValidateModules();
            Assert.IsType<LoadingState>(clientLogic.State);
        }
    }
}
