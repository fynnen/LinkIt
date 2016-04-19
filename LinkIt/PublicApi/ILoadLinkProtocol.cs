#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

namespace LinkIt.PublicApi
{
    //Responsible for creating a load linker for any root linked source type
    public interface ILoadLinkProtocol
    {
        ILoadLinker<TRootLinkedSource> LoadLink<TRootLinkedSource>();
    }
}