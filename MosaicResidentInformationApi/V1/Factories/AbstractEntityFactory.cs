using System.Collections.Generic;
using System.Linq;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Infrastructure;
using Address = MosaicResidentInformationApi.V1.Infrastructure.Address;

namespace MosaicResidentInformationApi.V1.Factories
{
    public abstract class AbstractEntityFactory
    {
        public abstract ResidentInformation ToDomain(Person person);
        public abstract PhoneNumber ToDomain(TelephoneNumber number);
        public abstract MosaicResidentInformationApi.V1.Domain.Address ToDomain(Address address);
        public List<ResidentInformation> ToDomain(IEnumerable<Person> result)
        {
            return result.Select(ToDomain).ToList();
        }
    }
}
