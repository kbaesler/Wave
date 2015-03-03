using System;

namespace ESRI.ArcGIS.Geometry
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ILine" /> interface.
    /// </summary>
    public static class LineExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the angle of the line to Arithmetic Rotation.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="double" /> representing the angle.</returns>
        public static double GetArithmeticAngle(this ILine source)
        {
            if (source == null) return 0;

            //      Calculated Angle          Arithmetic Angle         
            //            90                        90                     
            //             |                         |                   
            //             |                         |                  
            //             |                         |                
            //   +-180 ----------- 0          180 ---------- 0       
            //             |                         |                    
            //             |                         |                   
            //             |                         |                    
            //            -90                       270                    

            double radian = source.Angle;
            if (radian < 0)
                return 360 - (((radian*360)*-1)/(2*Math.PI));

            return (radian*360)/(2*Math.PI);
        }

        /// <summary>
        ///     Converts the angle of the line to Geographic Rotation.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="double" /> representing the angle.</returns>
        public static double GetGeographicAngle(this ILine source)
        {
            if (source == null) return 0;

            //      Calculated Angle          Geographic Angle         
            //            90                         0                     
            //             |                         |                   
            //             |                         |                  
            //             |                         |                
            //   +-180 ----------- 0          270 ---------- 90       
            //             |                         |                    
            //             |                         |                   
            //             |                         |                    
            //            -90                       180          
            double angle = 45 - (source.Angle - 45);
            if (angle < 0)
                return angle + 360;

            return angle;
        }

        #endregion
    }
}