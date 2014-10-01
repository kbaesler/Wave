using System.Linq;
using System.Windows.Documents;

namespace System.Windows
{
    /// <summary>
    ///     A static class providing methods for working with the adorners.
    /// </summary>
    internal static class AdornerExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Determines whether the specified <see cref="AdornerLayer" /> contains the adorner.
        /// </summary>
        /// <typeparam name="T">The type of adroner.</typeparam>
        /// <param name="layer">The layer.</param>
        /// <param name="element">The element.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="AdornerLayer" /> contains the adorner; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAdorner<T>(this AdornerLayer layer, UIElement element)
            where T : Adorner
        {
            Adorner[] adorners = layer.GetAdorners(element);

            if (adorners == null) return false;

            for (int i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Removes the adorners.
        /// </summary>
        /// <typeparam name="T">The type of adroner.</typeparam>
        /// <param name="layer">The layer.</param>
        /// <param name="element">The element.</param>
        public static void RemoveAdorners<T>(this AdornerLayer layer, UIElement element)
            where T : Adorner
        {
            if (layer != null)
            {
                Adorner[] adorners = layer.GetAdorners(element);
                if (adorners == null)
                    return;

                foreach (var adorner in adorners.OfType<T>())
                    layer.Remove(adorner);
            }
        }

        /// <summary>
        ///     Tries the add adorner to the element.
        /// </summary>
        /// <typeparam name="T">The type of adroner.</typeparam>
        /// <param name="element">The element.</param>
        /// <param name="adorner">The adorner.</param>
        public static void TryAddAdorner<T>(this UIElement element, Adorner adorner)
            where T : Adorner
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);
            if (layer != null && !layer.ContainsAdorner<T>(element))
                layer.Add(adorner);
        }

        /// <summary>
        ///     Tries to remove all of the adorners with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of adorner.</typeparam>
        /// <param name="element">The element.</param>
        public static void TryRemoveAdorners<T>(this UIElement element)
            where T : Adorner
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);
            if (layer != null)
                layer.RemoveAdorners<T>(element);
        }

        #endregion
    }
}