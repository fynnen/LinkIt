﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.Tests.Shared {
    
    public class FakeReferenceLoader2<TReference, TId>:IReferenceLoader
    {
        private readonly Func<TReference, TId> _getReferenceIdFunc;
        private readonly Dictionary<Type, IReferenceTypeConfig> _referenceTypeConfigByReferenceType;
        private bool _isConnectionOpen = false;

        //stle: Config must be inside fake reference loader in order to access connection?
        private List<IReferenceTypeConfig> GetDefaultReferenceTypeConfigs()
        {
            return new List<IReferenceTypeConfig>{
                new ReferenceTypeConfig<Image, string>(
                    ids => new ImageRepository(_isConnectionOpen).GetByIds(ids),
                    reference => reference.Id,
                    "ouglo"
                ),
                new ReferenceTypeConfig<Person, string>(
                    ids => new PersonRepository().GetByIds(ids),
                    reference => reference.Id
                )
            };
        }

        public FakeReferenceLoader2(Func<TReference, TId> getReferenceIdFunc, params IReferenceTypeConfig[] customReferenceTypeConfigs)
        {
            _getReferenceIdFunc = getReferenceIdFunc;
            var config = customReferenceTypeConfigs
                .Concat(GetDefaultReferenceTypeConfigs())
                .ToList();

            _referenceTypeConfigByReferenceType = config.ToDictionary(
                referenceTypeConfig => referenceTypeConfig.ReferenceType,
                referenceTypeConfig => referenceTypeConfig
            );
        }

        public void FixValue(TReference fixedValue) {
            var fixedReferenceTypeConfig = new ReferenceTypeConfig<TReference, TId>(
                ids => ids
                    .Select(id => fixedValue)
                    .Where(id => id != null)
                    .ToList(),
                _getReferenceIdFunc
            );

            _referenceTypeConfigByReferenceType[typeof (TReference)] = fixedReferenceTypeConfig;
        }

        public void LoadReferences(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext)
        {
            OpenConnectionIfRequired(lookupIdContext);

            foreach (var referenceType in lookupIdContext.GetReferenceTypes())
            {
                LoadReference(referenceType, lookupIdContext, loadedReferenceContext);
            }
        }

        private void LoadReference(Type referenceType, LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext)
        {
            if (!_referenceTypeConfigByReferenceType.ContainsKey(referenceType)){
                throw new NotImplementedException(
                    string.Format("There is no loader for reference of type {0}.", referenceType.Name)
                );
            }
            var referenceTypeConfig = _referenceTypeConfigByReferenceType[referenceType];
            referenceTypeConfig.Load(lookupIdContext, loadedReferenceContext);
        }

        private void OpenConnectionIfRequired(LookupIdContext lookupIdContext) {
            if (GetReferenceTypeThatRequiresOugloConnection()
                .Any(requiresOuglo =>
                    lookupIdContext.GetReferenceTypes().Contains(requiresOuglo)
                )) {
                _isConnectionOpen = true;
            }
        }

        private List<Type> GetReferenceTypeThatRequiresOugloConnection() {
            return _referenceTypeConfigByReferenceType.Values
                .Where(referenceTypeConfig => referenceTypeConfig.RequiredConnection == "ouglo")
                .Select(referenceTypeConfig => referenceTypeConfig.ReferenceType)
                .ToList();
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            _isConnectionOpen = false;
            IsDisposed = true;
        }
    }
}
