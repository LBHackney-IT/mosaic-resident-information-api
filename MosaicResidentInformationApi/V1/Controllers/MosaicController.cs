using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.UseCase.Interfaces;

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
        public MosaicController(IGetAllResidentsUseCase getAllResidentsUseCase, IGetEntityByIdUseCase getEntityByIdUseCase)
        {
            _getAllResidentsUseCase = getAllResidentsUseCase;
            _getEntityByIdUseCase = getEntityByIdUseCase;

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
        [HttpGet]
        [Route("{mosaicId}")]
        public IActionResult ViewRecord(int mosaicId)
        {

            return Ok(_getEntityByIdUseCase.Execute(mosaicId));
        }

    }
}
