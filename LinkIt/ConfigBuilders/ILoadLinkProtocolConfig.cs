#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

namespace LinkIt.ConfigBuilders
{
    public interface ILoadLinkProtocolConfig
    {
        void ConfigureLoadLinkProtocol(LoadLinkProtocolBuilder loadLinkProtocolBuilder);
    }
}