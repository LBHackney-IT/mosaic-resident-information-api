using System.Collections.Generic;
using System.Linq;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase.Interfaces;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public class GetAllResidentsUseCase : IGetAllResidentsUseCase
    {
        private IMosaicGateway _mosaicGateway;
        public GetAllResidentsUseCase(IMosaicGateway mosaicGateway)
        {
            _mosaicGateway = mosaicGateway;
        }

        public ResidentInformationList Execute()
        {
            var residents = _mosaicGateway.GetAllResidentsSelect();
            return new ResidentInformationList
            {
                Residents = new List<ResidentInformation>{
                    new ResidentInformation
                    {
                        FirstName = residents.First().FirstName,
                        LastName = residents.First().LastName,
                        DateOfBirth = residents.First().DateOfBirth
                    }
                }
            };
        }
    }
}
