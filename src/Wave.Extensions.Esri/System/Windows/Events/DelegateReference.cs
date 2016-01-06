using System.Reflection;

namespace System.Windows
{
    /// <summary>
    ///     Represents a reference to a <see cref="Delegate" /> that may contain a
    ///     <see cref="WeakReference" /> to the target. This class is used
    ///     internally by the Prism Library.
    /// </summary>
    public class DelegateReference : IDelegateReference
    {
        #region Fields

        private readonly Delegate _Delegate;
        private readonly Type _DelegateType;
        private readonly MethodInfo _Method;
        private readonly WeakReference _WeakReference;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of <see cref="DelegateReference" />.
        /// </summary>
        /// <param name="delegate">The original <see cref="Delegate" /> to create a reference for.</param>
        /// <param name="keepReferenceAlive">
        ///     If <see langword="false" /> the class will create a weak reference to the delegate,
        ///     allowing it to be garbage collected. Otherwise it will keep a strong reference to the target.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     If the passed <paramref name="delegate" /> is not assignable to
        ///     <see cref="Delegate" />.
        /// </exception>
        public DelegateReference(Delegate @delegate, bool keepReferenceAlive)
        {
            if (@delegate == null)
                throw new ArgumentNullException("delegate");

            if (keepReferenceAlive)
            {
                this._Delegate = @delegate;
            }
            else
            {
                _WeakReference = new WeakReference(@delegate.Target);
                _Method = @delegate.Method;
                _DelegateType = @delegate.GetType();
            }
        }

        #endregion

        #region IDelegateReference Members

        /// <summary>
        ///     Gets the <see cref="Delegate" /> (the target) referenced by the current <see cref="DelegateReference" /> object.
        /// </summary>
        /// <value>
        ///     <see langword="null" /> if the object referenced by the current <see cref="DelegateReference" /> object has been
        ///     garbage collected; otherwise, a reference to the <see cref="Delegate" /> referenced by the current
        ///     <see cref="DelegateReference" /> object.
        /// </value>
        public Delegate Target
        {
            get
            {
                if (_Delegate != null)
                {
                    return _Delegate;
                }
                else
                {
                    return TryGetDelegate();
                }
            }
        }

        #endregion

        #region Private Methods

        private Delegate TryGetDelegate()
        {
            if (_Method.IsStatic)
            {
                return Delegate.CreateDelegate(_DelegateType, _Method);
            }
            object target = _WeakReference.Target;
            if (target != null)
            {
                return Delegate.CreateDelegate(_DelegateType, target, _Method.Name);
            }
            return null;
        }

        #endregion
    }
}