using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     An row level entity object.
    /// </summary>
    /// <seealso cref="IEntity{ITable}" />
    [Serializable]
    public class Entity : IEntity<ITable>, INotifyPropertyChanged
    {
        #region Fields

        private readonly Dictionary<string, PropertyInfo> _EntityFieldAttributes;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Entity" /> class.
        /// </summary>
        protected Entity()
        {
            _EntityFieldAttributes = this.GetType()
                .GetProperties()
                .Select(p => new { p, a = Attribute.GetCustomAttributes(p).OfType<EntityFieldAttribute>().SingleOrDefault() })
                .Where(o => o.a != null)
                .ToDictionary(o => o.a, o => o.p)
                .ToDictionary(p => p.Key.FieldName, p => p.Value);
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     The ObjectID field.
        /// </summary>
        public int OID
        {
            get { return this.IsDataBound && this.Row.HasOID ? this.Row.OID : -1; }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets a value indicating whether this instance is bound to the context
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        protected bool IsDataBound
        {
            get { return this.Row != null; }
        }

        /// <summary>
        ///     Gets or sets the fields.
        /// </summary>
        /// <value>
        ///     The fields.
        /// </value>
        protected IDictionary<string, int> Fields { get; set; }

        /// <summary>
        ///     Gets or sets the row.
        /// </summary>
        /// <value>
        ///     The row.
        /// </value>
        protected IRow Row { get; set; }

        #endregion

        #region IEntity<ITable> Members

        /// <summary>
        ///     Deletes the underlying object from the table.
        /// </summary>
        public virtual void Delete()
        {
            if (this.IsDataBound)
            {
                this.Row.Delete();
                this.Row = null;
            }
        }

        /// <summary>
        ///     Inserts the entity into the table using an insert cursor.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     Returns a <see cref="T:System.Int32" /> representing the OID.
        /// </returns>
        public virtual int Insert(ITable context)
        {
            using (ComReleaser cr = new ComReleaser())
            {
                var cursor = context.Insert(true);
                cr.ManageLifetime(cursor);

                var buffer = context.CreateRowBuffer();
                this.CopyTo(buffer);

                var oid = (int)cursor.InsertRow(buffer);
                cursor.Flush();

                this.Bind(context, oid);

                return oid;
            }
        }

        /// <summary>
        ///     Commits changes to the underlying context.
        /// </summary>
        public virtual void Update()
        {
            if (this.IsDataBound)
            {
                foreach (var attribute in _EntityFieldAttributes)
                {
                    var value = attribute.Value.GetValue(this, null);
                    this.SetValue(attribute.Key, value);
                }

                this.Row.Store();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Copies the contents of the entity into the buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public virtual void CopyTo(IRowBuffer buffer)
        {
            var row = (IRow)buffer;
            var fields = row.Fields.ToDictionary();

            foreach (var fieldName in _EntityFieldAttributes.Keys)
            {
                if (!fields.ContainsKey(fieldName))
                    continue;

                var field = row.Table.Fields.Field[fields[fieldName]];
                if (field.Type != esriFieldType.esriFieldTypeGeometry &&
                    field.Type != esriFieldType.esriFieldTypeRaster &&
                    field.Type != esriFieldType.esriFieldTypeXML && field.Editable)
                {
                    row.Update(fields[fieldName], this.GetValue(fieldName), false);
                }
            }
        }

        /// <summary>
        ///     Instantiates a new object of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <returns>Returns a <see cref="Entity" /> representing the object.</returns>
        public static T Create<T>(IRow row)
            where T : Entity
        {
            var item = (T)Activator.CreateInstance(typeof(T));
            item.Bind(row);

            return item;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Binds the entity to specified object from the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="oid">The OBJECTID of the row to bind.</param>
        protected virtual void Bind(ITable context, int oid)
        {
            this.Bind(context.Fetch(oid));
        }

        /// <summary>
        ///     Binds the entity to specified object from the context.
        /// </summary>
        /// <param name="row"></param>
        protected void Bind(IRow row)
        {
            this.Row = row;

            if (this.IsDataBound)
            {
                this.Fields = row.Fields.ToDictionary(f => _EntityFieldAttributes.ContainsKey(f.Name));

                foreach (var field in _EntityFieldAttributes)
                {
                    var value = this.GetValue(field.Key);
                    field.Value.SetValue(this, Convert.IsDBNull(value) ? null : value, null);
                }
            }
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.MissingFieldException"></exception>
        protected object GetValue(string fieldName)
        {
            if (this.IsDataBound)
            {
                var value = this.Row.GetValue<object>(this.Fields[fieldName], null);
                if (value != DBNull.Value)
                {
                    if (this.Row.Fields.Field[this.Fields[fieldName]].Type == esriFieldType.esriFieldTypeBlob)
                    {
                        ((IMemoryBlobStreamVariant)value).ExportToVariant(out value);
                    }
                }

                return value;
            }


            if (!_EntityFieldAttributes.ContainsKey(fieldName))
                throw new MissingFieldException(string.Format("Field '{0}' has not been defined.", fieldName));

            return _EntityFieldAttributes[fieldName].GetValue(this, null);
        }

        /// <summary>
        ///     Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">The property selector.</param>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertySelector)
        {
            var expression = propertySelector.Body as MemberExpression;
            if (expression != null)
                OnPropertyChanged(expression.Member.Name);
        }

        /// <summary>
        ///     Sets the value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        protected void SetValue(string fieldName, object value)
        {
            if (this.IsDataBound)
            {
                if (value is byte[])
                {
                    var ms = (IMemoryBlobStreamVariant)new MemoryBlobStream();
                    ms.ImportFromVariant(value);
                    value = ms;
                }

                this.Row.Update(this.Fields[fieldName], value, false);
            }
            else
            {
                if (_EntityFieldAttributes.ContainsKey(fieldName))
                    _EntityFieldAttributes[fieldName].SetValue(this, value, null);
            }

            if (_EntityFieldAttributes.ContainsKey(fieldName))
                this.OnPropertyChanged(_EntityFieldAttributes[fieldName].Name);
        }

        #endregion
    }

    /// <summary>
    ///     An feature entity.
    /// </summary>
    /// <typeparam name="TShape">The type of the shape.</typeparam>
    /// <seealso cref="IEntity{ITable}" />
    /// <seealso cref="INotifyPropertyChanged" />
    public class Entity<TShape> : Entity, IEntity<IFeatureClass, TShape>
        where TShape : class, IGeometry
    {
        #region Fields

        internal IGeometry TemporaryShape;

        #endregion

        #region Public Properties

        /// <summary>
        ///     The geometry of this item.
        /// </summary>
        public TShape Shape
        {
            get { return (this.IsDataBound ? ((IFeature)this.Row).Shape : this.TemporaryShape) as TShape; }
            set
            {
                if (this.IsDataBound)
                {
                    var feature = this.Row as IFeature;
                    if (feature != null) feature.Shape = value;
                }
                else
                {
                    this.TemporaryShape = value;
                }

                this.OnPropertyChanged(() => Shape);
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets a value indicating whether this instance has shape.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has shape; otherwise, <c>false</c>.
        /// </value>
        protected bool HasShape
        {
            get { return this.Shape != null && this.Row is IFeature; }
        }

        #endregion

        #region IEntity<IFeatureClass,TShape> Members

        /// <summary>
        ///     Inserts the entity into specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     Returns a <see cref="T:System.Int32" /> representing the OID.
        /// </returns>
        public int Insert(IFeatureClass context)
        {
            var oid = this.Insert(((ITable)context));

            if (this.HasShape)
                this.TemporaryShape = ((IFeature)this.Row).Shape;

            return oid;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Binds the entity to specified object from the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="oid">The OBJECTID of the row to bind.</param>
        public void Bind(IFeatureClass context, int oid)
        {
            this.Bind(context.Fetch(oid));

            if (this.HasShape)
                this.TemporaryShape = ((IFeature)this.Row).Shape;
        }

        /// <summary>
        ///     Copies the contents of the entity into the buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void CopyTo(IRowBuffer buffer)
        {
            base.CopyTo(buffer);

            if (this.HasShape)
            {
                var feature = buffer as IFeature;

                if (feature != null)
                {
                    feature.Shape = this.Shape;
                }
            }
        }

        #endregion
    }
}