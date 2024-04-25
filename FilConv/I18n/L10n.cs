using System;
using System.Collections.Generic;
using System.Resources;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml;

namespace FilConv.I18n
{
    /// <summary>
    /// Localization facility.
    /// </summary>
    /// This class is NOT thread-safe and must be accessed only from the UI thread.
    public class L10n : MarkupExtension
    {
        private static readonly ResourceManager _manager;
        private static CultureInfo? _culture;
        private static readonly List<LocalizedProperty> _localizedProperties;

        static L10n()
        {
            _manager = new ResourceManager("FilConv.Properties.Resources", typeof(L10n).Assembly);
            _localizedProperties = new List<LocalizedProperty>();
        }

        /// <summary>
        /// Get or set culture of the localized resources.
        /// </summary>
        /// Setting this property automatically updates all previously localized properties.
        public static CultureInfo? Culture
        {
            get => _culture;
            set
            {
                _culture = value;
                UpdateLocalizedProperties();
            }
        }

        /// <summary>
        /// Get localized resource.
        /// </summary>
        /// <param name="name">name of the resource</param>
        /// <returns>
        /// The localized resource. If the resource is not found, a string
        /// is returned of the form "#name".
        /// </returns>
        public static object GetObject(string name)
        {
            return _manager.GetObject(name, Culture) ?? "#" + name;
        }

        /// <summary>
        /// Add localized property.
        /// </summary>
        /// <param name="target">target object</param>
        /// <param name="property">
        /// target property. Must be either a <see cref="AvaloniaProperty"/> if the target is
        /// a <see cref="AvaloniaObject"/>, or a <see cref="ClrPropertyInfo"/> otherwise
        /// </param>
        /// <param name="resourceName">resource name</param>
        /// <returns></returns>
        public static LocalizedProperty AddLocalizedProperty(object target, object property, string resourceName)
        {
            if (!(property is ClrPropertyInfo || (property is AvaloniaProperty && target is AvaloniaObject)))
            {
                throw new ArgumentException(
                    $"Unsupported property type {property.GetType()} for target type {target.GetType()}");
            }

            LocalizedProperty lp = new LocalizedProperty(target, property, resourceName);
            _localizedProperties.Add(lp);
            return lp;
        }

        /// <summary>
        /// Update all the previously added properties with resources from the current locale.
        /// </summary>
        public static void UpdateLocalizedProperties()
        {
            _localizedProperties.RemoveAll(lp => !lp.Update());
        }

        public L10n()
        {
        }

        public L10n(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Get or set property resource name.
        /// </summary>
        public string? Key { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget? pvt = (IProvideValueTarget?)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (pvt == null || Key == null)
                return "<error>";
            return AddLocalizedProperty(pvt.TargetObject, pvt.TargetProperty, Key).Value;
        }

        /// <summary>
        /// A cached reference to a localized property.
        /// </summary>
        /// Instances of this class are used to remember which properties were
        /// provided with localized data. Later when the application culture is
        /// changed these properties are re-populated with localized data for
        /// the new culture.
        public class LocalizedProperty
        {
            private readonly WeakReference _weakTarget;
            private readonly object _property;
            private readonly string _key;

            internal LocalizedProperty(object targetObject, object targetProperty, string key)
            {
                _weakTarget = new WeakReference(targetObject);
                _property = targetProperty;
                _key = key;
            }

            /// <summary>
            /// Get value of the property in the current culture.
            /// </summary>
            public object Value
            {
                get
                {
                    return L10n.GetObject(_key);
                }
            }

            /// <summary>
            /// Update the property with a value for the current culture.
            /// </summary>
            /// <returns>
            /// True if the property was updated successfully, false if the target
            /// object is already garbage-collected.
            /// </returns>
            public bool Update()
            {
                object? target = _weakTarget.Target;

                if (target == null)
                {
                    return false;
                }

                if (target is AvaloniaObject dob && _property is AvaloniaProperty dp)
                {
                    dob.SetValue(dp, Value);
                    return true;
                }

                if (_property is ClrPropertyInfo pi)
                {
                    pi.Set(target, Value);
                    return true;
                }

                throw new InvalidOperationException(String.Format(
                    "Cannot set property type {0} on object type {1}",
                    _property.GetType(),
                    target.GetType()));
            }
        }
    }
}
