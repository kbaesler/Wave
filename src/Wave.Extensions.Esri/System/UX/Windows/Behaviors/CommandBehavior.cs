using System.Windows.Input;

namespace System.Windows.Behaviors
{
    /// <summary>
    ///     Defines the attached properties to create a <see cref="CommandBehaviorBinding" /> using Commanding.
    /// </summary>
    public static class CommandBehavior
    {
        #region Fields

        /// <summary>
        ///     Behavior Attached Dependency Property
        /// </summary>
        private static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.RegisterAttached("Behavior", typeof (CommandBehaviorBinding), typeof (CommandBehavior),
                new FrameworkPropertyMetadata((CommandBehaviorBinding) null));

        /// <summary>
        ///     CommandParameter Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof (object), typeof (CommandBehavior),
                new FrameworkPropertyMetadata(null,
                    OnCommandParameterChanged));

        /// <summary>
        ///     Command Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof (ICommand), typeof (CommandBehavior),
                new FrameworkPropertyMetadata(null,
                    OnCommandChanged));

        /// <summary>
        ///     Event Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.RegisterAttached("Event", typeof (string), typeof (CommandBehavior),
                new FrameworkPropertyMetadata(String.Empty,
                    OnEventChanged));

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the Command property.
        /// </summary>
        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand) d.GetValue(CommandProperty);
        }

        /// <summary>
        ///     Gets the CommandParameter property.
        /// </summary>
        public static object GetCommandParameter(DependencyObject d)
        {
            return d.GetValue(CommandParameterProperty);
        }

        /// <summary>
        ///     Gets the Event dependency property.
        /// </summary>
        public static string GetEvent(DependencyObject d)
        {
            return (string) d.GetValue(EventProperty);
        }

        /// <summary>
        ///     Sets the Command property.
        /// </summary>
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        /// <summary>
        ///     Sets the CommandParameter property.
        /// </summary>
        public static void SetCommandParameter(DependencyObject d, object value)
        {
            d.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        ///     Sets the Event dependency property.
        /// </summary>
        public static void SetEvent(DependencyObject d, string value)
        {
            d.SetValue(EventProperty, value);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the Behavior property.
        /// </summary>
        private static CommandBehaviorBinding GetBehavior(DependencyObject d)
        {
            return (CommandBehaviorBinding) d.GetValue(BehaviorProperty);
        }

        /// <summary>
        ///     Fetches the or creates binding.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <returns>The command bynding</returns>
        private static CommandBehaviorBinding GetOrCreateBinding(DependencyObject d)
        {
            CommandBehaviorBinding binding = GetBehavior(d);
            if (binding == null)
            {
                binding = new CommandBehaviorBinding();
                SetBehavior(d, binding);
            }
            return binding;
        }

        /// <summary>
        ///     Handles changes to the Command property.
        /// </summary>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandBehaviorBinding binding = GetOrCreateBinding(d);
            binding.Command = (ICommand) e.NewValue;
        }

        /// <summary>
        ///     Handles changes to the CommandParameter property.
        /// </summary>
        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandBehaviorBinding binding = GetOrCreateBinding(d);
            binding.CommandParameter = e.NewValue;
        }

        /// <summary>
        ///     Handles changes to the Event property.
        /// </summary>
        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandBehaviorBinding binding = GetOrCreateBinding(d);

            // Rebind the Command to the new event and unregister the old event if one is already created.
            if (binding.Event != null && binding.Owner != null)
                binding.Dispose();

            if (e.NewValue == null) return;

            string eventName = e.NewValue.ToString();
            if (string.IsNullOrEmpty(eventName)) return;

            // Bind the new event to the command
            binding.BindEvent(d, eventName);
        }

        /// <summary>
        ///     Sets the Behavior property.
        /// </summary>
        private static void SetBehavior(DependencyObject d, CommandBehaviorBinding value)
        {
            d.SetValue(BehaviorProperty, value);
        }

        #endregion
    }
}