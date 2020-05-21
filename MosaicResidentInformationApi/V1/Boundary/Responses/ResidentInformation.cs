using System.Collections.Generic;

namespace MosaicResidentInformationApi.V1.Boundary.Responses
{
    public class ResidentInformation
    {
        /// <example>
        /// Ciasom
        /// </example>
        public string FirstName { get; set; }
        /// <example>
        /// Tessellate
        /// </example>
        public string LastName { get; set; }
        /// <example>
        /// 1000000000
        /// </example>
        public string Uprn { get; set; }
        /// <example>
        /// 2020-05-15
        /// </example>
        public string DateOfBirth { get; set; }
        public List<Phone> PhoneNumber { get; set; }
        public List<Address> AddressList { get; set; }
        /// <example>
        /// 450 557 7104
        /// </example>
        public string NhsNumber { get; set; }
    }
}
