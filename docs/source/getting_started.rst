Getting Started
================================
Wave is C# library extends the ArcGIS for Desktop and ArcFM Solution APIs in an effort to simplify customizing these products. The library has been designed to use extension methods to expose the features of the library.

.. note::
    It's always best to consult the :doc:`apidocs` documentation to understand the purpose of the methods and classes provided by Wave.

GitHub
---------------------
You can visit `GitHub <https://github.com/Jumpercables/Wave>`_ to download and compiled the source. Once compiled you can reference the assemblies in your projects.

Microsoft Package Manager
--------------------------------------
Wave is publicly distributed using the `Nuget <http://www.nuget.org>`_ service that is available to the .NET community within Visual Studios.

You can download a release of the library using the Microsoft NuGet Package Manager within Visual Studio.

To install **Wave Extensions for ArcGIS**, run the following command in the Package Manager Console.

.. code-block:: c#

	PM> Install-Package Wave.Extensions.Esri

To install **Wave Extensions for ArcFM**, run the following command in the Package Manager Console.

.. code-block:: c#

	PM> Install-Package Wave.Extensions.Miner

.. note::
  The **Wave.Extensions.Esri** package features are included in the **Wave.Extensions.Miner** package, as the *ArcFM Solution* product is tightly coupled with the *ArcGIS for Desktop* product.

Requirements
--------------------
The prerequisites for developing and using Wave requires both open source and commercial software.

- `ArcGIS Desktop 10.2 <http://www.esri.com/software/arcgis>`_
- `ArcFM Solution 10.2 <http://www.schneider-electric.com/products/ww/en/6100-network-management-software/6120-geographic-information-system-arcfm-solution/62051-arcfm/>`_
- `Apache log4net 2.0.3 <https://github.com/apache/log4net>`_
- `Microsoft .NET Framework 3.5 SP1 <http://www.microsoft.com/en-us/download/details.aspx?id=22>`_
