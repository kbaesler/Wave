Examples
================================

Licensing Applications
---------------------------------
The ArcFM and ArcGIS software packages require a license when using the APIs outside of the ArcGIS platform (i.e. in stand-alone applications or services). You can check-out/in licenses using the ``RuntimeAuthorization`` class provided in Wave.

.. code-block:: c#

  using(RuntimeAuthorization lic = new RuntimeAuthorization())
  {

      // Check-out the licenses specified by the enumerations.
      if(lic.Initialize(esriLicensedProductCode.esriLicensedProductCodeArcEditor, mmLicensedProductCode.mmLPArcFM))
      {
          // Do work.
      }

  } // Check-in the licenses.
