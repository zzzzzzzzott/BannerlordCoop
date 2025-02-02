﻿using Common.Logging;
using Common.Messaging;
using Common.Network;
using Coop.Core.Server.Connections.States;
using LiteNetLib;
using Serilog;

namespace Coop.Core.Server.Connections
{
    public interface IConnectionLogic : IConnectionState
    {
        NetPeer PlayerId { get; }
        string HeroStringId { get; set; }
        INetworkMessageBroker NetworkMessageBroker { get; }
        IConnectionState State { get; set; }    
    }

    public class ConnectionLogic : IConnectionLogic
    {
        private readonly ILogger Logger = LogManager.GetLogger<ConnectionLogic>();

        public NetPeer PlayerId { get; }
        public string HeroStringId { get; set; }

        public INetworkMessageBroker NetworkMessageBroker { get; }

        public IConnectionState State 
        {
            get { return _state; }
            set
            {
                Logger.Debug("Connection is changing to {state} State", value.GetType().Name);
                _state?.Dispose();
                _state = value;
            }
        }        

        private IConnectionState _state;

        public ConnectionLogic(NetPeer playerId, INetworkMessageBroker messageBroker)
        {
            PlayerId = playerId;
            NetworkMessageBroker = messageBroker;
            State = new ResolveCharacterState(this);
        }

        public void Dispose() => State.Dispose();

        public void CreateCharacter() => State.CreateCharacter();

        public void TransferSave() => State.TransferSave();

        public void Load() => State.Load();

        public void EnterCampaign() => State.EnterCampaign();

        public void EnterMission() => State.EnterMission();
    }
}
