using AutoMapper;
using System;
using System.Linq.Expressions;

namespace Ez.Framework.Core.Extensions
{
    /// <summary>
    /// Auto mapper extensions
    /// </summary>
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Ignore all non existing properties
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination>
            IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var typeMap = Mapper.FindTypeMapFor<TSource, TDestination>();

            // Get all unmapped Properties
            var unmappedProperties = typeMap.GetUnmappedPropertyNames();
            foreach (var property in unmappedProperties)
            {
                expression.ForMember(property, opt => opt.Ignore());
            }
            return expression;
        }

        /// <summary>
        /// Add action for all unmapped members
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="map"></param>
        /// <param name="memberOptions"></param>
        public static IMappingExpression<TSource, TDestination> ForAllUnmappedMembers<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> map,
            Action<IMemberConfigurationExpression<TSource>> memberOptions)
        {
            var typeMap = Mapper.FindTypeMapFor<TSource, TDestination>();
            foreach (var memberName in typeMap.GetUnmappedPropertyNames())
                map.ForMember(memberName, memberOptions);
            return map;
        }

        /// <summary>
        /// Ignore extension
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="map"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> map,
            Expression<Func<TDestination, object>> selector)
        {
            map.ForMember(selector, config => config.Ignore());
            return map;
        }

        /// <summary>
        /// Ignore extension
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="map"></param>
        /// <param name="selectors"></param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> map,
            params Expression<Func<TDestination, object>>[] selectors)
        {
            foreach (var selector in selectors)
            {
                map.ForMember(selector, config => config.Ignore());
            }
            return map;
        }

        /// <summary>
        /// Get value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Map<T>(this object source)
        {
            if (Mapper.FindTypeMapFor(source.GetType(), typeof(T)) == null)
            {
                Mapper.CreateMap(source.GetType(), typeof(T));
            }

            return (T)Mapper.Map(source, source.GetType(), typeof(T));
        }
    }
}
