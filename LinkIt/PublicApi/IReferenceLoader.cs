﻿#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;

namespace LinkIt.PublicApi
{
    //Responsible for loading references of a loading level.
    public interface IReferenceLoader
        //Dispose will always be invoked as soon as the load phase is completed or
        //if an exception is thrown
        : IDisposable
    {
        void LoadReferences(
            ILookupIdContext lookupIdContext, 
            ILoadedReferenceContext loadedReferenceContext
        );
    }
}