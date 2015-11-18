using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.Shared;

namespace LinkIt.LinkTargets
{
    //Simplification of the solution shown at:
    //http://stackoverflow.com/questions/7723744/expressionfunctmodel-string-to-expressionactiontmodel-getter-to-sette
    public static class LinkTargetFactory {

        #region Single Value
        public static ILinkTarget<TLinkedSource, TTargetProperty> Create<TLinkedSource, TTargetProperty>(
            Expression<Func<TLinkedSource, TTargetProperty>> getLinkTarget) {
            PropertyInfo property = GetPropertyFromGetter(getLinkTarget);

            return new SingleValueLinkTarget<TLinkedSource, TTargetProperty>(
                property.GetFullName(),
                CreateSingleValueLinkTargetSetterAction<TLinkedSource, TTargetProperty>(property)
            );
        }

        private static Action<TLinkedSource, TTargetProperty> CreateSingleValueLinkTargetSetterAction<TLinkedSource, TTargetProperty>(PropertyInfo property) {
            MethodInfo setter = property.GetSetMethod();
            return (Action<TLinkedSource, TTargetProperty>)Delegate.CreateDelegate(
                typeof(Action<TLinkedSource, TTargetProperty>),
                setter
            );
        } 
        #endregion

        #region Multi Value
        public static ILinkTarget<TLinkedSource, TTargetProperty> Create<TLinkedSource, TTargetProperty>(
            Expression<Func<TLinkedSource, List<TTargetProperty>>> getLinkTarget) {
            PropertyInfo property = GetPropertyFromGetter(getLinkTarget);

            return new MultiValueLinkTarget<TLinkedSource, TTargetProperty>(
                property.GetFullName(),
                getLinkTarget.Compile(),
                CreateMultiValueLinkTargetSetterAction<TLinkedSource, TTargetProperty>(property)
            );
        }

        private static Action<TLinkedSource, List<TTargetProperty>> CreateMultiValueLinkTargetSetterAction<TLinkedSource, TTargetProperty>(PropertyInfo property) {
            MethodInfo setter = property.GetSetMethod();
            return (Action<TLinkedSource, List<TTargetProperty>>)Delegate.CreateDelegate(
                typeof(Action<TLinkedSource, List<TTargetProperty>>),
                setter
            );
        } 
        #endregion

        #region Shared
        private static PropertyInfo GetPropertyFromGetter<TLinkedSource, TTargetProperty>(Expression<Func<TLinkedSource, TTargetProperty>> getter) {

            EnsureNoFunctionOrAction(getter);

            var getterBody = (MemberExpression)getter.Body;
            EnsureNoNestedProperty<TLinkedSource>(getterBody);
            EnsureNoMemberAccess<TLinkedSource>(getterBody);

            var property = (PropertyInfo)getterBody.Member;

            EnsureNoReadOnlyProperty<TLinkedSource>(property);
            
            //Impossible to have a write only property, since the setter is inferred from the getter
            
            return property;
        }

        private static void EnsureNoReadOnlyProperty<TLinkedSource>(PropertyInfo property) {
            if (!property.IsPublicReadWrite()) {
                throw new ArgumentException(
                    string.Format(
                        "{0}: Only read-write property are supported",
                        property.GetFullName()
                    )
                );
            }
        }

        private static void EnsureNoMemberAccess<TLinkedSource>(MemberExpression getterBody) {
            if (getterBody.Member.MemberType != MemberTypes.Property) {
                throw OnlyDirectGetterAreSupported<TLinkedSource>();
            }
        }

        private static void EnsureNoNestedProperty<TLinkedSource>(MemberExpression getterBody) {
            var getterBodyExpression = getterBody.Expression;
            if (getterBodyExpression.NodeType != ExpressionType.Parameter) {
                throw OnlyDirectGetterAreSupported<TLinkedSource>();
            }
        }

        private static void EnsureNoFunctionOrAction<TLinkedSource, TTargetProperty>(Expression<Func<TLinkedSource, TTargetProperty>> getter) {
            if (getter.Body.NodeType != ExpressionType.MemberAccess) {
                throw OnlyDirectGetterAreSupported<TLinkedSource>();
            }
        }

        private static ArgumentException OnlyDirectGetterAreSupported<TLinkedSource>() {
            return new ArgumentException(
                string.Format(
                    "{0}: Only direct getter are supported. Ex: p => p.Property",
                    typeof(TLinkedSource)
                )
            );
        }
        #endregion
    }
}