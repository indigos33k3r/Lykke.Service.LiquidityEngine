﻿using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class TimersSettingsEntity : AzureTableEntity
    {
        private TimeSpan _marketMaker;
        private TimeSpan _lykkeBalances;
        private TimeSpan _externalBalances;

        public TimersSettingsEntity()
        {
        }

        public TimersSettingsEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public TimeSpan MarketMaker
        {
            get => _marketMaker;
            set
            {
                if (_marketMaker != value)
                {
                    _marketMaker = value;
                    MarkValueTypePropertyAsDirty("MarketMaker");
                }
            }
        }

        public TimeSpan LykkeBalances
        {
            get => _lykkeBalances;
            set
            {
                if (_lykkeBalances != value)
                {
                    _lykkeBalances = value;
                    MarkValueTypePropertyAsDirty("LykkeBalances");
                }
            }
        }

        public TimeSpan ExternalBalances
        {
            get => _externalBalances;
            set
            {
                if (_externalBalances != value)
                {
                    _externalBalances = value;
                    MarkValueTypePropertyAsDirty("ExternalBalances");
                }
            }
        }
    }
}
