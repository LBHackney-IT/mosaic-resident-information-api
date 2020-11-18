using System;
using System.Linq;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Factories;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase.Interfaces;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public class GetAllResidentsUseCase : IGetAllResidentsUseCase
    {
        private IMosaicGateway _mosaicGateway;
        private IValidatePostcode _validatePostcode;
        public GetAllResidentsUseCase(IMosaicGateway mosaicGateway, IValidatePostcode validatePostcode)
        {
            _mosaicGateway = mosaicGateway;
            _validatePostcode = validatePostcode;
        }

        public ResidentInformationList Execute(ResidentQueryParam rqp, int cursor, int limit)
        {
            CheckPostcodeValid(rqp);
            limit = limit < 10 ? 10 : limit;
            limit = limit > 100 ? 100 : limit;
            var residents = _mosaicGateway.GetAllResidents(cursor: cursor, limit: limit, rqp.MosaicId, rqp.FirstName, rqp.LastName,
                rqp.DateOfBirth, rqp.Postcode, rqp.Address, rqp.ContextFlag);

            var nextCursor = residents.Count == limit ? residents.Max(r => Int64.Parse(r.MosaicId)).ToString() : "";
            return new ResidentInformationList
            {
                Residents = residents.ToResponse(),
                NextCursor = nextCursor
            };
        }

        private void CheckPostcodeValid(ResidentQueryParam rqp)
        {
            if (string.IsNullOrWhiteSpace(rqp.Postcode)) return;
            var validPostcode = _validatePostcode.Execute(rqp.Postcode);
            if (!validPostcode) throw new InvalidQueryParameterException("The Postcode given does not have a valid format");
        }
    }
}
