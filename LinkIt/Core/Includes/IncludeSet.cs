﻿#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.Shared;

namespace LinkIt.Core.Includes
{
    //Responsible for giving access to the includes of a specific link target.
    public class IncludeSet<TLinkedSource, TAbstractChildLinkedSource, TLink, TDiscriminant>
    {
        private readonly Dictionary<TDiscriminant, IInclude> _includes;
        private readonly Func<TLink, TDiscriminant> _getDiscriminant;

        public IncludeSet(Dictionary<TDiscriminant, IInclude> includes, Func<TLink, TDiscriminant> getDiscriminant)
        {
            _includes = includes;
            _getDiscriminant = getDiscriminant;
        }

        public IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink> GetIncludeWithCreateNestedLinkedSourceByIdForReferenceType(TLink link, Type referenceType) {
            var include = GetInclude<IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink>>(link);
            
            if (include == null || include.ReferenceType != referenceType){ return null; }

            return include;
        }

        public IIncludeWithCreateNestedLinkedSourceFromModel<TAbstractChildLinkedSource,TLink> GetIncludeWithCreateNestedLinkedSourceFromModel(TLink link) {
            return GetInclude<IIncludeWithCreateNestedLinkedSourceFromModel<TAbstractChildLinkedSource,TLink>>(link);
        }

        public IIncludeWithAddLookupId<TLink> GetIncludeWithAddLookupId(TLink linkForReferenceType) {
            return GetInclude<IIncludeWithAddLookupId<TLink>>(linkForReferenceType);
        }

        public IIncludeWithGetReference<TAbstractChildLinkedSource, TLink> GetIncludeWithGetReference(TLink link) {
            return GetInclude<IIncludeWithGetReference<TAbstractChildLinkedSource, TLink>>(link);
        }

        public List<IIncludeWithAddLookupId<TLink>> GetIncludesWithAddLookupId(){
            return GetIncludes<IIncludeWithAddLookupId<TLink>>();
        }

        public List<IIncludeWithChildLinkedSource> GetIncludesWithChildLinkedSource(){
            return GetIncludes<IIncludeWithChildLinkedSource>();
        }

        private TInclude GetInclude<TInclude>(TLink link) 
            where TInclude:class
        {
            AssumeNotNullLink<TInclude>(link);

            var discriminant = _getDiscriminant(link);
            AssumeIncludeExistsForDiscriminant<TInclude>(discriminant);

            var include = _includes[discriminant];

            return include as TInclude;
        }

        private void AssumeIncludeExistsForDiscriminant<TInclude>(TDiscriminant discriminant) where TInclude : class
        {
            if (!_includes.ContainsKey(discriminant))
            {
                throw new AssumptionFailed(
                    string.Format(
                        "{0}: Cannot invoke GetInclude for discriminant={1}",
                        typeof (TLinkedSource),
                        discriminant
                        )
                    );
            }
        }

        private static void AssumeNotNullLink<TInclude>(TLink link) where TInclude : class
        {
            if (link == null)
            {
                throw new AssumptionFailed(
                    string.Format(
                        "{0}: Cannot invoke GetInclude with a null link",
                        typeof (TLinkedSource)
                        )
                    );
            }
        }

        public List<TInclude> GetIncludes<TInclude>() 
            where TInclude:class,IInclude
        {
            return _includes.Values
                .Where(include => include is TInclude)
                .Cast<TInclude>()
                .ToList();
        }
    }
}