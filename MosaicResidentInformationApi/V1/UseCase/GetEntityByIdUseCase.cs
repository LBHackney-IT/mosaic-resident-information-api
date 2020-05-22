using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase.Interfaces;
using ResidentInformationResponse = MosaicResidentInformationApi.V1.Boundary.Responses.ResidentInformation;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public class GetEntityByIdUseCase : IGetEntityByIdUseCase
    {
        private IMosaicGateway _mosaicGateway;
        public GetEntityByIdUseCase(IMosaicGateway mosaicGateway)
        {
            _mosaicGateway = mosaicGateway;
        }

        public ResidentInformationResponse Execute(int id)
        {
            var residentInfo = _mosaicGateway.GetEntityById(id);
            return new ResidentInformationResponse
            {
                FirstName = residentInfo.FirstName,
                LastName = residentInfo.LastName,
                DateOfBirth = residentInfo.DateOfBirth
            };
        }
    }
}
