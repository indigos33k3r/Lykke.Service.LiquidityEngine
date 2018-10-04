﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.LiquidityEngine.DomainServices.Timers;
using Lykke.Service.LiquidityEngine.Rabbit.Subscribers;

namespace Lykke.Service.LiquidityEngine.Managers
{
    [UsedImplicitly]
    public class StartupManager : IStartupManager
    {
        private readonly LykkeBalancesTimer _lykkeBalancesTimer;
        private readonly ExternalBalancesTimer _externalBalancesTimer;
        private readonly MarketMakerTimer _marketMakerTimer;
        private readonly HedgingTimer _hedgingTimer;
        private readonly LykkeTradeSubscriber _lykkeTradeSubscriber;
        private readonly B2C2QuoteSubscriber _b2C2QuoteSubscriber;

        public StartupManager(
            LykkeBalancesTimer lykkeBalancesTimer,
            ExternalBalancesTimer externalBalancesTimer,
            MarketMakerTimer marketMakerTimer,
            HedgingTimer hedgingTimer,
            LykkeTradeSubscriber lykkeTradeSubscriber,
            B2C2QuoteSubscriber b2C2QuoteSubscriber)
        {
            _lykkeBalancesTimer = lykkeBalancesTimer;
            _externalBalancesTimer = externalBalancesTimer;
            _marketMakerTimer = marketMakerTimer;
            _hedgingTimer = hedgingTimer;
            _lykkeTradeSubscriber = lykkeTradeSubscriber;
            _b2C2QuoteSubscriber = b2C2QuoteSubscriber;
        }

        public Task StartAsync()
        {
            _b2C2QuoteSubscriber.Start();
            
            _lykkeTradeSubscriber.Start();

            _lykkeBalancesTimer.Start();

            _externalBalancesTimer.Start();

            _marketMakerTimer.Start();
            
            _hedgingTimer.Start();
            
            return Task.CompletedTask;
        }
    }
}
