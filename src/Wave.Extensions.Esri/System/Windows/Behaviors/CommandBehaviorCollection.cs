using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace System.Windows.Behaviors
{
    /// <summary>
    ///     Collection to store the list of behaviors. This is done so that you can intiniate it from XAML
    ///     This inherits from freezable so that it gets inheritance context for DataBinding to work
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class BehaviorBindingCollection : FreezableCollection<BehaviorBinding>
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the Owner of the binding
        /// </summary>
        public DependencyObject Owner { get; set; }

        #endregion
    }

    /// <summary>
    ///     A collection of command behaviors.
    /// </summary>
    public static class CommandBehaviorCollection
    {
        #region Fields

        /// <summary>
        ///     Behaviors Read-Only Dependency Property
        ///     As you can see the Attached readonly property has a name registered different (BehaviorsInternal) than the property
        ///     name, this is a tricks os that we can construct the collection as we want
        ///     Read more about this here http://wekempf.spaces.live.com/blog/cns!D18C3EC06EA971CF!468.entry
        /// </summary>
        private static readonly DependencyPropertyKey BehaviorsPropertyKey = DependencyProperty.RegisterAttachedReadOnly("BehaviorsInternal", typeof (BehaviorBindingCollection), typeof (CommandBehaviorCollection),
            new FrameworkPropertyMetadata((BehaviorBindingCollection) null));

        /// <summary>
        ///     The behaviors dependency property
        /// </summary>
        public static DependencyProperty BehaviorsProperty;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="CommandBehaviorCollection" /> class.
        /// </summary>
        static CommandBehaviorCollection()
        {
            BehaviorsProperty = BehaviorsPropertyKey.DependencyProperty;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the Behaviors property.
        ///     Here we initialze the collection and set the Owner property
        /// </summary>
        /// <param name="d">The dependecy object.</param>
        /// <returns>The collection of <see cref="BehaviorBinding" /> objects</returns>
        public static BehaviorBindingCollection GetBehaviors(DependencyObject d)
        {
            if (d == null)
                throw new InvalidOperationException("The dependency object trying to attach to is set to null");

            BehaviorBindingCollection collection = d.GetValue(BehaviorsProperty) as BehaviorBindingCollection;
            if (collection == null)
            {
                collection = new BehaviorBindingCollection();
                collection.Owner = d;
                SetBehaviors(d, collection);
            }
            return collection;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Handles the <see cref="CollectionChanged" /> event of the behavior collection.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        ///     The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing
        ///     the event data.
        /// </param>
        private static void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BehaviorBindingCollection sourceCollection = (BehaviorBindingCollection) sender;
            switch (e.Action)
            {
                    // When an item(s) is added we need to set the Owner property implicitly
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                        foreach (BehaviorBinding item in e.NewItems)
                            item.Owner = sourceCollection.Owner;
                    break;

                    // When an item(s) is removed we should Dispose the BehaviorBinding
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                        foreach (BehaviorBinding item in e.OldItems)
                            item.Behavior.Dispose();
                    break;

                    // Here we have to set the owner property to the new item and unregister the old item
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                        foreach (BehaviorBinding item in e.NewItems)
                            item.Owner = sourceCollection.Owner;

                    if (e.OldItems != null)
                        foreach (BehaviorBinding item in e.OldItems)
                            item.Behavior.Dispose();
                    break;
            }
        }

        /// <summary>
        ///     Provides a secure method for setting the Behaviors property.
        ///     This dependency property indicates ....
        /// </summary>
        private static void SetBehaviors(DependencyObject d, BehaviorBindingCollection value)
        {
            d.SetValue(BehaviorsPropertyKey, value);
            INotifyCollectionChanged collection = (INotifyCollectionChanged) value;
            collection.CollectionChanged += CollectionChanged;
        }

        #endregion
    }
}