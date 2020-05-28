using System.Collections.Generic;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;

namespace MosaicResidentInformationApi.V1.Gateways
{
    public interface IMosaicGateway
    {
        List<ResidentInformation> GetAllResidents(string firstname = null, string lastname = null, string postcode = null);
        ResidentInformation GetEntityById(int id);
    }
}
