Tomin.TotalCmd.AzureBlob
========================

Total Commander plugin for Azure Blob service

Prerequisites
---------------
- .NET Framework 4.0 Client Profile
- Total Commander 8.0+
- To work with Development storage - latest Storage Emulator is required.

Download
---------------
**Pre-Release:**
https://github.com/alex-tomin/Tomin.TotalCmd.AzureBlob/releases/download/samplebuild/AzureBlob.zip

**All Releases:**
https://github.com/alex-tomin/Tomin.TotalCmd.AzureBlob/releases

Known issues
---------------
- 64 bit not supported (cannot compile)
- Cannot remove account once added (all blobs will be removed)
- Background copying not supported
- No Progress

Changes
---------------

## 0.2 - Alfa

- Support page blobs
- Remove dependency on Azure SDK
- Show last modified time for directories
- Downgraded to .Net 4.0 for wider support

## 0.1 - Alpha.
The first release.

Credits
---------------
Used total commander plugin framework for .NET: https://code.google.com/p/totalcommander-plugin-donnet/
updated to the last version of MSBuild. Thanks to the author.
