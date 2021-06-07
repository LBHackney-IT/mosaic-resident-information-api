using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Factories;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase.Interfaces;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public class AddResidentUseCase : IAddResidentUseCase
    {
        private IMosaicGateway _mosaicGateway;
        public AddResidentUseCase(IMosaicGateway mosaicGateway)
        {
            _mosaicGateway = mosaicGateway;
        }

        public ResidentInformation Execute(AddResidentRequest resident)
        {
            var residentInformation = _mosaicGateway.InsertResident(firstName: resident.FirstName, lastName: resident.LastName);

            return residentInformation.ToResponse();
        }
    }
}
