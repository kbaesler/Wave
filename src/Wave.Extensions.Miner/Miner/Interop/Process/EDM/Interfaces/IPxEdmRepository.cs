using System.Collections.Generic;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides accessor methods for the repository.
    /// </summary>
    public interface IPxEdmRepository
    {
        #region Public Methods

        /// <summary>
        ///     Delete all the EDM data from the Work Request, Design, Work Location and CUs associated with the design id.
        /// </summary>
        /// <param name="designId">The design identifier.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of records that have been deleted.
        /// </returns>
        int Delete(int designId);

        /// <summary>
        ///     Loads all of the EDM data from the Work Request Design, Work Location and CUs associated with the design id.
        /// </summary>
        /// <param name="designId">The design identifier.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{K, V}" /> representing the table and EDMs for the design.
        /// </returns>
        Dictionary<string, List<EDM>> GetEdms(int designId);

        #endregion
    }
}