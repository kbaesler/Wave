using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.SystemUI;

using Miner.Interop;

namespace Miner
{
    /// <summary>
    ///     Container class for <see cref="Miner.ArcFM" /> classes.
    /// </summary>
    public static class ArcFM
    {
        #region Nested Type: Commands

        /// <summary>
        ///     Container class for <see cref="Miner.ArcFM.Commands.Name" /> class used to identify the common ArcFM commands
        ///     which
        ///     may be
        ///     used for customization.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Commands
        {
            #region Nested Type: Name

            /// <summary>
            ///     Represents the common command names for the ArcFM commands which may be used for customization.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
            public static class Name
            {
                #region Constants

                /// <summary>
                ///     The name of the command that will clear the trace results.
                /// </summary>
                public const string ClearResults = "MMTraceUI.clsClear";

                /// <summary>
                ///     The name of the button that will indicate if the trace results should be a selection or graphic.
                /// </summary>
                public const string ResultsAsSelection = "MMTraceUI.clsResultsAsSelection";

                /// <summary>
                ///     The name of the button that will zoom to the results of a trace.
                /// </summary>
                public const string ZoomToResults = "MMTraceUI.clsZoomToResults";

                #endregion
            }

            #endregion

            #region Nested Type: Trace

            /// <summary>
            ///     Provides access to the product trace commands.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
            public static class Trace
            {
                #region Public Properties

                /// <summary>
                ///     Gets a value indicating whether the results are displayed as a selection.
                /// </summary>
                /// <value><c>true</c> if the results are displayed as a selection; otherwise, <c>false</c>.</value>
                public static bool ResultsAsSelection
                {
                    get
                    {
                        if (ArcMap.Application != null)
                        {
                            ICommand cmd = ArcMap.Application.GetCommandItem(Name.ResultsAsSelection) as ICommand;
                            return (cmd != null && cmd.Checked);
                        }

                        return false;
                    }
                }

                /// <summary>
                ///     Gets a value indicating whether the trace should zoom to results.
                /// </summary>
                /// <value><c>true</c> if the trace should zoom to results; otherwise, <c>false</c>.</value>
                public static bool ZoomToResults
                {
                    get
                    {
                        if (ArcMap.Application != null)
                        {
                            ICommand cmd = ArcMap.Application.GetCommandItem(Name.ZoomToResults) as ICommand;
                            return (cmd != null && cmd.Checked);
                        }

                        return false;
                    }
                }

                #endregion
            }

            #endregion
        }

        #endregion

        #region Nested Type: Extensions

        /// <summary>
        ///     Container for the <see cref="Miner.ArcFM.Extensions.Name" /> and
        ///     <see cref="Miner.ArcFM.Extensions.Guid" />
        ///     classes used to identify the common extensions for the ArcFM extensions which may be used for customization.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Extensions
        {
            #region Nested Type: Guid

            /// <summary>
            ///     Represents the common extension GUIDs for the ArcFM extensions which may be used for customization.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
            public static class Guid
            {
                #region Constants

                /// <summary>
                ///     The GUID for the ArcFM Attribute Editor.
                /// </summary>
                public const string AttributeEditor = "{B2575F11-BD46-11D3-BD53-00500462EE0B}";

                /// <summary>
                ///     The GUID of the compatible units tab extension for CU tab.
                /// </summary>
                public const string CUTab = "{89BAB0A4-5B17-11D3-8901-00104B9F25F6}";

                /// <summary>
                ///     The GUID of the design tab extension for the ArcFM Attribute Editor (Designer only).
                /// </summary>
                public const string DesignTab = "{F370324A-4AB8-11D3-88FC-00104B9F25F6}";

                /// <summary>
                ///     The GUID of the features tab extension that list of all features on the Features tab that are available for
                ///     placement.
                /// </summary>
                public const string FeaturesTab = "{6C209705-9C40-4A65-8070-DDAF7458BB65}";

                /// <summary>
                ///     The GUID of the feeder space extension for ArcFM Feeder Manager.
                /// </summary>
                public const string FeederSpace = "{9B5DBAF2-B28D-4E73-9AE9-5202D72A3BC5}";

                /// <summary>
                ///     The GUID of the process framework extension for the ArcFM Session Manager or Workflow Manager.
                /// </summary>
                public const string ProcessFrameworkCache = "{8B0CA484-454F-4462-9A48-619A9F026E57}";

                /// <summary>
                ///     The GUID of the QAQC tab extension for the ArcFM Attribute Editor.
                /// </summary>
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "QAQC")]
                public const string QAQCTab = "{E5AC0B62-E4BE-11D3-B4A0-006008AD9A5E}";

                /// <summary>
                ///     The GUID of the section tab extension for the ArcFM Attribute Editor.
                /// </summary>
                public const string SelectionTab = "{23130747-7104-11D3-8905-00104B9F25F6}";

                /// <summary>
                ///     The GUID of the targets tab extension for the ArcFM Attribute Editor.
                /// </summary>
                public const string TargetsTab = "{23130742-7104-11D3-8905-00104B9F25F6}";

                #endregion
            }

            #endregion

            #region Nested Type: Name

            /// <summary>
            ///     Represents the extension names for the ArcFM extensions which may be used for customization.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
            public static class Name
            {
                #region Constants

                /// <summary>
                ///     The name of the compatible units tab extension for CU tab.
                /// </summary>
                public const string CUTab = "CUTopLevel";

                /// <summary>
                ///     The name of the design tab extension for the ArcFM Attribute Editor (Designer only).
                /// </summary>
                public const string DesignTab = "DesignerTopLevel";

                /// <summary>
                ///     The name of the Designer extension.
                /// </summary>
                public const string Designer = "Designer XML API Extension";

                /// <summary>
                ///     The name of the features tab extension that list of all features on the Features tab that are available for
                ///     placement.
                /// </summary>
                public const string FeaturesTab = "FeatureTopLevel";

                /// <summary>
                ///     The name of the feeder space extension for ArcFM Feeder Manager.
                /// </summary>
                public const string FeederSpace = "MMFeeder";

                /// <summary>
                ///     The name of the process framework extension for the ArcFM Session Manager or Workflow Manager.
                /// </summary>
                public const string ProcessFrameworkCache = "Process Framework Integration";

                /// <summary>
                ///     The name of the ArcFM Properties extension.
                /// </summary>
                public const string Properties = "MMPropertiesExt";

                /// <summary>
                ///     The name of the QAQC tab extension for the ArcFM Attribute Editor.
                /// </summary>
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "QAQC")]
                public const string QAQCTab = "QAQCTopLevel";

                /// <summary>
                ///     The name of the section tab extension for the ArcFM Attribute Editor.
                /// </summary>
                public const string SelectionTab = "FeSelTopLevel";

                /// <summary>
                ///     The name of the targets tab extension for the ArcFM Attribute Editor.
                /// </summary>
                public const string TargetsTab = "CUSelTopLevel";

                /// <summary>
                ///     The name of the trace bridge extension used by the <see cref="IMMTraceBridge" /> interface.
                /// </summary>
                public const string TraceBridge = "ElectricGasWaterExt";

                #endregion
            }

            #endregion
        }

        #endregion

        #region Nested Type: Process

        /// <summary>
        ///     Container class for <see cref="Miner.ArcFM.Process.SessionManager" /> and
        ///     <see cref="Miner.ArcFM.Process.WorkflowManager" /> classes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
        public static class Process
        {
            #region Nested Type: SessionManager

            /// <summary>
            ///     Container for the <see cref="SessionManager.Configurations" />, <see cref="SessionManager.Tasks" />,
            ///     <see cref="SessionManager.Tables" />, <see cref="SessionManager.Filters" /> and <see cref="SessionManager.Roles" />
            ///     classes class used to identify the common configurations for Session Manager.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
            public static class SessionManager
            {
                #region Constants

                /// <summary>
                ///     The name extension.
                /// </summary>
                public const string Name = "MMSessionManager";

                #endregion

                #region Nested Type: Configurations

                /// <summary>
                ///     Container for the <see cref="SessionManager.Configurations" /> class used to identify
                ///     the common properties for Session Manager which may be used for customization.
                /// </summary>
                public static class Configurations
                {
                    #region Constants

                    /// <summary>
                    ///     The configuration name for the enterprise unc path location for the mobile packets.
                    /// </summary>
                    public const string Enterprise = "ProcessFrameworkEnterprise";

                    /// <summary>
                    ///     The configuration name for the field path location for the mobile packets.
                    /// </summary>
                    public const string Field = "ProcessFrameworkField";

                    #endregion
                }

                #endregion

                #region Nested Type: Filters

                /// <summary>
                ///     Container for the <see cref="SessionManager.Filters" /> class used to identify
                ///     the common filters for Session Manager.
                /// </summary>
                public static class Filters
                {
                    #region Constants

                    /// <summary>
                    ///     The name for the all sessions filter.
                    /// </summary>
                    public const string AllSessions = "All Sessions";

                    /// <summary>
                    ///     The name for the user sessions filter.
                    /// </summary>
                    public const string UserSessions = "My Sessions";

                    #endregion
                }

                #endregion

                #region Nested Type: Roles

                /// <summary>
                ///     Container for the <see cref="SessionManager.Roles" /> class used to identify
                ///     the common roles for Session Manager.
                /// </summary>
                public static class Roles
                {
                    #region Constants

                    /// <summary>
                    ///     The always mobile session user role.
                    /// </summary>
                    public const string AlwaysMobile = "SESSION_ALWAYS_MOBILE_USER";

                    /// <summary>
                    ///     The approval officer user role.
                    /// </summary>
                    public const string ApprovalOfficer = "SESSION_APPROVE";

                    /// <summary>
                    ///     The editor user role.
                    /// </summary>
                    public const string Editor = "SESSION_EDITOR";

                    /// <summary>
                    ///     The redline techician user role.
                    /// </summary>
                    public const string RedlineTechnician = "SESSION_REDLINE_TECH";

                    #endregion
                }

                #endregion

                #region Nested Type: Tables

                /// <summary>
                ///     Container for the <see cref="SessionManager.Tables" /> class used to identify
                ///     the common tasks for Session Manager.
                /// </summary>
                public static class Tables
                {
                    #region Constants

                    /// <summary>
                    ///     The session table name.
                    /// </summary>
                    public const string Session = "MM_SESSION";

                    #endregion
                }

                #endregion

                #region Nested Type: Tasks

                /// <summary>
                ///     Container for the <see cref="SessionManager.Tasks" /> class used to identify
                ///     the common tasks for Session Manager.
                /// </summary>
                public static class Tasks
                {
                    #region Constants

                    /// <summary>
                    ///     The task that will close the session.
                    /// </summary>
                    public const string CloseSession = "Close Session";

                    /// <summary>
                    ///     The task will Create a new session in the database.
                    /// </summary>
                    public const string CreateSession = "Create Session";

                    /// <summary>
                    ///     The task will delete the session from the database.
                    /// </summary>
                    public const string DeleteSession = "Delete Session";

                    /// <summary>
                    ///     The task that will open a redline session.
                    /// </summary>
                    public const string OpenRedline = "Open Redline Session";

                    /// <summary>
                    ///     The task that will open the session for editing.
                    /// </summary>
                    public const string OpenSession = "Open Edit Session";

                    /// <summary>
                    ///     The task that will save the session.
                    /// </summary>
                    public const string SaveSession = "Save Session";

                    /// <summary>
                    ///     The task will perform the send/receive task for session data.
                    /// </summary>
                    public const string SendAndReceive = "Send/Receive Mobile Data";

                    /// <summary>
                    ///     The task will send the session back to the enterprise or field.
                    /// </summary>
                    public const string SendSession = "Send Session";

                    #endregion
                }

                #endregion
            }

            #endregion

            #region Nested Type: WorkflowManager

            /// <summary>
            ///     Container for the <see cref="WorkflowManager.Configurations" />, <see cref="WorkflowManager.Tasks" />,
            ///     <see cref="WorkflowManager.Tables" />, <see cref="WorkflowManager.Filters" /> and
            ///     <see cref="WorkflowManager.Roles" /> classes used to identify the common properties for Workflow Manager which may
            ///     be used for customization.
            /// </summary>
            public static class WorkflowManager
            {
                #region Constants

                /// <summary>
                ///     The name extension.
                /// </summary>
                public const string Name = "MMWorkflowManager";

                #endregion

                #region Nested Type: Configurations

                /// <summary>
                ///     Container for the <see cref="WorkflowManager.Configurations" /> class used to identify
                ///     the common configurations for Workflow Manager.
                /// </summary>
                public static class Configurations
                {
                    #region Constants

                    /// <summary>
                    ///     The configuration name for the enterprise unc path location for the mobile packets.
                    /// </summary>
                    public const string Enterprise = "ProcessFrameworkEnterprise";

                    /// <summary>
                    ///     The configuration name for the field path location for the mobile packets.
                    /// </summary>
                    public const string Field = "ProcessFrameworkField";

                    #endregion
                }

                #endregion

                #region Nested Type: Filters

                /// <summary>
                ///     Container for the <see cref="WorkflowManager.Filters" /> class used to identify
                ///     the common filters for Workflow Manager.
                /// </summary>
                public static class Filters
                {
                    #region Constants

                    /// <summary>
                    ///     The name for the all work request filter.
                    /// </summary>
                    public const string AllWorkRequests = "All Work Requests";

                    /// <summary>
                    ///     The name for the user work request filter.
                    /// </summary>
                    public const string UserWorkRequests = "My Work Requests";

                    #endregion
                }

                #endregion

                #region Nested Type: Roles

                /// <summary>
                ///     Container for the <see cref="WorkflowManager.Roles" /> class used to identify
                ///     the common roles for Workflow Manager.
                /// </summary>
                public static class Roles
                {
                    #region Constants

                    /// <summary>
                    ///     The designer user role.
                    /// </summary>
                    public const string Designer = "WMS_DESIGNER";

                    /// <summary>
                    ///     The mobile designer user role.
                    /// </summary>
                    public const string Mobile = "WMS_MOBILE_DESIGNER";

                    #endregion
                }

                #endregion

                #region Nested Type: Tables

                /// <summary>
                ///     Container for the <see cref="WorkflowManager.Tables" /> class used to identify
                ///     the common tables for Workflow Manager.
                /// </summary>
                public static class Tables
                {
                    #region Constants

                    /// <summary>
                    ///     The approved design table name.
                    /// </summary>
                    public const string ApprovedDesign = "MM_WMS_APPROVED_DESIGNS";

                    /// <summary>
                    ///     The compatible unit table name.
                    /// </summary>
                    public const string CompatibleUnit = "MM_WMS_COMPATIBLE_UNIT";

                    /// <summary>
                    ///     The design table name.
                    /// </summary>
                    public const string Design = "MM_WMS_DESIGN";

                    /// <summary>
                    ///     The work location table name.
                    /// </summary>
                    public const string WorkLocation = "MM_WMS_WORK_LOCATION";

                    /// <summary>
                    ///     The work request table name.
                    /// </summary>
                    public const string WorkRequest = "MM_WMS_WORK_REQUEST";

                    #endregion
                }

                #endregion

                #region Nested Type: Tasks

                /// <summary>
                ///     Container for the <see cref="WorkflowManager.Tasks" /> class used to identify
                ///     the common tasks for Workflow Manager.
                /// </summary>
                public static class Tasks
                {
                    #region Constants

                    /// <summary>
                    ///     The task that will close the design.
                    /// </summary>
                    public const string CloseDesign = "Close Design";

                    /// <summary>
                    ///     The task that will create a design.
                    /// </summary>
                    public const string CreateDesign = "Create Design";

                    /// <summary>
                    ///     The task that will create a work request.
                    /// </summary>
                    public const string CreateWorkRequest = "Create Work Request";

                    /// <summary>
                    ///     The task that will delete the design or work request.
                    /// </summary>
                    public const string Delete = "Delete";

                    /// <summary>
                    ///     The task that will open the design.
                    /// </summary>
                    public const string OpenDesign = "Open Design";

                    /// <summary>
                    ///     The task that will save the design.
                    /// </summary>
                    public const string SaveDesign = "Save Design";

                    /// <summary>
                    ///     The task that will submit the design.
                    /// </summary>
                    public const string SubmitDesign = "Submit Design";

                    #endregion
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}