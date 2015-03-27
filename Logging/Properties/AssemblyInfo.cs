// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Tekla">
//   Copyright (c) by Tekla. All rights reserved.
// </copyright>
// <summary>
//   AssemblyInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Logging")]
[assembly: AssemblyDescription("Tekla logging abstraction layer and utils")]
#if (Debug || DEBUG)

[assembly: AssemblyConfiguration("Debug, HEAD")]
#else
[assembly: AssemblyConfiguration("Release, HEAD")]
#endif

[assembly: AssemblyCompany("Tekla")]
[assembly: AssemblyProduct("Logging")]
[assembly: AssemblyCopyright("Copyright © Tekla 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a _type in this assembly from 
// COM, set the ComVisible attribute to true on that _type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("f9bde7ce-eb94-4f82-98c2-19b8d0e330c8")]

// Version information for an assembly consists of the following four values:
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("21.0.0.0")]
[assembly: AssemblyFileVersion("21.0.0.0")]