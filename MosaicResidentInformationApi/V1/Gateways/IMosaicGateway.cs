using System.Collections.Generic;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;

namespace MosaicResidentInformationApi.V1.Gateways
{
    public interface IMosaicGateway
    {
        List<ResidentInformation> GetAllResidents(int cursor, int limit, long? id, string firstname = null, string lastname = null,
            string dateOfBirth = null, string postcode = null, string address = null, string contextFlag = null);

        ResidentInformation GetEntityById(long id, string contextflag = null);

        ResidentInformation InsertResident(string firstName, string lastName);
    }
}
