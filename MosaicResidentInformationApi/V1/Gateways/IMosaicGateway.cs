using System.Collections.Generic;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;

namespace MosaicResidentInformationApi.V1.Gateways
{
    public interface IMosaicGateway
    {
        List<ResidentInformation> GetAllResidents(int cursor, int limit, string firstname = null, string lastname = null,
            string postcode = null, string address = null);
        ResidentInformation GetEntityById(long id);
    }
}
