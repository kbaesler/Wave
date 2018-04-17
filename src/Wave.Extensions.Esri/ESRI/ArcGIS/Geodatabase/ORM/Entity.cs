using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     An row level entity object.
    /// </summary>
    /// <seealso cref="IEntity{ITable}" />
    public abstract class Entity : IEntity<ITable>, INotifyPropertyChanged
    {
        #region Fields

        private readonly EntityAttributesCollection _Attributes;

        private IDictionary<string, EntityAttributeProperty> _Fields;
        private IRow _Row;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Entity" /> class.
        /// </summary>
        protected Entity()
        {
            _Attributes = new EntityAttributesCollection(this.GetType());
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
        ///     Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get { return _Fields != null && _Fields.Keys.Any(o => o.Contains(".")); }
        }

        /// <summary>
        ///     The ObjectID field.
        /// </summary>
        public int OID
        {
            get { return this.IsDataBound && _Row.HasOID ? _Row.OID : -1; }
        }

        /// <summary>
        ///     Gets the row.
        /// </summary>
        /// <value>
        ///     The row.
        /// </value>
        public IRow Row
        {
            get { return _Row; }
            private set
            {
                _Row = value;
                _Fields = _Attributes.GetFields(value);
            }
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
            get { return _Row != null; }
        }

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
            this.Validate();

            using (ComReleaser cr = new ComReleaser())
            {
                var cursor = context.Insert(true);
                cr.ManageLifetime(cursor);

                var buffer = context.CreateRowBuffer();
                this.CopyTo(buffer);

                var oid = (int) cursor.InsertRow(buffer);
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
            if (this.IsDataBound && !this.IsReadOnly)
            {
                this.Validate();

                var editableFields = _Fields.Where(o => o.Value.Editable);
                foreach (var field in editableFields)
                {
                    var value = field.Value.Property.GetValue(this, null);
                    this.SetValue(field.Key, value);
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
            var row = (IRow) buffer;
            var fields = _Attributes.GetFields(row);
            var editableFields = fields.Where(o => o.Value.Editable).Select(o => o.Key);
            foreach (var fieldName in editableFields)
                row.Update(fieldName, this.GetValue(fieldName), false);
        }

        /// <summary>
        ///     Instantiates a new object of the specified type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="row">The row.</param>
        /// <returns>
        ///     Returns a <see cref="Entity" /> representing the object.
        /// </returns>
        public static TEntity Create<TEntity>(IRow row)
            where TEntity : Entity
        {
            var item = (TEntity) Activator.CreateInstance(typeof(TEntity));
            item.Bind(row);

            return item;
        }


        /// <summary>
        ///     Gets the field and property name pairs that have been decorated with the <see cref="EntityFieldAttribute" />
        ///     attribute.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>Returns a <see cref="Dictionary{TKey, TValue}" /> representing the field and property pairs.</returns>
        public static Dictionary<string, PropertyInfo> GetFieldNames<TEntity>()
        {
            Dictionary<string, PropertyInfo> fieldNames = new Dictionary<string, PropertyInfo>();

            foreach (var prop in typeof(TEntity).GetProperties())
            {
                var field = Attribute.GetCustomAttributes(prop).OfType<EntityFieldAttribute>().SingleOrDefault();
                if (field != null)
                {
                    fieldNames.Add(field.Name, prop);
                }
            }

            return fieldNames;
        }


        /// <summary>
        ///     Gets the full name that has been decorated to the entity using the <see cref="EntityTableAttribute" /> attribute.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the full name; otherwise <c>null</c>
        /// </returns>
        public static string GetFullName<TEntity>()
        {
            var attributes = typeof(TEntity).GetCustomAttributes(typeof(EntityTableAttribute));
            var attribute = attributes.SingleOrDefault() as EntityTableAttribute;
            return attribute?.FullName;
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
            if (oid > 0 && context.HasOID)
            {
                this.Bind(context.Fetch(oid));
            }
        }

        /// <summary>
        ///     Binds the entity to specified object from the context.
        /// </summary>
        /// <param name="row">The row.</param>
        protected void Bind(IRow row)
        {
            this.Row = row;

            if (this.IsDataBound)
            {
                foreach (var field in _Fields)
                {
                    var value = this.GetValue(field.Key);
                    if (field.Value.CanWrite)
                    {
                        field.Value.Property.SetValue(this, Convert.IsDBNull(value) ? null : value, null);
                    }
                }

                foreach (var dependencyProperty in _Attributes.Dependencies)
                {
                    var dependencyObject = (Entity) Activator.CreateInstance(dependencyProperty.PropertyType);
                    dependencyObject.Bind(row);

                    dependencyProperty.SetValue(this, dependencyObject);
                }
            }
        }

        /// <summary>
        ///     Gets the value from the property or the underlying data bound object.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Returns a <see cref="object" /> representing the value for field.</returns>
        /// <exception cref="System.MissingFieldException"></exception>
        protected object GetValue(string fieldName)
        {
            if (this.IsDataBound)
            {
                int index = _Row.Fields.FindField(fieldName);
                var value = _Row.GetValue<object>(index, null);
                if (value != DBNull.Value)
                {
                    if (_Row.Fields.Field[index].Type == esriFieldType.esriFieldTypeBlob)
                    {
                        ((IMemoryBlobStreamVariant) value).ExportToVariant(out value);
                    }
                }

                return _Attributes.Convert(fieldName, value);
            }

            var prop = _Attributes.GetProperty(fieldName);
            if (prop == null)
                throw new MissingFieldException(string.Format("Field '{0}' has not been defined.", fieldName));

            return prop.GetValue(this, null);
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
                this.OnPropertyChanged(expression.Member.Name);
        }

        /// <summary>
        ///     Sets the value on the property or underlying data bound object.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        protected void SetValue(string fieldName, object value)
        {
            if (this.IsDataBound)
            {
                if (value is byte[])
                {
                    var ms = (IMemoryBlobStreamVariant) new MemoryBlobStream();
                    ms.ImportFromVariant(value);
                    value = ms;
                }

                _Row.Update(fieldName, value, false);
                this.OnPropertyChanged(_Fields[fieldName].Property.Name);
            }
            else
            {
                var prop = _Attributes.GetProperty(fieldName);
                if (prop != null)
                {
                    prop.SetValue(this, value, null);

                    this.OnPropertyChanged(prop.Name);
                }
            }
        }

        /// <summary>
        ///     Determines whether the specified object is valid.
        /// </summary>
        /// <returns>
        ///     A collection that holds failed-validation information.
        /// </returns>
        protected virtual void Validate()
        {
            var validationContext = new ValidationContext(this, serviceProvider: null, items: null);
            Validator.ValidateObject(this, validationContext, true);
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
            get { return (this.IsDataBound ? ((IFeature) this.Row).Shape : this.TemporaryShape) as TShape; }
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
            var oid = this.Insert(((ITable) context));

            if (this.HasShape)
                this.TemporaryShape = ((IFeature) this.Row).Shape;

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
                this.TemporaryShape = ((IFeature) this.Row).Shape;
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