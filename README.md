[![Build status](https://ci.appveyor.com/api/projects/status/var20s6rdqf8bfv5/branch/master?svg=true)](https://ci.appveyor.com/project/dgriffen/whackwhackterminal/branch/master)

# Building
## Prerequisites
You need the following components installed in order to build WhackWhackTerminal
- Visual Studio 2017 with extensibility workload
- Node.js v8.9.4 LTS (**32-bit**)
- Python 2.7
- Visual C++ Build Tools 2015

## Steps
Once you have the prerequisites you can begin setting up your project to build.
1. Open a terminal window.
2. Run `npm config set msvs_version 2015`
3. Navigate to the EmbbededTerminal project directory
4. Run `npm install`

After this you can simply open the solution file in Visual Studio and hit f5 to start debugging.

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

.