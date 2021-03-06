﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Instruments;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class InstrumentsController : Controller, IInstrumentsApi
    {
        private readonly IInstrumentService _instrumentService;

        public InstrumentsController(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of instruments.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<InstrumentModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<InstrumentModel>> GetAllAsync()
        {
            IReadOnlyCollection<Instrument> instruments = await _instrumentService.GetAllAsync();

            return Mapper.Map<List<InstrumentModel>>(instruments);
        }

        /// <inheritdoc/>
        /// <response code="200">An instrument.</response>
        /// <response code="404">Instrument does not exist.</response>
        [HttpGet("{assetPairId}")]
        [ProducesResponseType(typeof(InstrumentModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task<InstrumentModel> GetByAssetPairIdAsync(string assetPairId)
        {
            try
            {
                Instrument instrument = await _instrumentService.GetByAssetPairIdAsync(assetPairId);

                return Mapper.Map<InstrumentModel>(instrument);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Instrument does not exist.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The instrument successfully added.</response>
        /// <response code="400">The instrument already used.</response>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task AddAsync([FromBody] InstrumentEditModel model)
        {
            try
            {
                var instrument = Mapper.Map<Instrument>(model);

                await _instrumentService.AddAsync(instrument);
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The instrument successfully updated.</response>
        /// <response code="404">Instrument does not exist.</response>
        [HttpPut]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task UpdateAsync([FromBody] InstrumentEditModel model)
        {
            try
            {
                var instrument = Mapper.Map<Instrument>(model);

                await _instrumentService.UpdateAsync(instrument);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Instrument does not exist.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The instrument successfully deleted.</response>
        /// <response code="400">An error occurred while deleting instrument.</response>
        /// <response code="404">Instrument does not exist.</response>
        [HttpDelete("{assetPairId}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task DeleteAsync(string assetPairId)
        {
            try
            {
                await _instrumentService.DeleteAsync(assetPairId);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Instrument does not exist.");
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The volume level successfully added to instrument.</response>
        /// <response code="400">The volume level already exists.</response>
        /// <response code="404">Instrument does not exist.</response>
        [HttpPost("{assetPairId}/levels")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task AddLevelAsync(string assetPairId, [FromBody] InstrumentLevelAddModel model)
        {
            try
            {
                var instrumentLevel = Mapper.Map<InstrumentLevel>(model);

                await _instrumentService.AddLevelAsync(assetPairId, instrumentLevel);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Instrument does not exist.");
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The volume level successfully updated.</response>
        /// <response code="400">The volume level does not exists.</response>
        /// <response code="404">Instrument does not exist.</response>
        [HttpPut("{assetPairId}/levels")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task UpdateLevelAsync(string assetPairId, [FromBody] InstrumentLevelModel model)
        {
            try
            {
                var instrumentLevel = Mapper.Map<InstrumentLevel>(model);

                await _instrumentService.UpdateLevelAsync(assetPairId, instrumentLevel);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Instrument does not exist.");
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The volume level successfully removed from instrument.</response>
        /// <response code="404">Instrument does not exist.</response>
        [Obsolete("Use identifier to remove level.")]
        [HttpDelete("{assetPairId}/levels/{levelNumber}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task RemoveLevelAsync(string assetPairId, int levelNumber)
        {
            try
            {
                await _instrumentService.RemoveLevelByNumberAsync(assetPairId, levelNumber);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Instrument does not exist.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The volume level successfully removed from instrument.</response>
        /// <response code="404">Instrument does not exist.</response>
        [HttpDelete("{assetPairId}/levels")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task RemoveLevelByIdAsync(string assetPairId, string levelId)
        {
            try
            {
                await _instrumentService.RemoveLevelAsync(assetPairId, levelId);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Instrument does not exist.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The cross instrument successfully added to instrument.</response>
        /// <response code="400">The instrument already used.</response>
        /// <response code="404">Instrument does not exist.</response>
        [HttpPost("{assetPairId}/cross")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task AddCrossInstrumentAsync(string assetPairId, [FromBody] CrossInstrumentModel model)
        {
            try
            {
                var crossInstrument = Mapper.Map<CrossInstrument>(model);

                await _instrumentService.AddCrossInstrumentAsync(assetPairId, crossInstrument);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Instrument does not exist.");
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The cross instrument successfully updated.</response>
        /// <response code="400">The cross instrument does not exists.</response>
        /// <response code="404">Instrument does not exist.</response>
        [HttpPut("{assetPairId}/cross")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task UpdateCrossInstrumentAsync(string assetPairId, [FromBody] CrossInstrumentModel model)
        {
            try
            {
                var crossInstrument = Mapper.Map<CrossInstrument>(model);

                await _instrumentService.UpdateCrossInstrumentAsync(assetPairId, crossInstrument);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Instrument does not exist.");
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The cross instrument successfully removed from instrument.</response>
        /// <response code="404">Instrument does not exist.</response>
        [HttpDelete("{assetPairId}/cross/{crossAssetPairId}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task RemoveCrossInstrumentAsync(string assetPairId, string crossAssetPairId)
        {
            try
            {
                await _instrumentService.RemoveCrossInstrumentAsync(assetPairId, crossAssetPairId);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Instrument does not exist.");
            }
        }
    }
}
