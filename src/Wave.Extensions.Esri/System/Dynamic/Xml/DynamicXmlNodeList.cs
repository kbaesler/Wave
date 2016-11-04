using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace System.Dynamic
{
    /// <summary>
    ///     Provides a dynamic version of the <see cref="XmlNodeList" />
    /// </summary>
    /// <seealso cref="System.Dynamic.DynamicObject" />
    /// <seealso cref="System.Collections.IEnumerable" />
    public sealed class DynamicXmlNodeList : DynamicObject, IEnumerable
    {
        #region Fields

        private readonly List<XElement> _Elements;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicXmlNodeList" /> class.
        /// </summary>
        /// <param name="elements">The elements.</param>
        internal DynamicXmlNodeList(IEnumerable<XElement> elements)
        {
            _Elements = new List<XElement>(elements);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the length.
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public int Length
        {
            get { return _Elements.Count; }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DynamicXmlNodeEnumerator(_Elements.GetEnumerator());
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _Elements.ToString();
        }

        /// <summary>
        ///     Provides the implementation for operations that get a value by index. Classes derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for
        ///     indexing operations.
        /// </summary>
        /// <param name="binder">Provides information about the operation.</param>
        /// <param name="indexes">
        ///     The indexes that are used in the operation. For example, for the sampleObject[3] operation in C#
        ///     (sampleObject(3) in Visual Basic), where sampleObject is derived from the DynamicObject class,
        ///     <paramref name="indexes[0]" /> is equal to 3.
        /// </param>
        /// <param name="result">The result of the index operation.</param>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the
        ///     language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <exception cref="NotSupportedException">Index ' + o + ' is not supported. Please use integer-based indexes only</exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            object o = indexes[0];
            if (!(o is int))
                throw new NotSupportedException("Index '" + o + "' is not supported. Please use integer-based indexes only");

            var index = (int) o;
            if (index > _Elements.Count - 1)
                throw new IndexOutOfRangeException();

            result = _Elements[index];

            return true;
        }

        #endregion

        #region Nested Type: ElementEnumerator

        /// <summary>
        ///     Provides an enumerator for <see cref="XElement" /> objects.
        /// </summary>
        /// <seealso cref="System.Collections.IEnumerator" />
        private sealed class DynamicXmlNodeEnumerator : IEnumerator
        {
            #region Fields

            private readonly IEnumerator<XElement> _Elements;

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="DynamicXmlNodeEnumerator" /> class.
            /// </summary>
            /// <param name="elements">The elements.</param>
            public DynamicXmlNodeEnumerator(IEnumerator<XElement> elements)
            {
                _Elements = elements;
            }

            #endregion

            #region IEnumerator Members

            /// <summary>
            ///     Gets the current element in the collection.
            /// </summary>
            public object Current
            {
                get { return new DynamicXmlNode(_Elements.Current); }
            }

            /// <summary>
            ///     Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            ///     true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of
            ///     the collection.
            /// </returns>
            public bool MoveNext()
            {
                return _Elements.MoveNext();
            }

            /// <summary>
            ///     Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                _Elements.Reset();
            }

            #endregion
        }

        #endregion
    }
}