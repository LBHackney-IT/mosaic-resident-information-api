using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.UseCase.Interfaces;
using ResidentInformation = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;
using ResidentCannotBeAddedException = MosaicResidentInformationApi.V1.Domain.ResidentCannotBeAddedException;

namespace MosaicResidentInformationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/residents")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class MosaicController : BaseController
    {
        private IGetAllResidentsUseCase _getAllResidentsUseCase;
        private IGetEntityByIdUseCase _getEntityByIdUseCase;
        private IAddResidentUseCase _addResidentUseCase;

        public MosaicController(IGetAllResidentsUseCase getAllResidentsUseCase, IGetEntityByIdUseCase getEntityByIdUseCase, IAddResidentUseCase addResidentUseCase)
        {
            _getAllResidentsUseCase = getAllResidentsUseCase;
            _getEntityByIdUseCase = getEntityByIdUseCase;
            _addResidentUseCase = addResidentUseCase;
        }

        /// <summary>
        /// Returns list of contacts who share the query search parameter
        /// </summary>
        /// <response code="200">Success. Returns a list of matching residents information</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResidentInformationList), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult ListContacts([FromQuery] ResidentQueryParam rqp, int? cursor = 0, int? limit = 20)
        {
            try
            {
                return Ok(_getAllResidentsUseCase.Execute(rqp, (int) cursor, (int) limit));
            }
            catch (InvalidQueryParameterException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// /// <summary>
        /// Find a resident by Mosaic ID
        /// </summary>
        /// <response code="200">Success. Returns resident related to the specified ID</response>
        /// <response code="404">No resident found for the specified ID</response>
        [ProducesResponseType(typeof(ResidentInformation), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("{mosaicId}")]
        public IActionResult ViewRecord(int mosaicId)
        {
            try
            {
                return Ok(_getEntityByIdUseCase.Execute(mosaicId));
            }
            catch (ResidentNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Create a resident
        /// </summary>
        /// <response code="201">Created. Returns resident information</response>
        /// <response code="400">Unable to create resident</response>
        [ProducesResponseType(typeof(ResidentInformation), StatusCodes.Status201Created)]
        [HttpPost]
        public IActionResult AddResident([FromBody] AddResidentRequest addResidentRequest)
        {
            try
            {
                var response = _addResidentUseCase.Execute(addResidentRequest);

                return CreatedAtAction("AddResident", new { id = response.MosaicId }, response);
            }
            catch (ResidentCannotBeAddedException)
            {
                return StatusCode(400, "Error: Unable to create resident.");
            }
        }
    }
}
