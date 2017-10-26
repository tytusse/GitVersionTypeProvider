using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly:AssemblyInformationalVersion(GitVersionTypeProvider.Tests.AssemblyInfo.Version)]

namespace GitVersionTypeProvider.Tests.CS
{
    public class Tests
    {
        //GitVersionTypeProvider.Tests.AssemblyInfo
        const string Version = GitVersionTypeProvider.Tests.AssemblyInfo.Version;
    }
}
