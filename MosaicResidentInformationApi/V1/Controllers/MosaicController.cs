using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.UseCase;

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
        [ProducesResponseType(typeof(ResidentInformationList), StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult ListContacts([FromQuery] ResidentQueryParam rqp)
        {

            return Ok(_getAllResidentsUseCase.Execute(rqp));
        }

        [HttpGet]
        [Route("{mosaicId}")]
        public IActionResult ViewRecord(int mosaicId)
        {

            return Ok(_getEntityByIdUseCase.Execute(mosaicId));
        }

    }
}
