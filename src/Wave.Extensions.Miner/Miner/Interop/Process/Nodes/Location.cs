namespace Miner.Interop.Process
{
    /// <summary>
    ///     A wrapper around the product <see cref="Miner.Interop.Process.IMMWMSLocation" /> interface into an workable object.
    /// </summary>
    public class Location : BaseWMSNode
    {
        #region Fields

        private readonly IMMWMSLocation _Location;

        private Address _Address;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Location" /> class.
        /// </summary>
        /// <param name="location">The location.</param>
        public Location(IMMWMSLocation location)
            : base(location as IMMWMSNode)
        {
            _Location = location;
            _Address = new Address(location.Address);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the address.
        /// </summary>
        /// <value>The address.</value>
        public Address Address
        {
            get { return _Address; }
        }

        /// <summary>
        ///     Gets or sets the administrative area ID.
        /// </summary>
        /// <value>The administrative area ID.</value>
        public int AdministrativeAreaID
        {
            get
            {
                IMMWMSPropertySet propset = _Location.PropertySet;
                return propset.GetValue("ADMINISTRATIVE_AREA_ID", 0);
            }
            set
            {
                IMMWMSPropertySet propset = _Location.PropertySet;
                if (propset.SetValue("ADMINISTRATIVE_AREA_ID", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the facility display field.
        /// </summary>
        /// <value>The facility display field.</value>
        public string FacilityDisplayField
        {
            get
            {
                IMMWMSPropertySet propset = _Location.PropertySet;
                return propset.GetValue("FACILITY_DISPLAY_FIELD", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Location.PropertySet;
                if (propset.SetValue("FACILITY_DISPLAY_FIELD", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the facility type ID.
        /// </summary>
        /// <value>The facility type ID.</value>
        public int FacilityTypeID
        {
            get
            {
                IMMWMSPropertySet propset = _Location.PropertySet;
                return propset.GetValue("FACILITY_TYPE_ID", 0);
            }
            set
            {
                IMMWMSPropertySet propset = _Location.PropertySet;
                if (propset.SetValue("FACILITY_TYPE_ID", value))
                    base.Dirty = true;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Updates the node by flushing the information to the database.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (_Address != null)
            {
                _Address.Update();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_Address != null)
            {
                _Address.Dispose();
                _Address = null;
            }
        }

        #endregion
    }
}