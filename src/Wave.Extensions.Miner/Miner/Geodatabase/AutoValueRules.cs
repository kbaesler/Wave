using System;
using System.Collections.Generic;
using System.Diagnostics;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;

namespace Miner.Geodatabase
{
    /// <summary>
    ///     A supporting class used to load the ArcFM configuration rules for the specified <see cref="mmEditEvent" />.
    /// </summary>
    [DebuggerDisplay("Name = {ClassName}")]
    public sealed class AutoValueRules
    {
        #region Fields

        private readonly IMMConfigTopLevel _ConfigTopLevel = ConfigTopLevel.Instance;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoValueRules" /> class.
        /// </summary>
        /// <param name="ruleClass">The rule class.</param>
        /// <param name="editEvent">The edit event.</param>
        public AutoValueRules(IObjectClass ruleClass, mmEditEvent editEvent)
        {
            this.ClassRulesBySubtype = new Dictionary<int, List<IUID>>();
            this.ClassRulesByName = new Dictionary<string, List<IUID>>();
            this.FieldRulesBySubtype = new Dictionary<int, Dictionary<string, List<IUID>>>();
            this.RelationshipRulesByName = new Dictionary<string, List<IUID>>();
            this.ClassName = ((IDataset) ruleClass).Name;
            this.LoadRules(ruleClass, editEvent);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the name of the class.
        /// </summary>
        public string ClassName { get; private set; }

        /// <summary>
        ///     Gets the dictionary mapping an List of <see cref="IUID" /> of rule IUIDs to the corresponding subtype name.
        /// </summary>
        public Dictionary<string, List<IUID>> ClassRulesByName { get; private set; }

        /// <summary>
        ///     Gets the dictionary mapping an List of <see cref="IUID" /> of rule IUIDs to the corresponding subtype code.
        /// </summary>
        public Dictionary<int, List<IUID>> ClassRulesBySubtype { get; private set; }

        /// <summary>
        ///     Gets the dictionary mapping another Dictionary of <see cref="IUID" /> (which maps an ArrayList of rule IUIDs to the
        ///     corresponding field name) to the corresponding subtype code.
        /// </summary>
        public Dictionary<int, Dictionary<string, List<IUID>>> FieldRulesBySubtype { get; private set; }

        /// <summary>
        ///     Gets the dictionary mapping an List of <see cref="IUID" /> of rule IUIDs to the corresponding relationship name.
        /// </summary>
        public Dictionary<string, List<IUID>> RelationshipRulesByName { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Combines rules from the ClassRulesBySubtype and FieldRulesBySubtype hashtable into a single list for the
        ///     specified subtype.
        /// </summary>
        /// <param name="subtypecode">The subtype code to get corresponding rules.</param>
        /// <returns>List of <see cref="IUID" /> of rule IUIDs to the corresponding subtype code.</returns>
        public List<IUID> AllRulesBySubtype(int subtypecode)
        {
            List<IUID> allRules = new List<IUID>();

            // Intialize the AllRules list with the list of class rules, since they will be distinct.
            if (this.ClassRulesBySubtype.ContainsKey(subtypecode))
                allRules = this.ClassRulesBySubtype[subtypecode];

            // Since field rules may be duplicated between fields, whittle down the list to distinct rules (so they are only validated once).
            if (this.FieldRulesBySubtype.ContainsKey(subtypecode))
            {
                Dictionary<string, List<IUID>> fieldRules = this.FieldRulesBySubtype[subtypecode];
                foreach (KeyValuePair<string, List<IUID>> entry in fieldRules)
                {
                    foreach (IUID uid in entry.Value)
                    {
                        if (!allRules.Contains(uid))
                            allRules.Add(uid);
                    }
                }
            }

            return allRules;
        }

        /// <summary>
        ///     Creates an instance of the class using the specified <see cref="IUID" />.
        /// </summary>
        /// <typeparam name="TSource">The type of the class.</typeparam>
        /// <param name="uid">The uid.</param>
        /// <returns>The class for the GUID; otherwise null.</returns>
        public TSource Create<TSource>(IUID uid)
        {
            if (uid == null) return default(TSource);

            // When the type could be located and matches the given type.
            Type t = Type.GetTypeFromCLSID(new Guid(uid.Value.ToString()));
            if (t == null) return default(TSource);

            object o = Activator.CreateInstance(t);
            if (o is TSource)
                return (TSource) o;

            return default(TSource);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Loads any field rules as configured for the specified object class.
        /// </summary>
        /// <param name="oclass">The object class to be analyzed.</param>
        /// <param name="mmsubtype">The MMSubtype object representing the specific subtype being analyzed.</param>
        /// <param name="editEvent">The edit event.</param>
        private void LoadFieldRules(IObjectClass oclass, IMMSubtype mmsubtype, mmEditEvent editEvent)
        {
            int subtypecode = mmsubtype.SubtypeCode;
            Dictionary<string, List<IUID>> fieldRules = new Dictionary<string, List<IUID>>();

            IFields fields = oclass.Fields;
            for (int i = 0; i < fields.FieldCount; i++)
            {
                IMMField mmfield = null;
                mmsubtype.GetField(i, ref mmfield);
                if (mmfield == null) continue;

                List<IUID> rules = this.RulesByEvent((ID8List) mmfield, editEvent);
                if (rules.Count == 0) continue;

                // Add the list of rules to the Hashtable mapping them to the subtype code.
                if (!fieldRules.ContainsKey(mmfield.FieldName))
                    fieldRules.Add(mmfield.FieldName, rules);
            }

            // Add the Hashtable of subtype-to-field rules to the RuleSet.
            if (!this.FieldRulesBySubtype.ContainsKey(subtypecode))
                this.FieldRulesBySubtype.Add(subtypecode, fieldRules);
        }

        /// <summary>
        ///     Combines rules from the relationship classes into a single list for the object class.
        /// </summary>
        /// <param name="oclass">The oclass.</param>
        /// <param name="editEvent">The edit event.</param>
        private void LoadRelationshipRules(IObjectClass oclass, mmEditEvent editEvent)
        {
            // Iterate through all of the relationship classes.
            IEnumRelationshipClass enumRelClass = oclass.RelationshipClasses[esriRelRole.esriRelRoleAny];
            enumRelClass.Reset();
            IRelationshipClass relClass;
            while ((relClass = enumRelClass.Next()) != null)
            {
                IMMRelationshipClass mmclass = _ConfigTopLevel.GetRelationshipClass(relClass);
                if (mmclass == null) continue;

                List<IUID> rules = this.RulesByEvent((ID8List) mmclass, editEvent);
                if (rules.Count == 0) continue;

                if (!this.RelationshipRulesByName.ContainsKey(mmclass.RelationshipClassName))
                    this.RelationshipRulesByName.Add(mmclass.RelationshipClassName, rules);
            }
        }

        /// <summary>
        ///     Loads the rules for the specified object class.
        /// </summary>
        /// <param name="oclass">The object class whose rules are needed.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <remarks>
        ///     Attribute rule assignment is determined from the ArcFM geodatabase configuration via the MMConfigTopLevel
        ///     object.
        /// </remarks>
        private void LoadRules(IObjectClass oclass, mmEditEvent editEvent)
        {
            // Iterate through all subtypes in the object class.
            ISubtypes subtypes = (ISubtypes) oclass;
            IEnumSubtype enumSubtype = subtypes.Subtypes;
            enumSubtype.Reset();

            int subtypecode;
            string subtypename;
            while ((subtypename = enumSubtype.Next(out subtypecode)) != null)
            {
                IMMSubtype mmsubtype = _ConfigTopLevel.GetSubtypeByID(oclass, subtypecode, false);
                if (mmsubtype == null) continue;

                // Iterate through all children of the MMSubtype instance.
                List<IUID> rules = this.RulesByEvent((ID8List) mmsubtype, editEvent);

                // Add the list of class rules to the set based on name and code.
                if (!this.ClassRulesByName.ContainsKey(subtypename))
                    this.ClassRulesByName.Add(subtypename, rules);

                if (!this.ClassRulesBySubtype.ContainsKey(subtypecode))
                    this.ClassRulesBySubtype.Add(subtypecode, rules);

                // Get field rules for this subtype.
                this.LoadFieldRules(oclass, mmsubtype, editEvent);
            }

            // Load the relationship rules.
            this.LoadRelationshipRules(oclass, editEvent);
        }

        /// <summary>
        ///     Creates a list of the <see cref="IUID" /> rules by the specified event.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>A list of <see cref="IUID" /> objects for the corresponding event.</returns>
        private List<IUID> RulesByEvent(ID8List list, mmEditEvent editEvent)
        {
            List<IUID> rules = new List<IUID>();

            list.Reset();
            ID8ListItem item;
            while ((item = list.Next(false)) != null)
            {
                if (item.ItemType == mmd8ItemType.mmitAutoValue)
                {
                    IMMAutoValue autovalue = (IMMAutoValue) item;
                    if (autovalue.EditEvent == editEvent)
                    {
                        if (autovalue.AutoGenID != null)
                        {
                            if (!rules.Contains(autovalue.AutoGenID))
                                rules.Add(autovalue.AutoGenID);
                        }
                    }
                }
            }

            return rules;
        }

        #endregion
    }
}