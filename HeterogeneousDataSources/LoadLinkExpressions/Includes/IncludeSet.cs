﻿using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.Shared;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public class IncludeSet<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> : IIncludeSet
    {
        private readonly Dictionary<TDiscriminant, IInclude> _includes;
        private readonly Func<TLink, TDiscriminant> _getDiscriminantFunc;

        public IncludeSet(Dictionary<TDiscriminant, IInclude> includes, Func<TLink, TDiscriminant> getDiscriminantFunc)
        {
            _includes = includes;
            _getDiscriminantFunc = getDiscriminantFunc;
        }

        public IIncludeWithCreateNestedLinkedSource<TLinkedSource, TIChildLinkedSource, TLink> GetIncludeWithCreateNestedLinkedSourceForReferenceType(TLink link, Type referenceType) {
            var include = GetInclude<IIncludeWithCreateNestedLinkedSource<TLinkedSource, TIChildLinkedSource, TLink>>(link);
            
            if (include == null || include.ReferenceType != referenceType){ return null; }

            return include;
        }

        public IIncludeWithCreateNestedLinkedSourceFromModel<TIChildLinkedSource,TLink> GetIncludeWithCreateNestedLinkedSourceFromModel(TLink link) {
            return GetInclude<IIncludeWithCreateNestedLinkedSourceFromModel<TIChildLinkedSource,TLink>>(link);
        }

        public IIncludeWithAddLookupId<TLink> GetIncludeWithAddLookupId(TLink linkForReferenceType) {
            return GetInclude<IIncludeWithAddLookupId<TLink>>(linkForReferenceType);
        }

        public IIncludeWithGetReference<TIChildLinkedSource, TLink> GetIncludeWithGetReference(TLink link) {
            return GetInclude<IIncludeWithGetReference<TIChildLinkedSource, TLink>>(link);
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

            var discriminant = _getDiscriminantFunc(link);
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