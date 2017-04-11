using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geometry;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     An row level entity object.
    /// </summary>
    /// <seealso cref="IEntity{ITable}" />
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
                .Select(p => new {p, a = Attribute.GetCustomAttributes(p).OfType<EntityFieldAttribute>().SingleOrDefault()})
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

        /// <summary>
        ///     The delete action
        /// </summary>
        public Action<IRow> DeleteAction { get; set; }

        /// <summary>
        ///     The update action
        /// </summary>
        public Action<IRow> UpdateAction { get; set; }

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
        ///     Gets or sets the row.
        /// </summary>
        /// <value>
        ///     The row.
        /// </value>
        protected IRow Row { get; set; }

        #endregion

        #region IEntity<ITable> Members

        /// <summary>
        ///     Deletes this item from the context.
        /// </summary>
        public virtual void Delete()
        {
            if (this.IsDataBound)
            {
                if (this.DeleteAction == null)
                    this.Row.Delete();
                else
                    this.DeleteAction(this.Row);

                this.Row = null;
            }
        }

        /// <summary>
        ///     Inserts the entity into specified context.
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

                return (int) cursor.InsertRow(buffer);
            }
        }

        /// <summary>
        ///     Commits changes to the underlying context.
        /// </summary>
        public virtual void Update()
        {
            if (this.IsDataBound)
            {
                if (this.UpdateAction == null)
                {
                    foreach (var attribute in _EntityFieldAttributes)
                    {
                        var value = attribute.Value.GetValue(this, null);
                        this.SetValue(attribute.Key, value);
                    }

                    this.Row.Store();
                }
                else
                {
                    this.UpdateAction(this.Row);
                }
            }
        }

        /// <summary>
        ///     Binds the entity to specified object from the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="oid">The OBJECTID of the row to bind.</param>
        public virtual void Bind(ITable context, int oid)
        {
            this.Bind(context.Fetch(oid));
        }

        /// <summary>
        ///     Binds the entity to specified object from the context.
        /// </summary>
        /// <param name="row"></param>
        public void Bind(IRow row)
        {
            this.Row = row;

            if (this.IsDataBound)
            {
                foreach (var attribute in _EntityFieldAttributes)
                {
                    var value = this.GetValue(attribute.Key);
                    attribute.Value.SetValue(this, Convert.IsDBNull(value) ? null : value, null);
                }
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
            var fields = row.Fields.ToDictionary();

            foreach (var fieldName in _EntityFieldAttributes.Keys)
            {
                if (!fields.ContainsKey(fieldName))
                    continue;

                var field = row.Table.Fields.Field[fields[fieldName]];
                if ((field.Type != esriFieldType.esriFieldTypeGeometry &&
                     field.Type != esriFieldType.esriFieldTypeRaster &&
                     field.Type != esriFieldType.esriFieldTypeXML) && field.Editable)
                {
                    row.Update(fields[fieldName], this.GetValue(fieldName));
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
            var item = (T) Activator.CreateInstance(typeof(T));
            item.Bind(row);

            return item;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Binds the entity to specified object from the context.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected void Bind(Entity entity)
        {
            if (entity == null)
                return;

            this.Bind(entity.Row);
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

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.MissingFieldException"></exception>
        private object GetValue(string fieldName)
        {
            if (this.IsDataBound)
                return this.Row.GetValue<object>(fieldName, null);

            if (!_EntityFieldAttributes.ContainsKey(fieldName))
                throw new MissingFieldException(string.Format("Field '{0}' has not been defined.", fieldName));

            return _EntityFieldAttributes[fieldName].GetValue(this, null);
        }

        /// <summary>
        ///     Sets the value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        private void SetValue(string fieldName, object value)
        {
            if (this.IsDataBound)
            {
                this.Row.Update(fieldName, value, true);
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

        #endregion

        #region Public Methods

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