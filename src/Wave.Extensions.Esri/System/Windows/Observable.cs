using System.ComponentModel;

namespace System.Windows
{
    /// <summary>
    ///     A partial abstract binding class that implements the <see cref="INotifyPropertyChanged" /> and
    ///     <see cref="INotifyPropertyChanging" /> interface
    ///     and exposes their respective methods for derived classes to raise the events.
    /// </summary>
    public abstract class Observable : DataValidation, INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region INotifyPropertyChanging Members

        /// <summary>
        ///     Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Raises the <see cref="PropertyChanging" /> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanging(string propertyName)
        {
            VerifyPropertyName(propertyName);

            PropertyChangingEventHandler eventHandler = this.PropertyChanging;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangingEventArgs(propertyName));
        }

        #endregion
    }
}