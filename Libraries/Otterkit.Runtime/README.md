# Otterkit COBOL Runtime Library

Otterkit COBOL's default runtime library. This library aims to implement all aspects of Standard COBOL's runtime behaviour.

Otterkit is a free and open source compiler for the [COBOL Programming Language](https://en.wikipedia.org/wiki/COBOL#History_and_specification) on the .NET platform.

Warning: The project is currently in pre-release, so not all of the standard has been implemented.

## About

COBOL was created in 1959 by the [CODASYL Committee](https://en.wikipedia.org/wiki/CODASYL) (With Rear Admiral Grace Hopper as a technical consultant to the committee), its design follows Grace Hopper's belief that programs should be written in a language that is close to English. It prioritizes readability, reliability, and long-term maintenance. The language has been implemented throughout the decades on many platforms with many dialects. Otterkit COBOL is a free and open source compiler that aims to implement the [ISO/IEC 1989:2023 COBOL Standard](https://www.iso.org/standard/74527.html) (COBOL 2023) on the .NET platform.

## Installation

### Quick Install

This runtime library is included by default with Otterkit's project templates.

Otterkit is available to install on the [Nuget package manager](https://www.nuget.org/packages/Otterkit/) ([.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) is required). To install, type these two lines into the command line:
```
dotnet new install Otterkit.Templates::1.0.10-alpha

dotnet tool install --global Otterkit --version 1.0.40-alpha
```

### Build from Source

First, clone the git repo from [https://github.com/otterkit/otterkit.git](https://github.com/otterkit/otterkit.git) to get the source code. To access the libotterkit submodule inside, use the `--recurse-submodules --remote-submodules` flag on the clone command. To run, navigate into the `src` folder (for the compiler, not libotterkit) and then type `dotnet run` into the command line.

## Standard Acknowledgement

Any organization interested in reproducing the COBOL standard and specifications in whole or in part,
using ideas from this document as the basis for an instruction manual or for any other purpose, is free
to do so. However, all such organizations are requested to reproduce the following acknowledgment
paragraphs in their entirety as part of the preface to any such publication (any organization using a
short passage from this document, such as in a book review, is requested to mention "COBOL" in
acknowledgment of the source, but need not quote the acknowledgment):

COBOL is an industry language and is not the property of any company or group of companies, or of any
organization or group of organizations.

No warranty, expressed or implied, is made by any contributor or by the CODASYL COBOL Committee
as to the accuracy and functioning of the programming system and language. Moreover, no
responsibility is assumed by any contributor, or by the committee, in connection therewith.

The authors and copyright holders of the copyrighted materials used herein:

- FLOW-MATIC® (trademark of Sperry Rand Corporation), Programming for the 'UNIVAC® I and
  II, Data Automation Systems copyrighted 1958,1959, by Sperry Rand Corporation;
- IBM Commercial Translator Form No F 28-8013, copyrighted 1959 by IBM;
- FACT, DSI 27A5260-2760, copyrighted 1960 by Minneapolis-Honeywell

Have specifically authorized the use of this material in whole or in part, in the COBOL specifications.
Such authorization extends to the reproduction and use of COBOL specifications in programming
manuals or similar publications.

## Contributing
If you have any questions, please feel free to reach out by [opening an issue](https://github.com/otterkit/otterkit/issues) or a [new discussion post](https://github.com/orgs/otterkit/discussions). We're happy to welcome new contributors!

Please keep in mind that the project is licensed under the Apache-2.0, and all contributions (unless otherwise specified) will be made available under the same terms (Apache-2.0, Submission of Contributions).

## Copyright 
Copyright © 2022-2023 Gabriel Gonçalves &lt;KTSnowy@outlook.com&gt;

Unless otherwise specified, all files in this repo (except the Otterkit logo) are licensed under the Apache-2.0 license.

See [LICENSE](https://github.com/otterkit/otterkit/blob/main/LICENSE) for the full license.

see [NOTICE](https://github.com/otterkit/otterkit/blob/main/NOTICE) for third-party software licenses.
