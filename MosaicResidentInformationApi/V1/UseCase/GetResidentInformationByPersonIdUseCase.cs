using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Gateways;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public class GetResidentInformationByPersonIdUseCase : IGetResidentInformationByPersonIdUseCase
    {
        private IMosaicGateway _iMosaicGateway;
        public GetResidentInformationByPersonIdUseCase(IMosaicGateway iMosaicGateway)
        {
            _iMosaicGateway = iMosaicGateway;
        }

        public ResidentInformation Execute(int id)
        {
            return _iMosaicGateway.GetResidentInformationByPersonId(id);
        }
    }
}
