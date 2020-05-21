using System.Collections.Generic;
using System.Linq;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Infrastructure;

namespace MosaicResidentInformationApi.V1.Factories
{
    public abstract class AbstractEntityFactory
    {
        public abstract ResidentInformation ToDomain(Person person);
        public abstract PhoneNumber ToDomain(TelephoneNumber number);
        public abstract Address ToDomain(AddressSchema address);
        public List<ResidentInformation> ToDomain(IEnumerable<Person> result)
        {
            return result.Select(ToDomain).ToList();
        }
    }
}
