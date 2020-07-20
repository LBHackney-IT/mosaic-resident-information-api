using System.ComponentModel;

namespace MosaicResidentInformationApi.V1.Boundary.Responses
{
    public enum PhoneType
    {
        /// <summary>
        /// Primary
        /// </summary>
        Primary,
        /// <summary>
        /// Home
        /// </summary>
        Home,
        /// <summary>
        /// Mobile
        /// </summary>
        Mobile,
        /// <summary>
        /// Fax
        /// </summary>
        Fax,
        /// <summary>
        /// Work
        /// </summary>
        Work,
        /// <summary>
        /// Pager
        /// </summary>
        Pager,
        /// <summary>
        /// Mobile - Secondary
        /// </summary>
        [Description("Mobile - Secondary")] Secondary,
        /// <summary>
        /// Ex-directory (do not disclose number)
        /// </summary>
        [Description("Ex-directory (do not disclose number)")] ExDirectory
    }
}
