namespace ESRI.ArcGIS.Geometry
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IPolyline" /> interface.
    /// </summary>
    public static class PolylineExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the angle of the line.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="double" /> representing the angle.</returns>
        public static double GetAngle(this IPolyline source)
        {
            ILine line = new LineClass();
            line.PutCoords(source.FromPoint, source.ToPoint);
            return line.Angle;
        }

        /// <summary>
        ///     Converts the angle of the line to Arithmetic Rotation.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="double" /> representing the angle.</returns>
        public static double GetArithmeticAngle(this IPolyline source)
        {
            ILine line = new LineClass();
            line.PutCoords(source.FromPoint, source.ToPoint);
            return line.GetArithmeticAngle();
        }

        /// <summary>
        ///     Converts the angle of the line to Geographic Rotation.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="double" /> representing the angle.</returns>
        public static double GetGeographicAngle(this IPolyline source)
        {
            ILine line = new LineClass();
            line.PutCoords(source.FromPoint, source.ToPoint);
            return line.GetGeographicAngle();
        }

        #endregion
    }
}