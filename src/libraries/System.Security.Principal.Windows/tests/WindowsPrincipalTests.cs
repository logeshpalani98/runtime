// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Security.Principal;
using Xunit;

public class WindowsPrincipalTests
{
    [Fact]
    public static void WindowsPrincipalIsInRoleNeg()
    {
        WindowsIdentity windowsIdentity = WindowsIdentity.GetAnonymous();
        WindowsPrincipal windowsPrincipal = new WindowsPrincipal(windowsIdentity);

        try
        {
            bool ret = windowsPrincipal.IsInRole("FAKEDOMAIN\\nonexist");
            Assert.False(ret);
        }
        catch (SystemException e)
        {
            // If a domain joined machine can't talk to the domain controller then it
            // can't determine if "FAKEDOMAIN" is resolvable within the AD forest, so it
            // fails with ERROR_TRUSTED_RELATIONSHIP_FAILURE (resulting in an exception).
            const int ERROR_TRUSTED_RELATIONSHIP_FAILURE = 0x6FD;
            Win32Exception win32Exception = new Win32Exception(ERROR_TRUSTED_RELATIONSHIP_FAILURE);

            // NetFx throws a plain SystemException which has the message built via FormatMessage.
            // CoreFx throws a Win32Exception based on the error, which gets the same message value.
            Assert.Equal(win32Exception.Message, e.Message);
        }
    }
}
