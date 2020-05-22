using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Gateways;
using MosaicResidentInformationApi.V1.UseCase.Interfaces;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public class GetAllResidentsUseCase : IGetAllResidentsUseCase
    {
        private IMosaicGateway _iMosaicGateway;
        public GetAllResidentsUseCase(IMosaicGateway iMosaicGateway)
        {
            _iMosaicGateway = iMosaicGateway;
        }

        public ResidentInformationList Execute()
        {
            var residents = _iMosaicGateway.GetAllResidentsSelect();
            return new ResidentInformationList();
        }
    }
}
