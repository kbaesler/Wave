namespace Miner.Interop.Process
{
    /// <summary>
    ///     A wrapper around the product <see cref="Miner.Interop.Process.IMMWMSCustomer" /> interface into an workable object.
    /// </summary>
    public class Customer : BaseWMSNode
    {
        #region Fields

        private readonly IMMWMSCustomer _Customer;

        private Address _Address;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Customer" /> class.
        /// </summary>
        /// <param name="customer">The customer.</param>
        public Customer(IMMWMSCustomer customer)
            : base(customer as IMMWMSNode)
        {
            _Customer = customer;
            _Address = (customer != null) ? new Address(customer.Address) : null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the account number.
        /// </summary>
        /// <value>The account number.</value>
        public string AccountNumber
        {
            get
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                return propset.GetValue("ACCOUNT_NUMBER", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                if (propset.SetValue("ACCOUNT_NUMBER", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets the address.
        /// </summary>
        /// <value>The address.</value>
        public Address Address
        {
            get { return _Address; }
        }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email
        {
            get
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                return propset.GetValue("CONTACT_EMAIL", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                if (propset.SetValue("CONTACT_EMAIL", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the fax.
        /// </summary>
        /// <value>The fax.</value>
        public string Fax
        {
            get
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                return propset.GetValue("CONTACT_FAX", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                if (propset.SetValue("CONTACT_FAX", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName
        {
            get
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                return propset.GetValue("CONTACT_FIRST_NAME", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                if (propset.SetValue("CONTACT_FIRST_NAME", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID
        {
            get { return _Customer.ID; }
        }

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName
        {
            get
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                return propset.GetValue("CONTACT_LAST_NAME", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                if (propset.SetValue("CONTACT_LAST_NAME", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the mobile.
        /// </summary>
        /// <value>The mobile.</value>
        public string Mobile
        {
            get
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                return propset.GetValue("CONTACT_MOBILE", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                if (propset.SetValue("CONTACT_MOBILE", value))
                    base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the customer.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Customer.get_Name(); }
            set
            {
                _Customer.set_Name(ref value);

                base.Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the phone.
        /// </summary>
        /// <value>The phone.</value>
        public string Phone
        {
            get
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                return propset.GetValue("CONTACT_PHONE", string.Empty);
            }
            set
            {
                IMMWMSPropertySet propset = _Customer.PropertySet;
                if (propset.SetValue("CONTACT_PHONE", value))
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