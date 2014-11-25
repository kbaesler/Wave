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

Switching ArcFM Auto Updater Modes
--------------------------------------
The ArcFM Solution provides a very useful edit based triggers that allow for adding behaviors to features and tables while editing. However, an edit can cause a cascading effect when AUs are assigned to features that update another feature. You can disable the event cascading by telling the ArcFM Framework to temporarily disable the executing of AUs using the ``AutoUpdaterModeReverter`` class.

.. code-block:: c#

  using (new AutoUpdaterModeReverter(mmAutoUpdaterMode.mmAUMNoEvents))
  {
      // Do something that shouldn't trigger additional ArcFM AUs.
  }
