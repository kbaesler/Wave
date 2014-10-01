namespace Miner.Interop.Process
{
    /// <summary>
    ///     A wrapper around the product <see cref="Miner.Interop.Process.IMMWMSAddress" /> interface into an workable object.
    /// </summary>
    public class Address : BaseWMSNode
    {
        #region Fields

        private readonly IMMWMSAddress _Address;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Address" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        public Address(IMMWMSAddress address)
            : base(address as IMMWMSNode)
        {
            _Address = address;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the address1.
        /// </summary>
        /// <value>The address1.</value>
        public string Address1
        {
            get
            {
                IMMWMSPropertySet propset = _Address.PropertySet;
                return propset.GetValue("ADDRESS1", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Address.PropertySet;
                if (propset.SetValue("ADDRESS1", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the address2.
        /// </summary>
        /// <value>The address2.</value>
        public string Address2
        {
            get
            {
                IMMWMSPropertySet propset = _Address.PropertySet;
                return propset.GetValue("ADDRESS2", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Address.PropertySet;
                if (propset.SetValue("ADDRESS2", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        public string City
        {
            get
            {
                IMMWMSPropertySet propset = _Address.PropertySet;
                return propset.GetValue("CITY", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Address.PropertySet;
                if (propset.SetValue("CITY", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID
        {
            get { return _Address.ID; }
        }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public string State
        {
            get
            {
                IMMWMSPropertySet propset = _Address.PropertySet;
                return propset.GetValue("STATE", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Address.PropertySet;
                if (propset.SetValue("STATE", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the zip.
        /// </summary>
        /// <value>The zip.</value>
        public string Zip
        {
            get
            {
                IMMWMSPropertySet propset = _Address.PropertySet;
                return propset.GetValue("ZIP", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Address.PropertySet;
                if (propset.SetValue("ZIP", value))
                    base.Dirty = true;
            }
        }

        #endregion
    }
}